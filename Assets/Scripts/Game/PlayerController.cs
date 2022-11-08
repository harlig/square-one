using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private Rigidbody rb;
    private float movementX, movementY;

    public Terrain terrain;

    // Start is called before the first frame update
    void Start() {
        // assumes that a Rigidbody exists on this GameObject
        this.rb = this.GetComponent<Rigidbody>();
    }

    void OnTriggerEnter(Collider other) {
        // TODO
    }

    // ripped this code, rethink it
    [SerializeField] private float _rollSpeed = 5;
    private bool _isMoving;

    // OnMove comes from the InputActions action defined Move
    void OnMove(InputValue movementValue) {
        if (_isMoving) return;

        Vector2 movementVector = movementValue.Get<Vector2>();
        if (Mathf.Abs(movementVector.x) != 1.0f && Mathf.Abs(movementVector.y) != 1.0f) return;
        this.movementX = movementVector.x;
        this.movementY = movementVector.y;


        Debug.LogFormat("Moving play w this vector: {0}", movementVector);
        // TODO: quickly inputting one direction then another causes this input to freeze, need to fix
        if (!_isMoving) {
            _isMoving = true;
            if (this.movementX == -1) Assemble(Vector3.left);
            else if (this.movementX == 1) Assemble(Vector3.right);
            else if (this.movementY == 1) Assemble(Vector3.forward);
            else if (this.movementY == -1) Assemble(Vector3.back);
        } else {
            return;
        }
 
        void Assemble(Vector3 dir) {
            var anchor = transform.position + (Vector3.down + dir) * 0.5f;
            var axis = Vector3.Cross(Vector3.up, dir);
            // I think I want less of a Roll and more of a fixed one unit movement
            float rotationRemaining = 90;

            StartCoroutine(Roll(anchor, axis));

            IEnumerator Roll(Vector3 anchor, Vector3 axis) {
                for (var i = 0; i < 90 / _rollSpeed; i++) {
                    float rotationAngle = Mathf.Min(_rollSpeed, rotationRemaining);
                    transform.RotateAround(anchor, axis, rotationAngle);
                    rotationRemaining -= _rollSpeed;
                    // yield return new WaitForSeconds(0.01f);
                    yield return null;
                }
                _isMoving = false;
                PaintGround();
                yield return null;
            }
        }
    }

    void PaintGround() {
        Debug.Log("painting ground");
    }
}

