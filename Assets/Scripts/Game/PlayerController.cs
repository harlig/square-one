using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : Singleton<PlayerController>
{
    // currentPosition here is before player moves
    public delegate void MoveStartAction(Vector2Int positionBeforeMove);
    public static event MoveStartAction OnMoveStart;

    // currentPosition here is after player moves one single movement. You probably want to use OnMoveFullyCompleted
    public delegate void SingleMoveFinishAction(Vector2Int positionAfterMove, bool moveShouldCount);
    public static event SingleMoveFinishAction OnSingleMoveFinish;

    // currentPosition here is after player moves
    public delegate void MoveFullyCompletedAction(Vector2Int positionAfterMove, bool oneMoveInThereShouldCount);
    public static event MoveFullyCompletedAction OnMoveFullyCompleted;

    // enable movement by default but can be toggled
    public bool MovementEnabled { get; set; } = true;

    private Vector3 originalPosition;

    private bool inTerminalGameState = false;
    private bool shouldCountMoves = true;
    private int moveCount;
    private GameObject playerInstanceGameObject;
    private Cube Cube { get; set; }

    private Vector2Int currentPosition;
    private bool _isMoving;

    private Func<int, int, bool> tileAtLocationWillMovePlayer;
    private bool forcedStopMoving = false;

    public void SpawnPlayer(int row, int col, Func<int, int, bool> tileAtLocationWillMovePlayer)
    {
        originalPosition = new Vector3(row, 1.5f, col);
        transform.localPosition = originalPosition;
        moveCount = 0;

        playerInstanceGameObject = gameObject;

        this.tileAtLocationWillMovePlayer = tileAtLocationWillMovePlayer;

        // defines roll speed and allows to roll. can pass through beforeMovePosition if we want it
        Cube = new(this, 4.4f, () => BeforeRollActions(), (moveCompleted, moveShouldCount, beforeMovePosition) => AfterRollActions(moveCompleted, moveShouldCount));
        currentPosition = GetRawCurrentPosition();
        CameraController.OnCameraRotate += TrackCameraLocation;
    }

#pragma warning disable IDE0051
    private void OnDisable()
    {
        CameraController.OnCameraRotate -= TrackCameraLocation;
    }

    Vector2? startTouchPosition, endTouchPosition;

    private static readonly int MIN_MOVEMENT_SWIPE_THRESHOLD = 20;

    // this is handling raw input for ideally only webGL
    void Update()
    {
        if (MovementEnabled)
        {
            if (Application.isMobilePlatform)
            {
                if (Input.touches.Length != 0)
                {
                    Debug.Log("user has touched screen");
                    Touch touch = Input.touches[0];
                    if (touch.phase == UnityEngine.TouchPhase.Began)
                    {
                        Debug.Log("start touch");
                        startTouchPosition = touch.position;
                    }
                    else if (touch.phase == UnityEngine.TouchPhase.Ended)
                    {
                        Debug.Log("end touch");
                        endTouchPosition = touch.position;

                    }
                    else if (touch.phase == UnityEngine.TouchPhase.Canceled)
                    {
                        Debug.Log("canceled touch");
                        startTouchPosition = endTouchPosition = null;
                    }
                    else
                    {
                        Debug.LogFormat("unhandled touch: {0}", touch.phase);
                    }
                }

                if (startTouchPosition != null && endTouchPosition != null)
                {
                    Debug.Log("PLAYER: we have both a start and end touch position, time to calculate");
                    float xDiff = (float)(endTouchPosition?.x - startTouchPosition?.x);
                    float yDiff = (float)(endTouchPosition?.y - startTouchPosition?.y);
                    if (Mathf.Abs(xDiff) <= MIN_MOVEMENT_SWIPE_THRESHOLD || Mathf.Abs(yDiff) <= MIN_MOVEMENT_SWIPE_THRESHOLD)
                    {
                        Debug.LogWarningFormat("Swipe was tiny, let's just not count this one. xDiff: {0}, yDiff: {1}", xDiff, yDiff);
                    }
                    else if (xDiff <= 0 && yDiff >= 0)
                    {
                        MoveUp();
                    }
                    else if (xDiff <= 0 && yDiff <= 0)
                    {
                        MoveLeft();
                    }
                    else if (xDiff >= 0 && yDiff <= 0)
                    {
                        MoveDown();
                    }
                    else
                    {
                        MoveRight();
                    }

                    startTouchPosition = endTouchPosition = null;
                }
            }
            else
            {
                if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
                {
                    MoveUp();
                }
                else if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
                {
                    MoveLeft();
                }
                else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
                {
                    MoveDown();
                }
                else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
                {
                    MoveRight();
                }
            }

            void MoveUp()
            {
                TryMove(new Vector2Int(0, 1));
            }
            void MoveLeft()
            {
                TryMove(new Vector2Int(-1, 0));
            }
            void MoveDown()
            {
                TryMove(new Vector2Int(0, -1));
            }
            void MoveRight()
            {
                TryMove(new Vector2Int(1, 0));
            }
        }
    }

    void TryMove(Vector2 movementVector)
    {
        if (Mathf.Abs(movementVector.x) != 1.0f && Mathf.Abs(movementVector.y) != 1.0f) return;

        int movementX = Mathf.RoundToInt(movementVector.x);
        int movementY = Mathf.RoundToInt(movementVector.y);

        if (_isMoving || forcedStopMoving) return;

        Vector3Int relativeMoveDirection = GetRelativeMoveDirectionWithCameraOffset(movementX, movementY);
        Cube.MoveInDirectionIfNotMoving(relativeMoveDirection, Cube.MoveType.ROLL, shouldCountMoves && !inTerminalGameState);

        // TODO player can float by constant input, how to disallow? prev solution below

        // downwards force disallows wall climbing, constant was chosen because it plays well
        // this solution isn't great but seems good enough, feel free to update it to be cleaner
        // rb.AddForce(Vector3.down * 25, ForceMode.Force);
    }
#pragma warning restore IDE0051

    private void TrackCameraLocation(Vector2Int direction)
    {
        if (direction == Vector2Int.right)
        {
            RotateRight();
        }
        else if (direction == Vector2Int.left)
        {
            RotateLeft();
        }
        else
        {
            Debug.LogErrorFormat("Camera moved in unexpected direction: {0}", direction);
        }
    }

    public int GetMoveCount()
    {
        return moveCount;
    }

    public void ResetMoveCount()
    {
        moveCount = 0;
    }

    public void ResetPosition()
    {
        playerInstanceGameObject.transform.localPosition = originalPosition;
        Cube.ResetPhysics();
        currentPosition = GetRawCurrentPosition();
    }

    void BeforeRollActions()
    {
        OnMoveStart?.Invoke(currentPosition);
        _isMoving = true;
    }

    bool anyMoveCompleted = false;
    bool shouldAnyMoveBeCounted = false;

    void AfterRollActions(bool moveCompleted, bool shouldMoveBeCounted)
    {
        _isMoving = false;

        currentPosition = GetRawCurrentPosition();
        OnSingleMoveFinish?.Invoke(currentPosition, moveCompleted && shouldMoveBeCounted);

        anyMoveCompleted |= moveCompleted;
        shouldAnyMoveBeCounted |= shouldMoveBeCounted;

        bool willTileMovePlayer = tileAtLocationWillMovePlayer.Invoke(currentPosition.x, currentPosition.y);

        if (!forcedStopMoving && willTileMovePlayer)
        {
            return;
        }

        bool moveHappenedAndShouldBeCount = anyMoveCompleted && shouldAnyMoveBeCounted;

        if (moveHappenedAndShouldBeCount)
        {
            moveCount++;
        }

        // snap player position. do I want this?
        playerInstanceGameObject.transform.localPosition = new Vector3(currentPosition.x, playerInstanceGameObject.transform.localPosition.y, currentPosition.y);

        OnMoveFullyCompleted?.Invoke(currentPosition, moveHappenedAndShouldBeCount);

        forcedStopMoving = false;
        anyMoveCompleted = false;
        shouldAnyMoveBeCounted = false;
    }

    /**
        This internal method should only be used to get the raw current position of the player. Its public counterpart
        should be used to determine the player's position since it handles the roll animation in its calculation.
    */
    private Vector2Int GetRawCurrentPosition()
    {
        return new Vector2Int(Mathf.RoundToInt(playerInstanceGameObject.transform.position.x), Mathf.RoundToInt(playerInstanceGameObject.transform.position.z));
    }

    private static readonly List<Vector3Int> playerInputDirections = new(){
        Vector3Int.forward, //up 
        Vector3Int.left, // left
        Vector3Int.back, // down
        Vector3Int.right //right
    };

    private int playerInputDirectionCameraOffset = 0;

    void RotateLeft()
    {
        if (playerInputDirectionCameraOffset == 0) { playerInputDirectionCameraOffset = playerInputDirections.Count - 1; }
        else { playerInputDirectionCameraOffset--; }
    }

    void RotateRight()
    {
        if (playerInputDirectionCameraOffset == playerInputDirections.Count - 1) { playerInputDirectionCameraOffset = 0; }
        else { playerInputDirectionCameraOffset++; }
    }

    Vector3Int GetRelativeMoveDirectionWithCameraOffset(int movementX, int movementY)
    {
        if (movementX == -1) return PlayerInputDirectionsFromOffset(1);
        else if (movementX == 1) return PlayerInputDirectionsFromOffset(3);
        else if (movementY == 1) return PlayerInputDirectionsFromOffset(0);
        else if (movementY == -1) return PlayerInputDirectionsFromOffset(2);
        else
        {
            Debug.LogErrorFormat("Tried to move player without one unit movements - movementX: {0}; movementY: {1}", movementX, movementY);
            throw new Exception("Probably shouldn't be an exception but wtf you do?!");
        }

        Vector3Int PlayerInputDirectionsFromOffset(int directionOffset)
        {
            return playerInputDirections[(directionOffset + playerInputDirectionCameraOffset) % playerInputDirections.Count];
        }
    }

    public bool ShouldCountMoves()
    {
        return shouldCountMoves;
    }

    public void StartCountingMoves()
    {
        shouldCountMoves = true;
    }

    public void StopCountingMoves()
    {
        shouldCountMoves = false;
    }

    public void EnterTerminalGameState()
    {
        inTerminalGameState = true;
    }

    /**
        Get player's location taking into account move animation. This position only updates once a move animation completes.
    */
    public Vector2Int GetCurrentPosition()
    {
        return currentPosition;
    }

    public float GetHeight()
    {
        return transform.localPosition.y;
    }

    /**
        Update the player's known location after it has moved via something other than player;
        e.g. something else (obstacle) interacting with player and causing player to move.

    */
    public void StartUpdatingLocation()
    {
        // this is a bad solution, but it kinda works
        StartCoroutine(StartUpdatingLocation(1.0f));
    }

    /**
        For $seconds seconds, update the player's externally-exposed currentPosition to its current position.
    */
    IEnumerator StartUpdatingLocation(float seconds)
    {
        float interval = 0.1f;
        float elapsed = 0.0f;
        while (elapsed < seconds)
        {
            // if you notice the player's position is screwed up after colliding with an obstacle, validate this is working
            currentPosition = GetRawCurrentPosition();
            yield return new WaitForSeconds(interval);
            elapsed += interval;
        };
    }

    public void ForceMoveInDirection(Vector3 direction)
    {
        StopCountingMoves();
        OnSingleMoveFinish += StartCountingMovesAndDeregisterOnMoveFinish;
        Cube.MoveInDirectionIfNotMoving(direction, Cube.MoveType.SLIDE, false);
    }

    private void StartCountingMovesAndDeregisterOnMoveFinish(Vector2Int pos, bool _)
    {
        StartCountingMoves();
        OnSingleMoveFinish -= StartCountingMovesAndDeregisterOnMoveFinish;
    }

    public void FinishMovingThenStopMovement()
    {
        Cube.FinishMovingThenStopMovement();
        forcedStopMoving = true;
    }

    public void ResetMovement()
    {
        StopMoving();
        ResetPosition();
        StartMoving();
    }

    public void StopMoving()
    {
        Cube.StopMoving();
        forcedStopMoving = true;
    }

    public void StartMoving()
    {
        _isMoving = false;
        Cube.StartMoving();
        forcedStopMoving = false;
    }

    public float GetRollSpeed()
    {
        return Cube.GetRollSpeed();
    }

    public static bool IsColliderPlayer(Collider other)
    {
        if (!other.gameObject.CompareTag("Player"))
        {
            // Debug.LogFormat("Obstacle collided with non-player entity: {0}", other);
            return false;
        }

        PlayerController playerController = other.GetComponent<PlayerController>();
        if (playerController == null)
        {
            Debug.LogAssertion("Something tagged `Player` with no PlayerController collided with this obstacle!");
            return false;
        }
        return true;
    }

    public IEnumerator StopMovementThenStartMovement(float delayBeforeStartMovementSeconds, Action afterStopCallback, Action beforeStartCallback)
    {
        StopMoving();
        afterStopCallback?.Invoke();
        yield return new WaitForSeconds(delayBeforeStartMovementSeconds);
        beforeStartCallback?.Invoke();
        StartMoving();
    }
}
