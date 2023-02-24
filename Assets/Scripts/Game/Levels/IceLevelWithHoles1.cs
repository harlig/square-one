using UnityEngine;

public class IceLevelWithHoles1 : LevelManager
{
#pragma warning disable IDE0051
    void Start()
#pragma warning restore IDE0051
    {
        gridSizeX = gridSizeY = 5;
        turnLimit = 30;

        SetupLevel(gridSizeX - 1, gridSizeY - 1);

        Waypoint[] waypointsInOrder = new[] {
            Waypoint.Of(0, gridSizeY-1),
            Waypoint.Of(gridSizeX-1, 1),
            Waypoint.Of(0, 0),
            Waypoint.Of(squareOne.x, squareOne.y),
        };


        gsm.SetWaypoints(waypointsInOrder);
        SetTurnLimit(turnLimit);
        gsm.ManageGameState();

        gridController.SpawnIceTile(2, 1, OnIceTileSteppedOn);
        gridController.SpawnIceTile(1, 2, OnIceTileSteppedOn);
        gridController.SpawnIceTile(3, 3, OnIceTileSteppedOn);

        gridController.RemoveTileAtLocation(gridSizeX - 2, gridSizeY - 1);
        gridController.RemoveTileAtLocation(2, 0);
    }
}