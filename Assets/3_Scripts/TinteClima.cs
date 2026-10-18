using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class TinteClima : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private ClimatizacionHabitacion clima;
    [SerializeField] private Volume volumen;

    [Header("Colores (suaves)")]
    [SerializeField] private Color colorFrio = new Color(0.65f, 0.82f, 1f);
    [SerializeField] private Color colorCalor = new Color(1f, 0.78f, 0.55f);

    [Header("Intensidad")]
    [Range(0f, 1f)]
    [SerializeField] private float intensidadMaxima = 1f;
    [Tooltip("Qué tan rápido entra y sale el tinte")]
    [SerializeField] private float velocidad = 1.5f;

    private ColorAdjustments ajuste;

    private void Start()
    {
        if (volumen == null || clima == null)
        {
            Debug.LogWarning("[Tinte] Faltan Volumen o Clima");
            return;
        }

        volumen.weight = 0f;

        if (!volumen.profile.TryGet(out ajuste))
            Debug.LogWarning("[Tinte] El Volume no tiene el override Color Adjustments");
    }

    private void Update()
    {
        if (volumen == null || clima == null || ajuste == null) return;

        float objetivo = 0f;
        ModoClima efectivo;

        if (clima.TieneEfecto(out efectivo))
        {
            if (efectivo == ModoClima.Frio)
            {
                ajuste.colorFilter.value = colorFrio;
                objetivo = intensidadMaxima;
            }
            else if (efectivo == ModoClima.Calor)
            {
                ajuste.colorFilter.value = colorCalor;
                objetivo = intensidadMaxima;
            }
        }

        volumen.weight = Mathf.MoveTowards(volumen.weight, objetivo,
                                           velocidad * Time.unscaledDeltaTime);
    }
}
