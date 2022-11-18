using System.Collections;
using UnityEngine;

public class ObstacleController : MonoBehaviour
{
    private enum MoveDirection
    {
        TOWARDS_START_POSITION,
        TOWARDS_PATROL_POSITION
    }

    public void LogFromObstacle()
    {
        Debug.Log("You're calling an obstacle here!");
    }

    public void SetName(string name)
    {
        gameObject.name = name;
    }

    private Vector2Int spawnPosition;
    private bool _isPatrolling;

    public void SetPatrolPosition(Vector2Int patrolPosition)
    {
        spawnPosition = GetPositionAsVector2Int();

        StartCoroutine(MoveObstacleOnPatrolCourse(MoveDirection.TOWARDS_PATROL_POSITION));

        // this is bound to cause index errors but I can't be bothered to deal with that right now
        IEnumerator MoveObstacleOnPatrolCourse(MoveDirection moveDirection)
        {
            Debug.LogFormat("Moving obstacle in this direction: {0}", moveDirection);
            Vector2Int endPosition;
            if (moveDirection == MoveDirection.TOWARDS_START_POSITION)
            {
                endPosition = spawnPosition;
            }
            else if (moveDirection == MoveDirection.TOWARDS_PATROL_POSITION)
            {
                endPosition = patrolPosition;
            }
            else
            {
                throw new System.Exception("Unexpected move direction!");
            }

            _isPatrolling = true;
            StartCoroutine(ChangePatrolDirectionWhenDonePatrolling(moveDirection));
            Vector2Int curPosition = GetPositionAsVector2Int();
            int xDiff = Mathf.Abs(curPosition.x - endPosition.x);
            int yDiff = Mathf.Abs(curPosition.y - endPosition.y);
            while (xDiff != 0 || yDiff != 0)
            {
                // if bigger x deficit, move that way first
                if (xDiff > yDiff)
                {
                    if (curPosition.x > endPosition.x)
                    {
                        Assemble(Vector3.left);
                    }
                    else
                    {
                        Assemble(Vector3.right);
                    }
                }
                // TODO, add this back in and think of a more explicit solution for when xDiff and yDiff are equivalent
                // else if (xDiff < yDiff)
                else
                {
                    {
                        if (curPosition.y > endPosition.y)
                        {
                            Assemble(Vector3.forward);
                        }
                        else
                        {
                            Assemble(Vector3.back);
                        }
                    }

                }
                // should maybe wait a second or two? we need to let the obstacle finish rolling too?
                yield return null;

                // TODO ethan
                // wait also we want to floor the values when they're close to the max size, and we need to do it specifically for x and z i think?

                // when we get into the 1 range, we don't want to snap to a zero deficit when we're at like 0.4
                if (xDiff == 1 || yDiff == 1)
                {
                    Debug.Log("You have entered my ceil rounding path, make yourself at home.");
                    curPosition = Vector2Int.CeilToInt(new Vector2(transform.position.x, transform.position.z));
                }
                else
                {
                    curPosition = GetPositionAsVector2Int();
                }
                xDiff = Mathf.Abs(curPosition.x - endPosition.x);
                yDiff = Mathf.Abs(curPosition.y - endPosition.y);
            }
            _isPatrolling = false;
            Debug.Log("Done patrolling");
        }

        IEnumerator ChangePatrolDirectionWhenDonePatrolling(MoveDirection oldMoveDirection)
        {
            Debug.Log("Checking for patrolling 1");
            while (_isPatrolling)
            {
                // can modify to determine how long should rest at the final position before switching directions
                yield return new WaitForSeconds(0.5f);
            }
            Debug.Log("Done checking for patrolling");
            // I think this is okay because this execution of the method will end after the StartCoroutine call
            // and a new execution begins, but the old garbage is freed. if obstacle movement is slow this could be why though
            if (oldMoveDirection == MoveDirection.TOWARDS_START_POSITION)
            {
                StartCoroutine(MoveObstacleOnPatrolCourse(MoveDirection.TOWARDS_PATROL_POSITION));
            }
            else if (oldMoveDirection == MoveDirection.TOWARDS_PATROL_POSITION)
            {
                StartCoroutine(MoveObstacleOnPatrolCourse(MoveDirection.TOWARDS_START_POSITION));
            }
            else
            {
                throw new System.Exception("Unexpected move direction!");
            }

        }
    }

    private Vector2Int GetPositionAsVector2Int()
    {
        return Vector2Int.RoundToInt(new Vector2(transform.position.x, transform.position.z));
    }

    // TODO combine this with the code in PlayerController
    // DON'T UPDATE THIS WITHOUT UPDATING PlayerController
    [SerializeField] private float _rollSpeed = 1.0f;
    private bool _isRotating;

    void Assemble(Vector3 dir)
    {
        if (_isRotating) return;

        // lock
        _isRotating = true;

        var anchor = gameObject.transform.localPosition + (Vector3.down + dir) * 0.5f;
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
                gameObject.transform.RotateAround(anchor, axis, rotationAngle);
                rotationRemaining -= _rollSpeed;
                yield return null;
            }

            Vector3 pos = gameObject.transform.position;
            gameObject.transform.localPosition = Vector3Int.RoundToInt(pos);
            ResetObstaclePhysics();

            _isRotating = false;
        }

    }

    // TODO combine this with the code in PlayerController
    // DON'T UPDATE THIS WITHOUT UPDATING PlayerController
    void ResetObstaclePhysics()
    {
        // rb.velocity = Vector3.zero;
        // rb.angularVelocity = Vector3.zero;
        RotateToNearestRightAngles();

        void RotateToNearestRightAngles()
        {
            Quaternion roundedRotation = new(
                ClosestRightAngle(gameObject.transform.rotation.x),
                ClosestRightAngle(gameObject.transform.rotation.y),
                ClosestRightAngle(gameObject.transform.rotation.z),
                gameObject.transform.rotation.w);

            gameObject.transform.rotation = roundedRotation;

            static int ClosestRightAngle(float rotation)
            {
                bool isPositive = rotation > 0;
                return Mathf.RoundToInt(rotation) * 90 * (isPositive ? 1 : -1);
            }
        }
    }

}