using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridController : MonoBehaviour
{
    public GameObject tilePrefab;

    // Start is called before the first frame update
    void Start()
    {
    }

    public void SetupGrid(int width, int length) {
        for (int row = 0; row < width; row++) {
            GameObject rowObj = new GameObject(string.Format("row{0}", row));
            for (int col = 0; col < length; col++) {
                GameObject tile = Instantiate(tilePrefab); //string.Format("col{0}", row));
                tile.transform.position = new Vector3(row, 0, col);
            }
            rowObj.transform.parent = this.transform;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
