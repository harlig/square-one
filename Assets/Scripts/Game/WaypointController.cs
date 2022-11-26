using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaypointController : MonoBehaviour
{
    public delegate void OnTriggeredAction();
    public event OnTriggeredAction OnTriggered;

    void OnTriggerEnter(Collider other)
    {
        if (!PlayerController.IsColliderPlayer(other)) return;
        gameObject.SetActive(false);

        OnTriggered?.Invoke();
    }
}
