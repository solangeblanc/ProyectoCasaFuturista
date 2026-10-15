using UnityEngine;

public class MenuTele : MonoBehaviour
{
    public GameObject menuPrincipal;
    public GameObject panelParedes;
    public GameObject panelPisos;
    public GameObject panelVentanas;
    public GameObject canvasCambios;
    public AbrirTele abrirTeleScript;

    public void MostrarParedes() { menuPrincipal.SetActive(false); panelParedes.SetActive(true); }
    public void MostrarPisos() { menuPrincipal.SetActive(false); panelPisos.SetActive(true); }
    public void MostrarVentanas() { menuPrincipal.SetActive(false); panelVentanas.SetActive(true); }
    public void VolverMenu() { panelParedes.SetActive(false); panelPisos.SetActive(false); panelVentanas.SetActive(false); menuPrincipal.SetActive(true); }

    public void SalirTele()
    {
        canvasCambios.SetActive(false);
        abrirTeleScript.Descongelar();
    }
}