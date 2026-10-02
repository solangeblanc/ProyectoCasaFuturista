using UnityEngine;

public class CartelEscalera : MonoBehaviour
{
    public GameObject canvasCartel;

    void Start() { canvasCartel.SetActive(false); }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) canvasCartel.SetActive(true);
    }
    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player")) canvasCartel.SetActive(false);
    }
}