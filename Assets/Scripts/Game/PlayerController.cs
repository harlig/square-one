using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : Singleton<PlayerController>
{
    // currentPosition here is before player moves
    public delegate void MoveStartAction(Vector2Int positionBeforeMove);
    public static event MoveStartAction OnMoveStart;

    // currentPosition here is after player moves
    public delegate void MoveFinishAction(Vector2Int positionAfterMove);
    public static event MoveFinishAction OnMoveFinish;

    private Vector3 originalPosition;

    private bool shouldCountMoves = true;
    private int moveCount;
    private GameObject playerInstanceGameObject;
    // TODO probably shouldn't be public hmm
    public Cube Cube { get; set; }

    private Vector2Int currentPosition;

    public void SpawnPlayer(int row, int col)
    {
        originalPosition = new Vector3(row, 1.5f, col);
        transform.localPosition = originalPosition;
        moveCount = 0;

        playerInstanceGameObject = gameObject;

        // defines roll speed and allows to roll
        Cube = new(this, 3.0f, BeforeRollActions(), AfterRollActions());
        currentPosition = GetRawCurrentPosition();
        CameraController.OnCameraRotate += TrackCameraLocation;
    }

    private void OnDisable()
    {
        CameraController.OnCameraRotate -= TrackCameraLocation;
    }

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

    public void ResetPosition()
    {
        playerInstanceGameObject.transform.localPosition = originalPosition;
        Cube.ResetPhysics();
    }

    Action BeforeRollActions()
    {
        return () =>
        {
            OnMoveStart?.Invoke(currentPosition);
            _isMoving = true;
        };
    }

    Action AfterRollActions()
    {
        return () =>
        {
            // player should have their move count increased once they've finished moving
            if (shouldCountMoves) moveCount++;
            _isMoving = false;

            currentPosition = GetRawCurrentPosition();
            OnMoveFinish?.Invoke(currentPosition);
        };
    }

    /**
        This internal method should only be used to get the raw current position of the player. Its public counterpart
        should be used to determine the player's position since it handles the roll animation in its calculation.
    */
    private Vector2Int GetRawCurrentPosition()
    {
        return new Vector2Int(Mathf.RoundToInt(playerInstanceGameObject.transform.position.x), Mathf.RoundToInt(playerInstanceGameObject.transform.position.z));
    }

    private bool _isMoving;

    private static List<Vector3Int> playerInputDirections = new List<Vector3Int>(){
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
        Debug.LogFormat("playerInputStuff: {0} for this moveDirection: [{1}, {2}]", playerInputDirectionCameraOffset, movementX, movementY);
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

    // OnMove comes from the InputActions action defined Move
    void OnMove(InputValue movementValue)
    {
        if (_isMoving) return;

        Vector2 movementVector = movementValue.Get<Vector2>();

        if (Mathf.Abs(movementVector.x) != 1.0f && Mathf.Abs(movementVector.y) != 1.0f) return;

        int movementX = Mathf.RoundToInt(movementVector.x);
        int movementY = Mathf.RoundToInt(movementVector.y);

        Vector3Int relativeMoveDirection = GetRelativeMoveDirectionWithCameraOffset(movementX, movementY);
        Cube.MoveInDirectionIfNotMoving(relativeMoveDirection);

        // TODO player can float by constant input, how to disallow? prev solution below

        // downwards force disallows wall climbing, constant was chosen because it plays well
        // this solution isn't great but seems good enough, feel free to update it to be cleaner
        // rb.AddForce(Vector3.down * 25, ForceMode.Force);
    }

    public void DisableInput()
    {
        GetComponent<InputAction>().Disable();
    }

    public void EnableInput()
    {
        GetComponent<InputAction>().Enable();
    }

    public bool ShouldCountMoves()
    {
        return shouldCountMoves;
    }

    public void StopCountingMoves()
    {
        shouldCountMoves = false;
    }

    // TODO need to update GetCurrentPosition when the player gets moved by some other obstacle
    /**
        Get player's location taking into account roll animation. This position only updates once a roll animation completes.
    */
    public Vector2Int GetCurrentPosition()
    {
        return currentPosition;
    }

    // TODO need to do something like this to stop player movement and wait for it to finish
    public void ForceMoveInDirection(Vector3 direction)
    {
        StopCountingMoves();
        OnMoveFinish += StartCountingMovesAndDeregisterOnMoveFinish;
        Cube.ForceMoveInDirection(direction);
    }

    private void StartCountingMovesAndDeregisterOnMoveFinish(Vector2Int pos)
    {
        StartCountingMoves();
        OnMoveFinish -= StartCountingMovesAndDeregisterOnMoveFinish;
    }

    private void StartCountingMoves()
    {
        shouldCountMoves = true;
    }
}
