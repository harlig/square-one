using System.Collections;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using UnityEngine;

public abstract class LevelManager : MonoBehaviour
{
    protected GridController gridController;
    protected PlayerController playerController;
    protected CameraController cameraController;
    protected LevelUIElements levelUIElements;

    protected int gridSizeX, gridSizeY;

    protected Vector2Int squareOne;
    protected bool levelActive;

    protected LevelStateManager gsm;

    protected int turnLimit, turnsLeft;

    protected void SetupLevel()
    {
        int playerOffsetX = gridSizeX / 2;
        int playerOffsetY = gridSizeY / 2;

        SetupLevel(playerOffsetX, playerOffsetY);
    }

    // TODO should have a way to setup pre-setup and post-setup commands for children. children shouldn't have to call Start
    // add like an interface for them to implement
    protected void SetupLevel(int playerOffsetX, int playerOffsetY)
    {
        playerController = (PlayerController)PlayerController.Instance;
        gridController = (GridController)GridController.Instance;
        cameraController = (CameraController)CameraController.Instance;
        levelUIElements = (LevelUIElements)LevelUIElements.Instance;

        gridController.SetupGrid(gridSizeX, gridSizeY);

        playerController.SpawnPlayer(playerOffsetX, playerOffsetY, (x, y) => gridController.TileWillMovePlayer(x, y));
        playerController.gameObject.SetActive(true);

        cameraController.CenterCameraOnOffset(gridSizeX / 2.0f, gridSizeY / 2.0f);

        squareOne = new(playerOffsetX, playerOffsetY);

        turnsLeft = turnLimit;

        SetMoveCountText();

        levelActive = true;

        gsm = new LevelStateManager(playerController, gridController);

        gsm.OnStateChange += OnStageChange;
        LevelUIElements.OnTogglePause += TogglePause;


        // this is not an ideal time for this but we need to make sure the level UI elements have been awakened, so can't do this in OnEnable
        if (GameManager.Instance.allLevelsSaveData.levelNameToSaveData.ContainsKey(GetType().Name))
        {
            var prevBest = GameManager.Instance.allLevelsSaveData.levelNameToSaveData[GetType().Name].numStars;

            string starsText;
            if (prevBest == 1)
            {
                starsText = "1 star";
            }
            else
            {
                starsText = $"{prevBest} stars";
            }
            Debug.LogFormat("Level UI Elements: {0}", levelUIElements);
            levelUIElements.SetPreviousBestStarsText($"Prev best: {starsText}");
            levelUIElements.EnablePreviousBestStarsText();
        }
    }

    /**
        handles setting the game to SUCCESS or FAILED
    */
    private void SetTerminalGameState(GameObject textElementToEnable)
    {
        SetTerminalGameState(textElementToEnable, 0.2f);
    }

    /**
        handles setting the game to SUCCESS or FAILED with a variable waitDelaySeconds
    */
    private void SetTerminalGameState(GameObject textElementToEnable, float waitDelaySeconds)
    {
        levelActive = false;
        playerController.EnterTerminalGameState();

        StartCoroutine(SetElementAfterDelay(textElementToEnable, waitDelaySeconds));

        static IEnumerator SetElementAfterDelay(GameObject element, float waitDelaySeconds)
        {
            yield return new WaitForSeconds(waitDelaySeconds);
            element.SetActive(true);
        }
    }

    protected void SetTurnLimit(int turnLimit)
    {
        playerController.ResetMoveCount();
        this.turnLimit = turnLimit;
        turnsLeft = turnLimit;
        gsm.SetTurnLimit(turnLimit);
    }

    protected void SetMoveCountText()
    {
        if (gsm != null && gsm.TurnLimitEnabled)
        {
            levelUIElements.EnableMoveCountText();
            levelUIElements.SetMoveCountText($"Turns remaining: {turnsLeft}");
        }
    }

