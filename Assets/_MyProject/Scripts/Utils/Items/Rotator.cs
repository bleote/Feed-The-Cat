using UnityEngine;

public class Rotator : MonoBehaviour
{
    [SerializeField] float movingSpeed = 0.018f;
    int sign = 1;

    void FixedUpdate()
    {
        transform.position = transform.position + Vector3.up * sign * movingSpeed;
        sign = transform.position.y > 10 ? -sign : (transform.position.y < 9 ? -sign : sign);
    }
}
