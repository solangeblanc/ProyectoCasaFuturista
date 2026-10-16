using UnityEngine;

public class BienvenidaTV : MonoBehaviour
{
    public GameObject textoEspacio;
    public GameObject menu;
    public GameObject jugadorReal;
    private bool enRango = false;

    void OnTriggerEnter(Collider c)
    {
        if (!c.CompareTag("Player")) return;
        enRango = true;
        if (menu != null && !menu.activeSelf && textoEspacio != null)
            textoEspacio.SetActive(true);
    }

    void OnTriggerExit(Collider c)
    {
        if (!c.CompareTag("Player")) return;
        enRango = false;
        if (textoEspacio != null) textoEspacio.SetActive(false);
        if (menu != null) menu.SetActive(false);
        LiberarJugador();
    }

    void Update()
    {
        if (enRango && menu != null && !menu.activeSelf && Input.GetKeyDown(KeyCode.Space))
        {
            if (textoEspacio != null) textoEspacio.SetActive(false);
            menu.SetActive(true);
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    public void CerrarMenu()
    {
        if (menu != null) menu.SetActive(false);
        if (enRango && textoEspacio != null) textoEspacio.SetActive(true);
        LiberarJugador();
    }

    void LiberarJugador()
    {
        if (jugadorReal == null) return;

        var componentes = jugadorReal.GetComponents<Behaviour>();
        foreach (var comp in componentes)
        {
            if (comp == null) continue; // salta el script roto
            comp.enabled = true;
        }
        var cc = jugadorReal.GetComponent<CharacterController>();
        if (cc != null) cc.enabled = true;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}