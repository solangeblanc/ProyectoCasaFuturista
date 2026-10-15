using UnityEngine;

public class AbrirTele : MonoBehaviour
{
    public GameObject canvasCambios;
    public GameObject menuPrincipal;
    public GameObject panelParedes;
    public GameObject panelPisos;
    public GameObject panelVentanas;
    public KeyCode tecla = KeyCode.Space;
    public GameObject jugador;
    private bool jugadorCerca = false;
    private MonoBehaviour[] scriptsJugador;

    void Awake()
    {
        if (jugador != null)
            scriptsJugador = jugador.GetComponents<MonoBehaviour>();
    }

    void OnTriggerEnter(Collider other) { if (other.CompareTag("Player")) jugadorCerca = true; }
    void OnTriggerExit(Collider other) { if (other.CompareTag("Player")) jugadorCerca = false; }

    void Update()
    {
        if (jugadorCerca && Input.GetKeyDown(tecla))
        {
            canvasCambios.SetActive(true);
            menuPrincipal.SetActive(true);
            panelParedes.SetActive(false);
            panelPisos.SetActive(false);
            panelVentanas.SetActive(false);

            // congela todo lo del jugador menos este script
            foreach (var s in scriptsJugador) { if (s != null && s.enabled) s.enabled = false; }

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }
    public void Descongelar()
    {
        foreach (var s in scriptsJugador) { if (s != null) s.enabled = true; }
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}