using UnityEngine;

public class CartelEscalera : MonoBehaviour
{
    public GameObject canvasCartel;

    void Start() { canvasCartel.SetActive(false); } //oculta el cartel al iniciar

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) canvasCartel.SetActive(true); // si el player entra en zona muestra el cartel
    }
    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player")) canvasCartel.SetActive(false);
    }
}