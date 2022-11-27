using System.Collections.Generic;
using UnityEngine;

// player has to complete tasks while keeping moving obstacle away
public class ChaseLevel2 : LevelManager
{
    private List<GameState> gameStateOrder;
    private GameState currentGameState;

    enum GameState
    {
        START,
        GREEN_SETUP,
        GREEN_HIT,
        RED_SETUP,
        RED_HIT,
        BLUE_SETUP,
        BLUE_HIT,
        SUCCESS,
        FAILED,
    };

#pragma warning disable IDE0051
    void Start()
    {
        gridSizeX = gridSizeY = 13;
        turnLimit = 70;

        gameStateOrder = new List<GameState>
        {
            GameState.START,
            GameState.GREEN_SETUP,
            GameState.GREEN_HIT,
            GameState.RED_SETUP,
            GameState.RED_HIT,
            GameState.BLUE_SETUP,
            GameState.BLUE_HIT,
            GameState.SUCCESS
        };

        SetupLevel(1, 1);

        obstacles = new List<MovingObstacle>();

        for (int x = 0; x < gridSizeX; x++)
        {
            if (x > 3 && x < 7) continue;
            gridController.AddStationaryObstacleAtPosition(x, gridSizeY - 2);
        }

        for (int x = 0; x < gridSizeX; x++)
        {
            if (x > 2 && x < 5 || x == gridSizeX - 1) continue;
            gridController.AddStationaryObstacleAtPosition(x, gridSizeY - 4);
        }

        for (int y = 2; y < 5; y++)
        {
            gridController.AddStationaryObstacleAtPosition(gridSizeX - 3, y);
        }

        gridController.AddStationaryObstacleAtPosition(2, 3);
        gridController.AddStationaryObstacleAtPosition(3, 3);
        gridController.AddStationaryObstacleAtPosition(3, 4);

        waypoints = new() {
            new Vector2Int(gridSizeX - 1, gridSizeY - 5),
            new Vector2Int(gridSizeX - 1, 2),
            new Vector2Int(squareOne.x, squareOne.y),
        };
        SpawnNextWaypoint(waypoints);

        // TODO should setup level with waypoints

        MovingObstacle followerUpperLeft = gridController.AddMovingObstacleAtPosition(2, gridSizeY - 1);
        followerUpperLeft.MoveTowardsPlayer(playerController, gridController.GetCurrentStationaryObstaclesAction(), false);

        MovingObstacle followerLowerRight = gridController.AddMovingObstacleAtPosition(gridSizeX - 2, 3);
        followerLowerRight.MoveTowardsPlayer(playerController, gridController.GetCurrentStationaryObstaclesAction(), false);

        // can maybe use disable button as bait?
        // gridController.PaintTileAtLocation(new Vector2Int(1, gridSizeY - 2), Color.white);

        currentGameState = GameState.START;
    }

    void Update()
    {
        SetMoveCountText();
        if (levelActive)
        {
            ManageGameState();
        }
    }
#pragma warning disable IDE0051

    override protected void OnPlayerMoveFinish(Vector2Int playerPosition)
    {
        if (levelActive)
        {
            turnsLeft = turnLimit - playerController.GetMoveCount();
        }
    }

    void ManageGameState()
    {
        Vector2Int playerPos = playerController.GetCurrentPosition();

        // allow devMode to not fall out of map
        if (!gridController.IsWithinGrid(playerPos))
        {
            Debug.Log("Player has exited map.");
            currentGameState = GameState.FAILED;
        }

        // TODO comment back in for button to disable moving obstacles
        // white tile color disables all moving obstacles
        // if (gridController.TileColorAtLocation(playerPos) == Color.white)
        // {
        //     Debug.Log("Stopping obstacle movement!");
        //     foreach (MovingObstacle obstacle in obstacles)
        //     {
        //         obstacle.StopMovement();
        //     }
        // }

        // game state handler
        switch (currentGameState)
        {
            case GameState.START:
                // secret location? kinda shitty to make player guess and check
                if (playerPos.x == 9 && playerPos.y == 9)
                {
                    TransitionState();
                }
                break;
            case GameState.GREEN_SETUP:
                gridController.PaintTileAtLocation(0, 0, Color.green);
                TransitionState();
                break;
            case GameState.GREEN_HIT:
                if (gridController.TileColorAtLocation(playerPos) == Color.green)
                {
                    TransitionState();
                }
                break;
            case GameState.RED_SETUP:
                gridController.PaintTileAtLocation(4, gridSizeY - 1, Color.red);
                TransitionState();
                break;
            case GameState.RED_HIT:
                if (gridController.TileColorAtLocation(playerPos) == Color.red)
                {
                    TransitionState();
                }
                break;
            case GameState.BLUE_SETUP:
                // last step is back to square one
                gridController.PaintTileAtLocation(squareOne.x, squareOne.y, Color.blue);
                TransitionState();
                break;
            case GameState.BLUE_HIT:
                if (gridController.TileColorAtLocation(playerPos) == Color.blue)
                {
                    TransitionState();
                    // you must manage game state here before falling through, otherwise you could be transitioning
                    // into a success game state when you're at zero turns!
                    ManageGameState();
                }
                break;
            case GameState.SUCCESS:
                Debug.Log("Player has won!");
                SetTerminalGameState(successElements);
                break;
            case GameState.FAILED:
                Debug.Log("Player has failed.");
                SetTerminalGameState(failedElements);
                break;
            default:
                Debug.LogErrorFormat("Encountered unexpected game state: {0}", currentGameState);
                break;
        }

        if (currentGameState != GameState.SUCCESS && turnsLeft <= 0)
        {
            Debug.Log("Player exceeded move count");
            currentGameState = GameState.FAILED;
        }

        void TransitionState()
        {
            // could probably use a better data structure as the state machine that allows a failure state as defined by the state machine
            currentGameState = gameStateOrder[gameStateOrder.IndexOf(currentGameState) + 1];
        }
    }
}
