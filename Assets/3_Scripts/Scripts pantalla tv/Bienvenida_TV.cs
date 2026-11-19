using UnityEngine;
using UnityEngine.InputSystem;

// Va en el objeto de la TV (con un collider "Is Trigger").
// Cuando el jugador está cerca y aprieta Espacio, abre el menú de la TV y congela al jugador.
public class Bienvenida_TV : MonoBehaviour
{
    public GameObject TextoEspacio; // PanelHintEspacio
    public GameObject Menu;         // Panel menu principal

    private bool jugadorCerca = false;
    private GameObject jugador;
    private PlayerControl playerControlScript; // el script que mueve al jugador (MVC/PlayerControl.cs)
    private PlayerInput playerInputScript;     // el componente del Input System

    void Start()
    {
        if (TextoEspacio != null) TextoEspacio.SetActive(false);
        if (Menu != null) Menu.SetActive(false);
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        jugadorCerca = true;
        // Busca al jugador completo por si se detecta solo una parte del cuerpo.
        jugador = other.transform.root.gameObject;

        // Se buscan directamente los scripts de movimiento y de input.
        playerControlScript = jugador.GetComponentInChildren<PlayerControl>();
        playerInputScript = jugador.GetComponentInChildren<PlayerInput>();

        // Solo muestra el hint si el menú NO está abierto.
        if (Menu != null && !Menu.activeSelf && TextoEspacio != null)
            TextoEspacio.SetActive(true);
    }

    void OnTriggerStay(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        // Si el menú está abierto, fuerza el hint apagado.
        if (Menu != null && Menu.activeSelf && TextoEspacio != null && TextoEspacio.activeSelf)
            TextoEspacio.SetActive(false);
    }

    void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        jugadorCerca = false;
        if (TextoEspacio != null) TextoEspacio.SetActive(false);
    }

    void Update()
    {
        // No abre el menú si el juego está en pausa o en la pantalla de bienvenida.
        if (GameManager.Instance != null && !GameManager.Instance.EstaJugando) return;

        if (jugadorCerca && Input.GetKeyDown(KeyCode.Space) && Menu != null && !Menu.activeSelf)
        {
            // Apaga el hint ANTES de prender el menú.
            if (TextoEspacio != null) TextoEspacio.SetActive(false);
            Menu.SetActive(true);

            // Avisa al registro de descubrimientos que el jugador usó la TV.
            RegistroDescubrimientos.Descubrir("tv");

            Cursor.lockState = CursorLockMode.None; // libera el mouse
            Cursor.visible = true;

            // Congela al jugador.
            if (playerControlScript != null) playerControlScript.enabled = false;
            if (playerInputScript != null) playerInputScript.enabled = false;
        }
    }

    // Lo llama el botón "Salir" desde Menu_Tele.
    public void CerrarMenu()
    {
        if (Menu != null) Menu.SetActive(false);

        Cursor.lockState = CursorLockMode.Locked; // bloquea y oculta el cursor
        Cursor.visible = false;

        // Devuelve el control al jugador.
        if (playerControlScript != null) playerControlScript.enabled = true;
        if (playerInputScript != null) playerInputScript.enabled = true;

        // Si por alguna razón la referencia al jugador se perdió, se vuelve a buscar.
        if (jugador == null) jugador = GameObject.FindGameObjectWithTag("Player");

        // Si sigue parado frente a la tele, vuelve a mostrar el aviso de presionar Espacio.
        if (jugadorCerca && TextoEspacio != null) TextoEspacio.SetActive(true);
    }
}