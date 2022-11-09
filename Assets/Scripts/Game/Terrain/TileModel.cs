using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileModel : MonoBehaviour
{
    private GameObject tile;

    public TileModel(GameObject tile) {
        this.tile = tile;
    }

    public GameObject GetTile() {
        return this.tile;
    }

    public Color GetColor() {
        return this.tile.GetComponent<MeshRenderer>().material.color;
    }

    public void Paint(Color color) {
        Debug.LogFormat("Painting this tile to color: {0}", color);

        this.tile.GetComponent<MeshRenderer>().material.color = color;
    }
}