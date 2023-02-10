using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelSeletorController : MonoBehaviour
{
    [SerializeField] GameObject levelSelectionPrefab;
    [SerializeField] GameObject levelSelectionGroupMenu;
    [SerializeField] GameObject levelSelectionMenu;
    [SerializeField] Button backButton;

    private static readonly float X_PADDING = 0.05f;
    private static readonly float X_BORDER_DISTANCE = 1f - (X_PADDING * 2);

    private static readonly int NUM_LEVELS_PER_ROW = 5;

    // Start is called before the first frame update
    void Start()
    {
        levelSelectionGroupMenu.SetActive(false);
        levelSelectionMenu.SetActive(false);
        backButton.onClick.RemoveAllListeners();
        backButton.onClick.AddListener(() => LevelTransitioner.ToMenu());

        // num levels - 1 because padding after last one is taken care of by border distance

        var levelScenes = GetLevelScenes();

        SpawnLevelSelectorGroupings(levelScenes, 5);

        // TODO handle multiple rows
        // for (int ndx = 0; ndx < levelScenes.Count; ndx++)
        // {
        //     string levelName = levelScenes[ndx];
        //     SpawnLevelSelector(width, levelName, ndx, levelSelectionMenu);
        // }
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

    private void SpawnLevelSelectorGroupings(List<string> levelScenes, int numLevelsPerGrouping)
    {
        int numGroupings = (int)Mathf.Ceil(levelScenes.Count / (float)numLevelsPerGrouping);
        // num levels - 1 because padding after last one is taken care of by border distance
        float width = (X_BORDER_DISTANCE - ((numGroupings - 1) * X_PADDING)) / numGroupings;

        int numGroupsCreated = 0;
        List<string> levelsForThisGroup = new();
        int startGroupNdx = 0, endGroupNdx = 0;

        for (int ndx = 0; ndx < levelScenes.Count; ndx++)
        {
            endGroupNdx++;
            levelsForThisGroup.Add(levelScenes[ndx]);
            // create the grouping every numLevelsPerGrouping and for last group
            if ((ndx + 1) % numLevelsPerGrouping == 0 || ndx == levelScenes.Count - 1)
            {
                // instantiate a level selection prefab, set its name to "levels x - y", set its onClick to disable the grouping page and show the level selection prefab
                SpawnLevelSelectorGrouping(width, startGroupNdx, ndx, numGroupsCreated, levelsForThisGroup);

                levelsForThisGroup = new();
                numGroupsCreated++;
                startGroupNdx = endGroupNdx = ndx + 1;
            }
        }

        levelSelectionGroupMenu.SetActive(true);
    }

    private void SpawnLevelSelectorGrouping(float width, int startGroupNdx, int endGroupNdx, int xOffset, List<string> levelsInGroup)
    {
        string groupName = $"Levels {startGroupNdx + 1} - {endGroupNdx + 1}";
        Debug.Log($"Time to spawn {groupName}");
        // spawn LevelSelectionModels
        GameObject levelSelectorGroup = Instantiate(levelSelectionPrefab, Vector3.zero, Quaternion.identity, levelSelectionGroupMenu.transform);
        levelSelectorGroup.transform.localPosition = Vector3.zero;
        levelSelectorGroup.name = groupName;

        var rectTransform = levelSelectorGroup.GetComponent<RectTransform>();

        float originalWidth = rectTransform.anchorMax.x - rectTransform.anchorMin.x;
        float originalHeight = rectTransform.anchorMax.y - rectTransform.anchorMin.y;

        float xAnchorMin = X_PADDING + (xOffset * width) + (xOffset * X_PADDING);
        float newSizeRelativeToOldSize = width / originalWidth;

        float height = originalHeight * newSizeRelativeToOldSize;

        // TODO compute y anchor position
        rectTransform.anchorMin = new Vector2(xAnchorMin, 0.1f);
        rectTransform.anchorMax = new Vector2(xAnchorMin + width, 0.1f + height);

        rectTransform.anchoredPosition = Vector2.zero;
        levelSelectorGroup.GetComponent<LevelSelectionModel>().SetLevelSelectFields(groupName);

        levelSelectorGroup.GetComponent<LevelSelectionModel>().AddOnClickListener(() =>
        {
            Debug.Log($"Time to spawn stuff in this group with size {levelsInGroup.Count}");
            AudioController.Instance.PlayMenuClick();

            // delete all existing selectors in the menu

            for (int ndx = 0; ndx < levelsInGroup.Count; ndx++)
            {
                Debug.Log($"Spawning this: {levelsInGroup[ndx]}");
                SpawnLevelSelector(levelsInGroup[ndx], ndx, levelSelectionMenu);
            }

            backButton.onClick.RemoveAllListeners();
            backButton.onClick.AddListener(() =>
            {
                AudioController.Instance.PlayMenuClick();
                levelSelectionGroupMenu.SetActive(true);
                levelSelectionMenu.SetActive(false);
                foreach (Transform levelSelectionMenuChild in levelSelectionMenu.transform)
                {
                    Destroy(levelSelectionMenuChild.gameObject);
                }
                backButton.onClick.RemoveAllListeners();
                backButton.onClick.AddListener(() => LevelTransitioner.ToMenu());
            });

            levelSelectionGroupMenu.SetActive(false);
            levelSelectionMenu.SetActive(true);
        });
    }

    private void SpawnLevelSelector(string levelName, int xOffset, GameObject parent)
    {
        float width = (X_BORDER_DISTANCE - ((NUM_LEVELS_PER_ROW - 1) * X_PADDING)) / NUM_LEVELS_PER_ROW;
        Debug.Log($"Time to spawn {levelName}");
        // spawn LevelSelectionModels
        GameObject levelSelector = Instantiate(levelSelectionPrefab, Vector3.zero, Quaternion.identity, parent.transform);
        levelSelector.transform.localPosition = Vector3.zero;
        levelSelector.name = levelName;

        var rectTransform = levelSelector.GetComponent<RectTransform>();

        float originalWidth = rectTransform.anchorMax.x - rectTransform.anchorMin.x;
        float originalHeight = rectTransform.anchorMax.y - rectTransform.anchorMin.y;

        float xAnchorMin = X_PADDING + (xOffset * width) + (xOffset * X_PADDING);
        float newSizeRelativeToOldSize = width / originalWidth;

        float height = originalHeight * newSizeRelativeToOldSize;

        // TODO compute y anchor position
        rectTransform.anchorMin = new Vector2(xAnchorMin, 0.4f);
        rectTransform.anchorMax = new Vector2(xAnchorMin + width, 0.4f + height);

        rectTransform.anchoredPosition = Vector2.zero;
        levelSelector.GetComponent<LevelSelectionModel>().SetLevelSelectFields(levelName);
        levelSelector.GetComponent<LevelSelectionModel>().AddOnClickListener(() => LevelTransitioner.ToNamedScene(levelName));
    }
}
