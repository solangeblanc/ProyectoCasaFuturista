using System.Collections;
using UnityEngine;

// Va en el objeto que se muestra (el "Panel" de la terminal eléctrica).
// Cuando ese objeto se activa, sea cual sea el script o evento que lo active, marca la función en la lista.
// No cambia nada de cómo se muestra u oculta el panel.
public class MarcarDescubrimientoAlMostrarse : MonoBehaviour
{
    [Tooltip("Tiene que ser igual al id del registro: \"paneles\" = Paneles solares")]
    public string idDescubrimiento = "paneles";

    private void OnEnable()
    {
        StartCoroutine(MarcarSiSigueVisible());
    }

    private IEnumerator MarcarSiSigueVisible()
    {
        // Espera un frame: si otro script apaga este objeto al empezar el juego, no se marca.
        yield return null;

        if (gameObject.activeInHierarchy)
            RegistroDescubrimientos.Descubrir(idDescubrimiento);
    }
}
