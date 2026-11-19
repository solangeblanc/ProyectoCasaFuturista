// Importa las clases básicas de Unity (MonoBehaviour, Color, Mathf, Time, etc.)
using UnityEngine;
// Importa el sistema de Volume (post-procesado).
using UnityEngine.Rendering;
// Importa los overrides de URP: WhiteBalance y ColorAdjustments.
using UnityEngine.Rendering.Universal;

// Cambia el "clima visual" de TODA la escena (casa, patio, etc.) según la temperatura de AFUERA,
// usando post-procesado. NO depende de la luz bakeada: funciona igual con todo bakeado.
// Frío afuera -> imagen azulada y algo desaturada. Calor afuera -> imagen cálida y más saturada.
// Lee la temperatura de ClimaExterior, así que también cambia sola si ese script está en modo automático.
//
// Necesita un Volume GLOBAL propio para el clima de afuera (distinto del Volume del aire acondicionado).
// Va en un objeto que siempre esté activo (por ejemplo un objeto vacío "ClimaAmbiente").
public class AmbienteClima : MonoBehaviour
{
    [Header("Volume del clima de AFUERA (Global, aparte del que usa el aire)")]
    [SerializeField] private Volume volumen;

    [Header("Temperaturas de referencia (°C)")]
    [SerializeField] private float temperaturaFria = 14f;
    [SerializeField] private float temperaturaNeutra = 22f;
    [SerializeField] private float temperaturaCalida = 30f;

    [Header("Balance de blancos (-100 = muy frío, 0 = normal, 100 = muy cálido)")]
    [SerializeField] private float balanceFrio = -40f;
    [SerializeField] private float balanceNeutro = 0f;
    [SerializeField] private float balanceCalido = 40f;

    [Header("Filtro de color")]
    [SerializeField] private Color filtroFrio = new Color(0.7f, 0.85f, 1f);
    [SerializeField] private Color filtroNeutro = Color.white;
    [SerializeField] private Color filtroCalido = new Color(1f, 0.82f, 0.6f);

    [Header("Saturación (-100 a 100)")]
    [SerializeField] private float saturacionFria = -15f;
    [SerializeField] private float saturacionNeutra = 0f;
    [SerializeField] private float saturacionCalida = 15f;

    [Header("Suavizado")]
    [Tooltip("Qué tan rápido acompaña el cambio de temperatura. Más alto = más rápido.")]
    [SerializeField] private float velocidad = 2f;

    private WhiteBalance balance;
    private ColorAdjustments ajuste;

    private float balanceActual;
    private Color filtroActual;
    private float saturacionActual;

    private void Start()
    {
        if (volumen == null)
        {
            Debug.LogWarning("AmbienteClima: falta arrastrar el Volume en 'Volumen'.", this);
            enabled = false;
            return;
        }

        // "profile" es una copia propia del perfil: los cambios en juego no tocan el archivo original.
        VolumeProfile perfil = volumen.profile;

        // Si el perfil todavía no tiene estos efectos, los agrega.
        if (!perfil.TryGet(out balance)) balance = perfil.Add<WhiteBalance>(false);
        if (!perfil.TryGet(out ajuste)) ajuste = perfil.Add<ColorAdjustments>(false);

        volumen.isGlobal = true;
        volumen.weight = 1f;

        // Arranca directamente en el valor correcto para que no haya un salto al empezar.
        Calcular(out balanceActual, out filtroActual, out saturacionActual);
        Aplicar();
    }

    private void Update()
    {
        Calcular(out float balanceObj, out Color filtroObj, out float saturacionObj);

        // Acerca suavemente los valores actuales a los objetivos (funciona también en pausa).
        float paso = Mathf.Clamp01(velocidad * Time.unscaledDeltaTime);
        balanceActual = Mathf.Lerp(balanceActual, balanceObj, paso);
        filtroActual = Color.Lerp(filtroActual, filtroObj, paso);
        saturacionActual = Mathf.Lerp(saturacionActual, saturacionObj, paso);

        Aplicar();
    }

    // Según la temperatura de afuera, calcula los valores objetivo.
    private void Calcular(out float bal, out Color filtro, out float sat)
    {
        // Si no hay ClimaExterior en la escena, usa la temperatura neutra (no cambia nada).
        float temp = ClimaExterior.Instance != null ? ClimaExterior.Instance.Temperatura : temperaturaNeutra;

        if (temp <= temperaturaNeutra)
        {
            float t = Mathf.InverseLerp(temperaturaFria, temperaturaNeutra, temp);
            bal = Mathf.Lerp(balanceFrio, balanceNeutro, t);
            filtro = Color.Lerp(filtroFrio, filtroNeutro, t);
            sat = Mathf.Lerp(saturacionFria, saturacionNeutra, t);
        }
        else
        {
            float t = Mathf.InverseLerp(temperaturaNeutra, temperaturaCalida, temp);
            bal = Mathf.Lerp(balanceNeutro, balanceCalido, t);
            filtro = Color.Lerp(filtroNeutro, filtroCalido, t);
            sat = Mathf.Lerp(saturacionNeutra, saturacionCalida, t);
        }
    }

    // Escribe los valores en el perfil. Override() activa el parámetro y le asigna el valor.
    private void Aplicar()
    {
        balance.temperature.Override(balanceActual);
        ajuste.colorFilter.Override(filtroActual);
        ajuste.saturation.Override(saturacionActual);
    }
}