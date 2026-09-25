using UnityEngine;
using UnityEngine.InputSystem;

public class CambioParedes : MonoBehaviour
{
    [Header("Referencia a la carpeta 'casa modulos'")]
    public Transform casaModulos;

    [Header("Lista de materiales para las paredes")]
    public Material[] materialesParedes;
    private int indiceMaterialPared = 0;

    [Header("Cámara del Jugador")]
    public Transform _camara;

    public void OnCambiarPared(InputValue value)
    {
        if (value.isPressed && materialesParedes.Length > 0 && casaModulos != null)
        {
            Ray ray = new Ray(_camara.position, _camara.forward);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                // Verificamos si el objeto que estamos apuntando está dentro de "casa modulos"
                if (hit.collider.transform.IsChildOf(casaModulos))
                {
                    // Obtenemos todos los Renderer de las paredes que están dentro de la carpeta
                    Renderer[] renderersParedes = casaModulos.GetComponentsInChildren<Renderer>();

                    // Cambiamos el material a todas las paredes de la lista
                    foreach (Renderer rend in renderersParedes)
                    {
                        rend.material = materialesParedes[indiceMaterialPared];
                    }

                    // Avanzamos al siguiente material de la lista (vuelve al inicio si llega al final)
                    indiceMaterialPared = (indiceMaterialPared + 1) % materialesParedes.Length;
                }
            }
        }
    }
}
