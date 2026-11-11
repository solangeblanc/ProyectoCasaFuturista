// Importa las clases básicas de Unity (MonoBehaviour, GameObject, Collider, KeyCode, Input, etc.)
using UnityEngine;

// Script de un interruptor de luz: cuando el jugador está cerca y aprieta una tecla, enciende o apaga
// todas las luces que tiene asignadas. Muestra un texto de ayuda mientras el jugador está en la zona.
// Va en un objeto con un collider marcado como "Is Trigger".
// El nombre de la clase tiene que coincidir con el nombre del archivo.
public class InterruptorLuz : MonoBehaviour
{
    // Lista de objetos de luz que controla este interruptor. Se arrastran desde la Hierarchy en el Inspector.
    public GameObject[] luces; // todas las luces que controla el interruptor 
    // Texto o cartel que indica que se puede interactuar (por ejemplo, "ESPACIO para encender").
    // Se muestra al entrar a la zona y se oculta al salir.
    public GameObject textoInteraccion;
    // Tecla que activa el interruptor. Por defecto, la barra espaciadora. Se puede cambiar en el Inspector.
    public KeyCode tecla = KeyCode.Space; // tecla con la cual se activa
    // NUEVO: si está tildado, las luces de este interruptor arrancan encendidas; si no, arrancan apagadas.
    // Por defecto está sin tildar, así que todas las luces empiezan apagadas.
    public bool empiezanEncendidas = false;

    // Recuerda si el jugador está dentro de la zona del interruptor en este momento.
    private bool jugadorCerca = false;

    // NUEVO: Start se ejecuta una sola vez, antes del primer frame (antes de que se dibuje nada).
    void Start()
    {
        // Recorre todas las luces asignadas, una por una.
        foreach (GameObject luz in luces)
        {
            // Las deja todas en el mismo estado inicial, el que indique "empiezanEncendidas".
            // Así el estado no depende de cómo hayan quedado tildadas en la escena,
            // y todas las luces del interruptor quedan sincronizadas.
            luz.SetActive(empiezanEncendidas);
        }
    }

    // Update se ejecuta en cada frame mientras el juego corre.
    void Update()
    {
        // Solo actúa si se cumplen tres condiciones a la vez:
        // 1) el jugador está dentro de la zona,
        // 2) el juego está en curso (no en el menú ni en pausa),
        // 3) la tecla se apretó justo en este frame (GetKeyDown es true solo en el frame en que se aprieta).
        if (jugadorCerca && GameManager.Instance.EstaJugando && Input.GetKeyDown(tecla))
        {
            // Recorre todas las luces asignadas, una por una.
            foreach (GameObject luz in luces)
            {
                // Invierte el estado de cada luz: si estaba encendida (activa) la apaga, y viceversa.
                luz.SetActive(!luz.activeSelf);
            }
        }
    }

    // Unity llama a este método automáticamente cuando otro objeto entra al collider de este objeto.
    // Para que funcione, el collider tiene que tener marcado "Is Trigger" y alguno de los dos objetos
    // (el jugador o el interruptor) tiene que tener un Rigidbody (el CharacterController también cuenta).
    void OnTriggerEnter(Collider other)
    {
        // Comprueba que quien entró sea el jugador, mirando su Tag "Player".
        if (other.CompareTag("Player"))
        {
            // Anota que el jugador está en la zona y muestra el texto de ayuda.
            jugadorCerca = true;
            textoInteraccion.SetActive(true);
        }
    }

    // Unity llama a este método automáticamente cuando otro objeto sale del collider de este objeto.
    void OnTriggerExit(Collider other)
    {
        // Comprueba que quien salió sea el jugador.
        if (other.CompareTag("Player"))
        {
            // Anota que el jugador ya no está en la zona y oculta el texto de ayuda.
            jugadorCerca = false;
            textoInteraccion.SetActive(false);
        }
    }
}