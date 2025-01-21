using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class ResumeHandler : MonoBehaviour
{
    public static ResumeHandler Instance;
    [SerializeField] private GameObject resumeCountDownDisplay;
    [SerializeField] private Sprite[] resumeCountDownImages;
    
    private void Awake()
    {
        Instance = this;
    }

    public void Resume()
    {
        StartCoroutine(ResumeRoutine());
    }


    private IEnumerator ResumeRoutine()
    {
        resumeCountDownDisplay.GetComponent<Image>().sprite = resumeCountDownImages[3];
        
        resumeCountDownDisplay.SetActive(true);
        yield return new WaitForSecondsRealtime(1);
        
        resumeCountDownDisplay.GetComponent<Image>().sprite = resumeCountDownImages[2];
        yield return new WaitForSecondsRealtime(1);
        
        resumeCountDownDisplay.GetComponent<Image>().sprite = resumeCountDownImages[1];
        yield return new WaitForSecondsRealtime(1);

        resumeCountDownDisplay.GetComponent<Image>().sprite = resumeCountDownImages[0];

        GamePlayManager.Play();

        yield return new WaitForSecondsRealtime(0.5f);
        
        resumeCountDownDisplay.SetActive(false);
    }
}
