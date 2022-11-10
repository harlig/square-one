using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LevelOneManager : MonoBehaviour
{
    public int gridSizeX, gridSizeY = 10;
    public int turnLimit = 20;

    private GameObject player;
    private readonly GameObject cameraPivot;

    public TextMeshProUGUI moveCountText;

    public GridController gridController;
    public PlayerController playerController;
    public CameraController cameraController;

    private bool levelActive;

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

    /**
    Level one is a tile painting level. 
    When the player moves once, a square one unit away from them turns green.
    When the player touches the green square, we trigger an orange and red square towards the nearest corner.
    Touching either results in a blue square back at square one. Touching blue square wins game.
    */
    void Start()
    {
        player = playerController.gameObject;

        gridController.SetupGrid(gridSizeX, gridSizeY);

        levelActive = true;

        int playerOffsetX = gridSizeX / 2;
        int playerOffsetY = gridSizeY / 2;

        playerController.SpawnPlayer(playerOffsetX, playerOffsetY);
        cameraController.CenterCameraOnOffset(playerOffsetX, playerOffsetY);
        SetMoveCountText();

        gameStateOrder = new List<GameState>
        {
            GameState.START,
            GameState.GREEN_SETUP,
            GameState.GREEN_HIT,
            GameState.RED_SETUP,
            GameState.RED_HIT,
            GameState.BLUE_SETUP,
            GameState.BLUE_HIT
        };

        currentGameState = GameState.START;

        player.SetActive(true);
    }

    void Update()
    {
        if (levelActive)
        {
            SetMoveCountText();
            ManageGameState();
        }

    }

    void ManageGameState()
    {
        Vector2 playerPos = GetRoundedPlayerPosition();

        // failure game states first
        if (!gridController.IsWithinGrid(playerPos))
        {
            Debug.Log("Player has exited map.");
            currentGameState = GameState.FAILED;
        }
        if (playerController.GetMoveCount() >= turnLimit)
        {
            Debug.Log("Player exceeded move count");
            currentGameState = GameState.FAILED;
        }

        switch (currentGameState)
        {
            case GameState.START:
                // welcome text or interaction?
                if (playerController.GetMoveCount() == 1)
                {
                    TransitionState();
                }
                break;
            case GameState.GREEN_SETUP:
                gridController.PaintTilesAdjacentToLocation(playerPos, Color.green);
                TransitionState();
                break;
            case GameState.GREEN_HIT:
                if (gridController.TileColorAtLocation(playerPos) == Color.green)
                {
                    TransitionState();
                }
                break;
            case GameState.RED_SETUP:
                gridController.PaintTileAtLocation(1, 1, Color.red);
                TransitionState();
                break;
            case GameState.RED_HIT:
                if (gridController.TileColorAtLocation(playerPos) == Color.red)
                {
                    TransitionState();
                }
                break;
            case GameState.BLUE_SETUP:
                gridController.PaintTileAtLocation(3, 8, Color.blue);
                TransitionState();
                break;
            case GameState.BLUE_HIT:
                if (gridController.TileColorAtLocation(playerPos) == Color.red)
                {
                    TransitionState();
                }
                break;
            case GameState.FAILED:
                Debug.Log("Player has failed.");
                levelActive = false;
                break;
            default:
                // if we're on a green tile and this path hasn't been hit yet, paint an orange and red tile at the nearest corner 
                // should the line above and below be two different states? like GREEN_HIT and ORANGE_RED_READY
                // if I hit an orange or red tile, transition to blue 
                break;
        }

        // this method kinda sucks
        void TransitionState()
        {
            int gameStateNdx = gameStateOrder.IndexOf(currentGameState);
            Debug.LogFormat("transitioning state from {0} with gameStateNdx {1} out of {2}", currentGameState, gameStateNdx, gameStateOrder.Count);
            if (gameStateNdx == gameStateOrder.Count - 1)
            {
                currentGameState = GameState.SUCCESS;
                Debug.Log("You have won the game!!");
            }
            else
            {
                currentGameState = gameStateOrder[gameStateOrder.IndexOf(currentGameState) + 1];
            }
        }
    }

    Vector2 GetRoundedPlayerPosition()
    {
        return new Vector2(Mathf.RoundToInt(player.transform.position.x), Mathf.RoundToInt(player.transform.position.z));
    }

    void SetMoveCountText()
    {
        moveCountText.text = $"Turns remaining: {turnLimit - playerController.GetMoveCount()}";
    }
}
