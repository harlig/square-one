using System.Collections.Generic;
using UnityEngine;

public class LevelTwoManager : LevelManager
{
    private Dictionary<TileController, bool> paintedTiles;

    void Start()
    {
        gridSizeX = gridSizeY = 10;
        turnLimit = 20;

        SetupLevel();
        PlayerController.OnMoveAction += OnPlayerMove;

        paintedTiles = new Dictionary<TileController, bool>();
    }

    void Destroy()
    {
        PlayerController.OnMoveAction -= OnPlayerMove;
    }

    void OnPlayerMove()
    {
        turnsLeft--;
        SetMoveCountText();

        Vector2Int playerPos = playerController.GetRoundedPosition();

        TileController tile = gridController.TileAtLocation(playerPos);

        if (HasTile(tile))
        {
            SetTerminalGameState(failedElements);
        }
        else
        {
            gridController.PaintTileAtLocation(playerPos, Color.red);
            paintedTiles.Add(tile, true);
        }
    }

    private bool HasTile(TileController tile)
    {
        try
        {
            return paintedTiles[tile];
        }
        catch (KeyNotFoundException)
        {
            return false;
        }
    }
}
