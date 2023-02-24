using UnityEngine;

public class IceLevelWithHoles5 : LevelManager
{
#pragma warning disable IDE0051
    void Start()
#pragma warning restore IDE0051
    {
        gridSizeX = gridSizeY = 6;
        turnLimit = 30;

        SetupLevel(2, 3);

        Waypoint[] waypointsInOrder = new[] {
            Waypoint.Of(gridSizeX-2, gridSizeY-1),
            Waypoint.Of(0, 1),
            Waypoint.Of(0, gridSizeY-1),
            Waypoint.Of(gridSizeX-1, 0),
            Waypoint.Of(squareOne.x, squareOne.y),
        };


        gsm.SetWaypoints(waypointsInOrder);
        SetTurnLimit(turnLimit);
        gsm.ManageGameState();

        gridController.SpawnIceTile(2, 1, OnIceTileSteppedOn);
        gridController.SpawnIceTile(gridSizeX - 1, 1, OnIceTileSteppedOn);
        gridController.SpawnIceTile(3, 3, OnIceTileSteppedOn);
        gridController.SpawnIceTile(0, gridSizeY - 2, OnIceTileSteppedOn);
        gridController.SpawnIceTile(gridSizeX - 1, gridSizeY - 2, OnIceTileSteppedOn);
        gridController.SpawnIceTile(2, gridSizeY - 1, OnIceTileSteppedOn);

        gridController.RemoveTileAtLocation(1, 2);
        gridController.RemoveTileAtLocation(3, 2);
        gridController.RemoveTileAtLocation(1, 4);
        gridController.RemoveTileAtLocation(3, 4);
    }
}