using UnityEngine;

public class SelectorDeVentanas : MonoBehaviour
{
    [Header("Arrastrá acá la casa")]
    public Transform casaModulos;

    [Header("Arrastrá acá los materiales de ventana")]
    public Material[] materialesVentanas;

    public void CambiarVentana(int indice)
    {// Recibe un número (índice) para saber cuál material de la lista usar.
        if (casaModulos == null)
        {
            Debug.LogError("FALTA casaModulos en SelectorDeVentanas");
            return;
        }
        if (indice < 0 || indice >= materialesVentanas.Length) return;
        if (materialesVentanas[indice] == null) return;
        // Si pides un número que no existe en la lista o el material está vacío, no hace nada
        MeshRenderer[] todos = casaModulos.GetComponentsInChildren<MeshRenderer>(true);
        int cambiados = 0;
        // Busca todos los "MeshRenderer" (las piezas visibles 3D) dentro de la casa, incluso si están desactivados.
        foreach (MeshRenderer m in todos)
        {//REVISAR PIEZA POR PIEZA:
            string nombre = m.gameObject.name.ToLower();
            if (nombre.Contains("ventana") || nombre.Contains("vidrio") || nombre.Contains("marco") || nombre.Contains("window") || nombre.Contains("cristal"))
            {
                m.material = materialesVentanas[indice];
                cambiados++;
            }
            //cambiar el material por el que elegi y sumar 1 al contador
        }
        Debug.Log("Ventanas cambiadas: " + cambiados);
    }//indicando cuántas ventanas/marcos se actualizaron con éxito.

    // conectar a los botones de la interfaz 
    public void SeleccionarMaterialVentana(int indice)
    {
        CambiarVentana(indice);
    }
}