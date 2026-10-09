using UnityEngine;

public class InterruptorLuz : MonoBehaviour
{
    public GameObject[] luces; // todas las luces que controla el interruptor 
    public GameObject textoInteraccion;
    public KeyCode tecla = KeyCode.Space; // tecla con la cual se activa

    private bool jugadorCerca = false;

    void Update()
    {
        if (jugadorCerca && Input.GetKeyDown(tecla)) // si esta apagada se prende 
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