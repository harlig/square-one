using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaypointController : MonoBehaviour
{
    public delegate void OnTriggeredAction();
    public event OnTriggeredAction OnTriggered;

    public delegate void BeforeNextWaypointSpawnAction();
    public event BeforeNextWaypointSpawnAction BeforeNextWaypointSpawn;

    public delegate void SpawnNextWaypointAction();
    public event SpawnNextWaypointAction SpawnNextWaypoint;

    void OnTriggerEnter(Collider other)
    {
        if (!PlayerController.IsColliderPlayer(other)) return;
        gameObject.SetActive(false);

        Debug.Log("Time for before spawn actions");
        BeforeNextWaypointSpawn?.Invoke();
        Debug.Log("Done with before spawn actions, time to spawn next waypoint");
        SpawnNextWaypoint?.Invoke();
        Debug.Log("Spawned next waypoint");

        OnTriggered?.Invoke();
    }
}
