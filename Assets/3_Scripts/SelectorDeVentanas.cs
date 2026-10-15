using UnityEngine;

public class SelectorDeVentanas : MonoBehaviour
{
    [Header("Arrastrá acá la casa")]
    public Transform casaModulos;

    [Header("Arrastrá acá los materiales de ventana")]
    public Material[] materialesVentanas;

    public void CambiarVentana(int indice)
    {
        if (casaModulos == null)
        {
            Debug.LogError("FALTA casaModulos en SelectorDeVentanas");
            return;
        }
        if (indice < 0 || indice >= materialesVentanas.Length) return;
        if (materialesVentanas[indice] == null) return;

        MeshRenderer[] todos = casaModulos.GetComponentsInChildren<MeshRenderer>(true);
        int cambiados = 0;

        foreach (MeshRenderer m in todos)
        {
            string nombre = m.gameObject.name.ToLower();
            if (nombre.Contains("ventana") || nombre.Contains("vidrio") || nombre.Contains("marco") || nombre.Contains("window") || nombre.Contains("cristal"))
            {
                m.material = materialesVentanas[indice];
                cambiados++;
            }
        }
        Debug.Log("Ventanas cambiadas: " + cambiados);
    }

    // Para que te sigan andando tus botones viejos
    public void SeleccionarMaterialVentana(int indice)
    {
        CambiarVentana(indice);
    }
}