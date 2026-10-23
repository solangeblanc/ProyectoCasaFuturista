using UnityEngine;

public class selectorDePared : MonoBehaviour
{
    [Header("Solo necesitas esto")]
    public Transform casaModulos; // arrastra aca tu casa / paredes
    public Material[] materialesPared; // 0=piedra, 1=gris, 2=piedra roja, etc

    public void CambiarPared(int indice)
    {
        if (casaModulos == null || materialesPared.Length == 0) return;
        if (indice < 0 || indice >= materialesPared.Length) return;

        // Cambia a todas las paredes hijas
        Renderer[] renderers = casaModulos.GetComponentsInChildren<Renderer>();
        foreach (var r in renderers)
        {
            // solo cambia si es pared (opcional, si no borra el if)
            r.material = materialesPared[indice];
        }
    }
}