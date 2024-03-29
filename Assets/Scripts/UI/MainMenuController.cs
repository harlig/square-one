using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    public static readonly int MenuSceneIndex = 5;

    public Slider mainVolumeSlider;
    public Slider musicVolumeSlider;
    public Slider sfxVolumeSlider;

    [SerializeField] private GameObject freshGameMenu;
    [SerializeField] private GameObject hasSavedGameMenu;

    void Start()
    {
        MusicController.Instance.PlayTitleMusic();

        mainVolumeSlider.onValueChanged.AddListener(delegate { AdjustVolume(mainVolumeSlider, "main"); });
        musicVolumeSlider.onValueChanged.AddListener(delegate { AdjustVolume(musicVolumeSlider, "music"); });
        sfxVolumeSlider.onValueChanged.AddListener(delegate { AdjustVolume(sfxVolumeSlider, "sfx"); });

        mainVolumeSlider.value = GameManager.MainVolume;
        musicVolumeSlider.value = MusicController.Volume;
        sfxVolumeSlider.value = AudioController.Volume;

        SetActiveMenu();
    }

    public void SetActiveMenu()
    {
        if (GameManager.Instance.LastBuildIndex.HasValue)
        {
            freshGameMenu.SetActive(false);
            hasSavedGameMenu.SetActive(true);
        }
        else
        {
            freshGameMenu.SetActive(true);
            hasSavedGameMenu.SetActive(false);
        }
    }

    void OnDestroy()
    {
        mainVolumeSlider.onValueChanged.RemoveAllListeners();
        musicVolumeSlider.onValueChanged.RemoveAllListeners();
        sfxVolumeSlider.onValueChanged.RemoveAllListeners();
    }

    public void PlayGame()
    {
        Debug.LogFormat("If play button is not working, is the first level right after menu?");
        if (GameManager.Instance.LastBuildIndex.HasValue)
        {
            // TODO this should use LevelTransitioner which should handle audio
            SceneManager.LoadSceneAsync(GameManager.Instance.LastBuildIndex.Value, LoadSceneMode.Single);
        }
        else
        {
            ToLevelSelector();
        }
    }

    private void ToLevelSelector()
    {
        LevelTransitioner.ToLevelSelector();
    }

    public void QuitGame()
    {
        AudioController.Instance.PlayMenuClick();
        Application.Quit();

        DeselectCurrentlySelectedGameObject();
    }

    public void PlayClickSound()
    {
        AudioController.Instance.PlayMenuClick();
    }

    public void DeselectCurrentlySelectedGameObject()
    {
        GameObject.Find("EventSystem").GetComponent<UnityEngine.EventSystems.EventSystem>().SetSelectedGameObject(null);
    }

    void AdjustVolume(Slider slider, string type)
    {
        switch (type)
        {
            case "main":
                GameManager.MainVolume = slider.value;
                MusicController.Instance.AdjustVolume();
                AudioController.Instance.AdjustVolume();
                break;
            case "music":
                MusicController.Volume = slider.value;
                MusicController.Instance.AdjustVolume();
                break;
            case "sfx":
                AudioController.Volume = slider.value;
                AudioController.Instance.AdjustVolume();
                break;
        }
    }

    public void ClearBuildIndex()
    {
        LevelTransitioner.ClearBuildIndex();
    }

    public void DeleteSaveData()
    {
        GameManager.Instance.allLevelsSaveData.DeleteSaveData();
        GameManager.Instance.LastBuildIndex = null;
        SetActiveMenu();
    }
}
