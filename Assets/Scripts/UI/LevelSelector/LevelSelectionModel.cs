using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public abstract class LevelSelectionModel : MonoBehaviour
{
    [SerializeField] GameObject levelImage;
    [SerializeField] TextMeshProUGUI levelNameText;
    [SerializeField] protected Button toLevelButton;
    [SerializeField] protected TextMeshProUGUI starsAchievedText;
    protected string levelName;
    public int StarsAchieved { get; protected set; }

    protected abstract void SetStarsAchieved();

    protected void SetupBasicLevelSelectFields(string levelName)
    {
        this.levelName = levelName;
        // TODO what if level doesn't exist?

        levelImage.GetComponent<Image>().overrideSprite = Resources.Load<Sprite>($"Images/{levelName}");
        levelNameText.text = levelName;

        // TODO is this safe?
        toLevelButton.onClick.RemoveAllListeners();
    }

    public void AddOnClickListener(UnityAction onClickEvent)
    {
        toLevelButton.onClick.AddListener(onClickEvent);
    }

    protected void SetStarsAchieved(int starsAchieved)
    {
        StarsAchieved = starsAchieved;

        string starsText;
        if (starsAchieved == 1)
        {
            starsText = "1 star";
        }
        else
        {
            starsText = $"{starsAchieved} stars";
        }
        starsAchievedText.text = starsText;

    }
}
