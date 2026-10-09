using UnityEngine;
using TMPro;

public class SelectorDePiso : MonoBehaviour
{
    [Header("Referencias UI")]
    public GameObject cartelAviso;
    public GameObject panelPiso;

    [Header("Estructura")]
    public Transform casaModulos;

    [Header("Materiales Disponibles")]
    public Material[] materialesPiso;

    [Header("Player")]
    public MonoBehaviour playerMove;
    public MonoBehaviour mouseLook;

    private bool jugadorEnZona = false;
    private bool panelAbierto = false;

    void Start()
    {
        if (cartelAviso != null) cartelAviso.SetActive(false);
        if (panelPiso != null) panelPiso.SetActive(false);
    }

    void Update()
    {
        if (jugadorEnZona && !panelAbierto && Input.GetKeyDown(KeyCode.Space))//si el jug esta en la zona y el panel esta cerrado  y aprieta M
        {
            AbrirPanel();//se abre el panel de eleccion de colores
        }
    }

    private void OnTriggerEnter(Collider other)//se activa cuando algo entra en la zona marcado como on trigger
    {
        if (other.CompareTag("Player"))//si tiene la etiqueta player
        {
            jugadorEnZona = true;//esta el jug en la zona
            if (cartelAviso != null && !panelAbierto)//el cartel de aviso esta mostrandose
            {
                cartelAviso.SetActive(true);//muestra el panel para seleccionar
            }
        }
    }

    private void OnTriggerExit(Collider other)//on trigger se activa cuando el jug sale del area
    {
        if (other.CompareTag("Player"))
        {
            jugadorEnZona = false;
            if (cartelAviso != null)
            {
                cartelAviso.SetActive(false);//oculta el cartel
            }
            if (panelAbierto)
            {
                CerrarPanel();//si esta abierto lo cierra
            }
        }
    }

    void AbrirPanel()
    {
        panelAbierto = true;//cambia el estado a abierto
        if (cartelAviso != null) cartelAviso.SetActive(false);//oculta el cartel de aviso
        if (panelPiso != null) panelPiso.SetActive(true);

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;//muestra el cursor en pantalla asi el jug puede seleccionar los botones
        Time.timeScale = 0f;//pausa el tiempo de juego

        if (playerMove != null) playerMove.enabled = false;//desactiva el movimiento del jugad para que se quede quieto
        if (mouseLook != null) mouseLook.enabled = false;
    }

    public void SeleccionarMaterialPiso(int indice)
    {
        if (indice >= 0 && indice < materialesPiso.Length && casaModulos != null)//recibe un indice de material para que se ponga el material
        {
            MeshRenderer[] todosLosMesh = casaModulos.GetComponentsInChildren<MeshRenderer>();

            foreach (MeshRenderer m in todosLosMesh)//revisa, recorre todos los indices
            {
                string nombre = m.gameObject.name.ToLower();
                if (nombre.Contains("piso") || nombre.Contains("suelo"))//busca piso o suelo
                {
                    m.material = materialesPiso[indice];//asigna el material que se elige
                }
            }
        }
    }

    public void ConfirmarSeleccion()
    {
        CerrarPanel();//si se confirma con el boton seleccionar ciera el panel de opciones
    }


    void CerrarPanel()
    {
        panelAbierto = false;
        if (panelPiso != null) panelPiso.SetActive(false);//si panel esta cerrado y el panelde seleccion esta abierto lo cierra

        if (jugadorEnZona && cartelAviso != null)//el jug esta en la zona y el cartel esta apagado
        {
            cartelAviso.SetActive(true);//lo activa
        }

        Time.timeScale = 1f;//reanuda el tiempo de juego

        if (playerMove != null) playerMove.enabled = true;
        if (mouseLook != null) mouseLook.enabled = true;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}