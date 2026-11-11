// Importa las clases básicas de Unity (MonoBehaviour, Transform, Camera, Gizmos, etc.)
using UnityEngine;
// Importa el sistema de interfaz de Unity, necesario para usar el tipo Image.
using UnityEngine.UI;
// Importa el nuevo Input System, necesario para leer el teclado con Keyboard.current y Key.
using UnityEngine.InputSystem;
// Importa TextMeshPro, necesario para usar el tipo TMP_Text.
using TMPro;

// Script que muestra un cartel con información del clima cuando el jugador se acerca al aire acondicionado
// de un cuarto, y permite encenderlo, cambiar de modo o apagarlo con una tecla.
// Depende de ClimaExterior (temperatura de afuera) y de ClimatizacionHabitacion (el equipo del cuarto).
public class ZonaClima : MonoBehaviour
{
    [Header("Cuarto")]
    // Nombre del cuarto que se muestra como primera línea del cartel.
    [SerializeField] private string nombreCuarto = "Living";
    [Tooltip("Se usa solo si no hay un script de climatización asignado")]
    // Temperatura de respaldo del cuarto, para mostrar algo cuando no hay un ClimatizacionHabitacion asignado.
    [SerializeField] private float temperaturaActual = 26f;

    [Header("Detección por cercanía")]
    // Punto de referencia desde el cual se mide la cercanía (el aire acondicionado).
    [SerializeField] private Transform puntoAire;
    // Distancia horizontal máxima, en unidades de Unity, a la que el jugador ve el cartel.
    [SerializeField] private float radio = 4f;
    // CORREGIDO: altura máxima (Y global) que puede tener el jugador para ver el cartel.
    // El aire está en la planta baja, así que si el jugador está más arriba que este valor (primer piso),
    // el cartel no aparece aunque esté justo encima del aire.
    // Tiene que quedar entre la Y del jugador en la planta baja (aprox. -6) y en el primer piso (aprox. 67).
    [Tooltip("El cartel solo se ve si el jugador está por debajo de esta altura (Y global). Ponelo entre la Y de la planta baja y la del primer piso")]
    [SerializeField] private float alturaMaximaJugador = 30f;
    // Referencia al jugador (la cápsula). Si se deja vacía, Start usa la cámara principal.
    [SerializeField] private Transform jugador;
    [Tooltip("Dejalo en 0: el cartel queda mientras estés cerca")]
    // Si es mayor que 0, el cartel se oculta solo después de esa cantidad de segundos, aunque el jugador siga cerca.
    [SerializeField] private float segundosVisible = 0f;

    [Header("Tecla")]
    // Tecla que cicla el estado del aire (Apagado -> Frío -> Calor -> Apagado). Por defecto, la barra espaciadora.
    [SerializeField] private Key teclaAire = Key.Space;

    [Header("Cartel")]
    // Panel de la interfaz que se muestra u oculta.
    [SerializeField] private GameObject panelInfo;
    // Texto principal del cartel, donde se escribe la información del clima.
    [SerializeField] private TMP_Text texto;
    // Script del equipo de aire del cuarto. Si está vacío, el cartel solo muestra información.
    [SerializeField] private ClimatizacionHabitacion clima;

    [Header("Botón del cartel")]
    // Texto del botón del cartel, donde se explica qué hace la tecla en este momento.
    [SerializeField] private TMP_Text textoBoton;
    // Imagen de fondo del botón, cuyo color cambia según el estado del aire.
    [SerializeField] private Image imagenBoton;
    // Color del botón cuando el aire está apagado (gris).
    [SerializeField] private Color colorApagado = new Color(0.35f, 0.35f, 0.4f);
    // Color del botón cuando el aire está en modo frío (azul).
    [SerializeField] private Color colorFrio = new Color(0.2f, 0.6f, 0.85f);
    // Color del botón cuando el aire está en modo calor (naranja).
    [SerializeField] private Color colorCalor = new Color(0.9f, 0.5f, 0.2f);

    // Recuerda si en el frame anterior el jugador estaba cerca, para detectar el momento en que entra a la zona.
    private bool estabaCerca = false;
    // Indica que ya se agotó el tiempo de visibilidad (solo se usa si segundosVisible es mayor que 0).
    private bool agotado = false;
    // Segundos que lleva visible el cartel desde que el jugador se acercó.
    private float tiempoMostrado = 0f;

    // Start se ejecuta una sola vez, antes del primer frame.
    private void Start()
    {
        // Si no se asignó el jugador en el Inspector, usa la cámara principal como respaldo.
        if (jugador == null)
        {
            if (Camera.main != null) jugador = Camera.main.transform;
        }

        // El cartel arranca oculto.
        Mostrar(false);
    }

