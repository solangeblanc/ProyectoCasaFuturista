using UnityEngine;

public class InterruptorLuz : MonoBehaviour
{
    public GameObject[] luces; // Acá arrastrás tus luces/focos
    public GameObject textoInteraccion;
    public KeyCode tecla = KeyCode.E;

    private bool jugadorCerca = false;

    void Update()
    {
        if (jugadorCerca && Input.GetKeyDown(tecla))
        {
            foreach (GameObject luz in luces)
            {
                luz.SetActive(!luz.activeSelf);
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            jugadorCerca = true;
            textoInteraccion.SetActive(true);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            jugadorCerca = false;
            textoInteraccion.SetActive(false);
        }
    }
}