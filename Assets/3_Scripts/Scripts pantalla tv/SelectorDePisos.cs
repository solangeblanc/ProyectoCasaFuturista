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
        if (casaModulos == null)
        {
            Debug.LogError("Casa Modulos sin asignar en SelectorDePiso");
            return;
        }
        if (indice < 0 || indice >= materialesPiso.Length) return;

        MeshRenderer[] todosLosMesh = casaModulos.GetComponentsInChildren<MeshRenderer>(true);

        foreach (MeshRenderer m in todosLosMesh)
        {
            string nombre = m.gameObject.name.ToLower();
            if (nombre.Contains("piso") || nombre.Contains("suelo"))
            {
                m.material = materialesPiso[indice];
            }
        }
        Debug.Log("Piso cambiado a: " + materialesPiso[indice].name);
    }

    // Para que te ande con tu nombre viejo también
    public void SeleccionarMaterialPiso(int indice)
    {
        CambiarPiso(indice);
    }
}