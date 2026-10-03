using UnityEngine;
using TMPro;

public class SelectorDePared : MonoBehaviour
{
    [Header("Referencias UI")]
    public GameObject cartelAviso;      // El cartelito de "Pulsa C"
    public GameObject panelColores;     // El PanelCambioColorPared

    [Header("Estructura de Paredes")]
    public Transform casaModulos;       // Arrastra aquí el objeto "casa Modulos"

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

        // Mostrar y liberar el cursor para que puedas hacer clic sin problemas
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        Time.timeScale = 0f; // Pausa el juego
    }

    public void SeleccionarMaterial(int indice)
    {
        if (indice >= 0 && indice < materialesPared.Length && casaModulos != null)
        {
            MeshRenderer[] todasLasParedes = casaModulos.GetComponentsInChildren<MeshRenderer>();

            foreach (MeshRenderer pared in todasLasParedes)
            {
                if (pared.gameObject.name.ToLower().Contains("pared"))
                {
                    pared.material = materialesPared[indice];
                }
            }
        }
    }

    public void ConfirmarSeleccion()
    {
        CerrarPanel();
        // Destruye este cartel para que no vuelva a aparecer
        //Destroy(gameObject);
    }

    void CerrarPanel()
    {
        panelAbierto = false;
        if (panelColores != null) panelColores.SetActive(false);

        Time.timeScale = 1f; // Reanuda el juego

        // --- PEQUEÑO CAMBIO CLAVE ---
        // Si el jugador sigue pisando la zona al cerrar el panel, volvemos a mostrar el cartel de inmediato:
        if (jugadorEnZona && cartelAviso != null)
        {
            cartelAviso.SetActive(true);
        }
    }
}