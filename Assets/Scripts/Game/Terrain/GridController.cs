using System.Collections.Generic;
using UnityEngine;

public class GridController : Singleton<GridController>
{
    [SerializeField] private GameObject paintTilePrefab;
    [SerializeField] private GameObject obstaclePrefab;

    private List<List<TileController>> gridRows;

    private Color startingColor;

    public void SetupGrid(int width, int length)
    {
        List<List<TileController>> rows = new();

        for (int row = 0; row < width; row++)
        {
            List<TileController> thisRow = new();
            GameObject rowObj = new(string.Format("row{0}", row));
            for (int col = 0; col < length; col++)
            {
                GameObject tile = Instantiate(paintTilePrefab);
                tile.transform.localPosition = new Vector3(row, 0, col);

                tile.name = string.Format("col{0}", col);
                tile.transform.parent = rowObj.transform;

                thisRow.Add(tile.GetComponent<TileController>());

                if (startingColor == null)
                {
                    startingColor = thisRow[0].GetComponent<MeshRenderer>().material.color;
                }
            }
            rowObj.transform.parent = transform;
            rows.Add(thisRow);
        }
        gridRows = rows;
    }


    private GameObject _obstacleGameObject;

    public ObstacleController AddObstacleAtPosition(int row, int col)
    {
        if (_obstacleGameObject == null)
        {
            _obstacleGameObject = new("Obstacles");
        }
        // obstacles spawn on top of floor
        ObstacleController obstacle = Instantiate(obstaclePrefab, new Vector3Int(row, 1, col), Quaternion.identity).GetComponent<ObstacleController>();
        obstacle.SetName($"row{row}col{col}");

        obstacle.transform.parent = _obstacleGameObject.transform;

        Debug.LogFormat("Made obstacle: {0}", obstacle);
        obstacle.LogFromObstacle();

        return obstacle;
    }

    public TileController TileAtLocation(int x, int y)
    {
        return TileAtLocation(new Vector2Int(x, y));
    }

    /**
    * Get Tile at Vector2Int position. Returns null if position is not within grid.
    */
    public TileController TileAtLocation(Vector2Int position)
    {
        if (!IsWithinGrid(position))
        {
            return null;
        }
        return gridRows[position.x][position.y];
    }

    public void ResetTileColorAtLocation(Vector2Int position)
    {
        PaintTileAtLocation(position, startingColor);
    }

    public Color? TileColorAtLocation(Vector2Int position)
    {
        if (!IsWithinGrid(position))
        {
            return null;
        }
        if (TileAtLocation(position) is not PaintTile)
        {
            Debug.Log("Tile color at location returned a non-paint tile!");
            return null;
        }

        return ((PaintTile)TileAtLocation(position)).GetColor();
    }

    public void PaintTileAtLocation(Vector2Int position, Color color)
    {
        PaintTileAtLocation(position.x, position.y, color);
    }

    public void PaintTileAtLocation(int x, int z, Color color)
    {
        if (!IsWithinGrid(x, z))
        {
            return;
        }
        if (TileAtLocation(x, z) is not PaintTile)
        {
            Debug.Log("Paint tile at location returned a non-paint tile!");
            return;
        }
        ((PaintTile)gridRows[x][z]).Paint(color);
    }

    public void PaintTilesAdjacentToLocation(Vector2 position, Color color)
    {
        PaintTilesAdjacentToLocation((int)position.x, (int)position.y, color);
    }

    public void PaintTilesAdjacentToLocation(int x, int z, Color color)
    {
        PaintTileAtLocation(x - 1, z, color);
        PaintTileAtLocation(x + 1, z, color);
        PaintTileAtLocation(x, z - 1, color);
        PaintTileAtLocation(x, z + 1, color);
    }

    public bool IsWithinGrid(Vector2Int position)
    {
        return IsWithinGrid(position.x, position.y);
    }

    public bool IsWithinGrid(int x, int z)
    {
        return x >= 0 && x < gridRows.Count && z >= 0 && z < gridRows.Count;
    }
}
