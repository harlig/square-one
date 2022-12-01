using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnakeLevel1 : LevelManager
{
    private SnakeController snakeController;

#pragma warning disable IDE0051
    void Start()
    {
        gridSizeX = gridSizeY = 8;

        SetupLevel(5, 4);

        Vector2Int[] waypointsInOrder = new[] {
            new Vector2Int(gridSizeX - 1, 2),
            new Vector2Int(0, 0),
            new Vector2Int(gridSizeY - 2, 3),
            new Vector2Int(squareOne.x, squareOne.y),
        };


        gsm.SetWaypoints(waypointsInOrder);
        gsm.SetTurnLimit(25);
        gsm.ManageGameState();

        snakeController = new SnakeController(playerController, gridController);
        snakeController.OnPaintedTileHit += OnPaintedTileHit;
    }

#pragma warning restore IDE0051


    private void OnPaintedTileHit(Vector2Int playerPos)
    {
        gsm.SetFailedState();
    }
}
