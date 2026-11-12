using UnityEngine;

public class selectorDePared : MonoBehaviour
{
    [Header("Paredes")]
    public Transform casaModulos; // donde se arrastra lo que se va a modificar
    public Material[] materialesPared; //lista de materiales para sumar

    public void CambiarPared(int indice)
    {
        if (casaModulos == null || materialesPared.Length == 0) return;
        // Si olvidaste asignar la casa o no pusiste materiales en la lista, no hace nada
        if (indice < 0 || indice >= materialesPared.Length) return;
        // Si pedis un número que no existe en la lista (por ejemplo, el 10 cuando solo tienes 3), se detiene.
        // Cambia a todas las paredes hijas
        Renderer[] renderers = casaModulos.GetComponentsInChildren<Renderer>();
        // Busca todos los componentes "Renderer" (lo que hace que se vea un objeto) en la casa y sus hijos.
        foreach (var r in renderers)
        {
            // A cada pedazo encontrado le asigna el nuevo material elegido según el número (índice).
            r.material = materialesPared[indice];
        }
    }
}