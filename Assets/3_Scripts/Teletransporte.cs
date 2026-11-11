// Importa las clases básicas de Unity (MonoBehaviour, Transform, Collider, CharacterController, etc.)
using UnityEngine;
// Importa el nuevo Input System, necesario para leer el teclado con Keyboard.current y Key.
using UnityEngine.InputSystem;

// Script que teletransporta al jugador a otro punto (por ejemplo, de un piso al otro por la escalera)
// cuando está dentro de una zona y aprieta una tecla. Va en un objeto con un collider marcado como "Is Trigger".
// El nombre de la clase tiene que coincidir con el nombre del archivo: Teletransporte.cs
public class Teletransporte : MonoBehaviour
{
    // Punto de destino al que se mueve el jugador. Se arrastra desde la Hierarchy en el Inspector.
    // Se usa tanto su posición como su rotación, así que el jugador queda mirando hacia donde mire este objeto.
    public Transform destino;

    // Header solo agrega un título en el Inspector para ordenar las variables, no afecta la lógica.
    [Header("Tecla")]
    // [SerializeField] muestra una variable privada en el Inspector sin hacerla pública para otros scripts.
    // Tecla que activa el teletransporte. Por defecto, la barra espaciadora.
    [SerializeField] private Key teclaTeletransporte = Key.Space;

    [Header("Cartel de la escalera")]
    // Cartel (Canvas o panel) que avisa que se puede usar la escalera. Se muestra al entrar a la zona y se oculta al salir.
    [SerializeField] private GameObject cartelEscalera;

    // Recuerda si el jugador está dentro de la zona en este momento.
    private bool jugadorCerca = false;
    // Referencia al objeto del jugador que entró a la zona. Se guarda en OnTriggerEnter.
    private GameObject jugador;
    // Referencia al CharacterController del jugador, que hay que desactivar un instante para poder moverlo de lugar.
    private CharacterController controllerJugador;

    // Update se ejecuta en cada frame mientras el juego corre.
    private void Update()
    {
        // Solo teletransporta si se cumplen cuatro condiciones a la vez:
        // 1) el jugador está dentro de la zona,
        // 2) el juego está en curso (no en el menú ni en pausa),
        // 3) hay un teclado disponible,
        // 4) la tecla se apretó justo en este frame (wasPressedThisFrame es true solo en el frame en que se aprieta).
        if (jugadorCerca && GameManager.Instance.EstaJugando && Keyboard.current != null && Keyboard.current[teclaTeletransporte].wasPressedThisFrame)
        {
            Teletransportar();
        }
    }

    // Mueve al jugador al punto de destino.
    private void Teletransportar()
    {
        // Si falta el jugador o el destino (no se asignó en el Inspector), no hace nada.
        if (jugador == null || destino == null) return;

        // Desactiva el CharacterController antes de mover al jugador.
        // Si estuviera activo, pisaría el cambio de posición y el jugador volvería al lugar de origen.
        if (controllerJugador != null)
            controllerJugador.enabled = false;

        // Coloca al jugador en la posición del destino y le copia su rotación.
        jugador.transform.position = destino.position;
        jugador.transform.rotation = destino.rotation;

        // Vuelve a activar el CharacterController para que el jugador pueda moverse normalmente.
        if (controllerJugador != null)
            controllerJugador.enabled = true;

        // Oculta el cartel de la escalera, porque el jugador ya no está en la zona.
        if (cartelEscalera != null)
            cartelEscalera.SetActive(false);

        jugadorCerca = false; // para que no se teletransporte 2 veces seguidas
    }

    // Unity llama a este método automáticamente cuando otro objeto entra al collider de este objeto.
    // Para que funcione, el collider tiene que tener marcado "Is Trigger" y alguno de los dos objetos
    // (el jugador o la zona) tiene que tener un Rigidbody (el CharacterController también cuenta para esto).
    private void OnTriggerEnter(Collider other)
    {
        // Comprueba que quien entró sea el jugador, mirando su Tag "Player".
        if (other.CompareTag("Player"))
        {
            // Anota que el jugador está en la zona y guarda las referencias que necesita Teletransportar.
            jugadorCerca = true;
            jugador = other.gameObject;
            // Busca el CharacterController en el jugador. Si no tiene uno, queda en null y el código lo tiene en cuenta.
            controllerJugador = other.GetComponent<CharacterController>();

            // Muestra el cartel de la escalera.
            if (cartelEscalera != null)
                cartelEscalera.SetActive(true);
        }
    }

    // Unity llama a este método automáticamente cuando otro objeto sale del collider de este objeto.
    private void OnTriggerExit(Collider other)
    {
        // Comprueba que quien salió sea el jugador.
        if (other.CompareTag("Player"))
        {
            // Anota que el jugador ya no está en la zona y libera la referencia.
            jugadorCerca = false;
            jugador = null;

            // Oculta el cartel de la escalera.
            if (cartelEscalera != null)
                cartelEscalera.SetActive(false);
        }
    }
}