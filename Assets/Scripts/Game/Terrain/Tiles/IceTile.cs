using UnityEngine;

public class IceTile : TileController
{
    private Vector2Int _beforeMovePosition;

    public delegate void SteppedOnAction(Vector3Int direction);
    public event SteppedOnAction WhenSteppedOn;

    void OnPlayerMoveStart(Vector2Int playerPosition)
    {
        // store player's position, will be used to determine in which direction they moved
        _beforeMovePosition = playerPosition;
    }

    // TODO figure out how to get player's position here
    void OnPlayerMoveFinish(Vector2Int playerPosition)
    {
        // if player is on top of me, move them one unit in the same direction they moved from their prev position (in MoveStart) to now
        Debug.LogFormat("We are in the ice tile and here is player's position: {0}!", playerPosition);

        Vector3Int direction;

        if (playerPosition.x != _beforeMovePosition.x)
        {
            if (playerPosition.x > _beforeMovePosition.x)
            {
                direction = Vector3Int.right;
            }
            else
            {
                direction = Vector3Int.left;
            }

        }
        else
        {
            if (playerPosition.y > _beforeMovePosition.y)
            {
                direction = Vector3Int.forward;
            }
            else
            {
                direction = Vector3Int.back;
            }

        }

        if (playerPosition == GetPosition())
        {
            WhenSteppedOn?.Invoke(direction);
        }
    }

    void OnEnable()
    {
        PlayerController.OnMoveStart += OnPlayerMoveStart;
        PlayerController.OnMoveFinish += OnPlayerMoveFinish;
    }

    void OnDisable()
    {
        PlayerController.OnMoveStart -= OnPlayerMoveStart;
        PlayerController.OnMoveFinish -= OnPlayerMoveFinish;
    }
}
