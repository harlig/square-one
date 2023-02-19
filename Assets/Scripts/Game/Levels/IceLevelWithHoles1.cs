using UnityEngine;

public class IceLevelWithHoles1 : LevelManager
{
#pragma warning disable IDE0051
    void Start()
#pragma warning restore IDE0051
    {
        gridSizeX = gridSizeY = 7;
        turnLimit = 25;

        SetupLevel();

        Waypoint[] waypointsInOrder = new[] {
            Waypoint.Of(1, 0),
            Waypoint.Of(gridSizeX-1, 1),
            Waypoint.Of(squareOne.x, squareOne.y),
        };


        gsm.SetWaypoints(waypointsInOrder);
        SetTurnLimit(turnLimit);
        gsm.ManageGameState();

        gridController.SpawnIceTile(0, 1, OnIceTileSteppedOn);
        gridController.SpawnIceTile(0, 3, OnIceTileSteppedOn);
        gridController.SpawnIceTile(0, 4, OnIceTileSteppedOn);
        gridController.SpawnIceTile(1, gridSizeY - 1, OnIceTileSteppedOn);
        gridController.SpawnIceTile(3, 1, OnIceTileSteppedOn);
        gridController.SpawnIceTile(3, 2, OnIceTileSteppedOn);
        gridController.SpawnIceTile(3, 3, OnIceTileSteppedOn);
        gridController.SpawnIceTile(2, 3, OnIceTileSteppedOn);
        gridController.SpawnIceTile(gridSizeX - 2, gridSizeY - 1, OnIceTileSteppedOn);
        gridController.SpawnIceTile(gridSizeX - 2, gridSizeY - 2, OnIceTileSteppedOn);
        gridController.SpawnIceTile(gridSizeX - 3, gridSizeY - 2, OnIceTileSteppedOn);
        gridController.SpawnIceTile(gridSizeX - 1, 3, OnIceTileSteppedOn);
        gridController.SpawnIceTile(gridSizeX - 1, 4, OnIceTileSteppedOn);
        gridController.SpawnIceTilesAroundPosition(gridSizeX - 1, 1, OnIceTileSteppedOn);

        gridController.RemoveTileAtLocation(new Vector2Int(1, 1));
        gridController.RemoveTileAtLocation(new Vector2Int(1, 2));
        gridController.RemoveTileAtLocation(new Vector2Int(2, 1));
        gridController.RemoveTileAtLocation(new Vector2Int(2, 0));
        gridController.RemoveTileAtLocation(new Vector2Int(2, 5));
        gridController.RemoveTileAtLocation(new Vector2Int(5, 4));
        gridController.RemoveTileAtLocation(new Vector2Int(4, 4));
    }
}