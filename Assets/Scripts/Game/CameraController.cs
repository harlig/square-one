using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    public float spd = 0.3f;

    private Vector3 targetEulerAngles;
    private Quaternion startRotation;
    private float progress = 0;

    void Start()
    {
        // NEED to set this otherwise framerate is uncapped
        Application.targetFrameRate = 144;
    }

    private bool _isRotating = false;
    // OnRotate comes from the InputActions action defined Rotate
    void OnRotate(InputValue movementValue) {
        if (_isRotating) {
            Debug.Log("Tried to call rotate when _isRotating is true");
            return;
        }

        _isRotating = true;
        float moveDirection = movementValue.Get<float>();

        // TODO  doesn't work once player has moved. Should just center on map?
        if (moveDirection > 0) {
            Debug.Log("positive move direction");
            Vector3 rot = this.transform.eulerAngles;
            this.targetEulerAngles = new Vector3(rot.x, rot.y - 90, rot.z);
        } else if (moveDirection < 0) {
            Debug.Log("negative move direction");
            Vector3 rot = this.transform.eulerAngles;
            this.targetEulerAngles = new Vector3(rot.x, rot.y + 90, rot.z);
        }
    }

    public void FixedUpdate() {
        if (_isRotating) {
            Rotate();
        }
    }

    private void Rotate() {
        if (progress == 0) {
            startRotation = this.transform.rotation;
        }
        this.transform.rotation = Quaternion.Lerp(startRotation, Quaternion.Euler(targetEulerAngles), progress);

        // move with frame rate
        progress += spd * Time.fixedDeltaTime;
        if (progress >= 1) {
            _isRotating = false;
            progress = 0;
        }
    }

}
