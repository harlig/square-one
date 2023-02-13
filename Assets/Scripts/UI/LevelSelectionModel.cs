using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class LevelSelectionModel : MonoBehaviour
{
    [SerializeField] GameObject levelImage;
    [SerializeField] TextMeshProUGUI levelNameText;
    [SerializeField] Button toLevelButton;
    [SerializeField] TextMeshProUGUI starsAchievedText;

    public void SetLevelSelectFields(string levelName)
    {
        // TODO what if level doesn't exist?

        levelImage.GetComponent<Image>().overrideSprite = Resources.Load<Sprite>($"Images/{levelName}");
        levelNameText.text = levelName;

        // TODO is this safe?
        toLevelButton.onClick.RemoveAllListeners();
    }

    public void SetStarsText(int starsAchieved)
    {
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

    public void AddOnClickListener(UnityAction onClickEvent)
    {
        toLevelButton.onClick.AddListener(onClickEvent);
    }
}
