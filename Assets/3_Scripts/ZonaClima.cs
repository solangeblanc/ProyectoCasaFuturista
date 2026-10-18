using UnityEngine;
using TMPro;

public class ZonaClima : MonoBehaviour
{
    [Header("Cuarto")]
    [SerializeField] private string nombreCuarto = "Living";
    [Tooltip("Se usa solo si no hay un script de climatización asignado")]
    [SerializeField] private float temperaturaActual = 26f;

    [Header("Detección por cercanía")]
    [SerializeField] private Transform puntoAire;
    [SerializeField] private float radio = 4f;
    [SerializeField] private Transform jugador;
    [Tooltip("Segundos que se muestra el cartel. 0 = mientras esté cerca")]
    [SerializeField] private float segundosVisible = 6f;

    [Header("Cartel")]
    [SerializeField] private GameObject panelInfo;
    [SerializeField] private TMP_Text texto;
    [SerializeField] private ClimatizacionHabitacion clima;

    [Header("Diagnóstico")]
    [SerializeField] private bool mostrarLogs = false;

    private bool estabaCerca = false;
    private bool agotado = false;
    private float tiempoMostrado = 0f;
    private float proximoLog = 0f;

    private void Start()
    {
        if (jugador == null)
        {
            if (Camera.main != null) jugador = Camera.main.transform;
        }

        if (mostrarLogs)
        {
            if (puntoAire == null) Debug.LogWarning("[Clima] Punto Aire está vacío");
            if (jugador == null) Debug.LogWarning("[Clima] No hay jugador");
            if (panelInfo == null) Debug.LogWarning("[Clima] Panel Info está vacío");
            if (texto == null) Debug.LogWarning("[Clima] Texto está vacío");
            if (ClimaExterior.Instance == null) Debug.LogWarning("[Clima] No hay ClimaExterior en la escena");
        }

        Mostrar(false);
    }

    private void Update()
    {
        GameManager gm = GameManager.Instance;

        if (gm == null || !gm.EstaJugando || puntoAire == null || jugador == null)
        {
            Mostrar(false);
            return;
        }

        Vector3 a = jugador.position;
        Vector3 b = puntoAire.position;
        a.y = 0f;
        b.y = 0f;
        float distancia = Vector3.Distance(a, b);
        bool cerca = distancia <= radio;

        if (cerca && !estabaCerca)
        {
            tiempoMostrado = 0f;
            agotado = false;
        }
        estabaCerca = cerca;

        bool visible = cerca && !agotado;

        if (visible && segundosVisible > 0f)
        {
            tiempoMostrado += Time.unscaledDeltaTime;
            if (tiempoMostrado >= segundosVisible)
            {
                agotado = true;
                visible = false;
            }
        }

        Log("Distancia: " + distancia.ToString("0.0") + " | Cerca: " + cerca + " | Visible: " + visible);

        Mostrar(visible);

        if (!visible) return;
        if (texto == null) return;

        if (ClimaExterior.Instance == null)
        {
            texto.text = "Falta el script ClimaExterior en el GameManager";
            return;
        }

        float exterior = ClimaExterior.Instance.Temperatura;
        float recomendada = ClimaExterior.Recomendada(exterior);
        string modoRecomendado = ClimaExterior.ModoRecomendado(exterior);

        float ambiente = temperaturaActual;
        string lineaAire = "";
        if (clima != null)
        {
            ambiente = clima.Temperatura;
            lineaAire = "\nAire: " + clima.EstadoTexto;
        }

        texto.text = nombreCuarto + "\n"
                   + "Exterior: " + exterior.ToString("0") + " °C\n"
                   + "Ambiente: " + ambiente.ToString("0.0") + " °C\n"
                   + "Recomendada: " + recomendada.ToString("0") + " °C (" + modoRecomendado + ")"
                   + lineaAire;
    }

    private void Mostrar(bool mostrar)
    {
        if (panelInfo == null) return;
        if (panelInfo.activeSelf == mostrar) return;
        panelInfo.SetActive(mostrar);
    }

    private void Log(string mensaje)
    {
        if (!mostrarLogs) return;
        if (Time.unscaledTime < proximoLog) return;

        proximoLog = Time.unscaledTime + 1f;
        Debug.Log("[Clima] " + mensaje);
    }

    private void OnDrawGizmosSelected()
    {
        if (puntoAire == null) return;
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(puntoAire.position, radio);
    }
}