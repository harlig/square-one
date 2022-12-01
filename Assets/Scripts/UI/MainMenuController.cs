using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    public static readonly int MenuSceneIndex = 2;

    public Slider volumeSlider;

    void Start()
    {
        MusicController.Instance.PlayTitleMusic();

        volumeSlider.onValueChanged.AddListener(delegate { AdjustVolume(); });
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

    void AdjustVolume()
    {
        Debug.LogFormat("Slider val {0}", volumeSlider.value);
        MusicController.Instance.AdjustVolume(volumeSlider.value);
    }

}
