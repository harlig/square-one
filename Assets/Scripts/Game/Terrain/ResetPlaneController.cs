using UnityEngine;

public class ResetPlaneController : Singleton<ResetPlaneController>
{
#pragma warning disable IDE0051
    void OnTriggerEnter(Collider other)
    {
        Debug.LogFormat("Something hit me!! {0}", other.gameObject.transform.position);
        if (!other.CompareTag("Player"))
        {
            other.gameObject.SetActive(false);
            return;
        }
        LevelStateManager.Instance.SetFailedState();
        other.gameObject.GetComponent<PlayerController>().ResetMovement();
    }
#pragma warning restore IDE0051
}
