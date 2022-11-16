using System.Collections;
using TMPro;
using UnityEngine;

public abstract class LevelManager : MonoBehaviour
{
    [SerializeField] protected GridController gridController;
    [SerializeField] protected PlayerController playerController;
    [SerializeField] protected CameraController cameraController;

    [SerializeField] protected TextMeshProUGUI moveCountText;

    [SerializeField] protected GameObject successElements;
    [SerializeField] protected GameObject failedElements;

    protected int gridSizeX, gridSizeY, turnLimit, turnsLeft;

    protected Vector2Int squareOne;
    protected bool levelActive;

    [SerializeField] protected bool devMode = false;
    [SerializeField] private int devModeGridSizeX, devModeGridSizeY, devModeTurnLimit;

    protected void SetupLevel()
    {

        if (devMode)
        {
            gridSizeX = devModeGridSizeX;
            gridSizeY = devModeGridSizeY;
            turnLimit = devModeTurnLimit;
        }

        playerController = (PlayerController)PlayerController.Instance;
        gridController = (GridController)GridController.Instance;
        cameraController = (CameraController)CameraController.Instance;

        gridController.SetupGrid(devModeGridSizeX, devModeGridSizeY);

        int playerOffsetX = devModeGridSizeX / 2;
        int playerOffsetY = devModeGridSizeY / 2;

        playerController.SpawnPlayer(playerOffsetX, playerOffsetY);
        playerController.gameObject.SetActive(true);

        cameraController.CenterCameraOnOffset(playerOffsetX, playerOffsetY);

        squareOne = new(playerOffsetX, playerOffsetY);

        turnsLeft = devModeTurnLimit;

        SetMoveCountText();

        levelActive = true;
    }

    /**
    handles setting the game to SUCCESS or FAILED
    ideas included below
    */
    // set back to square one text
    // stop input from this script, now we should spawn a NextGamePortal and head there
    // also spawn a plane below you which can reset you into middle of map if you fall off at this point
    protected void SetTerminalGameState(GameObject textElementToEnable)
    {
        SetTerminalGameState(textElementToEnable, 0.2f);
    }

    /**
    handles setting the game to SUCCESS or FAILED with a variable waitDelaySeconds
    */
    protected void SetTerminalGameState(GameObject textElementToEnable, float waitDelaySeconds)
    {
        levelActive = false;
        playerController.StopCountingMoves();
        StartCoroutine(SetElementAfterDelay(textElementToEnable, waitDelaySeconds));

        static IEnumerator SetElementAfterDelay(GameObject element, float waitDelaySeconds)
        {
            yield return new WaitForSeconds(waitDelaySeconds);
            element.SetActive(true);
        }
    }

    protected void SetMoveCountText()
    {
        moveCountText.text = $"Turns remaining: {turnsLeft}";
    }

    // handle player movement. override in child classes if they want to access these events
    // prefer to use OnPlayerMoveStart unless you need specific behavior at the end of the movement
    protected virtual void OnPlayerMoveStart() { }
    protected virtual void OnPlayerMoveFinish() { }

    // required naming for events
    void OnEnable()
    {
        Debug.Log("Enabling player event");
        PlayerController.OnMoveStart += OnPlayerMoveStart;
        PlayerController.OnMoveFinish += OnPlayerMoveFinish;
    }

    // required naming for events
    void OnDisable()
    {
        Debug.Log("Disabling player event");
        PlayerController.OnMoveStart -= OnPlayerMoveStart;
        PlayerController.OnMoveFinish -= OnPlayerMoveFinish;
    }


}
