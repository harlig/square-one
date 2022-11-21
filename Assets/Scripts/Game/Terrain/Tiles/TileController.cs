using UnityEngine;

// TODO refactor into PaintTile class with like TileController interface
// can also have IceTile, TrapTile, etc.
public class TileController : MonoBehaviour
{
    public GameObject GetTile()
    {
        return gameObject;
    }
}