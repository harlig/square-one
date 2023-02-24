using UnityEngine;

public class IceLevelWithHoles4 : LevelManager
{
#pragma warning disable IDE0051
    void Start()
#pragma warning restore IDE0051
    {
        gridSizeX = gridSizeY = 6;
        turnLimit = 25;

        SetupLevel();

        Waypoint[] waypointsInOrder = new[] {
            Waypoint.Of(2, gridSizeY-1),
            Waypoint.Of(0, 2),
            Waypoint.Of(squareOne.x, squareOne.y),
        };


        gsm.SetWaypoints(waypointsInOrder);
        SetTurnLimit(turnLimit);
        gsm.ManageGameState();

        gridController.SpawnIceTile(0, 1, OnIceTileSteppedOn);
        gridController.SpawnIceTile(1, 4, OnIceTileSteppedOn);
        gridController.SpawnIceTile(1, 3, OnIceTileSteppedOn);
        gridController.SpawnIceTile(1, 2, OnIceTileSteppedOn);
        gridController.SpawnIceTile(0, 3, OnIceTileSteppedOn);
        gridController.SpawnIceTile(1, 0, OnIceTileSteppedOn);
        gridController.SpawnIceTile(2, 0, OnIceTileSteppedOn);
        gridController.SpawnIceTile(3, 2, OnIceTileSteppedOn);
        gridController.SpawnIceTile(3, 3, OnIceTileSteppedOn);
        gridController.SpawnIceTile(3, 4, OnIceTileSteppedOn);
        gridController.SpawnIceTile(3, 5, OnIceTileSteppedOn);
        gridController.SpawnIceTile(4, 1, OnIceTileSteppedOn);
        gridController.SpawnIceTile(5, 2, OnIceTileSteppedOn);
        gridController.SpawnIceTile(5, 4, OnIceTileSteppedOn);

        gridController.RemoveTileAtLocation(new Vector2Int(1, 1));
        gridController.RemoveTileAtLocation(new Vector2Int(2, 1));
        gridController.RemoveTileAtLocation(new Vector2Int(2, 2));
        gridController.RemoveTileAtLocation(new Vector2Int(2, 3));
        gridController.RemoveTileAtLocation(new Vector2Int(2, 4));
        gridController.RemoveTileAtLocation(new Vector2Int(0, 4));
        gridController.RemoveTileAtLocation(new Vector2Int(4, 4));
        gridController.RemoveTileAtLocation(new Vector2Int(4, 3));
        gridController.RemoveTileAtLocation(new Vector2Int(4, 2));
        gridController.RemoveTileAtLocation(new Vector2Int(4, 0));
    }
}