using UnityEngine;
using TMPro;

public class ZonaClima : MonoBehaviour
{
    [Header("Cuarto")]
    [SerializeField] private string nombreCuarto = "Living";
    [SerializeField] private ClimatizacionHabitacion clima;
    [Tooltip("Temporal: luego la maneja el script de climatización")]
    [SerializeField] private float temperaturaActual = 26f;

    [Header("Detección por cercanía")]
    [Tooltip("Arrastrá acá el aire acondicionado (aireLiving)")]
    [SerializeField] private Transform puntoAire;
    [SerializeField] private float radio = 3f;
    [Tooltip("Si queda vacío usa la cámara con tag MainCamera")]
    [SerializeField] private Transform jugador;
    [Tooltip("Segundos que se muestra el cartel. 0 = mientras esté cerca")]
    [SerializeField] private float segundosVisible = 6f;

    [Header("Cartel")]
    [SerializeField] private GameObject panelInfo;
    [SerializeField] private TMP_Text texto;

    private bool estabaCerca = false;
    private bool agotado = false;
    private float tiempoMostrado = 0f;

    private void Start()
    {
        if (jugador == null && Camera.main != null)
            jugador = Camera.main.transform;

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

        // Distancia en horizontal (el equipo está alto en la pared)
        Vector3 a = jugador.position;
        Vector3 b = puntoAire.position;
        a.y = 0f;
        b.y = 0f;
        bool cerca = Vector3.Distance(a, b) <= radio;

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

        Mostrar(visible);

        if (!visible) return;
        if (texto == null) return;
        if (ClimaExterior.Instance == null) return;

        float exterior = ClimaExterior.Instance.Temperatura;
        float recomendada = ClimaExterior.Recomendada(exterior);
        string modo = ClimaExterior.ModoRecomendado(exterior);

        float ambiente = temperaturaActual;
        string aire = "";
        if (clima != null)
        {
            ambiente = clima.Temperatura;
            aire = "\nAire: " + clima.EstadoTexto;
        }

        texto.text = nombreCuarto + "\n"
                   + "Exterior: " + exterior.ToString("0") + " °C\n"
                   + "Ambiente: " + ambiente.ToString("0.0") + " °C\n"
                   + "Recomendada: " + recomendada.ToString("0") + " °C (" + modo + ")"
                   + aire;
    }

    private void Mostrar(bool mostrar)
    {
        if (panelInfo == null) return;
        if (panelInfo.activeSelf == mostrar) return;
        panelInfo.SetActive(mostrar);
    }

    // Dibuja el radio en la Scene cuando seleccionás el objeto
    private void OnDrawGizmosSelected()
    {
        if (puntoAire == null) return;
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(puntoAire.position, radio);
    }
}
