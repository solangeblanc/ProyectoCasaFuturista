
// Importa las clases básicas de Unity (MonoBehaviour, Transform, Physics, ParticleSystem, etc.)
using UnityEngine;
// Importa el nuevo Input System, necesario para leer el teclado con Keyboard.current y Key.
using UnityEngine.InputSystem;

// Script del sistema de riego del jardín: cuando el jugador mira el botón del riego (este objeto o sus hijos)
// y aprieta una tecla, se enciende o se apaga el agua. Además muestra dos carteles:
// uno que avisa que se puede regar y otro con información de las especies mientras se riega.
// El nombre de la clase tiene que coincidir con el nombre del archivo: SistemaRiego.cs
public class SistemaRiego : MonoBehaviour
{
    // Header solo agrega un título en el Inspector para ordenar las variables, no afecta la lógica.
    [Header("Tecla")]
    // [SerializeField] muestra una variable privada en el Inspector sin hacerla pública para otros scripts.
    // Tecla que enciende o apaga el riego. Por defecto, la barra espaciadora.
    [SerializeField] private Key teclaRiego = Key.Space;

    [Header("Rayo")]
    // Transform desde el que sale el rayo que detecta a qué está mirando el jugador (normalmente la cámara).
    // Si se deja vacío, Start usa la cámara principal.
    [SerializeField] private Transform camara;
    // Distancia máxima, en unidades de Unity, a la que el jugador puede estar para interactuar con el botón.
    [SerializeField] private float distanciaMaxima = 4f;
    // Capas de objetos que el rayo puede detectar. El valor ~0 es "todas las capas".
    [SerializeField] private LayerMask capas = ~0;

    [Header("Agua")]
    // Sistema de partículas que representa el agua del riego.
    [SerializeField] private ParticleSystem agua;

    [Header("Cartel 'ESPACIO para regar'")]
    // Cartel que avisa que se puede apretar la tecla para regar. Se ve mirando el botón, antes de regar.
    [SerializeField] private GameObject canvasCartel;

    [Header("Canvas informativo de especies")]
    // Canvas con información de las especies de plantas. Se ve mirando el botón, mientras se riega.
    [SerializeField] private GameObject canvasInfo;

    // Recuerda si el riego está encendido en este momento.
    private bool regando = false;

    // Start se ejecuta una sola vez, antes del primer frame.
    private void Start()
    {
        // Si no se asignó la cámara en el Inspector...
        if (camara == null)
        {
            // ...usa la cámara principal como respaldo.
            if (Camera.main != null) camara = Camera.main.transform;
        }

        // Si hay agua asignada, la detiene y borra las partículas, para que no haya agua al arrancar.
        if (agua != null)
            agua.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);

        // Los dos carteles arrancan ocultos.
        Mostrar(canvasCartel, false);
        Mostrar(canvasInfo, false);
    }

    // Update se ejecuta en cada frame mientras el juego corre.
    private void Update()
    {
        // Obtiene el GameManager para saber si el juego está en curso.
        GameManager gm = GameManager.Instance;

        // Si el juego no está en marcha o falta la cámara, oculta los dos carteles y corta la ejecución.
        if (gm == null || !gm.EstaJugando || camara == null)
        {
            Mostrar(canvasCartel, false);
            Mostrar(canvasInfo, false);
            return;
        }

        // Comprueba si el jugador está mirando el botón del riego en este momento.
        bool mirando = MirandoAlBoton();

        // Antes de regar: cartel "ESPACIO". Regando: canvas con las especies.
        // Cada cartel solo se muestra si el jugador está mirando el botón y el riego está en el estado correspondiente.
        Mostrar(canvasCartel, mirando && !regando);
        Mostrar(canvasInfo, mirando && regando);

        // Si no está mirando el botón, no se puede interactuar: corta acá.
        if (!mirando) return;
        // Si no hay teclado disponible, tampoco se puede leer la tecla: corta acá.
        if (Keyboard.current == null) return;

        // wasPressedThisFrame es true solo en el frame en que se apretó la tecla.
        if (Keyboard.current[teclaRiego].wasPressedThisFrame)
            // Enciende el riego si estaba apagado, o lo apaga si estaba encendido.
            AlternarRiego();
    }

    // Devuelve true si el jugador está mirando este objeto (o alguno de sus hijos) a una distancia razonable.
    private bool MirandoAlBoton()
    {
        // Lanza un rayo desde la cámara hacia adelante y devuelve TODOS los objetos que atraviesa
        // (no solo el primero), hasta la distancia máxima y solo en las capas indicadas.
        // QueryTriggerInteraction.Ignore hace que el rayo ignore los colliders marcados como "Is Trigger".
        RaycastHit[] hits = Physics.RaycastAll(camara.position, camara.forward,
                                               distanciaMaxima, capas,
                                               QueryTriggerInteraction.Ignore);

        // Recorre uno por uno los objetos que tocó el rayo.
        foreach (RaycastHit hit in hits)
        {
            // Si el objeto tocado es este mismo objeto o uno de sus hijos, el jugador está mirando el botón.
            if (hit.collider.transform.IsChildOf(transform))
                return true;
        }
        // Si ningún objeto tocado pertenece a este botón, el jugador no lo está mirando.
        return false;
    }

    // Método público: también se puede llamar desde otro script o desde un botón de la UI.
    public void AlternarRiego()
    {
        // Si no hay sistema de partículas asignado, no hay nada que encender.
        if (agua == null) return;

        // Invierte el estado: si estaba regando pasa a no regar, y viceversa.
        regando = !regando;

        // Si ahora está regando, arranca las partículas; si no, las detiene (las que ya salieron terminan su recorrido).
        if (regando) agua.Play();
        else agua.Stop();
    }

    // Muestra u oculta un objeto, evitando llamar a SetActive si ya está en el estado pedido.
    private void Mostrar(GameObject objeto, bool mostrar)
    {
        // Si el objeto no está asignado, no hace nada.
        if (objeto == null) return;
        // Si ya está en el estado pedido, no hace nada (evita llamadas innecesarias en cada frame).
        if (objeto.activeSelf == mostrar) return;
        // Cambia el estado del objeto.
        objeto.SetActive(mostrar);
    }
}