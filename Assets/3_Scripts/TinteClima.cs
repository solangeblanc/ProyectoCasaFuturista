// Importa las clases básicas de Unity (MonoBehaviour, Color, Mathf, Time, etc.)
using UnityEngine;
// Importa el sistema de Volume (post-procesado), necesario para el tipo Volume.
using UnityEngine.Rendering;
// Importa las clases del Universal Render Pipeline, necesarias para el tipo ColorAdjustments.
using UnityEngine.Rendering.Universal;

// Script que tiñe suavemente la pantalla de azul (frío) o naranja (calor) cuando el aire acondicionado está encendido.
// Lo hace controlando el peso (weight) de un Volume de post-procesado que tiene un override Color Adjustments.
// Depende de ClimatizacionHabitacion para saber el estado del aire.
// El nombre de la clase tiene que coincidir con el nombre del archivo: TinteClima.cs
public class TinteClima : MonoBehaviour
{
    // Header solo agrega un título en el Inspector para ordenar las variables, no afecta la lógica.
    [Header("Referencias")]
    // [SerializeField] muestra una variable privada en el Inspector sin hacerla pública para otros scripts.
    // Equipo de aire del cuarto, del que se consulta si está encendido y en qué modo.
    [SerializeField] private ClimatizacionHabitacion clima;
    // Volume de post-procesado que aplica el tinte. Tiene que tener el override Color Adjustments en su perfil.
    [SerializeField] private Volume volumen;

    [Header("Colores (suaves)")]
    // Color del tinte cuando el aire está en modo frío (celeste suave).
    [SerializeField] private Color colorFrio = new Color(0.65f, 0.82f, 1f);
    // Color del tinte cuando el aire está en modo calor (naranja suave).
    [SerializeField] private Color colorCalor = new Color(1f, 0.78f, 0.55f);

    [Header("Intensidad")]
    // Range muestra la variable como un slider en el Inspector, limitada entre 0 y 1.
    [Range(0f, 1f)]
    // Intensidad máxima del tinte: 0 = sin tinte, 1 = tinte completo.
    [SerializeField] private float intensidadMaxima = 1f;
    // Tooltip muestra un texto de ayuda al dejar el mouse sobre la variable en el Inspector.
    [Tooltip("Qué tan rápido entra y sale el tinte")]
    // Velocidad con la que el tinte aparece y desaparece. Más alto = transición más rápida.
    [SerializeField] private float velocidad = 1.5f;

    // Referencia al override Color Adjustments del perfil del Volume. Se obtiene en Start.
    private ColorAdjustments ajuste;

    // Start se ejecuta una sola vez, antes del primer frame.
    private void Start()
    {
        // Si falta el Volume o el equipo de aire (no se asignaron en el Inspector), no se puede hacer nada.
        if (volumen == null || clima == null)
        {
            // Corta la ejecución. Como "ajuste" queda sin asignar, Update tampoco hará nada.
            return;
        }

        // Arranca con el tinte invisible (peso 0), para que no se vea nada hasta que se encienda el aire.
        volumen.weight = 0f;

        // Busca en el perfil del Volume el override Color Adjustments y lo guarda en "ajuste".
        // TryGet devuelve true si lo encuentra. El "out" es lo que entrega el override.
        // Si no lo encuentra, "ajuste" queda en null y Update no hace nada.
        volumen.profile.TryGet(out ajuste);
    }

    // Update se ejecuta en cada frame mientras el juego corre.
    private void Update()
    {
        // Si falta alguna referencia o no se encontró el override, no hace nada.
        if (volumen == null || clima == null || ajuste == null) return;

        // Intensidad a la que debe llegar el tinte. Por defecto 0 (sin tinte).
        float objetivo = 0f;
        // Variable donde se guardará el modo con el que está trabajando el aire.
        ModoClima efectivo;

        // TieneEfecto devuelve true si el aire está encendido, y entrega por "out" el modo efectivo
        // (si está en Automático, el modo real que se está usando según el clima exterior).
        if (clima.TieneEfecto(out efectivo))
        {
            // Si el aire enfría, pone el color frío y fija la intensidad máxima como meta.
            if (efectivo == ModoClima.Frio)
            {
                ajuste.colorFilter.value = colorFrio;
                objetivo = intensidadMaxima;
            }
            // Si el aire calienta, pone el color cálido y fija la intensidad máxima como meta.
            else if (efectivo == ModoClima.Calor)
            {
                ajuste.colorFilter.value = colorCalor;
                objetivo = intensidadMaxima;
            }
            // Si está en ventilación, no se tiñe nada y la meta sigue en 0.
        }

        // Acerca suavemente el peso actual del Volume al objetivo, sin pasarse.
        // Mathf.MoveTowards avanza como máximo "velocidad * Time.unscaledDeltaTime" por frame.
        // unscaledDeltaTime no se ve afectado por pausas (Time.timeScale = 0), así la transición siempre fluye.
        // Si el aire se apaga, el objetivo es 0 y el tinte se desvanece solo.
        volumen.weight = Mathf.MoveTowards(volumen.weight, objetivo,
                                           velocidad * Time.unscaledDeltaTime);
    }
}