using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelOneManager : MonoBehaviour
{
    // register Ground gameObject
    public GameObject grid;
    public GameObject player;
    public GameObject cameraPivot;

    // register Player 

    // Start is called before the first frame update
    void Start()
    {
        grid.GetComponent<GridController>().SetupGrid(10, 10);
        player.GetComponent<PlayerController>().SpawnPlayer(5, 5);
        cameraPivot.GetComponent<CameraController>().CenterCameraOnOffset(5, 5);
        player.SetActive(true);

        // grid should be cube prefab which has PaintController on it and is collider

        // define what level looks like
        // 10x10 grid & (2,2) should light up red when stepped on

        // attach PaintController to tiles that should be painted when walked over
        // (maybe can do ^ manually)
    }

    // Update is called once per frame
    void Update()
    {

        // if (PlayerController.transform.localRotation.x == 2 && PlayerController.transnform.location.z == 2) {
        // PaintController.colorRed(grid[2][2]);
        // }
    }
}
