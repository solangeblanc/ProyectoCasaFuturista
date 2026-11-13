using UnityEngine;

public class Menu_Tele : MonoBehaviour
{//objetos para arrastrar al inspector
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
    {//al presionar volver se apaga todo y vuelve al menu principal
        panelParedes.SetActive(false);
        panelPisos.SetActive(false);
        panelVentanas.SetActive(false);
        panelPolarizado.SetActive(false);
        menuPrincipal.SetActive(true);
    }

    public void SalirTele()
    {
        // todo termina apagado despues de tocar salir 
        menuPrincipal.SetActive(false);
        panelParedes.SetActive(false);
        panelPisos.SetActive(false);
        panelVentanas.SetActive(false);
        panelPolarizado.SetActive(false);

        if (abrirTeleScript != null)
            abrirTeleScript.Descongelar();//llama a la funcion descongelar de tv script 
        else
            FindAnyObjectByType<Bienvenida_TV>().CerrarMenu();
        //si no, busca automáticamente el script nuevo (Bienvenida_TV) para cerrar el menú y devolver el control.
    }
}