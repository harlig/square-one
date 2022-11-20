using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : Singleton<PlayerController>
{
    public delegate void MoveStartAction();
    public static event MoveStartAction OnMoveStart;

    public delegate void MoveFinishAction();
    public static event MoveFinishAction OnMoveFinish;

    private Vector3 originalPosition;

    private bool shouldCountMoves = true;
    private int moveCount;
    private GameObject playerInstanceGameObject;
    private Cube cube;

    public void SpawnPlayer(int row, int col)
    {
        originalPosition = new Vector3(row, 1.5f, col);
        transform.localPosition = originalPosition;
        moveCount = 0;

        playerInstanceGameObject = gameObject;

        // defines roll speed and allows to roll
        cube = new(this, 3.0f, BeforeRollActions(), AfterRollActions());
    }

    public int GetMoveCount()
    {
        return moveCount;
    }

    public void ResetPosition()
    {
        playerInstanceGameObject.transform.localPosition = originalPosition;
        cube.ResetPhysics();
    }

    Action BeforeRollActions()
    {
        return () =>
        {
            OnMoveStart?.Invoke();
            _isMoving = true;
        };
    }

    Action AfterRollActions()
    {
        return () =>
        {
            // player should have their move count increased once they've finished moving
            if (shouldCountMoves) moveCount++;
            OnMoveFinish?.Invoke();
            _isMoving = false;
        };
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

        if (movementX == -1) cube.MoveInDirectionIfNotMoving(Vector3.left);
        else if (movementX == 1) cube.MoveInDirectionIfNotMoving(Vector3.right);
        else if (movementY == 1) cube.MoveInDirectionIfNotMoving(Vector3.forward);
        else if (movementY == -1) cube.MoveInDirectionIfNotMoving(Vector3.back);
    }

    public void StopCountingMoves()
    {
        shouldCountMoves = false;
    }

    public Vector2Int GetRoundedPosition()
    {
        return new Vector2Int(Mathf.RoundToInt(playerInstanceGameObject.transform.position.x), Mathf.RoundToInt(playerInstanceGameObject.transform.position.z));
    }

}
