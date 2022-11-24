using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridController : Singleton<GridController>
{
    [SerializeField] private GameObject paintTilePrefab;
    [SerializeField] private GameObject iceTilePrefab;
    [SerializeField] private GameObject obstaclePrefab;

    private List<List<TileController>> gridRows;

    private Color startingColor;

    // TODO need a way to specify a default prefab to use (like paintTilePrefab) and then
    // a list of other prefabs and a list of their locations. Like "use paintTilePrefab everywhere
    // except put IceTiles where I specify in this list"
    public void SetupGrid(int xSize, int ySize)
    {
        List<List<TileController>> rows = new();

        for (int x = 0; x < xSize; x++)
        {
            List<TileController> thisRow = new();
            GameObject xRowObj = new(string.Format("X{0}", x));
            for (int y = 0; y < ySize; y++)
            {
                GameObject tile;

                tile = Instantiate(paintTilePrefab);
                tile.transform.localPosition = new Vector3(x, 0, y);
                tile.name = string.Format("Y{0} - Paint", y);
                tile.transform.parent = xRowObj.transform;

                thisRow.Add(tile.GetComponent<TileController>());

                if (startingColor == null)
                {
                    startingColor = thisRow[0].GetComponent<MeshRenderer>().material.color;
                }

            }
            xRowObj.transform.parent = transform;
            rows.Add(thisRow);
        }
        gridRows = rows;
    }

    public IceTile SpawnIceTile(int x, int y, IceTile.SteppedOnAction steppedOnAction)
    {
        if (IsWithinGrid(x, y))
        {
            TileAtLocation(x, y).GetTile().SetActive(false);
            Transform parent = transform.GetChild(x).transform;

            GameObject tile = Instantiate(iceTilePrefab);
            tile.transform.localPosition = new Vector3(x, 0, y);
            tile.name = string.Format("y{0} - Ice", y);
            tile.transform.parent = parent;

            IceTile iceTile = tile.GetComponent<IceTile>();
            iceTile.WhenSteppedOn += steppedOnAction;

            gridRows[x][y] = iceTile;
            return iceTile;
        }

        return null;
    }

    public List<IceTile> SpawnIceTilesAroundPosition(int x, int y, IceTile.SteppedOnAction steppedOnAction)
    {
        List<IceTile> iceTilesCreated = new();
        for (int xAttempt = x - 1; xAttempt <= x + 1; xAttempt++)
        {
            for (int yAttempt = y - 1; yAttempt <= y + 1; yAttempt++)
            {
                // don't spawn ice tile here
                if (xAttempt == x && yAttempt == y) continue;

                IceTile iceTile = SpawnIceTile(xAttempt, yAttempt, steppedOnAction);
                if (iceTile == null) continue;

                iceTilesCreated.Add(iceTile);
            }
        }
        return iceTilesCreated;
    }

    private GameObject _obstacleGameObject;

    public ObstacleController AddObstacleAtPosition(int x, int y)
    {
        if (_obstacleGameObject == null)
        {
            _obstacleGameObject = new("Obstacles");
        }
        // obstacles spawn on top of floor
        ObstacleController obstacle = Instantiate(obstaclePrefab, new Vector3Int(x, 1, y), Quaternion.identity).GetComponent<ObstacleController>();
        obstacle.SetName($"row{x}col{y}");

        obstacle.transform.parent = _obstacleGameObject.transform;

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
