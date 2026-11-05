using UnityEngine;

public class Menu_Tele : MonoBehaviour
{
    public GameObject menuPrincipal;
    public GameObject panelParedes;
    public GameObject panelPisos;
    public GameObject panelVentanas;
    public GameObject panelPolarizado;
    public GameObject canvasCambios;
    public AbrirTele abrirTeleScript;

    public void MostrarParedes() { menuPrincipal.SetActive(false); panelParedes.SetActive(true); }
    public void MostrarPisos() { menuPrincipal.SetActive(false); panelPisos.SetActive(true); }
    public void MostrarVentanas() { menuPrincipal.SetActive(false); panelVentanas.SetActive(true); }
    public void MostrarPolarizado() { menuPrincipal.SetActive(false); panelPolarizado.SetActive(true); }

    public void VolverMenu()
    {
        panelParedes.SetActive(false);
        panelPisos.SetActive(false);
        panelVentanas.SetActive(false);
        panelPolarizado.SetActive(false);
        menuPrincipal.SetActive(true);
    }

    public void SalirTele()
    {
        // NO apagues el Canvas entero, porque ahí adentro está el hint
        menuPrincipal.SetActive(false);
        panelParedes.SetActive(false);
        panelPisos.SetActive(false);
        panelVentanas.SetActive(false);
        panelPolarizado.SetActive(false);
        // canvasCambios.SetActive(false); <-- BORRA O COMENTA ESTA LINEA

        if (abrirTeleScript != null)
            abrirTeleScript.Descongelar();
        else
            FindObjectOfType<Bienvenida_TV>().CerrarMenu();
    }
}