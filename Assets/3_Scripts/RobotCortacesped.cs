using UnityEngine;
using UnityEngine.InputSystem;

public class RobotCortacesped : MonoBehaviour
{
    public GameObject panelInformacionRobot;
    public MovimientoNPC movimientoNPC;

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

        if (!jugadorCerca || movimientoNPC == null) return;
        if (Keyboard.current == null) return;

        if (Keyboard.current[Key.Space].wasPressedThisFrame)
        {
            if (movimientoNPC.EnMovimiento) movimientoNPC.Detener();
            else movimientoNPC.IniciarRecorrido();
        }
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

