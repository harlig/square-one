using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelTransitioner : MonoBehaviour
{

    public static void NextLevel()
    {
        AudioController.Instance.PlayMenuClick();
        SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex + 1, LoadSceneMode.Single);
    }

    public static void RestartLevel()
    {
        AudioController.Instance.PlayMenuClick();
        SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex, LoadSceneMode.Single);
    }

    public static void ToMenu()
    {
        AudioController.Instance.PlayMenuClick();
        if (SceneManager.GetActiveScene().buildIndex >= MainMenuController.MenuSceneIndex)
        {
            // if we are transitioning from within game, we set the index
            GameManager.Instance.LastBuildIndex = SceneManager.GetActiveScene().buildIndex;
        }
        SceneManager.LoadSceneAsync("Menu", LoadSceneMode.Single);
    }

    public static void ClearBuildIndex()
    {
        GameManager.Instance.LastBuildIndex = null;
    }

    private static int GetNextPossibleSceneBuildIndex()
    {
        return SceneManager.GetActiveScene().buildIndex + 1;
    }

    public void ToNextTitleScene()
    {
        AudioController.Instance.PlayMenuClick();
        Debug.LogFormat("Something going wrong with title screen changing? Does this main menu scene index look right: {0}", MainMenuController.MenuSceneIndex);
        if (GetNextPossibleSceneBuildIndex() >= MainMenuController.MenuSceneIndex)
        {
            Debug.LogFormat("Would be going to next index of {0} when menu is at index {1}. Going back to first scene.", GetNextPossibleSceneBuildIndex(), MainMenuController.MenuSceneIndex);
            SceneManager.LoadSceneAsync(0, LoadSceneMode.Single);
            return;
        }
        NextLevel();
    }

    public static void ToLevelSelector()
    {
        AudioController.Instance.PlayMenuClick();
        SceneManager.LoadSceneAsync("Level Selector", LoadSceneMode.Single);
    }

    public static void ToIceLevel1()
    {
        AudioController.Instance.PlayMenuClick();
        SceneManager.LoadSceneAsync("Ice Level 1", LoadSceneMode.Single);
    }
}
