using System.Collections.Generic;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public static float MainVolume = 1f;
    public static readonly float DefaultVolume = 0.5f;

    public int? LastBuildIndex = null;
    public string LEVEL_DATA_FILE_SAVE_LOCATION;

#pragma warning disable IDE0051
    void Awake()
#pragma warning restore IDE0051
    {
        // If there is an instance and it's not me, delete myself.
        Debug.LogFormat("Spawning instance of {0}", GetType().ToString());

        if (Instance != null && Instance != this)
        {
            Debug.LogFormat("destroy {0}", GetType().ToString());
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        // locally this is stored at ~/Library/Application Support/DefaultCompany/SquareOne/LevelData.dat
        LEVEL_DATA_FILE_SAVE_LOCATION = $"{Application.persistentDataPath}/LevelData.dat";
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

    void OnEnable()
    {
        Debug.Log("trying to get some saved data");
        if (File.Exists(LEVEL_DATA_FILE_SAVE_LOCATION))
        {
            Debug.Log("Holy shit we have saved data");
            using var stream = File.Open(LEVEL_DATA_FILE_SAVE_LOCATION, FileMode.Open);
            using var reader = new BinaryReader(stream, Encoding.UTF8, false);
            var fileContents = reader.ReadString();
            Debug.Log($"We have data!! {fileContents}");
            allLevelsSaveData = JsonConvert.DeserializeObject<AllLevelsSaveData>(fileContents);
        }
        else
        {
            Debug.Log("No data exists here, let's make new stuff");
            allLevelsSaveData = new();
        }
    }

    public bool CompassEnabled { get; set; } = true;

    public AllLevelsSaveData allLevelsSaveData;

    public class AllLevelsSaveData
    {
        public Dictionary<string, LevelSaveData> levelNameToSaveData;

        public AllLevelsSaveData()
        {
            levelNameToSaveData = new();
        }

        public class LevelSaveData
        {
            // fields must be public to properly get serialized to JSON
            public string name;
            public int numStars;

            public LevelSaveData(string name, int numStars)
            {
                this.name = name;
                this.numStars = numStars;
            }

            public static string GetLevelSaveNameFromLevelName(string levelName)
            {
                // just replace spaces with nothing, should be fine for now
                return levelName.Replace(" ", "");
            }
        }

        public void SaveLevelData(LevelSaveData levelSaveData)
        {
            LevelSaveData existingLevelSaveData = null;

            // if this is a higher score than what we've already achieved, save it as high score for this level
            if (levelNameToSaveData.ContainsKey(levelSaveData.name))
            {
                existingLevelSaveData = levelNameToSaveData[levelSaveData.name];
            }

            if (existingLevelSaveData == null || levelSaveData.numStars > existingLevelSaveData.numStars)
            {
                Debug.LogFormat("New high score for the level named {0} with a score of {1}", levelSaveData.name, levelSaveData.numStars);
                levelNameToSaveData[levelSaveData.name] = levelSaveData;
            }
            else
            {
                Debug.LogFormat("NOT a new high score for the level named {0} with a score of {1}", levelSaveData.name, levelSaveData.numStars);
            }
            var asJson = JsonConvert.SerializeObject(Instance.allLevelsSaveData);

            Debug.Log($"Wrote some json {asJson}");

            using var stream = File.Open(Instance.LEVEL_DATA_FILE_SAVE_LOCATION, FileMode.Create);
            using var writer = new BinaryWriter(stream, Encoding.UTF8, false);
            writer.Write(asJson);
        }

        public void DeleteSaveData()
        {
            if (File.Exists(Instance.LEVEL_DATA_FILE_SAVE_LOCATION))
            {
                File.Delete(Instance.LEVEL_DATA_FILE_SAVE_LOCATION);
                Debug.Log("Deleted save data!");
            }
            else
            {
                Debug.Log("No save data exists!");
            }
            levelNameToSaveData = new();
        }
    }
}
