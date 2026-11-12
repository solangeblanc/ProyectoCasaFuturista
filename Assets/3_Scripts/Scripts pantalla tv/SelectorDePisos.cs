using UnityEngine;

public class SelectorDePiso : MonoBehaviour
{
    [Header("Arrastrá acá la casa")]
    public Transform casaModulos;

    [Header("Arrastrá acá los materiales de piso")]
    public Material[] materialesPiso;

    public void CambiarPiso(int indice)
    //Recibe un número (índice) para saber qué piso de la lista elegir.
    {// Busca todos los "MeshRenderer" (los componentes que dibujan las formas 3D) dentro de la casa y sus hijos, 
     // incluyendo incluso a los que están desactivados (true).
        var renderers = casaModulos.GetComponentsInChildren<MeshRenderer>(true);
        foreach (var r in renderers)
        {
            // Reemplaza los materiales de ese objeto dejándolo con un único material limpio (el que elegiste en la lista), 
            // evitando que se mezclen por error con otros materiales anteriores.
            r.sharedMaterials = new Material[] { materialesPiso[indice] };
        }
    }

    public void SeleccionarMaterialPiso(int indice)
    {// Esta función se puede conectar directamente a los botones de tu interfaz. 
     // Al hacer clic, simplemente llama a la función de arriba pasándole el número de piso que deseo.
        CambiarPiso(indice);
    }
}