using System.Collections.Generic;
using UnityEngine;

public class IceLevel1 : LevelManager
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

        SetupLevel();

        gridController.SpawnIceTilesAroundPosition(gridSizeX - 1, 2, OnIceTileSteppedOn);
        // gridController.SpawnIceTile(gridSizeX - 1, 3, OnIceTileSteppedOn);
        // gridController.SpawnIceTile(gridSizeX - 1, 1, OnIceTileSteppedOn);
        // gridController.SpawnIceTile(gridSizeX - 2, 3, OnIceTileSteppedOn);
        // gridController.SpawnIceTile(gridSizeX - 2, 2, OnIceTileSteppedOn);
        // gridController.SpawnIceTile(gridSizeX - 2, 1, OnIceTileSteppedOn);

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
        ManageGameState();

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

        // game state handler
        switch (currentGameState)
        {
            case GameState.START:
                TransitionState();
                break;
            case GameState.GREEN_SETUP:
                gridController.PaintTileAtLocation(gridSizeX - 1, 2, Color.green);
                TransitionState();
                break;
            case GameState.GREEN_HIT:
                if (gridController.TileColorAtLocation(playerPos) == Color.green)
                {
                    TransitionState();
                }
                break;
            case GameState.RED_SETUP:
                gridController.PaintTileAtLocation(1, 4, Color.red);
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
}