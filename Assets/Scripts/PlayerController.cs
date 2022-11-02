using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private Rigidbody rb;
    private float movementX, movementY;
    private Vector3 originalPosition;
    private int count;

    // Start is called before the first frame update
    void Start() {
        // assumes that a Rigidbody exists on this GameObject
        this.rb = this.GetComponent<Rigidbody>();
    }

    void OnMove(InputValue movementValue) {
        this.movementX = movementVector.x;
        this.movementY = movementVector.y;
    }

    void OnTriggerEnter(Collider other) {
        // TODO
    }

    void FixedUpdate() {

    }

    void Update() {
    }
}

