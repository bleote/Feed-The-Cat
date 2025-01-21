using UnityEngine;
using UnityEngine.UI;

public class MapScrollController : MonoBehaviour
{
    [SerializeField] private ScrollRect scrollRect;
    [SerializeField] private MapHandler mapHandler;
    [SerializeField] private RectTransform[] levels; // Array of level RectTransforms
    private int levelIndex;

    private void Start()
    {
        // Set the initial scroll position based on a specific level index
        
        levelIndex = mapHandler.unlockedLevels; // Change this index to set the scroll position for different levels

        if (levelIndex == 1)
        {
            SetScrollPositionByLevel(levelIndex);
        }
        else
        {
            SetScrollPositionByLevel(levelIndex -1);
        }

    }

    public void SetScrollPositionByLevel(int levelIndex)
    {
        if (levelIndex < 0 || levelIndex >= levels.Length)
        {
            // Debug.Log("Invalid level index");
            return;
        }

        // Get the Y positions of the first and last levels
        float bottomY = levels[0].position.y;
        float topY = levels[levels.Length - 1].position.y;

        // Get the Y position of the specific level
        float levelYPosition = levels[levelIndex].position.y;

        // Normalize this Y position relative to the height of the Level Handler
        float normalizedPosition = NormalizeYPosition(levelYPosition, bottomY, topY);

        // Set the verticalNormalizedPosition of the ScrollRect
        scrollRect.verticalNormalizedPosition = normalizedPosition;
    }

    private float NormalizeYPosition(float yPosition, float bottomY, float topY)
    {
        // Convert Y position to a normalized value between 0 and 1
        return Mathf.InverseLerp(bottomY, topY, yPosition);
    }
}
