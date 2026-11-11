// Importa las clases básicas de Unity (MonoBehaviour, Mathf, Time, etc.)
using UnityEngine;

// Enumeración: define una lista de valores posibles para el modo del climatizador.
// Es más claro y seguro que usar números o textos sueltos.
// "Automatico" significa que el equipo elige solo el modo según la temperatura de afuera.
public enum ModoClima { Frio, Calor, Ventilacion, Automatico }

// Script que simula el aire acondicionado de una habitación: la temperatura del cuarto sube o baja
// según el modo elegido, o se acerca a la temperatura exterior cuando el equipo está apagado.
// Depende del script ClimaExterior para saber qué temperatura hace afuera.
// El nombre de la clase tiene que coincidir con el nombre del archivo: ClimatizacionHabitacion.cs
public class ClimatizacionHabitacion : MonoBehaviour
{
    // Header solo agrega un título en el Inspector para ordenar las variables, no afecta la lógica.
    [Header("Cuarto")]
    // [SerializeField] muestra una variable privada en el Inspector sin hacerla pública para otros scripts.
    // Temperatura (en °C) con la que arranca el cuarto al empezar el juego.
    [SerializeField] private float temperaturaInicial = 26f;

    [Header("Control (se puede cambiar en Play para probar)")]
    // Indica si el equipo está encendido o apagado.
    [SerializeField] private bool encendido = false;
    // Modo de funcionamiento actual (Frío, Calor, Ventilación o Automático). En el Inspector aparece como desplegable.
    [SerializeField] private ModoClima modo = ModoClima.Frio;
    // Range muestra la variable como un slider en el Inspector, limitada entre 18 y 30 °C.
    [Range(18f, 30f)]
    // Temperatura a la que el usuario quiere dejar el cuarto.
    [SerializeField] private float objetivo = 24f;

    [Header("Temperaturas al ciclar con la barra espaciadora")]
    // Temperatura objetivo que se aplica automáticamente cuando el ciclo pasa al modo Frío.
    [SerializeField] private float objetivoFrio = 24f;
    // Temperatura objetivo que se aplica automáticamente cuando el ciclo pasa al modo Calor.
    [SerializeField] private float objetivoCalor = 26f;

    [Header("Simulación")]
    // Tooltip muestra un texto de ayuda al dejar el mouse sobre la variable en el Inspector.
    [Tooltip("Grados por segundo que enfría o calienta el equipo")]
    // Qué tan rápido cambia la temperatura cuando el equipo está trabajando.
    [SerializeField] private float velocidad = 0.2f;
    [Tooltip("Grados por segundo que el cuarto se acerca a la temperatura de afuera")]
    // Qué tan rápido se iguala el cuarto con el exterior de forma natural (es mucho más lento que el equipo).
    [SerializeField] private float deriva = 0.02f;

    // Temperatura actual del cuarto. Otros scripts la pueden leer, pero solo este la puede modificar.
    public float Temperatura { get; private set; }
    // Propiedades de solo lectura (la flecha => devuelve el valor de la variable privada).
    // Permiten que otros scripts (por ejemplo, la interfaz) consulten el estado sin poder cambiarlo directamente.
    public bool Encendido => encendido;
    public ModoClima Modo => modo;
    public float Objetivo => objetivo;
    public float ObjetivoFrio => objetivoFrio;
    public float ObjetivoCalor => objetivoCalor;

    // Propiedad que arma un texto con el estado actual, por ejemplo "Frío a 24 °C" o "Apagado".
    // Es útil para mostrarlo directamente en un cartel o pantalla.
    public string EstadoTexto
    {
        // "get" define qué se devuelve cuando alguien lee la propiedad.
        get
        {
            // Si el equipo está apagado, el estado es simplemente "Apagado".
            if (!encendido) return "Apagado";
            // Consulta la temperatura exterior actual.
            float ext = Exterior();
            // Arma el texto uniendo con + el nombre del modo, la palabra " a ", la temperatura objetivo y " °C".
            // ToString("0") convierte el número a texto sin decimales (24.0 se muestra como "24").
            return NombreModo(ModoEfectivo(ext)) + " a " + ObjetivoEfectivo(ext).ToString("0") + " °C";
        }
    }

    // Awake se ejecuta apenas se crea el objeto, antes que Start. Es el lugar ideal para inicializar cosas.
    private void Awake()
    {
        // El cuarto empieza con la temperatura inicial configurada en el Inspector.
        Temperatura = temperaturaInicial;
    }

