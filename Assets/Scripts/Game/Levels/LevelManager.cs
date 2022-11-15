using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public GridController gridController;
    public PlayerController playerController;

    public CameraController cameraController;

    public Vector2Int squareOne;

    [SerializeField] protected int turnLimit;

    public bool levelActive;
    protected void SetupLevel(int gridSizeX, int gridSizeY)
    {
        gridController.SetupGrid(gridSizeX, gridSizeY);

        int playerOffsetX = gridSizeX / 2;
        int playerOffsetY = gridSizeY / 2;

        playerController.SpawnPlayer(playerOffsetX, playerOffsetY);
        playerController.gameObject.SetActive(true);

        cameraController.CenterCameraOnOffset(playerOffsetX, playerOffsetY);

        squareOne = new(playerOffsetX, playerOffsetY);

        levelActive = true;
    }

    /**
    handles setting the game to SUCCESS or FAILED
    ideas included below
    */
    // set back to square one text
    // stop input from this script, now we should spawn a NextGamePortal and head there
    // also spawn a plane below you which can reset you into middle of map if you fall off at this point
    public void SetTerminalGameState(GameObject textElementToEnable)
    {
        levelActive = false;
        playerController.StopCountingMoves();
        StartCoroutine(SetElementAfterDelay(textElementToEnable));

        static IEnumerator SetElementAfterDelay(GameObject element)
        {
            yield return new WaitForSeconds(0.2f);
            element.SetActive(true);
        }
    }
}
