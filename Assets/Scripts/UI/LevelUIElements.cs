using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class LevelUIElements : Singleton<LevelUIElements>
{
    public delegate void TogglePauseAction(bool isPaused);
    public static event TogglePauseAction OnTogglePause;
    bool _isPaused = false;

    [SerializeField] private TextMeshProUGUI MoveCountText;
    [SerializeField] private TextMeshProUGUI PreviousBestStarsText;
    // TODO this isn't using the SuccessElements prefab, and is instead using the instance of SuccessElements in the GameLevelCanvas. Probably same for FailedElements. Should fix bc it's a problem
    [SerializeField] private GameObject SuccessElements;
    [SerializeField] private GameObject FailedElements;
    [SerializeField] private GameObject PauseMenuElements;
    [SerializeField] private GameObject PauseButton;
    [SerializeField] private GameObject Compass;

#pragma warning disable IDE0051
    void Start()
#pragma warning restore
    {
        if (Application.isMobilePlatform)
        {
            Compass.SetActive(false);
        }
        else if (GameManager.Instance.CompassEnabled)
        {
            Compass.SetActive(true);
        }
        else
        {
            Compass.SetActive(false);
        }
    }

#pragma warning disable IDE0051
    void Update()
    {
#pragma warning restore
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
    }


    public void SetMoveCountText(string text)
    {
        MoveCountText.text = text;
    }

    public void EnableMoveCountText()
    {
        if (!MoveCountText.gameObject.activeSelf)
            MoveCountText.gameObject.SetActive(true);
    }

    public void SetPreviousBestStarsText(string text)
    {
        PreviousBestStarsText.text = text;
    }

    public void EnablePreviousBestStarsText()
    {
        if (!PreviousBestStarsText.gameObject.activeSelf)
            PreviousBestStarsText.gameObject.SetActive(true);
    }

    // TODO possibly need a SuccessElements class if this gets annoying
    public void SetSuccessElementsStarsAchieved(int numStars)
    {
        TextMeshProUGUI starsAchievedText = null;
        TextMeshProUGUI[] childElements = SuccessElements.GetComponentsInChildren<TextMeshProUGUI>();
        foreach (TextMeshProUGUI element in childElements)
        {
            if (element.gameObject.tag == "Stars")
            {
                starsAchievedText = element;
                break;
            }
        }

        if (starsAchievedText == null)
        {
            throw new System.Exception("Not able to find text element to set stars achieved. Something is wrong with success elements.");
        }

        if (numStars == 1)
        {
            starsAchievedText.text = "1 star";
        }
        else
        {
            starsAchievedText.text = $"{numStars} stars";
        }
    }

    public GameObject GetSuccessElements()
    {
        return SuccessElements;
    }

    public GameObject GetFailedElements()
    {
        return FailedElements;
    }

    public void PauseLevel()
    {
        _isPaused = true;
        DoPause();
        MusicController.Instance.StopMusic();
        MusicController.Instance.PlayTitleMusic();
    }

    public void ResumeLevel()
    {
        _isPaused = false;
        DoPause();
        MusicController.Instance.StopMusic();
        MusicController.Instance.PlayGameMusic();
    }

    private void DoPause()
    {
        PauseButton.SetActive(!_isPaused);
        PauseMenuElements.SetActive(_isPaused);

        OnTogglePause?.Invoke(_isPaused);
    }

    private void TogglePause()
    {
        if (_isPaused)
        {
            ResumeLevel();
        }
        else
        {
            PauseLevel();
        }
    }

    public void ToggleCompass()
    {
        AudioController.Instance.PlayMenuClick();
        bool compassWillBeEnabled = !Compass.activeSelf;
        Compass.SetActive(compassWillBeEnabled);
        GameManager.Instance.CompassEnabled = compassWillBeEnabled;
    }

    public void DeselectClickedButton()
    {
        EventSystem.current.SetSelectedGameObject(null);
    }
}