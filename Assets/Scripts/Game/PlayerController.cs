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
    // TODO probably shouldn't be public hmm
    public Cube Cube { get; set; }

    public void SpawnPlayer(int row, int col)
    {
        originalPosition = new Vector3(row, 1.5f, col);
        transform.localPosition = originalPosition;
        moveCount = 0;

        playerInstanceGameObject = gameObject;

        // defines roll speed and allows to roll
        Cube = new(this, 3.0f, BeforeRollActions(), AfterRollActions());
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
            OnMoveStart?.Invoke();
            _isMoving = true;
        };
    }

    // TODO the player could keep track of its location post-movement, so like the player can use it
    // to detemrine what tile on it's on, and it won't be "on" a tile until the movement finishes
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

        if (movementX == -1) Cube.MoveInDirectionIfNotMoving(Vector3.left);
        else if (movementX == 1) Cube.MoveInDirectionIfNotMoving(Vector3.right);
        else if (movementY == 1) Cube.MoveInDirectionIfNotMoving(Vector3.forward);
        else if (movementY == -1) Cube.MoveInDirectionIfNotMoving(Vector3.back);

        // TODO player can float by constant input, how to disallow? prev solution below

        // downwards force disallows wall climbing, constant was chosen because it plays well
        // this solution isn't great but seems good enough, feel free to update it to be cleaner
        // rb.AddForce(Vector3.down * 25, ForceMode.Force);

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
