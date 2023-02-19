using System.Collections.Generic;
using UnityEngine;

// simple chase level that introduces player to the concept 
public class ChaseLevel3 : LevelManager
{

    private List<MovingObstacle> obstacles;

#pragma warning disable IDE0051
    void Start()
    {
#pragma warning restore IDE0051
        gridSizeX = gridSizeY = 9;
        turnLimit = 70;

        SetupLevel(gridSizeX / 2 + 4, gridSizeY / 2);
        SetTurnLimit(turnLimit);

        Waypoint[] waypointsInOrder = new[] {
            Waypoint.Of(7, 7),
            Waypoint.Of(0, 0),
            Waypoint.Of(4, gridSizeY - 1),
            Waypoint.Of(squareOne.x, squareOne.y)
        };

        gsm.SetWaypoints(waypointsInOrder);

        gsm.ManageGameState();

        obstacles = new List<MovingObstacle>();

        ObstacleController stationaryObstacle = gridController.AddStationaryObstacleAtPosition(4, 4);

        MovingObstacle obstacle = gridController.AddMovingObstacleAtPosition(2, 1);
        obstacle.MoveTowardsPlayer(playerController, gridController.GetCurrentStationaryObstaclesAction(), false);

        gridController.RemoveTileAtLocation(gridSizeX - 1, gridSizeY - 2);
        gridController.RemoveTileAtLocation(1, 3);
        gridController.RemoveTileAtLocation(3, 6);


        obstacles.Add(obstacle);
    }
}
