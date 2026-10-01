using UnityEngine;
using TMPro;

public class SelectorDePared : MonoBehaviour
{
    [Header("Referencias UI")]
    public GameObject cartelAviso;      // El cartelito de "Pulsa C"
    public GameObject panelColores;     // El PanelCambioColorPared
    public MeshRenderer paredRenderer;  // El MeshRenderer de la pared

    [Header("Materiales Disponibles")]
    public Material[] materialesPared;  // Tus 5 materiales de pared

    private bool jugadorEnZona = false;
    private bool panelAbierto = false;

    void Start()
    {
        if (cartelAviso != null) cartelAviso.SetActive(false);
        if (panelColores != null) panelColores.SetActive(false);
    }

    void Update()
    {
        // Detectar si el jugador está en la zona y presiona la tecla C
        if (jugadorEnZona && !panelAbierto && Input.GetKeyDown(KeyCode.C))
        {
            AbrirPanel();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            jugadorEnZona = true;
            if (cartelAviso != null && !panelAbierto)
            {
                cartelAviso.SetActive(true);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            jugadorEnZona = false;
            if (cartelAviso != null)
            {
                cartelAviso.SetActive(false);
            }
            if (panelAbierto)
            {
                CerrarPanel();
            }
        }
    }

    void AbrirPanel()
    {
        panelAbierto = true;
        if (cartelAviso != null) cartelAviso.SetActive(false);
        if (panelColores != null) panelColores.SetActive(true);
        Time.timeScale = 0f; // Pausa el juego opcionalmente para elegir cómodamente
    }

    public void SeleccionarMaterial(int indice)
    {
        if (paredRenderer != null && indice >= 0 && indice < materialesPared.Length)
        {
            paredRenderer.material = materialesPared[indice];
        }
    }

    public void ConfirmarSeleccion()
    {
        CerrarPanel();
        // Destruye este cartel para que no vuelva a aparecer nunca más
        Destroy(gameObject);
    }

    void CerrarPanel()
    {
        panelAbierto = false;
        if (panelColores != null) panelColores.SetActive(false);
        Time.timeScale = 1f; // Reanuda el juego
    }
}
