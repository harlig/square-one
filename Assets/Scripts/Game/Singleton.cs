using UnityEngine;

public abstract class Singleton<T> : MonoBehaviour
{
    // SINGLETON
    public static Singleton<T> Instance { get; private set; }

    // private static Singleton<T> _Instance;

    void Awake()
    {
        // If there is an instance and it's not me, delete myself.

        Debug.LogFormat("Spawning instance of {0}", GetType().ToString());

        if (Instance != null && Instance != this)
        {
            Debug.Log("destroy");
            Destroy(this);
        }
        else
        {
            Debug.Log("spawn");
            Instance = this;
            // don't destroy on load if we want to persist player instance throughout game
        }
    }
}
