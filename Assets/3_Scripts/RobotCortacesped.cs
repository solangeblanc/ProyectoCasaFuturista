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

    private bool jugadorCerca = false;

    private void Start()
    {
        MostrarPanel(false);
    }

    private void Update()
    {
        GameManager gm = GameManager.Instance;

        if (gm == null || !gm.EstaJugando)
        {
            MostrarPanel(false);
            return;
        }

        MostrarPanel(jugadorCerca);

        // Control de movimiento con la barra espaciadora
        if (jugadorCerca && movimientoNPC != null && Keyboard.current != null)
        {
            if (Keyboard.current[Key.Space].wasPressedThisFrame)
            {
                if (movimientoNPC.EnMovimiento) movimientoNPC.Detener();
                else movimientoNPC.IniciarRecorrido();
            }
        }

        // Si el robot está en movimiento, girar las ruedas
        if (movimientoNPC != null && movimientoNPC.EnMovimiento)
        {
            GirarRuedas();
        }
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
