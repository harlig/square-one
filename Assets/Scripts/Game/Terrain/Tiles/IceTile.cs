using UnityEngine;

public class IceTile : TileController
{
    void OnPlayerMoveStart(Vector2Int playerPosition)
    {
        // store player's position, will be used to determine in which direction they moved
    }

    // TODO figure out how to get player's position here
    void OnPlayerMoveFinish(Vector2Int playerPosition)
    {
        // if player is on top of me, move them one unit in the same direction they moved from their prev position (in MoveStart) to now
        Debug.LogFormat("We are in the ice tile and here is player's position: {0}!", playerPosition);
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
