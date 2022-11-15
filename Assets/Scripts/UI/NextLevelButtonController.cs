using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NextLevelButtonController : MonoBehaviour
{
    public void NextLevel()
    {
        Debug.Log("If play button is not working, is this the correct game level build index: {0}!");
        SceneManager.LoadScene("nice");
    }

}
