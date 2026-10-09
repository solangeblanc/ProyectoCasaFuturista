using UnityEngine;

public class SelectorDePared : MonoBehaviour
{
    [Header("Referencias UI")]
    public GameObject cartelAviso; // el cartelito de "Pulsa C"
    public GameObject panelColores; // el PanelCambioColorPared

    [Header("Estructura")]
    public Transform casaModulos; // rrastra aquí "casa Modulos"

    [Header("Materiales Disponibles")]
    public Material[] materialesPared; // materiales de pared

    [Header("Player")]
    public MonoBehaviour playerMove; // arrastra tu Character Controller / First Person Controller
    public MonoBehaviour mouseLook; // arrastra tu MouseLook / Camera Look

    private bool jugadorEnZona = false;
    private bool panelAbierto = false;

    void Start()
    {
        if (cartelAviso != null) cartelAviso.SetActive(false);
        if (panelColores != null) panelColores.SetActive(false);
    }

    void Update()
    {
        if (jugadorEnZona && !panelAbierto && Input.GetKeyDown(KeyCode.Space))
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

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        Time.timeScale = 0f; // Pausa el juego

        // Congela movimiento y camara
        if (playerMove != null) playerMove.enabled = false;
        if (mouseLook != null) mouseLook.enabled = false;
    }

    public void SeleccionarMaterial(int indice)
    {
        if (indice >= 0 && indice < materialesPared.Length && casaModulos != null)
        {
            MeshRenderer[] todosLosMesh = casaModulos.GetComponentsInChildren<MeshRenderer>();

            foreach (MeshRenderer m in todosLosMesh)
            {
                string nombre = m.gameObject.name.ToLower();
                if (nombre.Contains("pared"))
                {
                    m.material = materialesPared[indice];
                }
            }
        }
    }

    public void ConfirmarSeleccion()
    {
        CerrarPanel();
    }

    void CerrarPanel()
    {
        panelAbierto = false;
        if (panelColores != null) panelColores.SetActive(false);

        if (jugadorEnZona && cartelAviso != null)
        {
            cartelAviso.SetActive(true);
        }

        Time.timeScale = 1f; // Reanuda el juego

        // Reactiva movimiento y camara
        if (playerMove != null) playerMove.enabled = true;
        if (mouseLook != null) mouseLook.enabled = true;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}