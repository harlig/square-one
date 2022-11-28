using System.Collections.Generic;
using UnityEngine;

// full ice level with following obstacle
public class IceLevel3 : LevelManager
{

#pragma warning disable IDE0051
    void Start()
    {
        gridSizeX = gridSizeY = 8;
        turnLimit = 17;

        SetupLevel(1, 1);

        // TODO should rotate camera?
        // TODO should rely on moving the obstacle around so you can hit it as a stopper

        Vector2Int[] waypointsInOrder = new[] {
            new Vector2Int(gridSizeX - 2, 0),
            new Vector2Int(4, gridSizeY - 5),
            new Vector2Int(0, gridSizeY - 1),
            new Vector2Int(squareOne.x, squareOne.y),
        };


        gsm.SetWaypoints(waypointsInOrder, true);
        gsm.SetTurnLimit(turnLimit);
        gsm.ManageGameState();

        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                gridController.SpawnIceTile(x, y, OnIceTileSteppedOn);
            }
        }

        gridController.AddStationaryObstacleAtPosition(5, 0);
        gridController.AddStationaryObstacleAtPosition(gridSizeX - 1, 1);
        gridController.AddStationaryObstacleAtPosition(gridSizeX - 2, 6);
        gridController.AddStationaryObstacleAtPosition(2, 5);
        gridController.AddStationaryObstacleAtPosition(4, gridSizeY);
        gridController.AddStationaryObstacleAtPosition(3, 3);
        gridController.AddStationaryObstacleAtPosition(0, 4);
        gridController.AddStationaryObstacleAtPosition(1, gridSizeY);
        gridController.AddStationaryObstacleAtPosition(squareOne.x + 1, gridSizeY - 1);
        gridController.AddStationaryObstacleAtPosition(-1, gridSizeY - 1);

        gridController.AddStationaryObstacleAtPosition(gridSizeX - 2, -1);
        gridController.AddStationaryObstacleAtPosition(-1, gridSizeY - 2);
        gridController.AddStationaryObstacleAtPosition(1, -1);
        gridController.AddStationaryObstacleAtPosition(gridSizeX - 1, 2);

        PlayerController.OnStopMoving += OnPlayerStopMoving;
    }

    bool hasPlayerStoppedMoving = false;
    void OnPlayerStopMoving()
    {
        hasPlayerStoppedMoving = true;
    }

#pragma warning restore IDE0051

    bool isProcessingMovement = false;
    override protected void OnPlayerMoveFinishWithShouldCountMove(Vector2Int playerPosition, bool shouldCountMove)
    {
        Debug.LogFormat("playerStoppedMoving {0}, isIceTile {1}, isProcessing {2}", hasPlayerStoppedMoving, gridController.IsIceTile(playerPosition.x, playerPosition.y), isProcessingMovement);
        if (!hasPlayerStoppedMoving && gridController.IsIceTile(playerPosition.x, playerPosition.y))
        {
            Debug.Log("alright we're processing");
            isProcessingMovement = true;
            return;
        }

        if (shouldCountMove || isProcessingMovement)
        {
            turnsLeft = turnLimit - playerController.GetMoveCount();
            isProcessingMovement = false;
            hasPlayerStoppedMoving = false;
        }
    }
}