using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LevelOneManager : MonoBehaviour
{
    public int gridSizeX, gridSizeY = 10;
    public int turnLimit = 20;

    public bool devMode = false;

    private GameObject player;

    public TextMeshProUGUI moveCountText;

    public GameObject successElements;
    public GameObject failedElements;

    public GridController gridController;
    public PlayerController playerController;
    public CameraController cameraController;
    public ResetPlaneController resetPlaneController;

    private bool levelActive;

    private List<GameState> gameStateOrder;
    private GameState currentGameState;

    private Vector2Int squareOne;

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

        squareOne = new(playerOffsetX, playerOffsetY);

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
            GameState.BLUE_HIT,
            GameState.SUCCESS
        };

        currentGameState = GameState.START;

        player.SetActive(true);
    }

    void Update()
    {
        SetMoveCountText();
        if (levelActive)
        {
            ManageGameState();
        }
    }

    void ManageGameState()
    {
        Vector2Int playerPos = GetRoundedPlayerPosition();

        // allow devMode to not fall out of map
        if (!devMode && !gridController.IsWithinGrid(playerPos))
        {
            Debug.Log("Player has exited map.");
            currentGameState = GameState.FAILED;
        }
        if (playerController.GetMoveCount() >= turnLimit)
        {
            Debug.Log("Player exceeded move count");
            currentGameState = GameState.FAILED;
        }

        // game state handler
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

        /**
         handles setting the game to SUCCESS or FAILED
         ideas included below
        */
        // set back to square one text
        // stop input from this script, now we should spawn a NextGamePortal and head there
        // also spawn a plane below you which can reset you into middle of map if you fall off at this point
        void SetTerminalGameState(GameObject textElementToEnable)
        {
            levelActive = false;
            StartCoroutine(SetElementAfterDelay(textElementToEnable));

            IEnumerator SetElementAfterDelay(GameObject element)
            {
                yield return new WaitForSeconds(0.3f);
                element.SetActive(true);
                yield return null;
                // I'd like this to be up with setting levelActive, but it has to be at the bottom of this coroutine to guarantee 
                // we count the last move the player makes before entering a terminal game state. Since the player's move isn't counted
                // until the end of the player move's roll, we can't tell it to stop counting before then. But this is a hack and 
                // race condition so I'm gonna try to use a delegate
                playerController.StopCountingMoves();
            }
        }

        void TransitionState()
        {
            // could probably use a better data structure as the state machine that allows a failure state as defined by the state machine
            currentGameState = gameStateOrder[gameStateOrder.IndexOf(currentGameState) + 1];
        }
    }

    Vector2Int GetRoundedPlayerPosition()
    {
        return new Vector2Int(Mathf.RoundToInt(player.transform.position.x), Mathf.RoundToInt(player.transform.position.z));
    }

    void SetMoveCountText()
    {
        moveCountText.text = $"Turns remaining: {turnLimit - playerController.GetMoveCount()}";
    }
}
