using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private Rigidbody rb;
    private float movementX, movementY;

    // Start is called before the first frame update
    void Start() {
        // assumes that a Rigidbody exists on this GameObject
        this.rb = this.GetComponent<Rigidbody>();
    }

    void OnTriggerEnter(Collider other) {
        // TODO
    }

    void FixedUpdate() {
        // this.transform.Translate(this.movementX, 0, this.movementY);
        if (_isMoving) return;
    }

    // ripped this code, rethink it
    [SerializeField] private float _rollSpeed = 5;
    private bool _isMoving;

    // OnMove comes from the InputActions action defined Move
    void OnMove(InputValue movementValue) {
        print("Move player!");
        Vector2 movementVector = movementValue.Get<Vector2>();
        this.movementX = movementVector.x;
        this.movementY = movementVector.y;

        if (this.movementX == -1) Assemble(Vector3.left);
        else if (this.movementX == 1) Assemble(Vector3.right);
        else if (this.movementY == 1) Assemble(Vector3.forward);
        else if (this.movementY == -1) Assemble(Vector3.back);
 
        void Assemble(Vector3 dir) {
            var anchor = transform.position + (Vector3.down + dir) * 0.5f;
            var axis = Vector3.Cross(Vector3.up, dir);
            StartCoroutine(Roll(anchor, axis));
        }
    }
 
    private void Update() {
    }
 
    private IEnumerator Roll(Vector3 anchor, Vector3 axis) {
        _isMoving = true;
        for (var i = 0; i < 90 / _rollSpeed; i++) {
            transform.RotateAround(anchor, axis, _rollSpeed);
            yield return new WaitForSeconds(0.01f);
        }
        _isMoving = false;
    }
}

