using UnityEngine;
using TMPro;

public class SelectorDePiso : MonoBehaviour
{
    [Header("Referencias UI")]
    public GameObject cartelAviso;      // El cartelito de "Pulsa M"
    public GameObject panelPiso;        // El Panel de pisos del Canvas

    [Header("Estructura")]
    public Transform casaModulos;       // Arrastra aquí "casa Modulos"

    [Header("Materiales Disponibles")]
    public Material[] materialesPiso;   // Tus materiales de piso

    private bool jugadorEnZona = false;
    private bool panelAbierto = false;

    void Start()
    {
        if (cartelAviso != null) cartelAviso.SetActive(false);
        if (panelPiso != null) panelPiso.SetActive(false);
    }

    void Update()
    {
        if (jugadorEnZona && !panelAbierto && Input.GetKeyDown(KeyCode.M))
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
        if (panelPiso != null) panelPiso.SetActive(true);

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        Time.timeScale = 0f; // Pausa el juego
    }

    public void SeleccionarMaterialPiso(int indice)
    {
        if (indice >= 0 && indice < materialesPiso.Length && casaModulos != null)
        {
            MeshRenderer[] todosLosMesh = casaModulos.GetComponentsInChildren<MeshRenderer>();

            foreach (MeshRenderer m in todosLosMesh)
            {
                string nombre = m.gameObject.name.ToLower();
                if (nombre.Contains("piso") || nombre.Contains("suelo"))
                {
                    m.material = materialesPiso[indice];
                }
            }
        }
    }

    public void ConfirmarSeleccion()
    {
        CerrarPanel();
        Destroy(gameObject); // Borra la zona de activación
    }

    void CerrarPanel()
    {
        panelAbierto = false;
        if (panelPiso != null) panelPiso.SetActive(false);
        Time.timeScale = 1f; // Reanuda el juego
    }
}