using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridController : MonoBehaviour
{
    public GameObject tilePrefab;

    private List<List<TileController>> gridRows;

    public void SetupGrid(int width, int length)
    {
        List<List<TileController>> rows = new List<List<TileController>>();

        for (int row = 0; row < width; row++)
        {
            List<TileController> thisRow = new List<TileController>();
            GameObject rowObj = new GameObject(string.Format("row{0}", row));
            for (int col = 0; col < length; col++)
            {
                GameObject tile = Instantiate(tilePrefab);
                tile.transform.localPosition = new Vector3(row, 0, col);

                tile.name = string.Format("col{0}", col);
                tile.transform.parent = rowObj.transform;

                thisRow.Add(tile.GetComponent<TileController>());
            }
            rowObj.transform.parent = transform;
            rows.Add(thisRow);
        }
        gridRows = rows;
    }

    public Color TileColorAtLocation(Vector2 position)
    {
        return gridRows[((int)position.x)][((int)position.y)].GetColor();
    }

    public void PaintTileAtLocation(Vector2 position, Color color)
    {
        PaintTileAtLocation(((int)position.x), ((int)position.y), color);
    }

    public void PaintTileAtLocation(int x, int z, Color color)
    {
        gridRows[x][z].Paint(color);
    }

    public void PaintTilesAdjacentToLocation(Vector2 position, Color color)
    {
        PaintTilesAdjacentToLocation(((int)position.x), ((int)position.y), color);
    }

    public void PaintTilesAdjacentToLocation(int x, int z, Color color)
    {
        PaintTileAtLocation(x - 1, z, color);
        PaintTileAtLocation(x + 1, z, color);
        PaintTileAtLocation(x, z - 1, color);
        PaintTileAtLocation(x, z + 1, color);
    }

    public bool IsWithinGrid(Vector2 position)
    {
        return position.x >= 0 && position.x < gridRows.Count && position.y >= 0 && position.y < gridRows.Count;
    }

}
