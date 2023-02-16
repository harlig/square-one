using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroupLevelSelectionModel : LevelSelectionModel
{

    public void DisableButton(int prevGroupStarsAchievedToUnlock)
    {
        toLevelButton.interactable = false;
        starsAchievedText.text = $"Get {prevGroupStarsAchievedToUnlock} stars in the previous group to unlock";
    }
}
