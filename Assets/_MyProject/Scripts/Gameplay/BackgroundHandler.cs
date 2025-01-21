using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BackgroundHandler : MonoBehaviour
{
    [SerializeField] private Sprite relaxModeBackground;
    [SerializeField] private Sprite levelModeBackground;

    private Image currentBackground;

    private void Awake()
    {
        currentBackground = GetComponent<Image>();
    }

    void Start()
    {
        if (GamePlayManager.levelModeOn)
        {
            currentBackground.sprite = levelModeBackground;
        }
        else
        {
            currentBackground.sprite = relaxModeBackground;
        }
    }
}
