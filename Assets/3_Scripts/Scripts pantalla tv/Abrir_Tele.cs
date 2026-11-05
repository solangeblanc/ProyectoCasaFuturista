using UnityEngine;

public class AbrirTele : MonoBehaviour
{
    public GameObject canvasCambios;
    public GameObject menuPrincipal;
    public GameObject panelParedes;
    public GameObject panelPisos;
    public GameObject panelVentanas;
    public GameObject panelPolarizado;
    [Header("Hint de Espacio")]
    public GameObject panelHintEspacio;
    public KeyCode tecla = KeyCode.Space;
    public GameObject jugador;
    private bool jugadorCerca = false;
    private MonoBehaviour[] scriptsJugador;

    void Awake()
    {
        if (jugador != null)
            scriptsJugador = jugador.GetComponents<MonoBehaviour>();

        // NO apagamos todo el canvas, solo los paneles de adentro
        if (menuPrincipal != null) menuPrincipal.SetActive(false);
        if (panelParedes != null) panelParedes.SetActive(false);
        if (panelPisos != null) panelPisos.SetActive(false);
        if (panelVentanas != null) panelVentanas.SetActive(false);
        if (panelPolarizado != null) panelPolarizado.SetActive(false);

        // El hint SI inicia prendido como pediste
        if (panelHintEspacio != null) panelHintEspacio.SetActive(true);
        // El canvas padre tiene que quedar prendido
        if (canvasCambios != null) canvasCambios.SetActive(true);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            jugadorCerca = true;
            if (panelHintEspacio != null) panelHintEspacio.SetActive(true);
        }
    }
    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            jugadorCerca = false;
            if (panelHintEspacio != null) panelHintEspacio.SetActive(false);
        }
    }

    void Update()
    {
        if (jugadorCerca && Input.GetKeyDown(tecla))
        {
            menuPrincipal.SetActive(true);
            panelParedes.SetActive(false);
            panelPisos.SetActive(false);
            panelVentanas.SetActive(false);
            panelPolarizado.SetActive(false);

            // Esconder hint mientras está el menú
            if (panelHintEspacio != null) panelHintEspacio.SetActive(false);

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

        if (jugadorCerca && panelHintEspacio != null)
            panelHintEspacio.SetActive(true);
    }
}