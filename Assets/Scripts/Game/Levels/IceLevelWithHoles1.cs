using UnityEngine;

// simple level which introduces player to ice
public class IceLevelWithHoles1 : LevelManager
{
#pragma warning disable IDE0051
    void Start()
    {
        gridSizeX = gridSizeY = 6;
        turnLimit = 15;

        SetupLevel(5, 4);

        Waypoint[] waypointsInOrder = new[] {
            Waypoint.Of(gridSizeX - 1, 2),
            Waypoint.Of(0, 0),
            Waypoint.Of(squareOne.x, squareOne.y),
        };


        gsm.SetWaypoints(waypointsInOrder);
        SetTurnLimit(turnLimit);
        gsm.ManageGameState();

        gridController.SpawnIceTilesAroundPosition(waypointsInOrder[0].Position.x, waypointsInOrder[0].Position.y, OnIceTileSteppedOn);
        gridController.SpawnIceTile(3, 4, OnIceTileSteppedOn);
        gridController.SpawnIceTile(3, 3, OnIceTileSteppedOn);
        gridController.SpawnIceTile(1, 5, OnIceTileSteppedOn);
        gridController.SpawnIceTile(2, 0, OnIceTileSteppedOn);
        gridController.SpawnIceTile(3, 0, OnIceTileSteppedOn);

        gridController.RemoveTileAtLocation(new Vector2Int(1, 1));
    }

#pragma warning restore IDE0051
}