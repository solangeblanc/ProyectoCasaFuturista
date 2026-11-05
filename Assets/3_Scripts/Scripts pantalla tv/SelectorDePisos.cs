using UnityEngine;

public class SelectorDePiso : MonoBehaviour
{
    [Header("Arrastrá acá la casa")]
    public Transform casaModulos;

    [Header("Arrastrá acá los materiales de piso")]
    public Material[] materialesPiso;

    // Este es el que van a llamar los botones
    public void CambiarPiso(int indice)
    {
        var renderers = casaModulos.GetComponentsInChildren<MeshRenderer>(true);
        foreach (var r in renderers)
        {
            // Lo dejamos con 1 solo material, no 5
            r.sharedMaterials = new Material[] { materialesPiso[indice] };
        }
    }

    // Para que te ande con tu nombre viejo también
    public void SeleccionarMaterialPiso(int indice)
    {
        CambiarPiso(indice);
    }
}