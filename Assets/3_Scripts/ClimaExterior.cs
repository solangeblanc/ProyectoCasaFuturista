using UnityEngine;

public class ClimaExterior : MonoBehaviour
{
    public static ClimaExterior Instance { get; private set; }

    [Header("Modo de prueba")]
    [SerializeField] private bool usarManual = true;
    [SerializeField] private float temperaturaManual = 32f;

    [Header("Simulación automática (día completo)")]
    [SerializeField] private float temperaturaMedia = 22f;
    [SerializeField] private float amplitud = 8f;
    [SerializeField] private float segundosPorDia = 120f;

    public float Temperatura { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }
        Instance = this;
        Temperatura = temperaturaManual;
    }

    private void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }

    private void Update()
    {
        if (usarManual)
        {
            Temperatura = temperaturaManual;
            return;
        }

        float t = (Time.time % segundosPorDia) / segundosPorDia;
        Temperatura = temperaturaMedia + amplitud * Mathf.Sin((t - 0.25f) * 2f * Mathf.PI);
    }

    // Regla simple de recomendación
    public static float Recomendada(float exterior)
    {
        if (exterior >= 26f) return 24f;
        if (exterior <= 16f) return 21f;
        return 23f;
    }

    public static string ModoRecomendado(float exterior)
    {
        if (exterior >= 26f) return "Frío";
        if (exterior <= 16f) return "Calor";
        return "Ventilación";
    }
}
