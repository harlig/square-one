using UnityEngine;

public class TileController : MonoBehaviour
{
    public bool isPainted {
        get; private set;
    }
    public GameObject GetTile()
    {
        return gameObject;
    }

    public Color GetColor()
    {
        return gameObject.GetComponent<MeshRenderer>().material.color;
    }

    public void Paint(Color color)
    {
        Debug.LogFormat("Painting this tile to color: {0}", color);
        gameObject.GetComponent<MeshRenderer>().material.color = color;
        isPainted = true;
    }
}