using UnityEngine;

public class IceTile : TileController
{
    public void YepWeInHere()
    {
        Debug.Log("We are in the ice tile!");
    }

    void OnPlayerMoveStart()
    {
        // store player's position, will be used to determine in which direction they moved
    }

    void OnPlayerMoveFinish()
    {
        // if player is on top of me, move them one unit in the same direction they moved from their prev position (in MoveStart) to now
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
