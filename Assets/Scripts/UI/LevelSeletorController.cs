using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelSeletorController : MonoBehaviour
{
    [SerializeField] GameObject levelSelectionPrefab;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Time to spawn ice level1");
        // spawn LevelSelectionModels
        GameObject iceLevel1 = Instantiate(levelSelectionPrefab, Vector3.zero, Quaternion.identity, transform);
        iceLevel1.transform.localPosition = Vector3.zero;

        var rectTransform = iceLevel1.GetComponent<RectTransform>();
        rectTransform.anchorMin = new Vector2(0.25f, 0.6f);
        rectTransform.anchorMax = new Vector2(0.4f, 0.95f);
        rectTransform.anchoredPosition = Vector2.zero;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
