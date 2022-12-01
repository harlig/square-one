using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    public static readonly int MenuSceneIndex = 5;

    public Slider mainVolumeSlider;
    public Slider musicVolumeSlider;
    public Slider sfxVolumeSlider;

    public TextMeshProUGUI playButtonText;

    void Start()
    {
        MusicController.Instance.PlayTitleMusic();

        mainVolumeSlider.onValueChanged.AddListener(delegate { AdjustVolume(mainVolumeSlider, "main"); });
        musicVolumeSlider.onValueChanged.AddListener(delegate { AdjustVolume(musicVolumeSlider, "music"); });
        sfxVolumeSlider.onValueChanged.AddListener(delegate { AdjustVolume(sfxVolumeSlider, "sfx"); });

        mainVolumeSlider.value = GameManager.MainVolume;
        musicVolumeSlider.value = MusicController.Volume;
        sfxVolumeSlider.value = AudioController.Volume;

        if (GameManager.Instance.LastBuildIndex.HasValue)
        {
            playButtonText.text = "CONTINUE";
        }
    }

    public void PlayGame()
    {
        AudioController.Instance.PlayMenuClick();
        Debug.LogFormat("If play button is not working, is the first level right after menu?");
        if (GameManager.Instance.LastBuildIndex.HasValue)
        {
            SceneManager.LoadSceneAsync(GameManager.Instance.LastBuildIndex.Value, LoadSceneMode.Single);
        }
        else
        {
            LevelTransitioner.NextLevel();

        }
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
