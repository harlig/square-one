using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    public GameObject player;

    private Vector3 offset;

    public int CAMERA_MOVE_DIST_X = 10;
    public int CAMERA_MOVE_DIST_Y = 2;
    public int CAMERA_MOVE_DIST_Z = 10;

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
        Vector3 pos = this.transform.position;
        Debug.Log($"pos: {pos.ToString()}");

        // TODO  doesn't work once player has moved. Should just center on map?
        if (moveDirection > 0) {
            Debug.Log("positive move direction");
            if (atBottom(pos)) {
                Debug.Log("moving 10 x");
                this.transform.Translate(CAMERA_MOVE_DIST_X, CAMERA_MOVE_DIST_Y, 0, transformAround);
            } 
            else if (atRight(pos)) {
                Debug.Log("moving 10 z");
                this.transform.Translate(0, CAMERA_MOVE_DIST_Y, CAMERA_MOVE_DIST_Z, transformAround);
            } else if (atTop(pos)) {
                Debug.Log("moving -10 x");
                this.transform.Translate(-1 * CAMERA_MOVE_DIST_X, -1 * CAMERA_MOVE_DIST_Y, 0, transformAround);
            } else if (atLeft(pos)) {
                Debug.Log("moving -10 z");
                this.transform.Translate(0, -1 * CAMERA_MOVE_DIST_Y, -1 * CAMERA_MOVE_DIST_Z, transformAround);
            }
            Debug.Log("rotating -90 y");
            Vector3 rot = this.transform.eulerAngles;
            this.transform.eulerAngles = new Vector3(rot.x, rot.y - 90, rot.z);
        } else if (moveDirection < 0) {
            Debug.Log("negative move direction");
            if (atBottom(pos)) {
                Debug.Log("moving 10 z");
                this.transform.Translate(0, CAMERA_MOVE_DIST_Y, CAMERA_MOVE_DIST_Z, transformAround);
            } 
            else if (atLeft(pos)) {
                Debug.Log("moving 10 x");
                this.transform.Translate(CAMERA_MOVE_DIST_X, CAMERA_MOVE_DIST_Y, 0, transformAround);
            } else if (atTop(pos)) {
                Debug.Log("mmoving -10 z");
                this.transform.Translate(0, -1 * CAMERA_MOVE_DIST_Y, -1 * CAMERA_MOVE_DIST_Z, transformAround);
            } else if (atRight(pos)) {
                Debug.Log("moving -10 x");
                this.transform.Translate(-1 * CAMERA_MOVE_DIST_X, -1 * CAMERA_MOVE_DIST_Y, 0, transformAround);
            }
            Debug.Log("rotating 90 y");
            Vector3 rot = this.transform.eulerAngles;
            this.transform.eulerAngles = new Vector3(rot.x, rot.y + 90, rot.z);
        }

        bool isEqual(float a, float b, string msg)
        {
            if (Mathf.Abs(a - b) <= 1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        bool atBottom(Vector3 pos) {
            return isEqual(pos.x, 0, "at bottom") && isEqual(pos.z, 0, "at bottom");
        }

        bool atRight(Vector3 post) {
            return isEqual(pos.x, CAMERA_MOVE_DIST_X, "at right") && isEqual(pos.z, 0, "at right");
        }

        bool atTop(Vector3 post) {
            return isEqual(pos.x, CAMERA_MOVE_DIST_X, "at top") && isEqual(pos.z, CAMERA_MOVE_DIST_Z, "at top");
        }

        bool atLeft(Vector3 pos) {
            return isEqual(pos.x, 0, "at left") && isEqual(pos.z, CAMERA_MOVE_DIST_Z, "at left");
        }
    }
 
}
