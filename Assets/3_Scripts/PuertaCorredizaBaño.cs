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

    // sonidos de la puerta. Se arrastran desde el Project en el Inspector.
    [Header("Sonido")]
    // Sonido al abrir. Si no hay sonido de cerrar, este se usa para los dos casos.
    [SerializeField] private AudioClip sonidoAbrir;
    // Sonido al cerrar (opcional).
    [SerializeField] private AudioClip sonidoCerrar;
    // Volumen de las puertas, de 0 a 1.
    [Range(0f, 1f)]
    [SerializeField] private float volumenSonido = 1f;
    // Distancia hasta la cual el sonido se oye a volumen completo.
    // Tu escena es grande (las distancias son de decenas de unidades), por eso son valores altos.
    [SerializeField] private float distanciaMinimaSonido = 30f;
    // Distancia a partir de la cual el sonido ya no se oye.
    [SerializeField] private float distanciaMaximaSonido = 250f;

    private Vector3 posicionCerrada;
    private Vector3 posicionAbierta;

    private bool abierta = false;
    private bool jugadorCerca = false;

    //reproductor de sonido de esta puerta.
    private AudioSource audioPuerta;

    private void Start()
    {
        posicionCerrada = transform.localPosition;
        posicionAbierta = posicionCerrada + desplazamiento;

        if (canvasCartel != null)
        {
            canvasCartel.SetActive(false);
        }

        // usa el AudioSource de la puerta, o lo crea si no tiene uno.
        audioPuerta = GetComponent<AudioSource>();
        if (audioPuerta == null) audioPuerta = gameObject.AddComponent<AudioSource>();
        // No suena solo al empezar, y no se repite en bucle.
        audioPuerta.playOnAwake = false;
        audioPuerta.loop = false;
        // Sonido 3D: se oye más fuerte cuanto más cerca está la puerta del jugador.
        audioPuerta.spatialBlend = 1f;
        audioPuerta.minDistance = distanciaMinimaSonido;
        audioPuerta.maxDistance = distanciaMaximaSonido;
        audioPuerta.volume = volumenSonido;
    }

    private void Update()
    {

        // Solo permitimos interactuar si el jugador está cerca y presiona la tecla
        if (jugadorCerca && GameManager.Instance.EstaJugando && Keyboard.current != null && Keyboard.current[teclaAccion].wasPressedThisFrame)
        {
            abierta = !abierta; // Alterna el estado (si estaba abierta pasa a cerrada y viceversa)

            // reproduce el sonido que corresponde al nuevo estado.
            ReproducirSonido(abierta ? sonidoAbrir : sonidoCerrar);

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

    //  reproduce un sonido, cortando el anterior si todavía estaba sonando
    // (por si el jugador abre y cierra rápido).
    private void ReproducirSonido(AudioClip clip)
    {
        // Si no hay sonido de cerrar, usa el de abrir.
        if (clip == null) clip = sonidoAbrir;
        // Si no hay ningún sonido asignado, no hace nada.
        if (audioPuerta == null || clip == null) return;

        audioPuerta.Stop();
        audioPuerta.clip = clip;
        audioPuerta.Play();
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