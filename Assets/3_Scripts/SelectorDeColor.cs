using UnityEngine;
using UnityEngine.UI;

public class SelectorDeColor : MonoBehaviour
{
    [Header("Referencias UI")]
    public GameObject cartelAviso;     // El texto flotante que dice que presione una tecla
    public GameObject panelColores;    // El panel con los 5 botones (PanelCambioColor)
    public Renderer pisoRenderer;      // El Renderer del piso que va a cambiar de material

    [Header("Materiales disponibles")]
    public Material[] materialesDisponibles = new Material[5]; // Asigna 5 materiales desde el Inspector

    private bool jugadorEnZona = false;
    private bool interactuando = false;
    private Material materialOriginal;

    void Start()
    {
        if (pisoRenderer != null)
        {
            materialOriginal = pisoRenderer.material; // Guardamos el material inicial
        }

        if (cartelAviso) cartelAviso.SetActive(false);
        if (panelColores) panelColores.SetActive(false);
    }

    void Update()
    {
        if (jugadorEnZona && !interactuando && Input.GetKeyDown(KeyCode.E))
        {
            AbrirPanelColores();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            jugadorEnZona = true;
            if (cartelAviso && !interactuando) cartelAviso.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            jugadorEnZona = false;

            if (interactuando)
            {
                CerrarPanel(false);
            }

            if (cartelAviso) cartelAviso.SetActive(false);
        }
    }

    void AbrirPanelColores()
    {
        interactuando = true;
        if (cartelAviso) cartelAviso.SetActive(false);
        if (panelColores) panelColores.SetActive(true);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    // Este método se conecta desde los Botones de la UI (On Click)
    public void PrevisualizarMaterial(int indiceMaterial)
    {
        if (pisoRenderer != null && indiceMaterial >= 0 && indiceMaterial < materialesDisponibles.Length)
        {
            if (materialesDisponibles[indiceMaterial] != null)
            {
                pisoRenderer.material = materialesDisponibles[indiceMaterial];
            }
        }
    }

    // Botón de Confirmar
    public void ConfirmarSeleccion()
    {
        CerrarPanel(true);
    }

    void CerrarPanel(bool confirmado)
    {
        interactuando = false;
        if (panelColores) panelColores.SetActive(false);

        if (!confirmado)
        {
            if (pisoRenderer != null) pisoRenderer.material = materialOriginal;
        }
        else
        {
            Destroy(gameObject); // Destruye este cartel para que no vuelva a aparecer
        }

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}