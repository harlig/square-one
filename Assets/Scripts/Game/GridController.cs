using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridController : MonoBehaviour
{
    public GameObject tilePrefab;

    /**
    * returns grid
    */
    public ArrayList SetupGrid(int width, int length) {
        // TODO stronger type than ArrayLists, gonna suck to work with
        ArrayList rows = new ArrayList();

        for (int row = 0; row < width; row++) {
            ArrayList thisRow = new ArrayList();
            GameObject rowObj = new GameObject(string.Format("row{0}", row));
            for (int col = 0; col < length; col++) {
                GameObject tile = Instantiate(tilePrefab);
                tile.transform.localPosition = new Vector3(row, 0, col);

                tile.name = string.Format("col{0}", col);
                tile.transform.parent = rowObj.transform;

                thisRow.Add(tile);
            }
            rowObj.transform.parent = this.transform;
            rows.Add(thisRow);
        }
        return rows;
    }
}
