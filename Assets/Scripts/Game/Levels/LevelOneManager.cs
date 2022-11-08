using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelOneManager : MonoBehaviour
{
    public int gridSizeX, gridSizeY = 10;

    public GameObject grid;
    public GameObject player;
    public GameObject cameraPivot;

    private GridController gridController;
    private PlayerController playerController;
    private CameraController cameraController;

    void Start()
    {
        this.gridController = grid.GetComponent<GridController>();
        this.playerController = player.GetComponent<PlayerController>();
        this.cameraController = cameraPivot.GetComponent<CameraController>();

        ArrayList gridRows = gridController.SetupGrid(gridSizeX, gridSizeY);

        int playerOffsetX = gridSizeX/2;
        int playerOffsetY = gridSizeY/2;

        playerController.SpawnPlayer(playerOffsetX, playerOffsetY);
        cameraController.CenterCameraOnOffset(playerOffsetX, playerOffsetY);

        player.SetActive(true);


        // random coloring just to prove I can do it
        ArrayList row = (ArrayList) gridRows[5];
        ((GameObject) row[2]).GetComponent<PaintController>().Paint(Color.black);
        ((GameObject) row[7]).GetComponent<PaintController>().Paint(Color.black);

        row = (ArrayList) gridRows[8]; 
        ((GameObject) row[0]).GetComponent<PaintController>().Paint(Color.red);
        ((GameObject) row[5]).GetComponent<PaintController>().Paint(Color.red);

        row = (ArrayList) gridRows[2]; 
        ((GameObject) row[3]).GetComponent<PaintController>().Paint(Color.yellow);
        ((GameObject) row[9]).GetComponent<PaintController>().Paint(Color.yellow);
    }

    // Update is called once per frame
    void Update()
    {
        // if (PlayerController.transform.localRotation.x == 2 && PlayerController.transnform.location.z == 2) {
        // PaintController.colorRed(grid[2][2]);
        // }
    }
}