    // Update se ejecuta en cada frame mientras el juego corre. Acá se simula cómo cambia la temperatura.
    private void Update()
    {
        // Temperatura de afuera en este momento.
        float ext = Exterior();
        // Time.deltaTime es el tiempo que pasó desde el frame anterior.
        // Multiplicar por dt hace que los cambios sean iguales en cualquier computadora, sin importar los fps.
        float dt = Time.deltaTime;

        // Si el equipo está apagado...
        if (!encendido)
        {
            // ...el cuarto se acerca lentamente a la temperatura exterior.
            // Mathf.MoveTowards mueve un valor hacia otro, avanzando como máximo "deriva * dt" en este frame,
            // sin pasarse nunca del destino.
            Temperatura = Mathf.MoveTowards(Temperatura, ext, deriva * dt);
            // Corta acá: no se ejecuta la lógica del equipo encendido que viene abajo.
            return;
        }

        // Determina el modo real de trabajo. Si el modo es Automático, se convierte en Frío, Calor o Ventilación según el exterior.
        ModoClima m = ModoEfectivo(ext);
        // Determina la temperatura meta a la que debe llegar el cuarto.
        float meta = ObjetivoEfectivo(ext);

        // Modo Frío y el cuarto está más caliente que la meta: enfría hacia la meta a la velocidad del equipo.
        if (m == ModoClima.Frio && Temperatura > meta)
            Temperatura = Mathf.MoveTowards(Temperatura, meta, velocidad * dt);
        // Modo Calor y el cuarto está más frío que la meta: calienta hacia la meta a la velocidad del equipo.
        else if (m == ModoClima.Calor && Temperatura < meta)
            Temperatura = Mathf.MoveTowards(Temperatura, meta, velocidad * dt);
        // Modo Ventilación: no enfría ni calienta, solo renueva el aire, así que el cuarto se acerca al exterior.
        // Lo hace al 30 % de la velocidad del equipo (velocidad * 0.3f), más rápido que apagado pero más lento que enfriando.
        else if (m == ModoClima.Ventilacion)
            Temperatura = Mathf.MoveTowards(Temperatura, ext, velocidad * 0.3f * dt);
        // Cualquier otro caso (por ejemplo, ya se llegó a la meta): el equipo no hace nada
        // y el cuarto vuelve a derivar lentamente hacia la temperatura exterior.
        else
            Temperatura = Mathf.MoveTowards(Temperatura, ext, deriva * dt);
    }

    // Devuelve la temperatura exterior actual.
    private float Exterior()
    {
        // Si existe el script ClimaExterior en la escena, usa su temperatura.
        if (ClimaExterior.Instance != null) return ClimaExterior.Instance.Temperatura;
        // Si no existe (se olvidó agregarlo a la escena), usa la temperatura del propio cuarto como respaldo.
        // Así no hay errores y el cuarto simplemente no cambia por influencia externa.
        return Temperatura;
    }

    // Devuelve el modo con el que el equipo realmente va a trabajar.
    private ModoClima ModoEfectivo(float ext)
    {
        // Si el modo elegido no es Automático, se respeta tal cual.
        if (modo != ModoClima.Automatico) return modo;
        // En Automático decide según el exterior, con los mismos límites que usa ClimaExterior:
        // calor afuera (26 °C o más) -> enfría.
        if (ext >= 26f) return ModoClima.Frio;
        // frío afuera (16 °C o menos) -> calienta.
        if (ext <= 16f) return ModoClima.Calor;
        // temperatura intermedia -> alcanza con ventilar.
        return ModoClima.Ventilacion;
    }

    // Devuelve la temperatura meta que se debe usar.
    private float ObjetivoEfectivo(float ext)
    {
        // En Automático la meta la define la regla de recomendación de ClimaExterior.
        if (modo == ModoClima.Automatico) return ClimaExterior.Recomendada(ext);
        // En los demás modos se usa la temperatura que eligió el usuario.
        return objetivo;
    }

    // Convierte un modo (enum) en un texto legible, con tildes, para mostrar en pantalla.
    private string NombreModo(ModoClima m)
    {
        // switch compara el valor de "m" con cada caso y ejecuta el que coincide.
        switch (m)
        {
            case ModoClima.Frio: return "Frío";
            case ModoClima.Calor: return "Calor";
            case ModoClima.Ventilacion: return "Ventilación";
            // default cubre cualquier otro valor (en este caso, Automático).
            default: return "Automático";
        }
    }

    // Lo usa el tinte de pantalla
    // Método que le informa a otro script (el que tiñe la pantalla de azul o rojo) si el equipo está haciendo efecto
    // y con qué modo. "out" permite devolver un segundo valor además del bool que retorna.
    public bool TieneEfecto(out ModoClima efectivo)
    {
        // Entrega por el parámetro "out" el modo efectivo actual.
        efectivo = ModoEfectivo(Exterior());
        // Devuelve true si el equipo está encendido (el tinte solo tiene sentido en ese caso).
        return encendido;
    }

    // Barra espaciadora: Apagado -> Frío -> Calor -> Apagado
    // Método público pensado para llamarse desde otro script cada vez que se aprieta la barra espaciadora.
    public void Ciclar()
    {
        // Si está apagado, lo enciende en modo Frío con su temperatura objetivo.
        if (!encendido)
        {
            encendido = true;
            modo = ModoClima.Frio;
            objetivo = objetivoFrio;
        }
        // Si ya estaba en Frío, pasa a Calor con su temperatura objetivo.
        else if (modo == ModoClima.Frio)
        {
            modo = ModoClima.Calor;
            objetivo = objetivoCalor;
        }
        // En cualquier otro caso (estaba en Calor u otro modo), lo apaga y el ciclo vuelve a empezar.
        else
        {
            encendido = false;
        }
    }

    // Métodos públicos cortos para controlar el equipo desde botones de la UI u otros scripts.
    // Alternar: si estaba encendido lo apaga, y si estaba apagado lo enciende.
    public void Alternar() { encendido = !encendido; }
    // Enciende el equipo.
    public void Encender() { encendido = true; }
    // Apaga el equipo.
    public void Apagar() { encendido = false; }
    // Cambia el modo de funcionamiento al que se le pase como parámetro.
    public void SetModo(ModoClima nuevo) { modo = nuevo; }
    // Sube la temperatura objetivo 1 grado. Mathf.Min impide que pase de 30.
    public void SubirObjetivo() { objetivo = Mathf.Min(30f, objetivo + 1f); }
    // Baja la temperatura objetivo 1 grado. Mathf.Max impide que baje de 18.
    public void BajarObjetivo() { objetivo = Mathf.Max(18f, objetivo - 1f); }
}