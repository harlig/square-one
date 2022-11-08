using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter(Collider other) {
        Debug.Log("TRIGGER ENTERED");
        // paint this tile!
        gameObject.GetComponent<Renderer> ().material.color = Color.red;
    }
}
