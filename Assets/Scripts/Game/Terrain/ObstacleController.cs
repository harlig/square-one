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
    private Cube _cube;

    public void StartPatrolling(Vector2Int patrolPosition)
    {
        spawnPosition = GetPositionAsVector2Int();

        StartCoroutine(MoveObstacleOnPatrolCourse(MoveDirection.TOWARDS_PATROL_POSITION));

        // this is bound to go out of the map but I can't be bothered to deal with that right now
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
                        _cube.MoveInDirectionIfNotMoving(Vector3.left);
                    }
                    else
                    {
                        _cube.MoveInDirectionIfNotMoving(Vector3.right);
                    }
                }
                // TODO, add this back in and think of a more explicit solution for when xDiff and yDiff are equivalent
                // else if (xDiff < yDiff)
                else
                {
                    {
                        if (curPosition.y > endPosition.y)
                        {
                            _cube.MoveInDirectionIfNotMoving(Vector3.back);
                        }
                        else
                        {
                            _cube.MoveInDirectionIfNotMoving(Vector3.forward);
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

    void OnPlayerMoveFinish()
    {
        Debug.Log("Obstacle knows about player movement");
        if (_moveTowardsPlayer)
        {
            // move one unit torwads player's position

        }
    }

    void Awake()
    {
        // no need for any before/after roll actions right now
        _cube = new(this, 1.0f, () => { }, () => { });
    }

    // must be done at object enable time
    void OnEnable()
    {
        PlayerController.OnMoveFinish += OnPlayerMoveFinish;

    }

    // make sure to deregister at disable time
    void OnDisable()
    {
        PlayerController.OnMoveFinish -= OnPlayerMoveFinish;
    }

    private bool _moveTowardsPlayer = false;

    public void MoveTowards()
    {
        _moveTowardsPlayer = true;
    }

    private Vector2Int GetPositionAsVector2Int()
    {
        return Vector2Int.RoundToInt(new Vector2(transform.position.x, transform.position.z));
    }

}