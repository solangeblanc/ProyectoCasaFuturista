// Importa las clases básicas de Unity (MonoBehaviour, Time, Mathf, etc.)
using UnityEngine;

// Script que simula la temperatura exterior, ya sea con un valor fijo (modo manual)
// o con un ciclo automático que sube y baja como un día real.
// Otros scripts (como un aire acondicionado) pueden consultar la temperatura y las recomendaciones desde acá.
// El nombre de la clase tiene que coincidir con el nombre del archivo: ClimaExterior.cs
public class ClimaExterior : MonoBehaviour
{
    // Propiedad estática: hay una sola para todo el juego (patrón Singleton).
    // Cualquier script puede acceder con ClimaExterior.Instance sin buscar el objeto en la escena.
    // "get" es público (todos pueden leerla) pero "private set" hace que solo este script pueda asignarla.
    public static ClimaExterior Instance { get; private set; }

    // Header solo agrega un título en el Inspector para ordenar las variables, no afecta la lógica.
    [Header("Modo de prueba")]
    // [SerializeField] muestra una variable privada en el Inspector, sin hacerla pública para otros scripts.
    // Si está en true, la temperatura es siempre la manual. Si está en false, se usa la simulación automática.
    [SerializeField] private bool usarManual = true;
    // Temperatura fija (en °C) que se usa cuando "usarManual" está activado. Útil para probar sin esperar el ciclo.
    [SerializeField] private float temperaturaManual = 32f;

    // Otro título visual en el Inspector.
    [Header("Simulación automática (día completo)")]
    // Temperatura promedio del día: el centro alrededor del cual oscila la temperatura.
    [SerializeField] private float temperaturaMedia = 22f;
    // Cuántos grados sube y baja respecto de la media (con 22 y 8, va de 14 °C a 30 °C).
    [SerializeField] private float amplitud = 8f;
    // Duración de un "día" simulado, en segundos reales (120 = un día completo cada 2 minutos).
    [SerializeField] private float segundosPorDia = 120f;

    // Temperatura actual. Otros scripts la pueden leer, pero solo este la puede modificar.
    public float Temperatura { get; private set; }

    // Awake se ejecuta apenas se crea el objeto, antes que Start. Es el lugar ideal para inicializar cosas.
    private void Awake()
    {
        // Si ya existe otro ClimaExterior y no es este, es un duplicado.
        if (Instance != null && Instance != this)
        {
            // Destruye solo este componente (no el objeto entero) y corta con return.
            Destroy(this);
            return;
        }
        // Si llegó hasta acá, este es el único: se guarda como la instancia oficial.
        Instance = this;
        // Arranca con la temperatura manual para que haya un valor válido desde el primer frame.
        Temperatura = temperaturaManual;
    }

    // Unity llama a este método cuando el objeto o el componente se destruye (por ejemplo, al cerrar la escena).
    private void OnDestroy()
    {
        // Si este era la instancia oficial, la libera (la pone en null) para no dejar una referencia a algo que ya no existe.
        if (Instance == this) Instance = null;
    }

    // Update se ejecuta en cada frame mientras el juego corre.
    private void Update()
    {
        // Si está activado el modo manual...
        if (usarManual)
        {
            // ...la temperatura es exactamente la que pusiste en el Inspector.
            // Se actualiza en cada frame, así que podés cambiarla en vivo durante el Play.
            Temperatura = temperaturaManual;
            // Corta acá: no se ejecuta la simulación automática de abajo.
            return;
        }

        // Time.time son los segundos desde que arrancó el juego.
        // El operador % (resto) hace que el valor vuelva a 0 cada "segundosPorDia" segundos.
        // Al dividir por segundosPorDia, "t" queda entre 0 y 1: representa en qué momento del día estamos
        // (0 = inicio del día, 0.5 = mitad del día, 1 = fin del día).
        float t = (Time.time % segundosPorDia) / segundosPorDia;
        // Calcula la temperatura con una onda senoidal, que sube y baja de forma suave.
        // (t - 0.25f) desplaza la onda para que el punto más caliente caiga en la mitad del día (t = 0.5)
        // y el más frío al comienzo y al final (t = 0).
        // Multiplicar por 2 * PI convierte "t" en una vuelta completa de la onda.
        // Mathf.Sin devuelve un valor entre -1 y 1, que al multiplicarse por amplitud da la variación en grados.
        // Sumarle temperaturaMedia centra esa variación alrededor de la temperatura promedio.
        Temperatura = temperaturaMedia + amplitud * Mathf.Sin((t - 0.25f) * 2f * Mathf.PI);
    }

    // Regla simple de recomendación
    // Método estático: se llama directo con ClimaExterior.Recomendada(valor), sin necesitar una instancia.
    // Recibe la temperatura exterior y devuelve a qué temperatura conviene poner el aire acondicionado.
    public static float Recomendada(float exterior)
    {
        // Si afuera hace calor (26 °C o más), recomienda 24 °C adentro.
        if (exterior >= 26f) return 24f;
        // Si afuera hace frío (16 °C o menos), recomienda 26 °C adentro.
        if (exterior <= 16f) return 26f;
        // Si la temperatura está en el rango intermedio, recomienda 23 °C.
        return 23f;
    }

    // Método estático que, según la temperatura exterior, devuelve el modo de funcionamiento recomendado como texto.
    // Usa los mismos límites que el método anterior, así que ambos se mantienen coherentes.
    public static string ModoRecomendado(float exterior)
    {
        // Calor afuera: recomienda modo "Frío".
        if (exterior >= 26f) return "Frío";
        // Frío afuera: recomienda modo "Calor".
        if (exterior <= 16f) return "Calor";
        // Temperatura agradable: alcanza con ventilar.
        return "Ventilación";
    }
}
