using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaintController : MonoBehaviour
{
    public void Paint(Color color) {
        Debug.LogFormat("Painting this tile to color: {0}", color);
        this.GetComponent<MeshRenderer>().material.color = color;
    }
}
