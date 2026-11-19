using UnityEngine;
using UnityEngine.InputSystem;

// Marca una función como descubierta cuando el jugador está en la zona y aprieta la tecla.
// Va en el mismo objeto que el trigger de la ventana (collider "Is Trigger"), sin tocar el script
// que ya abre la ventana. Id para ventanas: "ventanas".
public class DescubrirAlInteractuar : MonoBehaviour
{
    [SerializeField] private string idDescubrimiento = "ventanas";
    [SerializeField] private Key tecla = Key.Space;

    private bool jugadorCerca = false;

    private void Update()
    {
        if (!jugadorCerca) return;
        if (GameManager.Instance != null && !GameManager.Instance.EstaJugando) return;

        if (Keyboard.current != null && Keyboard.current[tecla].wasPressedThisFrame)
            RegistroDescubrimientos.Descubrir(idDescubrimiento);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) jugadorCerca = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player")) jugadorCerca = false;
    }
}
