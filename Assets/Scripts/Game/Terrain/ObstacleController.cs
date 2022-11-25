using System;
using System.Collections;
using UnityEngine;

// TODO split into StationaryObstacle and MovingObstacle
public class ObstacleController : MonoBehaviour
{
    private enum MoveDirection
    {
        TOWARDS_START_POSITION,
        TOWARDS_PATROL_POSITION
    }

    public void SetName(string name)
    {
        gameObject.name = name;
    }

    private Vector2Int spawnPosition;
    private bool _isPatrolling;
    private Cube Cube { get; set; }

    public void StartPatrolling(Vector2Int patrolPosition)
    {
        spawnPosition = GetPositionAsVector2Int();

        StartCoroutine(MoveObstacleOnPatrolCourse(MoveDirection.TOWARDS_PATROL_POSITION));

        // this is bound to go out of the map but I can't be bothered to deal with that right now
        IEnumerator MoveObstacleOnPatrolCourse(MoveDirection moveDirection)
        {
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
                throw new Exception("Unexpected move direction!");
            }

            _isPatrolling = true;
            StartCoroutine(ChangePatrolDirectionWhenDonePatrolling(moveDirection));

            Vector2Int curPosition = GetPositionAsVector2Int();
            MoveTowardsPosition(out int xDiff, out int yDiff, curPosition, endPosition);

            while (xDiff != 0 || yDiff != 0)
            {
                // should maybe wait a second or two? we need to let the obstacle finish rolling too?
                yield return null;

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
                MoveTowardsPosition(out xDiff, out yDiff, curPosition, endPosition);
            }
            _isPatrolling = false;
        }

        IEnumerator ChangePatrolDirectionWhenDonePatrolling(MoveDirection oldMoveDirection)
        {
            while (_isPatrolling)
            {
                // can modify to determine how long should rest at the final position before switching directions
                yield return new WaitForSeconds(0.5f);
            }
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
                throw new Exception("Unexpected move direction!");
            }

        }
    }

    public void MoveInDirectionIfNotMovingAndDontEnqueue(Vector3 direction)
    {
        Cube.MoveInDirectionIfNotMovingAndDontEnqueue(direction, _moveType);
    }

    // obstacles always roll
    private static readonly Cube.MoveType _moveType = Cube.MoveType.ROLL;

    void MoveTowardsPosition(out int xDiff, out int yDiff, Vector2Int curPosition, Vector2Int endPosition)
    {
        xDiff = Mathf.Abs(curPosition.x - endPosition.x);
        yDiff = Mathf.Abs(curPosition.y - endPosition.y);

        if (xDiff == 0 && yDiff == 0) return;

        // if bigger x deficit, move that way first
        if (xDiff > yDiff)
        {
            if (curPosition.x > endPosition.x)
            {
                Cube.MoveInDirectionIfNotMoving(Vector3.left, _moveType, false);
            }
            else
            {
                Cube.MoveInDirectionIfNotMoving(Vector3.right, _moveType, false);
            }
        }
        // TODO, add this back in and think of a more explicit solution for when xDiff and yDiff are equivalent
        // else if (xDiff < yDiff)
        else
        {
            {
                if (curPosition.y > endPosition.y)
                {
                    Cube.MoveInDirectionIfNotMoving(Vector3.back, _moveType, false);
                }
                else
                {
                    Cube.MoveInDirectionIfNotMoving(Vector3.forward, _moveType, false);
                }
            }

        }
    }

    void OnPlayerMoveFinish(Vector2Int playerPosition)
    {
        if (_moveTowardsPlayer)
        {
            Debug.Log("Moving towards player");
            // move one unit torwads player's position
            MoveTowardsPosition(out _, out _, GetPositionAsVector2Int(), _playerController.GetCurrentPosition());

        }
    }

    public void SetAfterRollAction(Action<bool, bool> afterRoll)
    {
        Cube.SetAfterRollAction(afterRoll);
    }

    void Awake()
    {
        // no need for any before/after roll actions right now
        Cube = new(this, 1.0f, () => { }, (_, _) => { });
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
    private PlayerController _playerController;

    public void MoveTowardsPlayer(PlayerController playerController)
    {
        Cube.SetRollSpeed(playerController.GetRollSpeed());
        _moveTowardsPlayer = true;
        _playerController = playerController;
    }

    // TODO is this only applicable for _moveTowardsPlayer?
    public void StopMovement()
    {
        _moveTowardsPlayer = false;
    }

    private Vector2Int GetPositionAsVector2Int()
    {
        return Vector2Int.RoundToInt(new Vector2(transform.position.x, transform.position.z));
    }

    void OnTriggerEnter(Collider other)
    {
        if (!PlayerController.IsColliderPlayer(other)) return;

        PlayerController playerController = other.GetComponent<PlayerController>();
        playerController.StartUpdatingLocation();
        playerController.StopMoving();
    }

    void OnTriggerExit(Collider other)
    {
        if (!PlayerController.IsColliderPlayer(other)) return;

        PlayerController playerController = other.GetComponent<PlayerController>();
        playerController.StartMoving();
    }
}