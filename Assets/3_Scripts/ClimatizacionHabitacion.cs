using UnityEngine;

public enum ModoClima { Frio, Calor, Ventilacion, Automatico }

public class ClimatizacionHabitacion : MonoBehaviour
{
    [Header("Cuarto")]
    [SerializeField] private float temperaturaInicial = 26f;

    [Header("Control (se puede cambiar en Play para probar)")]
    [SerializeField] private bool encendido = false;
    [SerializeField] private ModoClima modo = ModoClima.Automatico;
    [Range(18f, 30f)]
    [SerializeField] private float objetivo = 24f;

    [Header("Simulación")]
    [Tooltip("Grados por segundo que enfría o calienta el equipo")]
    [SerializeField] private float velocidad = 0.2f;
    [Tooltip("Grados por segundo que el cuarto se acerca a la temperatura de afuera")]
    [SerializeField] private float deriva = 0.02f;

    public float Temperatura { get; private set; }
    public bool Encendido => encendido;
    public ModoClima Modo => modo;
    public float Objetivo => objetivo;

    public string EstadoTexto
    {
        get
        {
            if (!encendido) return "Apagado";
            float ext = Exterior();
            return NombreModo(ModoEfectivo(ext)) + " a " + ObjetivoEfectivo(ext).ToString("0") + " °C";
        }
    }

    private void Awake()
    {
        Temperatura = temperaturaInicial;
    }

    private void Update()
    {
        float ext = Exterior();
        float dt = Time.deltaTime;

        if (!encendido)
        {
            Temperatura = Mathf.MoveTowards(Temperatura, ext, deriva * dt);
            return;
        }

        ModoClima m = ModoEfectivo(ext);
        float meta = ObjetivoEfectivo(ext);

        if (m == ModoClima.Frio && Temperatura > meta)
            Temperatura = Mathf.MoveTowards(Temperatura, meta, velocidad * dt);
        else if (m == ModoClima.Calor && Temperatura < meta)
            Temperatura = Mathf.MoveTowards(Temperatura, meta, velocidad * dt);
        else if (m == ModoClima.Ventilacion)
            Temperatura = Mathf.MoveTowards(Temperatura, ext, velocidad * 0.3f * dt);
        else
            Temperatura = Mathf.MoveTowards(Temperatura, ext, deriva * dt);
    }

    private float Exterior()
    {
        if (ClimaExterior.Instance != null) return ClimaExterior.Instance.Temperatura;
        return Temperatura;
    }

    private ModoClima ModoEfectivo(float ext)
    {
        if (modo != ModoClima.Automatico) return modo;
        if (ext >= 26f) return ModoClima.Frio;
        if (ext <= 16f) return ModoClima.Calor;
        return ModoClima.Ventilacion;
    }

    private float ObjetivoEfectivo(float ext)
    {
        if (modo == ModoClima.Automatico) return ClimaExterior.Recomendada(ext);
        return objetivo;
    }

    private string NombreModo(ModoClima m)
    {
        switch (m)
        {
            case ModoClima.Frio: return "Frío";
            case ModoClima.Calor: return "Calor";
            case ModoClima.Ventilacion: return "Ventilación";
            default: return "Automático";
        }
    }

    // Lo usa el tinte de pantalla: dice si el aire está encendido y en qué modo real trabaja
    public bool TieneEfecto(out ModoClima efectivo)
    {
        efectivo = ModoEfectivo(Exterior());
        return encendido;
    }

    // Métodos públicos para el panel de programación (próximo paso)
    public void Alternar() { encendido = !encendido; }
    public void Encender() { encendido = true; }
    public void Apagar() { encendido = false; }
    public void SetModo(ModoClima nuevo) { modo = nuevo; }
    public void SubirObjetivo() { objetivo = Mathf.Min(30f, objetivo + 1f); }
    public void BajarObjetivo() { objetivo = Mathf.Max(18f, objetivo - 1f); }
}