using UnityEngine;
using UnityEngine.InputSystem;

public class RobotCortacesped : MonoBehaviour
{
    public GameObject panelInformacionRobot;
    public MovimientoNPC movimientoNPC;

    [Header("Ruedas")]
    public Transform ruedaIzquierda;
    public Transform ruedaDerecha;
    public float velocidadGiroRuedas = 500f; // Ajusta esta velocidad según necesites

    // AudioSource con el sonido de la cortadora. Se arrastra desde el Inspector.
    [Header("Sonido")]
    public AudioSource audioCortadora;

    private bool jugadorCerca = false;

    private void Start()
    {
        MostrarPanel(false);

        // si no se arrastró el AudioSource, lo busca en el robot que se mueve.
        if (audioCortadora == null && movimientoNPC != null)
            audioCortadora = movimientoNPC.GetComponentInChildren<AudioSource>();

        // arranca en silencio.
        if (audioCortadora != null)
            audioCortadora.Stop();
    }

    private void Update()
    {
        GameManager gm = GameManager.Instance;

        if (gm == null || !gm.EstaJugando)
        {
            MostrarPanel(false);
            ActualizarSonido(false);
            return;
        }

        MostrarPanel(jugadorCerca);

        // Control de movimiento con la barra espaciadora
        if (jugadorCerca && movimientoNPC != null && Keyboard.current != null)
        {
            if (Keyboard.current[Key.Space].wasPressedThisFrame)
            {
                if (movimientoNPC.EnMovimiento) movimientoNPC.Detener();
                else
                {
                    movimientoNPC.IniciarRecorrido();
                    // Avisa al registro de descubrimientos que el jugador activó el robot.
                    RegistroDescubrimientos.Descubrir("cortadora");
                }
            }
        }

        //  la cortadora suena solo mientras el robot está en movimiento.
        ActualizarSonido(movimientoNPC != null && movimientoNPC.EnMovimiento);

        // Si el robot está en movimiento, girar las ruedas
        if (movimientoNPC != null && movimientoNPC.EnMovimiento)
        {
            GirarRuedas();
        }
    }

    //  enciende o apaga el sonido. Comprueba isPlaying para no reiniciarlo en cada frame.
    private void ActualizarSonido(bool sonar)
    {
        if (audioCortadora == null || audioCortadora.clip == null) return;

        if (sonar && !audioCortadora.isPlaying) audioCortadora.Play();
        else if (!sonar && audioCortadora.isPlaying) audioCortadora.Stop();
    }

    private void GirarRuedas()
    {
        float giro = velocidadGiroRuedas * Time.deltaTime;

        // Dependiendo de hacia dónde apunte el eje local de tus ruedas, 
        // puedes cambiar el Vector3.right por Vector3.forward o Vector3.up.
        if (ruedaIzquierda != null)
            ruedaIzquierda.Rotate(Vector3.right, giro, Space.Self);

        if (ruedaDerecha != null)
            ruedaDerecha.Rotate(Vector3.right, giro, Space.Self);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) jugadorCerca = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player")) jugadorCerca = false;
    }

    private void MostrarPanel(bool mostrar)
    {
        if (panelInformacionRobot == null) return;
        if (panelInformacionRobot.activeSelf == mostrar) return;
        panelInformacionRobot.SetActive(mostrar);
    }
}