using UnityEngine;

public class AbrirTele : MonoBehaviour
{//diferentes objetos para arrastrar al inspector 
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
    //para saber si el jugador esta cerca, empieza en falso
    private MonoBehaviour[] scriptsJugador;//una lista para guardar los movimientos del jugador

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

        // El hint inicia prendido 
        if (panelHintEspacio != null) panelHintEspacio.SetActive(true);
        // El canvas padre tiene que quedar prendido
        if (canvasCambios != null) canvasCambios.SetActive(true);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))//cuando el jugador entra en zona 
        {
            jugadorCerca = true;
            if (panelHintEspacio != null) panelHintEspacio.SetActive(true);//activar panel
        }
    }
    void OnTriggerExit(Collider other)//cuando el jugador sale 
    {
        if (other.CompareTag("Player"))
        {
            jugadorCerca = false;
            if (panelHintEspacio != null) panelHintEspacio.SetActive(false);//desactivar panel
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
            // se apagan todos los scripts del jugador para que no se pueda mover cuando esta activo el menu
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    public void Descongelar()
    {
        foreach (var s in scriptsJugador) { if (s != null) s.enabled = true; }
        //se vuelven a encender los scripts 
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (jugadorCerca && panelHintEspacio != null)
            panelHintEspacio.SetActive(true);
    }// si sigue estando cerca de la tele se vuelve a encender el panel hint 
}