using UnityEngine;

public class CartelEscalera : MonoBehaviour
{
    [Header("Canvas")]
    [SerializeField] private GameObject canvasCartel;

    private void Start()
    {
        if (canvasCartel != null)
            canvasCartel.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (canvasCartel != null)
                canvasCartel.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (canvasCartel != null)
                canvasCartel.SetActive(false);
        }
    }
}