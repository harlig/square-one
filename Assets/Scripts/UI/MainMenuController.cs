using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    public static readonly int MenuSceneIndex = 2;

    public Slider mainVolumeSlider;
    public Slider musicVolumeSlider;
    public Slider sfxVolumeSlider;

    void Start()
    {
        MusicController.Instance.PlayTitleMusic();

        mainVolumeSlider.onValueChanged.AddListener(delegate { AdjustVolume(mainVolumeSlider, "main"); });
        musicVolumeSlider.onValueChanged.AddListener(delegate { AdjustVolume(musicVolumeSlider, "music"); });
        sfxVolumeSlider.onValueChanged.AddListener(delegate { AdjustVolume(sfxVolumeSlider, "sfx"); });

        mainVolumeSlider.value = GameManager.MainVolume;
        musicVolumeSlider.value = MusicController.Volume;
        sfxVolumeSlider.value = AudioController.Volume;
    }

    public void PlayGame()
    {
        Debug.LogFormat("If play button is not working, is the first level right after menu?");
        LevelTransitioner.NextLevel();
    }

    public void QuitGame()
    {
        Application.Quit();

        DeselectCurrentlySelectedGameObject();
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
                MusicController.Instance.AdjustVolume(slider.value);
                AudioController.Instance.AdjustVolume(slider.value);
                break;
            case "music":
                MusicController.Instance.AdjustVolume(slider.value);
                break;
            case "sfx":
                AudioController.Instance.AdjustVolume(slider.value);
                break;
        }
    }

}
