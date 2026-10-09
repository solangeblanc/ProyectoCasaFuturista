using UnityEngine;

public class CicloFlechaParpadeo : MonoBehaviour
{
    [Header("Configuración de Tiempos (en segundos)")]
    [Tooltip("Tiempo que la flecha se pasa parpadeando (2 minutos = 120 segundos).")]
    public float tiempoEncendida = 120f;

    [Tooltip("Tiempo que la flecha se queda completamente apagada (10 segundos).")]
    public float tiempoApagada = 10f;

    [Header("Configuración de Intermitencia")]
    [Tooltip("Velocidad del parpadeo (cuánto más bajo, más lento y sutil).")]
    public float velocidadTitileo = 1.5f;

    [Range(0f, 0.5f)]
    public float opacidadMinima = 0.1f;

    private SpriteRenderer spriteRenderer;
    private float cronometro = 0f;
    private bool estaEnCicloApagado = false;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (spriteRenderer == null)
        {
            Debug.LogError("¡Falta el componente SpriteRenderer en este objeto!");
            enabled = false;
            return;
        }

        cronometro = 0f;
        estaEnCicloApagado = false;
    }

    void Update()
    {
        // Avanzamos el tiempo del ciclo
        cronometro += Time.deltaTime;

        if (!estaEnCicloApagado)
        {
            // FASE 1: Parpadeando durante los minutos configurados
            if (cronometro >= tiempoEncendida)
            {
                // Se acabó el tiempo encendida, pasamos a la fase de apagón
                cronometro = 0f;
                estaEnCicloApagado = true;
                EstablecerAlfa(0f); // Apagada por completo
            }
            else
            {
                // Efecto de parpadeo lento y suave
                float alfa = Mathf.Lerp(opacidadMinima, 1f, (Mathf.Sin(Time.time * velocidadTitileo) + 1f) / 2f);
                EstablecerAlfa(alfa);
            }
        }
        else
        {
            // FASE 2: Apagada durante los segundos configurados (10s)
            EstablecerAlfa(0f);

            if (cronometro >= tiempoApagada)
            {
                // Se acabó el tiempo de descanso, reiniciamos el ciclo de parpadeo
                cronometro = 0f;
                estaEnCicloApagado = false;
            }
        }
    }

    private void EstablecerAlfa(float valorAlfa)
    {
        Color colorActual = spriteRenderer.color;
        colorActual.a = Mathf.Clamp01(valorAlfa);
        spriteRenderer.color = colorActual;
    }
}