    // handle player movement. override in child classes if they want to access these events
    // prefer to use OnPlayerMoveStart unless you need specific behavior at the end of the movement
    protected virtual void OnPlayerMoveStart(Vector2Int playerPositionBeforeMove) { }
    protected virtual void OnPlayerMoveFinish(Vector2Int playerPositionAfterMove) { }

    protected virtual void OnPlayerMoveFullyCompleted(Vector2Int playerPositionAfterMove, bool shouldCountMove)
    {
        UpdateTurnsLeft(shouldCountMove);
    }

    protected void UpdateTurnsLeft(bool shouldCountMove)
    {
        // TODO do we need to check if shouldCountMove? Shouldn't player move count be accurate?
        if (shouldCountMove)
        {
            turnsLeft = turnLimit - playerController.GetMoveCount();
        }
    }

    // using this doesn't guarantee that a move has finished, so you get no access to know if the move should count
    private void OnPlayerMoveFinish(Vector2Int playerPositionAfterMove, bool shouldCountMove)
    {
        if (shouldCountMove)
        {
            AudioController.Instance.PlayMoveAudio();

        }
        OnPlayerMoveFinish(playerPositionAfterMove);
    }

#pragma warning disable IDE0051
    // must be done at object enable time
    void OnEnable()
    {
        PlayerController.OnMoveStart += OnPlayerMoveStart;
        PlayerController.OnSingleMoveFinish += OnPlayerMoveFinish;
        PlayerController.OnMoveFullyCompleted += OnPlayerMoveFullyCompleted;
    }

    // make sure to deregister at disable time
    void OnDisable()
    {
        PlayerController.OnMoveStart -= OnPlayerMoveStart;
        PlayerController.OnSingleMoveFinish -= OnPlayerMoveFinish;
        PlayerController.OnMoveFullyCompleted -= OnPlayerMoveFullyCompleted;

        gsm.OnStateChange -= OnStageChange;
    }

    void Update()
    {
        SetMoveCountText();
        gsm.CheckPlayerState();
    }
#pragma warning restore IDE0051

    private void TogglePause(bool isPaused)
    {
        playerController.MovementEnabled = !isPaused;
        cameraController.RotationEnabled = !isPaused;
    }

    protected void OnIceTileSteppedOn(Vector3Int direction)
    {
        playerController.ForceMoveInDirection(direction);
    }

    private void OnStageChange(LevelStateManager.GameState state)
    {
        if (state == LevelStateManager.GameState.FAILED)
        {
            SetTerminalGameState(levelUIElements.GetFailedElements());
            AudioController.Instance.PlayLoseAudio();

        }
        else if (state == LevelStateManager.GameState.SUCCESS)
        {
            OnLevelSuccess();
        }
    }

    private void OnLevelSuccess()
    {
        int numStars = GetStarsForVictory(playerController.GetMoveCount());
        levelUIElements.SetSuccessElementsStarsAchieved(numStars);

        Debug.Log("Writing a save file");

        GameManager.AllLevelsSaveData.LevelSaveData levelSaveData = new(GetType().Name, numStars);
        GameManager.Instance.allLevelsSaveData.SaveLevelData(levelSaveData);

        SetTerminalGameState(levelUIElements.GetSuccessElements());
        AudioController.Instance.PlayWinAudio();
    }

    private int GetStarsForVictory(int numTurnsTakenToCompleteLevel)
    {
        Debug.LogFormat("Took {0} turns to complete when turn limit is {1}", numTurnsTakenToCompleteLevel, turnLimit);
        if (numTurnsTakenToCompleteLevel <= turnLimit - 5)
        {
            return 3;
        }
        else if (numTurnsTakenToCompleteLevel < turnLimit && numTurnsTakenToCompleteLevel > turnLimit - 5)
        {
            return 2;
        }
        else if (numTurnsTakenToCompleteLevel <= turnLimit)
        {
            return 1;
        }
        else
        {
            Debug.LogAssertion($"Player took too many turns: {numTurnsTakenToCompleteLevel}. How'd we even end up in this method?");
            return 0;
        }
    }
}
