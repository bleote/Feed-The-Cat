using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomScroller : MonoBehaviour
{
    public Vector2 lastMousePos;
    public float movementStartRange;
    public float movementEndRange;
    public Vector2 targetPosition;
    public RectTransform level;
    float startTime;
    [SerializeField] private float speed;
    public float swipeSpeed = 1;
    // Start is called before the first frame update
    void Start()
    {
        targetPosition = GetComponent<RectTransform>().anchoredPosition;
        level = GetComponent<RectTransform>();
    }

    // Update is called once per frame
    void Update()
    {
        level.anchoredPosition = Vector3.Lerp(level.anchoredPosition, targetPosition, speed );
    }
    public void Drag()
    {
        float change = Input.mousePosition.y - lastMousePos.y;
        swipeSpeed = (Time.time - startTime) > 0 ? (Time.time - startTime) : 1;
        targetPosition.y += (change) / (swipeSpeed*10);
        if (targetPosition.y < movementStartRange)
            targetPosition.y = movementStartRange;
        if (targetPosition.y > movementEndRange)
            targetPosition.y = movementEndRange;
        lastMousePos = Input.mousePosition;
        startTime = Time.time;
    }
    public void DragStart()
    {
        lastMousePos = Input.mousePosition;
        startTime = Time.time;

    }
    public void DragEnd()
    {
        float change = Input.mousePosition.y - lastMousePos.y;
        targetPosition.y += change;
        if (targetPosition.y < movementStartRange)
            targetPosition.y = movementStartRange;
        if (targetPosition.y > movementEndRange)
            targetPosition.y = movementEndRange;

    }
}
