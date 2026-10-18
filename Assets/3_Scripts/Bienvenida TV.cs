using UnityEngine;

public class BienvenidaTV : MonoBehaviour
{
    public GameObject TextoEspacio; // PanelHintEspacio
    public GameObject Menu; // Panel menu principal

    private bool jugadorCerca = false;
    private GameObject jugador;
    private MonoBehaviour playerControllerScript;
    private MonoBehaviour playerInputScript;

    void Start()
    {
        TextoEspacio.SetActive(false);
        Menu.SetActive(false);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            jugadorCerca = true;
            jugador = other.transform.root.gameObject;

            foreach (var s in jugador.GetComponentsInChildren<MonoBehaviour>())
            {
                if (s.GetType().Name == "PlayerController") playerControllerScript = s;
                if (s.GetType().Name == "PlayerInput") playerInputScript = s;
            }

            // Solo muestra el hint si el menú NO está abierto
            if (!Menu.activeSelf)
                TextoEspacio.SetActive(true);
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Si el menú está abierto, fuerza el hint apagado
            if (Menu.activeSelf && TextoEspacio.activeSelf)
                TextoEspacio.SetActive(false);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            jugadorCerca = false;
            TextoEspacio.SetActive(false);
        }
    }

    void Update()
    {
        if (jugadorCerca && Input.GetKeyDown(KeyCode.Space) && !Menu.activeSelf)
        {
            // APAGA el hint ANTES de prender el menú
            TextoEspacio.SetActive(false);
            Menu.SetActive(true);

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            if (playerControllerScript != null) playerControllerScript.enabled = false;
            if (playerInputScript != null) playerInputScript.enabled = false;
        }
    }

    public void CerrarMenu()
    {
        Menu.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (playerControllerScript != null) playerControllerScript.enabled = true;
        if (playerInputScript != null) playerInputScript.enabled = true;

        if (jugador == null) jugador = GameObject.FindGameObjectWithTag("Player");

        if (jugadorCerca) TextoEspacio.SetActive(true);
    }
}