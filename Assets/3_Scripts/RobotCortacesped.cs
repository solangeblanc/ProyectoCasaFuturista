
using UnityEngine;

public class RobotCortacesped : MonoBehaviour
{
    public GameObject panelInformacionRobot;
    public MovimientoNPC movimientoNPC;

    private bool jugadorCerca = false;

    private void Start()
    {
        panelInformacionRobot.SetActive(false);
    }

    private void Update()
    {
        if (jugadorCerca && Input.GetKeyDown(KeyCode.Space))
        {
            movimientoNPC.IniciarRecorrido();
            panelInformacionRobot.SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            jugadorCerca = true;
            panelInformacionRobot.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            jugadorCerca = false;
            panelInformacionRobot.SetActive(false);
        }
    }
}