    // Update se ejecuta en cada frame mientras el juego corre.
    private void Update()
    {
        // Obtiene el GameManager para saber si el juego está en curso.
        GameManager gm = GameManager.Instance;

        // Si el juego no está en marcha o falta alguna referencia, oculta el cartel y corta.
        if (gm == null || !gm.EstaJugando || puntoAire == null || jugador == null)
        {
            Mostrar(false);
            return;
        }

        // CORREGIDO: comprueba si el jugador está en la planta baja, mirando su altura global.
        // Se calcula ANTES de poner la Y en 0 más abajo; si se calculara después, siempre daría 0.
        bool mismoPiso = jugador.position.y <= alturaMaximaJugador;

        // Copia las posiciones del jugador y del aire.
        Vector3 a = jugador.position;
        Vector3 b = puntoAire.position;
        // Pone la altura (Y) en 0 en ambas, para medir la distancia solo en el plano horizontal.
        a.y = 0f;
        b.y = 0f;
        // CORREGIDO: ahora el jugador tiene que estar en el piso correcto Y dentro del radio horizontal.
        // Antes solo se comprobaba el radio, por eso el cartel aparecía también en el piso de arriba.
        bool cerca = mismoPiso && Vector3.Distance(a, b) <= radio;

        // Si el jugador acaba de entrar en la zona, reinicia el contador de tiempo.
        if (cerca && !estabaCerca)
        {
            tiempoMostrado = 0f;
            agotado = false;
        }
        // Guarda el estado actual para compararlo en el próximo frame.
        estabaCerca = cerca;

        // El cartel es visible si el jugador está cerca y todavía no se agotó el tiempo.
        bool visible = cerca && !agotado;

        // Si hay un tiempo límite configurado y el cartel está visible, va sumando el tiempo.
        if (visible && segundosVisible > 0f)
        {
            // unscaledDeltaTime no se ve afectado por pausas (Time.timeScale = 0).
            tiempoMostrado += Time.unscaledDeltaTime;
            if (tiempoMostrado >= segundosVisible)
            {
                agotado = true;
                visible = false;
            }
        }

        // Muestra u oculta el panel según corresponda.
        Mostrar(visible);

        // Si el cartel no está visible, no hace falta seguir.
        if (!visible) return;

        // Barra espaciadora: Apagado -> Frío -> Calor -> Apagado
        if (clima != null && Keyboard.current != null)
        {
            // wasPressedThisFrame es true solo en el frame en que se apretó la tecla.
            if (Keyboard.current[teclaAire].wasPressedThisFrame)
                clima.Ciclar();
        }

        // Actualiza el texto del cartel y el botón con los datos actuales.
        ActualizarTextos();
    }

    // Escribe en el cartel la información del clima y actualiza el botón según el estado del aire.
    private void ActualizarTextos()
    {
        // Si no existe ClimaExterior en la escena, avisa en el cartel y corta.
        if (ClimaExterior.Instance == null)
        {
            if (texto != null) texto.text = "Falta el script ClimaExterior en el GameManager";
            return;
        }

        // Temperatura de afuera y la temperatura y el modo que recomienda ClimaExterior.
        float exterior = ClimaExterior.Instance.Temperatura;
        float recomendada = ClimaExterior.Recomendada(exterior);
        string modoRecomendado = ClimaExterior.ModoRecomendado(exterior);

        // Temperatura del ambiente: por defecto la de respaldo.
        float ambiente = temperaturaActual;
        // Línea extra con el estado del aire; queda vacía si no hay equipo asignado.
        string lineaAire = "";

        // Si hay un equipo asignado, usa su temperatura real y agrega una línea con su estado.
        if (clima != null)
        {
            ambiente = clima.Temperatura;
            lineaAire = "\nAire: " + clima.EstadoTexto;
        }

        // Arma el mensaje del cartel.
        if (texto != null)
        {
            texto.text = nombreCuarto + "\n"
                       + "Exterior: " + exterior.ToString("0") + " °C\n"
                       + "Ambiente: " + ambiente.ToString("0.0") + " °C\n"
                       + "Recomendado: " + modoRecomendado + " a " + recomendada.ToString("0") + " °C"
                       + lineaAire;
        }

        // Si no hay equipo asignado, no hay botón que actualizar.
        if (clima == null) return;

        // Texto y color que va a tener el botón según el estado actual del aire.
        string accion;
        Color color;

        if (!clima.Encendido)
        {
            accion = "ESPACIO: Encender aire frío (" + clima.ObjetivoFrio.ToString("0") + " °C)";
            color = colorApagado;
        }
        else if (clima.Modo == ModoClima.Frio)
        {
            accion = "ESPACIO: Cambiar a calefacción (" + clima.ObjetivoCalor.ToString("0") + " °C)";
            color = colorFrio;
        }
        else
        {
            accion = "ESPACIO: Apagar";
            color = clima.Modo == ModoClima.Calor ? colorCalor : colorFrio;
        }

        // Aplica el texto y el color al botón, si están asignados.
        if (textoBoton != null) textoBoton.text = accion;
        if (imagenBoton != null) imagenBoton.color = color;
    }

    // Muestra u oculta el panel del cartel.
    private void Mostrar(bool mostrar)
    {
        if (panelInfo == null) return;
        // Si el panel ya está en el estado pedido, no hace nada.
        if (panelInfo.activeSelf == mostrar) return;
        panelInfo.SetActive(mostrar);
    }

    // Solo en el Editor, con el objeto seleccionado: dibuja el radio de detección en la ventana Scene.
    private void OnDrawGizmosSelected()
    {
        if (puntoAire == null) return;
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(puntoAire.position, radio);
    }
}