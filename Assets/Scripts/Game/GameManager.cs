using UnityEngine;
using UnityEngine.SceneManagement;


public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
#pragma warning disable IDE0051
    void Awake()
#pragma warning restore IDE0051
    {
        // If there is an instance and it's not me, delete myself.
        Debug.LogFormat("Spawning instance of {0}", GetType().ToString());

        if (Instance != null && Instance != this)
        {
            Debug.LogFormat("destroy {0}", GetType().ToString());
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
    }

#pragma warning disable IDE0051
    void Start()
    {
        PlayMusic();
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
#pragma warning restore IDE0051
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }


    [RuntimeInitializeOnLoadMethod]
    public static void OnGameLoad()
    {
        // put any singletons we want in every scene here
        GameObject audio = (GameObject)Resources.Load("Prefabs/Sounds");
        GameObject music = (GameObject)Resources.Load("Prefabs/Music");
        GameObject gm = (GameObject)Resources.Load("Prefabs/GameManager");

        Instantiate(audio);
        Instantiate(music);
        Instantiate(gm);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        PlayMusic();
    }

    private void PlayMusic()
    {
        if (SceneManager.GetActiveScene().buildIndex < MainMenuController.MenuSceneIndex)
        {
            MusicController.Instance.PlayTitleMusic();
        }
        else
        {
            MusicController.Instance.PlayGameMusic();

        }
    }
}
