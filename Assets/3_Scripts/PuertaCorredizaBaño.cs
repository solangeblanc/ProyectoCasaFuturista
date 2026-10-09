using UnityEngine;
using UnityEngine.InputSystem;

public class PuertaCorredizaBaño : MonoBehaviour
{
    [Header("Tecla")]
    [SerializeField] private Key teclaAccion = Key.Space;

    [Header("Movimiento de la puerta")]
    [SerializeField] private Vector3 desplazamiento = new Vector3(9f, 0f, 0f);
    [SerializeField] private float velocidad = 2f;

    [Header("Cartel / Canvas")]
    [SerializeField] private GameObject canvasCartel;

    private Vector3 posicionCerrada;
    private Vector3 posicionAbierta;

    private bool abierta = false;
    private bool jugadorCerca = false;

    private void Start()
    {
        posicionCerrada = transform.localPosition;
        posicionAbierta = posicionCerrada + desplazamiento;

        if (canvasCartel != null)
        {
            canvasCartel.SetActive(false);
        }
    }

    private void Update()
    {
        // Solo permitimos interactuar si el jugador está cerca y presiona la tecla
        if (jugadorCerca && Keyboard.current != null && Keyboard.current[teclaAccion].wasPressedThisFrame)
        {
            abierta = !abierta; // Alterna el estado (si estaba abierta pasa a cerrada y viceversa)

            // Gestionamos el cartel según el nuevo estado
            if (canvasCartel != null)
            {
                canvasCartel.SetActive(!abierta); // Si se abre, se oculta el cartel; si se cierra, se muestra
            }
        }

        // Movimiento suave hacia el destino actual
        Vector3 destino = abierta ? posicionAbierta : posicionCerrada;

        transform.localPosition = Vector3.MoveTowards(
            transform.localPosition,
            destino,
            velocidad * Time.deltaTime
        );
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            jugadorCerca = true;

            if (!abierta && canvasCartel != null)
            {
                canvasCartel.SetActive(true);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            jugadorCerca = false;

            if (canvasCartel != null)
            {
                canvasCartel.SetActive(false);
            }
        }
    }
}
