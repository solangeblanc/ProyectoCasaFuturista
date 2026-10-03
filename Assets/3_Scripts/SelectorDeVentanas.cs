using UnityEngine;

public class SelectorDeVentanas : MonoBehaviour
{
    [Header("Referencias UI")]
    public GameObject cartelAviso;
    public GameObject panelVentanas;

    [Header("Estructura")]
    public Transform casaModulos; // Arrastra el objeto padre donde están tus ventanas

    [Header("Materiales Disponibles")]
    public Material[] materialesVentanas; // 5 materiales para ventanas

    [Header("Player")]
    public MonoBehaviour playerMove;
    public MonoBehaviour mouseLook;

    private bool jugadorEnZona = false;
    private bool panelAbierto = false;

    void Start()
    {
        if (cartelAviso != null) cartelAviso.SetActive(false);
        if (panelVentanas != null) panelVentanas.SetActive(false);
    }

    void Update()
    {
        if (jugadorEnZona && !panelAbierto && Input.GetKeyDown(KeyCode.E))
        {
            AbrirPanel();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            jugadorEnZona = true;
            if (cartelAviso != null && !panelAbierto) cartelAviso.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            jugadorEnZona = false;
            if (cartelAviso != null) cartelAviso.SetActive(false);
            if (panelAbierto) CerrarPanel();
        }
    }

    void AbrirPanel()
    {
        panelAbierto = true;
        if (cartelAviso != null) cartelAviso.SetActive(false);
        if (panelVentanas != null) panelVentanas.SetActive(true);
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        Time.timeScale = 0f;
        if (playerMove != null) playerMove.enabled = false;
        if (mouseLook != null) mouseLook.enabled = false;
    }

    public void SeleccionarMaterialVentana(int indice)
    {
        if (casaModulos == null) { Debug.LogError("FALTA casaModulos en SelectorDeVentanas"); return; }
        if (indice < 0 || indice >= materialesVentanas.Length) { Debug.LogError("Indice fuera de rango"); return; }
        if (materialesVentanas[indice] == null) { Debug.LogError("Material " + indice + " esta vacio"); return; }

        MeshRenderer[] todos = casaModulos.GetComponentsInChildren<MeshRenderer>(true);
        int cambiados = 0;

        foreach (MeshRenderer m in todos)
        {
            string nombre = m.gameObject.name.ToLower();
            if (nombre.Contains("ventana") || nombre.Contains("vidrio") || nombre.Contains("marco") || nombre.Contains("window") || nombre.Contains("cristal"))
            {
                m.material = materialesVentanas[indice];
                cambiados++;
            }
        }
        Debug.Log("Ventanas cambiadas: " + cambiados);
        if (cambiados == 0) Debug.LogWarning("No encontro ventanas. Renombra tus ventanas para que contengan 'ventana' en el nombre");
    }

    public void ConfirmarSeleccion() { CerrarPanel(); }

    void CerrarPanel()
    {
        panelAbierto = false;
        if (panelVentanas != null) panelVentanas.SetActive(false);
        if (jugadorEnZona && cartelAviso != null) cartelAviso.SetActive(true);
        Time.timeScale = 1f;
        if (playerMove != null) playerMove.enabled = true;
        if (mouseLook != null) mouseLook.enabled = true;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}