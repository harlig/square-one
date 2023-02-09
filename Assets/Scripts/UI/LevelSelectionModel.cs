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
        levelImage.GetComponent<Image>().overrideSprite = Resources.Load<Sprite>($"Images/{levelName}");
        levelText.text = levelName;
        toLevelButton.onClick.AddListener(() => LevelTransitioner.ToIceLevel1());
    }
}
