using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
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
    [Tooltip("Dejalo en 0: el cartel queda mientras estés cerca")]
    [SerializeField] private float segundosVisible = 0f;

    [Header("Tecla")]
    [SerializeField] private Key teclaAire = Key.Space;

    [Header("Cartel")]
    [SerializeField] private GameObject panelInfo;
    [SerializeField] private TMP_Text texto;
    [SerializeField] private ClimatizacionHabitacion clima;

    [Header("Botón del cartel")]
    [SerializeField] private TMP_Text textoBoton;
    [SerializeField] private Image imagenBoton;
    [SerializeField] private Color colorApagado = new Color(0.35f, 0.35f, 0.4f);
    [SerializeField] private Color colorFrio = new Color(0.2f, 0.6f, 0.85f);
    [SerializeField] private Color colorCalor = new Color(0.9f, 0.5f, 0.2f);

    private bool estabaCerca = false;
    private bool agotado = false;
    private float tiempoMostrado = 0f;

    private void Start()
    {
        if (jugador == null)
        {
            if (Camera.main != null) jugador = Camera.main.transform;
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

        // Barra espaciadora: Apagado -> Frío -> Calor -> Apagado
        if (clima != null && Keyboard.current != null)
        {
            if (Keyboard.current[teclaAire].wasPressedThisFrame)
                clima.Ciclar();
        }

        ActualizarTextos();
    }

    private void ActualizarTextos()
    {
        if (ClimaExterior.Instance == null)
        {
            if (texto != null) texto.text = "Falta el script ClimaExterior en el GameManager";
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

        if (texto != null)
        {
            texto.text = nombreCuarto + "\n"
                       + "Exterior: " + exterior.ToString("0") + " °C\n"
                       + "Ambiente: " + ambiente.ToString("0.0") + " °C\n"
                       + "Recomendado: " + modoRecomendado + " a " + recomendada.ToString("0") + " °C"
                       + lineaAire;
        }

        if (clima == null) return;

        string accion;
        Color color;

        if (!clima.Encendido)
        {
            accion = "ESPACIO: Encender aire frío (" + clima.ObjetivoFrio.ToString("0") + " °C)";
            color = colorApagado;
        }
        else if (clima.Modo == ModoClima.Frio)
        {
            accion = "ESPACIO: Cambiar a calefacción (" + clima.ObjetivoCalor.ToString("0") + " °C)";
            color = colorFrio;
        }
        else
        {
            accion = "ESPACIO: Apagar";
            color = clima.Modo == ModoClima.Calor ? colorCalor : colorFrio;
        }

        if (textoBoton != null) textoBoton.text = accion;
        if (imagenBoton != null) imagenBoton.color = color;
    }

    private void Mostrar(bool mostrar)
    {
        if (panelInfo == null) return;
        if (panelInfo.activeSelf == mostrar) return;
        panelInfo.SetActive(mostrar);
    }

    private void OnDrawGizmosSelected()
    {
        if (puntoAire == null) return;
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(puntoAire.position, radio);
    }
}