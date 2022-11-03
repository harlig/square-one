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
        Vector2 movementVector = movementValue.Get<Vector2>();
        this.movementX = movementVector.x;
        this.movementY = movementVector.y;

        print($"moveX: {this.movementX}, moveY: {this.movementY}");
        // print($"isPressed: {movementValue.isPressed}");
    }

    void OnTriggerEnter(Collider other) {
        // TODO
    }

    void FixedUpdate() {

    }

    void Update() {
    }
}

