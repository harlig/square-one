using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : Singleton<CameraController>
{
    public bool RotationEnabled { get; set; } = true;
    public float spd = 0.3f;

    private Vector3 targetEulerAngles;
    private Quaternion startRotation;
    private float progress = 0;

#pragma warning disable IDE0051
    void Start()
    {
        // NEED to set this otherwise framerate is uncapped
        Application.targetFrameRate = 100;
    }

    private static readonly int MIN_MOVEMENT_SWIPE_THRESHOLD = 100;

    Vector2? startTouchPosition, endTouchPosition;
    // this is handling raw input for ideally only webGL
    void Update()
    {
        if (RotationEnabled)
        {
            if (Application.isMobilePlatform)
            {
                // for now, disable ability to swipe to turn camera
                return;
                if (Input.touches.Length != 0)
                {
                    Debug.Log("user has touched screen");
                    Touch touch = Input.touches[0];
                    if (touch.phase == UnityEngine.TouchPhase.Began)
                    {
                        Debug.Log("start touch");
                        startTouchPosition = touch.position;
                    }
                    else if (touch.phase == UnityEngine.TouchPhase.Ended)
                    {
                        Debug.Log("end touch");
                        endTouchPosition = touch.position;

                    }
                    else if (touch.phase == UnityEngine.TouchPhase.Canceled)
                    {
                        Debug.Log("canceled touch");
                        startTouchPosition = endTouchPosition = null;
                    }
                    else
                    {
                        Debug.LogFormat("unhandled touch: {0}", touch.phase);
                    }
                }

                // TODO need to merge this with PlayerController somehow so they don't both accidentally operate on the same swipe. bugs will follow
                if (startTouchPosition != null && endTouchPosition != null)
                {
                    Debug.Log("CAMERA: we have both a start and end touch position, time to calculate");
                    float xDiff = (float)(endTouchPosition?.x - startTouchPosition?.x);
                    float yDiff = (float)(endTouchPosition?.y - startTouchPosition?.y);
                    if (Mathf.Abs(yDiff) > MIN_MOVEMENT_SWIPE_THRESHOLD)
                    {
                        Debug.LogWarningFormat("Swipe was vertical, let's not do any camera movement here. xDiff: {0}, yDiff: {1}", xDiff, yDiff);
                    }
                    else if (Mathf.Abs(xDiff) <= MIN_MOVEMENT_SWIPE_THRESHOLD)
                    {
                        Debug.LogWarningFormat("Swipe was tiny horizontal, let's not do any camera movement here. xDiff: {0}, yDiff: {1}", xDiff, yDiff);
                    }
                    else if (xDiff < 0)
                    {
                        RotateLeft();
                    }
                    else
                    {
                        RotateRight();
                    }

                    startTouchPosition = endTouchPosition = null;
                }
            }
            else
            {
                if (Input.GetKeyDown(KeyCode.E))
                {
                    RotateRight();
                }
                else if (Input.GetKeyDown(KeyCode.Q))
                {
                    RotateLeft();
                }
            }
        }

    }

    public void RotateLeft()
    {
        TryRotate(-1.0f);
    }
    public void RotateRight()
    {
        TryRotate(1.0f);
    }

    // OnRotate comes from the InputActions action defined Rotate
    void OnRotate(InputValue movementValue)
    {

        float moveDirection = movementValue.Get<float>();
        TryRotate(moveDirection);
    }
    void TryRotate(float moveDirection)
    {
        if (_isRotating)
        {
            return;
        }

        Vector2Int relativeMoveDirection;

        if (moveDirection > 0)
        {
            Vector3 rot = transform.eulerAngles;
            targetEulerAngles = new Vector3(rot.x, rot.y - 90, rot.z);
            relativeMoveDirection = Vector2Int.right;
        }
        else if (moveDirection < 0)
        {
            Vector3 rot = transform.eulerAngles;
            targetEulerAngles = new Vector3(rot.x, rot.y + 90, rot.z);
            relativeMoveDirection = Vector2Int.left;
        }
        else
        {
            Debug.LogWarning("Move direction of 0 provided in camera controller!");
            return;
        }

        _isRotating = true;
        OnCameraRotate?.Invoke(relativeMoveDirection);
        StartCoroutine(Rotate());
    }
#pragma warning restore IDE0051

    public void CenterCameraOnOffset(float x, float z)
    {
        Vector3 pos = transform.position;
        transform.localPosition = new Vector3(pos.x + x, pos.y, pos.z + z);
    }

    // currentPosition here is before player moves
    public delegate void CameraRotateAction(Vector2Int rotateDirection);
    public static event CameraRotateAction OnCameraRotate;

    private bool _isRotating = false;

    private IEnumerator Rotate()
    {
        startRotation = transform.rotation;
        while (_isRotating && progress < 1)
        {
            transform.rotation = Quaternion.Lerp(startRotation, Quaternion.Euler(targetEulerAngles), progress);

            // move with frame rate
            progress += spd * Time.fixedDeltaTime;

            // halt iteration until next frame
            yield return null;
        }

        _isRotating = false;
        progress = 0;
    }

}
