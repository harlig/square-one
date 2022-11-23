using System;
using System.Collections;
using System.Collections.Generic;
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
    private readonly Action<bool> _afterRoll;
    private readonly Queue<Tuple<Vector3, MoveType>> queuedMovements;

    private bool circuitBreakMovement = false;

    public Cube(MonoBehaviour mb, float rollSpeed, Action beforeRoll, Action<bool> afterRoll)
    {
        _mb = mb;
        _rollSpeed = rollSpeed;
        _beforeRoll = beforeRoll;
        _afterRoll = afterRoll;
        queuedMovements = new();
    }

    protected Rigidbody rb;

    private bool _isMoving;

    public enum MoveType
    {
        ROLL,
        SLIDE
    }

    public void MoveInDirectionIfNotMoving(Vector3 dir, MoveType moveType)
    {
        if (queuedMovements.Count == 0)
        {
            Debug.LogFormat("Enqueueing a move in this direction of this type {0} {1}", dir, moveType);
            queuedMovements.Enqueue(new(dir, moveType));
        }
        if (_isMoving)
        {
            return;
        }

        DoQueuedRotation();
    }

    private void DoQueuedRotation()
    {
        if (_isMoving) return;

        Debug.LogFormat("Peeking for enqueued rotation {0}", queuedMovements.Count != 0 ? queuedMovements.Peek() : "EMPTY");
        if (queuedMovements.Count == 0) return;

        Tuple<Vector3, MoveType> movement = queuedMovements.Dequeue();

        Vector3 dir = movement.Item1;
        MoveType moveType = movement.Item2;

        // lock
        _isMoving = true;
        circuitBreakMovement = false;
        _beforeRoll.Invoke();

        if (rb == null)
        {
            rb = _mb.gameObject.GetComponent<Rigidbody>();
        }

        if (moveType == MoveType.ROLL)
        {
            var anchor = _mb.gameObject.transform.localPosition + (Vector3.down + dir) * 0.5f;
            var axis = Vector3.Cross(Vector3.up, dir);
            float rotationRemaining = 90;
            // TODO different math for tiny player?
            _mb.StartCoroutine(Roll(anchor, axis, rotationRemaining));
        }
        else if (moveType == MoveType.SLIDE)
        {
            _mb.StartCoroutine(Slide(dir));
        }
        else
        {
            throw new Exception($"Unknown moveType: {moveType}");
        }
    }

    IEnumerator Slide(Vector3 dir)
    {
        Vector3 currentPos = _mb.gameObject.transform.position;
        Vector3 targetPos = _mb.gameObject.transform.position + dir;

        bool finishedMove = true;
        int numSteps = 20;
        for (var i = 0; i < numSteps; i++)
        {
            if (circuitBreakMovement) { finishedMove = false; break; };

            _mb.gameObject.transform.position = Vector3.Lerp(currentPos, targetPos, 1.0f / numSteps * i);
            yield return null;
        }
        FinishMovement(finishedMove);
    }

    IEnumerator Roll(Vector3 anchor, Vector3 axis, float rotationRemaining)
    {
        bool finishedMove = true;
        for (var i = 0; i < 90 / _rollSpeed; i++)
        {
            if (circuitBreakMovement) { finishedMove = false; break; };

            float rotationAngle = Mathf.Min(_rollSpeed, rotationRemaining);
            _mb.gameObject.transform.RotateAround(anchor, axis, rotationAngle);
            rotationRemaining -= _rollSpeed;
            yield return null;
        }
        FinishMovement(finishedMove);
    }

    private void FinishMovement(bool didFinishMove)
    {
        Debug.LogFormat("Finished movement in cube: {0}", didFinishMove);
        Vector3 pos = _mb.gameObject.transform.position;
        _mb.gameObject.transform.localPosition = Vector3Int.RoundToInt(pos);
        ResetPhysics();

        // unlock
        _isMoving = false;
        circuitBreakMovement = false;

        _afterRoll?.Invoke(didFinishMove);

        DoQueuedRotation();
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

    public void StopMoving()
    {
        Debug.Log("Clearing queue");
        queuedMovements.Clear();
        ResetPhysics();
        circuitBreakMovement = true;
    }

    public void StartMoving()
    {
        Debug.Log("Starting moving");
        circuitBreakMovement = false;
    }
}
