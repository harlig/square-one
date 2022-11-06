using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    public GameObject player;

    private Vector3 offset;

    void Start()
    {
        // NEED to set this otherwise framerate is uncapped
        Application.targetFrameRate = 144;
    }

    private bool _isRotating = false;
    // OnRotate comes from the InputActions action defined Rotate
    void OnRotate(InputValue movementValue) {
        if (_isRotating) {
            _isRotating = false;
            return;
        }

        _isRotating = true;
        float moveDirection = movementValue.Get<float>();

        Transform transformAround = this.player.transform;
        print($"transformAround: {transformAround.position.ToString()}");

        Vector3 pos = this.transform.position;
        print($"pos: {pos.ToString()}");

        // TODO  doesn't work once player has moved. Should just center on map?
        if (moveDirection > 0) {
            print("positive move direction");
            if (atBottom(pos)) {
                print("one");
                this.transform.Translate(10, 0, 0, transformAround);
            } 
            else if (atRight(pos)) {
                print("two");
                this.transform.Translate(0, 0, 10, transformAround);
            } else if (atTop(pos)) {
                print("three");
                this.transform.Translate(-10, 0, 0, transformAround);
            } else if (atLeft(pos)) {
                print("four");
                this.transform.Translate(0, 0, -10, transformAround);
            }
            Vector3 rot = this.transform.eulerAngles;
            this.transform.eulerAngles = new Vector3(rot.x, rot.y - 90, rot.z);
        } else if (moveDirection < 0) {
            print("negative move direction");
            if (atBottom(pos)) {
                print("one");
                this.transform.Translate(0, 0, 10, transformAround);
            } 
            else if (atLeft(pos)) {
                print("two");
                this.transform.Translate(10, 0, 0, transformAround);
            } else if (atTop(pos)) {
                print("three");
                this.transform.Translate(0, 0, -10, transformAround);
            } else if (atRight(pos)) {
                print("four");
                this.transform.Translate(-10, 0, 0, transformAround);
            }
            Vector3 rot = this.transform.eulerAngles;
            this.transform.eulerAngles = new Vector3(rot.x, rot.y + 90, rot.z);
        }

        // this.transform.LookAt(this.player.transform);


        bool isEqual(float a, float b, string msg)
        {
            if (Mathf.Abs(a - b) <= 1)
            {
                print($"a: {a}, b: {b}, true, {msg}");
                return true;
            }
            else
            {
                print($"a: {a}, b: {b}, false, {msg}");
                print($"{b - Mathf.Epsilon} {b + Mathf.Epsilon} {a}");
                return false;
            }
        }

        bool atBottom(Vector3 pos) {
            return isEqual(pos.x, 0, "at bottom") && isEqual(pos.z, 0, "at bottom");
        }

        bool atRight(Vector3 post) {
            return isEqual(pos.x, 10, "at right") && isEqual(pos.z, 0, "at right");
        }

        bool atTop(Vector3 post) {
            return isEqual(pos.x, 10, "at top") && isEqual(pos.z, 10, "at top");
        }

        bool atLeft(Vector3 pos) {
            return isEqual(pos.x, 0, "at left") && isEqual(pos.z, 10, "at left");
        }
    }
 
}
