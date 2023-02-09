using System.IO;
using UnityEngine;

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
        DirectoryInfo dir = new($"{Application.dataPath}/Scenes/Game/");
        FileInfo[] gameScenes = dir.GetFiles("*.unity");

        float xBorderDistance = 1f - (X_PADDING * 2);
        // num levels - 1 because padding after last one is taken care of by border distance
        float width = (xBorderDistance - ((NUM_LEVELS_PER_ROW - 1) * X_PADDING)) / NUM_LEVELS_PER_ROW;

        // TODO handle multiple rows
        for (int ndx = 0; ndx < 5; ndx++)
        {
            FileInfo f = gameScenes[ndx];
            Debug.Log($"File name: {f.Name}");
            string levelName = f.Name.Split(".")[0];

            Debug.Log($"Time to spawn {levelName}");
            // spawn LevelSelectionModels
            GameObject levelSelector = Instantiate(levelSelectionPrefab, Vector3.zero, Quaternion.identity, transform);
            levelSelector.transform.localPosition = Vector3.zero;
            levelSelector.name = levelName;

            var rectTransform = levelSelector.GetComponent<RectTransform>();

            float originalWidth = rectTransform.anchorMax.x - rectTransform.anchorMin.x;
            float originalHeight = rectTransform.anchorMax.y - rectTransform.anchorMin.y;

            float xAnchorMin = X_PADDING + (ndx * width) + (ndx * X_PADDING);
            float newSizeRelativeToOldSize = width / originalWidth;

            float height = originalHeight * newSizeRelativeToOldSize;

            // TODO compute y anchor position
            rectTransform.anchorMin = new Vector2(xAnchorMin, 0.6f);
            rectTransform.anchorMax = new Vector2(xAnchorMin + width, 0.6f + height);

            rectTransform.anchoredPosition = Vector2.zero;
            levelSelector.GetComponent<LevelSelectionModel>().SetLevelSelectFields(levelName);
        }
    }
}
