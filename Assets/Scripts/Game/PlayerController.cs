using System;
using System.Collections;
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
        Debug.LogFormat("Camera is moving in this direction {0}", direction);
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

    // OnMove comes from the InputActions action defined Move
    void OnMove(InputValue movementValue)
    {
        if (_isMoving) return;

        Vector2 movementVector = movementValue.Get<Vector2>();
        if (Mathf.Abs(movementVector.x) != 1.0f && Mathf.Abs(movementVector.y) != 1.0f) return;
        float movementX = movementVector.x;
        float movementY = movementVector.y;

        // TODO ethan just take into account camera rotation and you can move accordingly
        if (movementX == -1) Cube.MoveInDirectionIfNotMoving(Vector3.left);
        else if (movementX == 1) Cube.MoveInDirectionIfNotMoving(Vector3.right);
        else if (movementY == 1) Cube.MoveInDirectionIfNotMoving(Vector3.forward);
        else if (movementY == -1) Cube.MoveInDirectionIfNotMoving(Vector3.back);

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
