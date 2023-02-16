using System.Collections.Generic;

public class GroupLevelSelectionModel : LevelSelectionModel
{
    public void SetLevelSelectFields(string levelName, List<string> levelsInGroup, bool isUnlocked, int starsRequiredToUnlock)
    {
        SetupBasicLevelSelectFields(levelName);
        LevelsInGroup = levelsInGroup;
        SetStarsAchieved();
        SetUnlocked(isUnlocked, starsRequiredToUnlock);
    }

    public void DisableButton(int prevGroupStarsAchievedToUnlock)
    {
        toLevelButton.interactable = false;
        starsAchievedText.text = $"Get {prevGroupStarsAchievedToUnlock} stars in the previous group to unlock";
    }

    public List<string> LevelsInGroup { get; set; } = new();

    protected override void SetStarsAchieved()
    {
        int totalStarsForGroup = 0;
        foreach (string levelName in LevelsInGroup)
        {
            int? starsForLevel = GameManager.Instance.allLevelsSaveData.GetStarsForLevel(GameManager.AllLevelsSaveData.LevelSaveData.GetLevelSaveNameFromLevelName(levelName));
            if (starsForLevel.HasValue)
            {
                totalStarsForGroup += starsForLevel.Value;
            }
        }
        SetStarsAchieved(totalStarsForGroup);
    }

    private void SetUnlocked(bool isUnlocked, int starsRequired)
    {
        if (!isUnlocked)
        {
            DisableButton(starsRequired);
        }

    }
}
