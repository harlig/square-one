using System;
using System.Collections;
using UnityEngine;

public class Cube
{
    // TODO ethan this seems super dangerous but I can't think of better solution for now
    private readonly MonoBehaviour _mb;
    private float _rollSpeed = 1.0f;

    public float GetRollSpeed()
    {
        return _rollSpeed;
    }

    public void SetRollSpeed(float rollSpeed)
    {
        _rollSpeed = rollSpeed;
    }

    private readonly Action _beforeRoll;
    private readonly Action _afterRoll;

    public Cube(MonoBehaviour mb, float rollSpeed, Action beforeRoll, Action afterRoll)
    {
        _mb = mb;
        _rollSpeed = rollSpeed;
        _beforeRoll = beforeRoll;
        _afterRoll = afterRoll;
    }

    protected Rigidbody rb;

    private bool _isRotating;

    public void MoveInDirectionIfNotMoving(Vector3 dir)
    {
        if (_isRotating) return;

        DoRotation(dir);
    }

    public void ForceMoveInDirection(Vector3 dir)
    {
        Debug.LogFormat("FORCE MOVING when _isRotating is: {0}", _isRotating);
        DoRotation(dir);
    }

    private void DoRotation(Vector3 dir)
    {
        // lock
        _isRotating = true;
        _beforeRoll.Invoke();

        if (rb == null)
        {
            rb = _mb.gameObject.GetComponent<Rigidbody>();
        }


        var anchor = _mb.gameObject.transform.localPosition + (Vector3.down + dir) * 0.5f;
        var axis = Vector3.Cross(Vector3.up, dir);
        // I think I want less of a Roll and more of a fixed one unit movement
        float rotationRemaining = 90;

        // TODO different math for tiny player?
        _mb.StartCoroutine(Roll(anchor, axis));

        IEnumerator Roll(Vector3 anchor, Vector3 axis)
        {
            for (var i = 0; i < 90 / _rollSpeed; i++)
            {
                float rotationAngle = Mathf.Min(_rollSpeed, rotationRemaining);
                _mb.gameObject.transform.RotateAround(anchor, axis, rotationAngle);
                rotationRemaining -= _rollSpeed;
                yield return null;
            }

            Vector3 pos = _mb.gameObject.transform.position;
            _mb.gameObject.transform.localPosition = Vector3Int.RoundToInt(pos);
            ResetPhysics();

            _afterRoll?.Invoke();

            // lock
            _isRotating = false;
        }
    }

    public void ResetPhysics()
    {
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        RotateToNearestRightAngles();

        void RotateToNearestRightAngles()
        {
            Quaternion roundedRotation = new(
                ClosestRightAngle(_mb.gameObject.transform.rotation.x),
                ClosestRightAngle(_mb.gameObject.transform.rotation.y),
                ClosestRightAngle(_mb.gameObject.transform.rotation.z),
                _mb.gameObject.transform.rotation.w);

            _mb.gameObject.transform.rotation = roundedRotation;

            static int ClosestRightAngle(float rotation)
            {
                bool isPositive = rotation > 0;
                return Mathf.RoundToInt(rotation) * 90 * (isPositive ? 1 : -1);
            }
        }
    }
}
