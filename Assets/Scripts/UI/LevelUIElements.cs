using TMPro;
using UnityEngine;

public class LevelUIElements : Singleton<LevelUIElements>
{
    [SerializeField] private TextMeshProUGUI moveCountText;
    [SerializeField] private GameObject SuccessElements;
    [SerializeField] private GameObject FailedElements;

    public void SetMoveCountText(string text)
    {
        moveCountText.text = text;
    }

    public GameObject GetSuccessElements()
    {
        return SuccessElements;
    }

    public GameObject GetFailedElements()
    {
        return FailedElements;
    }
}