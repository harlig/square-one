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

    public void CenterCameraOnOffset(int x, int z) {
        Vector3 pos = this.transform.position;
        this.transform.position = new Vector3(pos.x + x, pos.y, pos.z + z);
    }


    private bool _isRotating = false;
    // OnRotate comes from the InputActions action defined Rotate
    void OnRotate(InputValue movementValue) {
        if (_isRotating) {
            Debug.Log("Tried to call rotate when _isRotating is true");
            return;
        }

        float moveDirection = movementValue.Get<float>();

        if (moveDirection > 0) {
            Debug.Log("positive move direction");
            Vector3 rot = this.transform.eulerAngles;
            this.targetEulerAngles = new Vector3(rot.x, rot.y - 90, rot.z);
        } else if (moveDirection < 0) {
            Debug.Log("negative move direction");
            Vector3 rot = this.transform.eulerAngles;
            this.targetEulerAngles = new Vector3(rot.x, rot.y + 90, rot.z);
        }

        _isRotating = true;
        StartCoroutine(Rotate());
    }

    private IEnumerator Rotate() {
        startRotation = this.transform.rotation;
        while (_isRotating && progress < 1) {
            this.transform.rotation = Quaternion.Lerp(startRotation, Quaternion.Euler(targetEulerAngles), progress);

            // move with frame rate
            progress += spd * Time.fixedDeltaTime;

            // halt iteration until next frame
            yield return null;
        }

        _isRotating = false;
        progress = 0;

        yield return null;
    }

}
