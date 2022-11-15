using UnityEngine;

public abstract class Singleton<T> : MonoBehaviour
{
    // SINGLETON
    public static Singleton<T> Instance { get; private set; }

    void Awake()
    {
        // If there is an instance and it's not me, delete myself.

        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
            // don't destroy on load if we want to persist player instance throughout game
        }
    }
}
