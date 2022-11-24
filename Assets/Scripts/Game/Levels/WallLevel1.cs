using System.Collections.Generic;
using UnityEngine;

public class WallLevel1 : LevelManager
{
    private List<GameState> gameStateOrder;
    private GameState currentGameState;

    private List<ObstacleController> obstacles;

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

    void Start()
    {
        gridSizeX = gridSizeY = 6;
        turnLimit = 10;

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

        SetupLevel(2, 3);

        obstacles = new();
        for (int ndx = 0; ndx < gridSizeY; ndx++)
        {
            obstacles.Add(gridController.AddObstacleAtPosition(1, ndx));
        }

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

    override protected void OnPlayerMoveFinish(Vector2Int playerPosition)
    {
        if (levelActive && playerController.ShouldCountMoves())
        {
            turnsLeft--;
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

        // TODO let's make this possible to be in both RED_HIT and BLUE_HIT at the same time
        // game state handler
        switch (currentGameState)
        {
            case GameState.START:
                TransitionState();
                break;
            case GameState.GREEN_SETUP:
                gridController.PaintTileAtLocation(gridSizeX - 1, gridSizeY - 2, Color.green);
                TransitionState();
                break;
            case GameState.GREEN_HIT:
                if (gridController.TileColorAtLocation(playerPos) == Color.green)
                {
                    MoveObstacles(Vector3.right);
                    TransitionState();
                }
                break;
            case GameState.RED_SETUP:
                MoveObstacles(Vector3.right);
                gridController.PaintTileAtLocation(squareOne.x, squareOne.y, Color.blue);
                gridController.PaintTileAtLocation(gridSizeX - 2, gridSizeY - 1, Color.red);
                TransitionState();
                break;
            case GameState.RED_HIT:
                if (gridController.TileColorAtLocation(playerPos) == Color.red)
                {
                    gridController.SpawnIceTile(gridSizeX - 2, gridSizeY - 2, OnIceTileSteppedOn);
                    gridController.SpawnIceTile(gridSizeX - 2, gridSizeY - 3, OnIceTileSteppedOn);
                    gridController.SpawnIceTile(gridSizeX - 2, gridSizeY - 4, OnIceTileSteppedOn);
                    gridController.SpawnIceTile(gridSizeX - 2, gridSizeY - 5, OnIceTileSteppedOn);
                    TransitionState();
                }
                break;
            case GameState.BLUE_SETUP:
                // last step is back to square one
                TransitionState();
                break;
            case GameState.BLUE_HIT:
                if (gridController.TileColorAtLocation(playerPos) == Color.blue)
                {
                    TransitionState();
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

        if (turnsLeft <= 0)
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

    void MoveObstacles(Vector3 direction)
    {
        foreach (ObstacleController obstacle in obstacles)
        {
            if (Mathf.RoundToInt(obstacle.transform.position.z) == 0) continue;

            obstacle.MoveInDirection(direction);
        }
    }
}