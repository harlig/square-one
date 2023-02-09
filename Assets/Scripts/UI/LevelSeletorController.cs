using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelSeletorController : MonoBehaviour
{
    [SerializeField] GameObject levelSelectionPrefab;

    private readonly float X_PADDING = 0.05f;
    private readonly float Y_PADDING = 0.05f;

    private readonly int NUM_LEVELS_PER_ROW = 5;
    private readonly int NUM_ROWS = 2;

    // Start is called before the first frame update
    void Start()
    {
        float xBorderDistance = 1f - (X_PADDING * 2);
        // num levels - 1 because padding after last one is taken care of by border distance
        float width = (xBorderDistance - ((NUM_LEVELS_PER_ROW - 1) * X_PADDING)) / NUM_LEVELS_PER_ROW;

        var levelScenes = GetLevelScenes();

        // TODO handle multiple rows
        for (int ndx = 0; ndx < levelScenes.Count; ndx++)
        {
            string levelName = levelScenes[ndx];
            SpawnLevelSelector(width, levelName, ndx);
        }
    }

    private List<string> GetLevelScenes()
    {
        var levelScenes = new List<string>();
        for (int ndx = 0; ndx < SceneManager.sceneCountInBuildSettings; ndx++)
        {
            string sceneName = SceneUtility.GetScenePathByBuildIndex(ndx);
            Debug.Log(sceneName);
            if (!sceneName.Contains("/Scenes/Game/"))
            {
                continue;
            }
            string levelSceneName = sceneName[(sceneName.LastIndexOf("/") + 1)..].Split(".")[0];
            levelScenes.Add(levelSceneName);
        }
        return levelScenes;
    }

    private void SpawnLevelSelector(float width, string levelName, int xOffset)
    {
        Debug.Log($"Time to spawn {levelName}");
        // spawn LevelSelectionModels
        GameObject levelSelector = Instantiate(levelSelectionPrefab, Vector3.zero, Quaternion.identity, transform);
        levelSelector.transform.localPosition = Vector3.zero;
        levelSelector.name = levelName;

        var rectTransform = levelSelector.GetComponent<RectTransform>();

        float originalWidth = rectTransform.anchorMax.x - rectTransform.anchorMin.x;
        float originalHeight = rectTransform.anchorMax.y - rectTransform.anchorMin.y;

        float xAnchorMin = X_PADDING + (xOffset * width) + (xOffset * X_PADDING);
        float newSizeRelativeToOldSize = width / originalWidth;

        float height = originalHeight * newSizeRelativeToOldSize;

        // TODO compute y anchor position
        rectTransform.anchorMin = new Vector2(xAnchorMin, 0.6f);
        rectTransform.anchorMax = new Vector2(xAnchorMin + width, 0.6f + height);

        rectTransform.anchoredPosition = Vector2.zero;
        levelSelector.GetComponent<LevelSelectionModel>().SetLevelSelectFields(levelName);

    }
}
