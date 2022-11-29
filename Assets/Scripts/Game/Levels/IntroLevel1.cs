using UnityEngine;

// introduces player to game controls
public class IntroLevel1 : LevelManager
{
    void Start()
    {
        gridSizeX = gridSizeY = 6;
        turnLimit = 30;

        SetupLevel();

        Waypoint[] waypointPositionsInOrder = new[] {
            Waypoint.Of(playerController.GetCurrentPosition().x, playerController.GetCurrentPosition().y)
            .WithOnTriggeredAction(() => {
                Debug.Log("hit me right when you spawned now, did ya??");
                gsm.SpawnNextWaypoint();
            }),
            Waypoint.Of(gridSizeX - 1, gridSizeY - 2)
            .WithOnTriggeredAction(() => {
                gsm.SpawnNextWaypoint();
            }),
            Waypoint.Of(gridSizeX - 2, gridSizeY - 1)
            .WithOnTriggeredAction(() => {
                gsm.SpawnNextWaypoint();
            }),
            Waypoint.Of(squareOne.x, squareOne.y)
        };

        gsm.SetWaypoints(waypointPositionsInOrder);

        gsm.ManageGameState();
    }

    // TODO should move this to LevelManager?
    override protected void OnPlayerMoveFullyCompleted(Vector2Int playerPosition, bool shouldCountMove)
    {
        if (shouldCountMove)
        {
            turnsLeft = turnLimit - playerController.GetMoveCount();

        }
    }
}