using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : Singleton<PlayerController>
{
    public delegate void MoveStartAction();
    public static event MoveStartAction OnMoveStart;

    public delegate void MoveFinishAction();
    public static event MoveFinishAction OnMoveFinish;

    private float movementX, movementY;
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
        cube = new(this, 4.4f);
    }

    // Start is called before the first frame update
    void Start()
    {
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

    private bool _isMoving;

    // OnMove comes from the InputActions action defined Move
    void OnMove(InputValue movementValue)
    {
        if (_isMoving) return;

        Vector2 movementVector = movementValue.Get<Vector2>();
        if (Mathf.Abs(movementVector.x) != 1.0f && Mathf.Abs(movementVector.y) != 1.0f) return;
        movementX = movementVector.x;
        movementY = movementVector.y;


        if (!_isMoving)
        {
            // lock
            _isMoving = true;
            // call delegate
            OnMoveStart?.Invoke();

            if (movementX == -1) cube.Assemble(Vector3.left);
            else if (movementX == 1) cube.Assemble(Vector3.right);
            else if (movementY == 1) cube.Assemble(Vector3.forward);
            else if (movementY == -1) cube.Assemble(Vector3.back);
        }
        else
        {
            return;
        }
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
