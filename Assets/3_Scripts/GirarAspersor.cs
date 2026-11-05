using UnityEngine;

public class GirarAspersor : MonoBehaviour
{
    public float velocidad = 120f;

    void Update()
    {
        transform.Rotate(0f, 0f, velocidad * Time.deltaTime);
    }
}