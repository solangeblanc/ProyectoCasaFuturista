using UnityEngine;

public class Teletransporte : MonoBehaviour
{
    public Transform destino;

    [Header("Cartel de la escalera")]
    [SerializeField] private GameObject cartelEscalera; // <--- 1. Añadimos esta variable

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            CharacterController controller = other.GetComponent<CharacterController>();

            if (controller != null)
                controller.enabled = false;

            other.transform.position = destino.position;
            other.transform.rotation = destino.rotation;

            if (controller != null)
                controller.enabled = true;

            // <--- 2. APAGAMOS EL CARTEL AQUÍ A LA FUERZA
            if (cartelEscalera != null)
            {
                cartelEscalera.SetActive(false);
            }
        }
    }
}