using UnityEngine;

public class IceLevelWithHoles3 : LevelManager
{
#pragma warning disable IDE0051
    void Start()
#pragma warning restore IDE0051
    {
        gridSizeX = gridSizeY = 7;
        turnLimit = 40;

        SetupLevel(gridSizeX - 2, 0);

        Waypoint[] waypointsInOrder = new[] {
            Waypoint.Of(1, gridSizeY-1),
            Waypoint.Of(gridSizeX-2, 3),
            Waypoint.Of(gridSizeX-1, gridSizeY-1),
            Waypoint.Of(0, 1),
            Waypoint.Of(squareOne.x, squareOne.y),
        };


        gsm.SetWaypoints(waypointsInOrder);
        SetTurnLimit(turnLimit);
        gsm.ManageGameState();

        gridController.SpawnIceTile(1, 0, OnIceTileSteppedOn);
        gridController.SpawnIceTile(2, 0, OnIceTileSteppedOn);
        gridController.SpawnIceTile(0, 2, OnIceTileSteppedOn);
        gridController.SpawnIceTile(gridSizeX - 2, 1, OnIceTileSteppedOn);
        gridController.SpawnIceTile(gridSizeX - 2, 2, OnIceTileSteppedOn);
        gridController.SpawnIceTile(0, 2, OnIceTileSteppedOn);
        gridController.SpawnIceTile(gridSizeX - 1, 2, OnIceTileSteppedOn);
        gridController.SpawnIceTile(0, gridSizeY - 3, OnIceTileSteppedOn);
        gridController.SpawnIceTile(gridSizeX - 1, gridSizeY - 3, OnIceTileSteppedOn);
        gridController.SpawnIceTile(2, gridSizeY - 1, OnIceTileSteppedOn);
        gridController.SpawnIceTile(3, gridSizeY - 3, OnIceTileSteppedOn);
        gridController.SpawnIceTile(4, gridSizeY - 3, OnIceTileSteppedOn);
        gridController.SpawnIceTile(4, gridSizeY - 1, OnIceTileSteppedOn);
        gridController.SpawnIceTile(gridSizeX - 1, gridSizeY - 2, OnIceTileSteppedOn);

        gridController.RemoveTileAtLocation(1, 1);
        gridController.RemoveTileAtLocation(3, 2);
        gridController.RemoveTileAtLocation(4, 3);
        gridController.RemoveTileAtLocation(3, gridSizeY - 2);
        gridController.RemoveTileAtLocation(3, 3);
        gridController.RemoveTileAtLocation(0, gridSizeY - 2);
    }
}