using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridController : Singleton<GridController>
{
    // TODO will change to BaseTile and use a bare TileController
    [SerializeField] private GameObject paintTilePrefab;
    [SerializeField] private GameObject iceTilePrefab;
    [SerializeField] private GameObject obstaclePrefab;
    [SerializeField] private GameObject waypointPrefab;

    private List<List<TileController>> gridRows;

    private Color startingColor;

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

    public Color? TileColorAtLocation(Vector2Int position)
    {
        if (!IsWithinGrid(position))
        {
            return null;
        }
        if (TileAtLocation(position) is not PaintTile)
        {
            return null;
        }

        return ((PaintTile)TileAtLocation(position)).GetColor();
    }

    public void PaintTileAtLocation(Vector2Int position, Color color)
    {
        PaintTileAtLocation(position.x, position.y, color);
    }

    public void PaintTileAtLocation(int x, int y, Color color)
    {
        if (!IsWithinGrid(x, y))
        {
            return;
        }
        if (TileAtLocation(x, y) is not PaintTile)
        {
            Debug.Log("Paint tile at location returned a non-paint tile!");
            return;
        }
        ((PaintTile)gridRows[x][y]).Paint(color);
        SpawnWaypoint(x, y);
    }

    public void SpawnWaypoint(int x, int y)
    {
        GameObject waypoint = Instantiate(waypointPrefab);
        waypoint.transform.localPosition = new Vector3(x, 1.0f, y);
        waypoint.name = string.Format("Waypoint [{0}, {1}]", x, y);
        // TODO animate this lil guy and make them spin or something

        // TODO to return
        // waypoint.GetComponent<WaypointController>();
    }

    public void PaintTilesAdjacentToLocation(Vector2 position, Color color)
    {
        PaintTilesAdjacentToLocation((int)position.x, (int)position.y, color);
    }

    public void PaintTilesAdjacentToLocation(int x, int y, Color color)
    {
        PaintTileAtLocation(x - 1, y, color);
        PaintTileAtLocation(x + 1, y, color);
        PaintTileAtLocation(x, y - 1, color);
        PaintTileAtLocation(x, y + 1, color);
    }

    public bool IsWithinGrid(Vector2Int position)
    {
        return IsWithinGrid(position.x, position.y);
    }

    public bool IsWithinGrid(int x, int y)
    {
        return x >= 0 && x < gridRows.Count && y >= 0 && y < gridRows.Count;
    }
}
