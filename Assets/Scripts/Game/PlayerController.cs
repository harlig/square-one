using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private Rigidbody rb;
    private float movementX, movementY;
    private Vector3 originalPosition;

    private int moveCount;

    public void SpawnPlayer(int row, int col)
    {
        originalPosition = new Vector3(row, 1.5f, col);
        transform.localPosition = originalPosition;
        moveCount = 0;
    }

    // Start is called before the first frame update
    void Start()
    {
        // assumes that a Rigidbody exists on this GameObject
        rb = GetComponent<Rigidbody>();
    }

    public int GetMoveCount()
    {
        return moveCount;
    }

    public void ResetPosition()
    {
        transform.localPosition = originalPosition;
        ResetPlayerPhysics();
    }

    // ripped this code, rethink it
    [SerializeField] private readonly float _rollSpeed = 5;
    private bool _isMoving;

    // OnMove comes from the InputActions action defined Move
    void OnMove(InputValue movementValue)
    {
        if (_isMoving) return;

        Vector2 movementVector = movementValue.Get<Vector2>();
        if (Mathf.Abs(movementVector.x) != 1.0f && Mathf.Abs(movementVector.y) != 1.0f) return;
        movementX = movementVector.x;
        movementY = movementVector.y;


        // Debug.LogFormat("Moving player w this vector: {0}", movementVector);
        if (!_isMoving)
        {
            _isMoving = true;
            if (movementX == -1) Assemble(Vector3.left);
            else if (movementX == 1) Assemble(Vector3.right);
            else if (movementY == 1) Assemble(Vector3.forward);
            else if (movementY == -1) Assemble(Vector3.back);
        }
        else
        {
            return;
        }

        void Assemble(Vector3 dir)
        {
            var anchor = transform.localPosition + (Vector3.down + dir) * 0.5f;
            var axis = Vector3.Cross(Vector3.up, dir);
            // I think I want less of a Roll and more of a fixed one unit movement
            float rotationRemaining = 90;

            // TODO different math for tiny player?
            StartCoroutine(Roll(anchor, axis));

            IEnumerator Roll(Vector3 anchor, Vector3 axis)
            {
                for (var i = 0; i < 90 / _rollSpeed; i++)
                {
                    float rotationAngle = Mathf.Min(_rollSpeed, rotationRemaining);
                    transform.RotateAround(anchor, axis, rotationAngle);
                    rotationRemaining -= _rollSpeed;
                    // yield return new WaitForSeconds(0.01f);
                    yield return null;
                }

                Vector3 pos = transform.position;
                transform.localPosition = Vector3Int.RoundToInt(pos);
                ResetPlayerPhysics();

                moveCount++;
                _isMoving = false;
                yield return null;
            }

        }
    }

    void ResetPlayerPhysics()
    {
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        RotateToNearestRightAngles();

        void RotateToNearestRightAngles()
        {
            Quaternion roundedRotation = new(ClosestRightAngle(transform.rotation.x), ClosestRightAngle(transform.rotation.y), ClosestRightAngle(transform.rotation.z), transform.rotation.w);
            transform.rotation = roundedRotation;

            static int ClosestRightAngle(float rotation)
            {
                bool isPositive = rotation > 0;
                return Mathf.RoundToInt(rotation) * 90 * (isPositive ? 1 : -1);
            }
        }
    }
}
