using UnityEngine;

public class ResetPlaneController : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        Debug.LogFormat("Something hit me!! {0}", other.gameObject.transform.position);
        other.gameObject.GetComponent<PlayerController>().ResetPosition();
    }
}
