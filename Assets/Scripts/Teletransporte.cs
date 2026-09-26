using UnityEngine;

public class Teletransporte : MonoBehaviour
{
    [Header("Punto a donde se teletransportará el jugador")]
    public Transform destino;

    private void OnTriggerEnter(Collider other)
    {
        // Verificamos si el objeto que entra es el jugador (tiene que tener la etiqueta "Player")
        if (other.CompareTag("Player"))
        {
            // Desactivamos el CharacterController o el movimiento momentáneamente para evitar bugs de física al moverlo
            CharacterController cc = other.GetComponent<CharacterController>();
            if (cc != null)
            {
                cc.enabled = false;
            }

            // Movemos al jugador a la posición del destino
            other.transform.position = destino.position;

            // Volvemos a activar el CharacterController
            if (cc != null)
            {
                cc.enabled = true;
            }
        }
    }
}