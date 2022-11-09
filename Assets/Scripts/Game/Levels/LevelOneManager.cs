using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LevelOneManager : MonoBehaviour
{
    public int gridSizeX, gridSizeY = 10;
    public int turnLimit = 20;

    private GameObject player;
    private GameObject cameraPivot;

    public TextMeshProUGUI moveCountText;

    public GridController gridController;
    public PlayerController playerController;
    public CameraController cameraController;

    private bool levelActive;

    private List<GameState> gameStateOrder;
    private GameState currentGameState;

    enum GameState {
        START,
        GREEN_READY,
        GREEN_HIT,
        ORANGE_READY,
        ORANGE_HIT,
        BLUE_READY,
        BLUE_HIT,
        SUCCESS
    };

    /**
    Level one is a tile painting level. 
    When the player moves once, a square one unit away from them turns green.
    When the player touches the green square, we trigger an orange and red square towards the nearest corner.
    Touching either results in a blue square back at square one. Touching blue square wins game.
    */
    void Start()
    {
        this.player = this.playerController.gameObject;

        gridController.SetupGrid(gridSizeX, gridSizeY);

        this.levelActive = true;

        int playerOffsetX = gridSizeX/2;
        int playerOffsetY = gridSizeY/2;

        playerController.SpawnPlayer(playerOffsetX, playerOffsetY);
        cameraController.CenterCameraOnOffset(playerOffsetX, playerOffsetY);
        this.SetMoveCountText();

        this.gameStateOrder = new List<GameState>();
        gameStateOrder.Add(GameState.START);
        gameStateOrder.Add(GameState.GREEN_READY);
        gameStateOrder.Add(GameState.GREEN_HIT);
        gameStateOrder.Add(GameState.ORANGE_READY);
        gameStateOrder.Add(GameState.ORANGE_HIT);
        gameStateOrder.Add(GameState.BLUE_READY);
        gameStateOrder.Add(GameState.BLUE_HIT);

        this.currentGameState = GameState.START;

        player.SetActive(true);
    }

    void Update()
    {
        // TODO: seems poorly optimized to have this here, should be be tied to player OnMove but not sure how
        // check out Delegates to get a callback from the OnMove thing
        SetMoveCountText();
        ManageGameState();

        if (this.levelActive && this.playerController.GetMoveCount() >= this.turnLimit) {
            Debug.Log("Game over!");
            this.levelActive = false;
        }
    }

    void ManageGameState() {
        Vector2 playerPos = GetRoundedPlayerPosition();
        Debug.LogFormat("Player pos: {0}", playerPos);

        switch (this.currentGameState) {
            case GameState.START:
                // welcome text or interaction?
                Debug.Log("We're in start state and transitioning");
                TransitionState();
                break;
            case GameState.GREEN_READY: 
                Debug.Log("In green ready");
                if (this.playerController.GetMoveCount() == 1) {
                    Debug.Log("Transitioning");
                    this.gridController.PaintTilesAdjacentToLocation(playerPos, Color.green);
                    TransitionState();
                }
                break;
            case GameState.GREEN_HIT:
                Debug.Log("In green hit");
                this.gridController.PaintTileAtLocation(1, 1, Color.red);
                TransitionState();
                break;
            case GameState.ORANGE_READY:
                Debug.Log("In orange ready");
                if (playerPos.x == 1 && playerPos.y == 1) {
                    TransitionState();
                }
                break;
            case GameState.ORANGE_HIT:

            default:
                // if we're on a green tile and this path hasn't been hit yet, paint an orange and red tile at the nearest corner 
                // should the line above and below be two different states? like GREEN_HIT and ORANGE_RED_READY
                // if I hit an orange or red tile, transition to blue 
                break;
        }

        // this method kinda sucks
        void TransitionState() {
            int gameStateNdx = gameStateOrder.IndexOf(this.currentGameState);
            Debug.LogFormat("transitioning state from {0} with gameStateNdx {1} out of {2}", this.currentGameState, gameStateNdx, this.gameStateOrder.Count);
            if (this.currentGameState == GameState.SUCCESS) {
                // display message, allow for input? idk. maybe I overengineered this for no reason and we can remove SUCCESS enum, or can just add it into the game state order
            }
            else if (gameStateNdx == this.gameStateOrder.Count - 1) {
                // TODO freeze input or something
                this.currentGameState = GameState.SUCCESS;
                Debug.Log("You have won the game!!");
            } else {
                this.currentGameState = this.gameStateOrder[gameStateOrder.IndexOf(this.currentGameState)+1];
            }
        }
    }

    bool IsPlayerAtPosition(int x, int z) {
        return Mathf.RoundToInt(this.player.transform.position.x) == x && Mathf.RoundToInt(this.player.transform.position.z) == z;
    }

    Vector2 GetRoundedPlayerPosition() {
        return new Vector2(Mathf.RoundToInt(this.player.transform.position.x), Mathf.RoundToInt(this.player.transform.position.z));
    }

    void SetMoveCountText() {
        this.moveCountText.text = $"Turns remaining: {this.turnLimit - this.playerController.GetMoveCount()}";
    }
}
