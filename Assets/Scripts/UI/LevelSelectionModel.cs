using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelSelectionModel : MonoBehaviour
{
    [SerializeField] GameObject levelImage;
    [SerializeField] TextMeshProUGUI levelText;
    [SerializeField] Button toLevelButton;

    public void SetLevelSelectFields(string levelName)
    {
        // TODO what if level doesn't exist?

        levelImage.GetComponent<Image>().overrideSprite = Resources.Load<Sprite>($"Images/{levelName}");
        levelText.text = levelName;

        // TODO is this safe?
        toLevelButton.onClick.RemoveAllListeners();
        toLevelButton.onClick.AddListener(() => LevelTransitioner.ToNamedScene(levelName));
    }
}
