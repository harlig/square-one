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
        Vector3 pos = this.transform.position;
        Debug.Log($"pos: {pos.ToString()}");

        // TODO  doesn't work once player has moved. Should just center on map?
        if (moveDirection > 0) {
            Debug.Log("positive move direction");
            Debug.Log("rotating -90 y");
            Vector3 rot = this.transform.eulerAngles;
            this.transform.eulerAngles = new Vector3(rot.x, rot.y - 90, rot.z);
        } else if (moveDirection < 0) {
            Debug.Log("negative move direction");
            Debug.Log("rotating 90 y");
            Vector3 rot = this.transform.eulerAngles;
            this.transform.eulerAngles = new Vector3(rot.x, rot.y + 90, rot.z);
        }
    }
 
}
