using UnityEngine;

public class Polarizado : MonoBehaviour
{
    [Header("Ventanas")]
    public Renderer[] ventanas;
    public Material vidrioNormal;
    public Material vidrioPolarizado;

    [Header("Paneles")]
    public GameObject panelMenuPrincipal;
    public GameObject panelPolarizado;

    void CambiarSoloVidrio(Material nuevo)
    {
        foreach (var r in ventanas)
        {
            var mats = r.materials; // copia de los 2 materiales
            for (int i = 0; i < mats.Length; i++)
            {
                // Solo cambia si el nombre contiene "vidrio"
                if (mats[i].name.ToLower().Contains("vidrio"))
                {
                    mats[i] = nuevo;
                }
            }
            r.materials = mats;
        }
    }

    public void BotonActivar() => CambiarSoloVidrio(vidrioPolarizado);
    public void BotonDesactivar() => CambiarSoloVidrio(vidrioNormal);

    public void BotonConfirmar()
    {
        panelPolarizado.SetActive(false);
        panelMenuPrincipal.SetActive(true);
    }

    public void BotonVolver() => BotonConfirmar();
}