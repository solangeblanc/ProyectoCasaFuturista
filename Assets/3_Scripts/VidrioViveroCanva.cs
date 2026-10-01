using UnityEngine;
using UnityEngine.InputSystem;

public class VidrioVivero : MonoBehaviour
{
    [Header("Tecla")]
    [SerializeField] private Key teclaAccion = Key.E;

    [Header("Movimiento del vidrio")]
    [Tooltip("Hacia donde se corre el vidrio")]
    [SerializeField] private Vector3 desplazamiento = new Vector3(2f, 0f, 0f);
    [SerializeField] private float velocidad = 2f;

    [Header("Canvas")]
    [Tooltip("Arrastra aca tu Canvas del vivero")]
    [SerializeField] private GameObject canvasCartel;

    private Vector3 posicionCerrada;
    private Vector3 posicionAbierta;
    private bool abierto = false;
    private bool jugadorCerca = false;

    private void Start()
    {
        posicionCerrada = transform.localPosition;
        posicionAbierta = posicionCerrada + desplazamiento;

        if (canvasCartel != null)
            canvasCartel.SetActive(false);
    }

    private void Update()
    {
        if (jugadorCerca && Keyboard.current != null && Keyboard.current[teclaAccion].wasPressedThisFrame)
        {
            abierto = !abierto;
            if (canvasCartel != null)
                canvasCartel.SetActive(false);
        }

        Vector3 destino = abierto ? posicionAbierta : posicionCerrada;
        transform.localPosition = Vector3.MoveTowards(transform.localPosition, destino, velocidad * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            jugadorCerca = true;
            if (!abierto && canvasCartel != null)
                canvasCartel.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            jugadorCerca = false;
            if (canvasCartel != null)
                canvasCartel.SetActive(false);
        }
    }
}