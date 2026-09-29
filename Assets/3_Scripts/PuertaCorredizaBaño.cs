using UnityEngine;
using UnityEngine.InputSystem;

public class PuertaCorrediza : MonoBehaviour
{
    [Header("Tecla")]
    [Tooltip("Tecla que abre/cierra la puerta.")]
    [SerializeField] private Key teclaAccion = Key.Space;

    [Header("Movimiento de la puerta")]
    [Tooltip("Hacia dónde y cuánto se desliza la puerta, en espacio LOCAL (relativo a su posición inicial). Ej: (2,0,0) la desliza 2 unidades sobre el eje X.")]
    [SerializeField] private Vector3 desplazamiento = new Vector3(2f, 0f, 0f);

    [Tooltip("Velocidad de apertura/cierre (unidades por segundo).")]
    [SerializeField] private float velocidad = 2f;

    private Vector3 posicionCerrada;
    private Vector3 posicionAbierta;
    private bool abierta = false;

    private void Start()
    {
        // Guardamos la posición inicial como "cerrada" y calculamos la "abierta" sumando el desplazamiento
        posicionCerrada = transform.localPosition;
        posicionAbierta = posicionCerrada + desplazamiento;
    }

    private void Update()
    {
        // Detecta la tecla usando el Input System nuevo (no hace falta configurar Input Actions)
        if (Keyboard.current != null && Keyboard.current[teclaAccion].wasPressedThisFrame)
        {
            abierta = !abierta;
        }

        Vector3 destino = abierta ? posicionAbierta : posicionCerrada;
        transform.localPosition = Vector3.MoveTowards(transform.localPosition, destino, velocidad * Time.deltaTime);
    }
}