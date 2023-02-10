using System.Collections;
using System.Collections.Generic;
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

        string levelSaveDataName = GameManager.AllLevelsSaveData.LevelSaveData.GetLevelSaveNameFromLevelName(levelName);
        if (GameManager.Instance.allLevelsSaveData.levelNameToSaveData.ContainsKey(levelSaveDataName))
        {
            var prevBest = GameManager.Instance.allLevelsSaveData.levelNameToSaveData[levelSaveDataName].numStars;

            string starsText;
            if (prevBest == 1)
            {
                starsText = "1 star";
            }
            else
            {
                starsText = $"{prevBest} stars";
            }
            starsAchievedText.text = starsText;
        }

        // TODO is this safe?
        toLevelButton.onClick.RemoveAllListeners();
    }

    public void AddOnClickListener(UnityAction onClickEvent)
    {
        toLevelButton.onClick.AddListener(onClickEvent);
    }
}
