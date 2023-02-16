using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingleLevelSelectionModel : LevelSelectionModel
{
    public void SetLevelSelectFields(string levelName)
    {
        SetupBasicLevelSelectFields(levelName);
        SetStarsAchieved();
    }

    protected override void SetStarsAchieved()
    {
        string levelSaveDataName = GameManager.AllLevelsSaveData.LevelSaveData.GetLevelSaveNameFromLevelName(levelName);
        int? prevBestStars = GameManager.Instance.allLevelsSaveData.GetStarsForLevel(levelSaveDataName);
        if (prevBestStars.HasValue)
        {
            SetStarsAchieved(prevBestStars.Value);
        }
    }
}
