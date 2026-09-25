using UnityEngine;
using UnityEngine.InputSystem;

public class CambioVentanas : MonoBehaviour
{
    [Header("Lista de materiales/tonos para el vidrio")]
    public Material[] materialesVidrio;
    private int indiceMaterial = 0;

    [Header("Cámara del Jugador")]
    public Transform _camara;

    public void OnCambiarVentana(InputValue value)
    {
        if (value.isPressed && materialesVidrio.Length > 0)
        {
            Ray ray = new Ray(_camara.position, _camara.forward);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.CompareTag("Ventana"))
                {
                    Renderer ventanaRenderer = hit.collider.GetComponent<Renderer>();
                    if (ventanaRenderer != null)
                    {
                        // Obtenemos el array completo de materiales que tiene el objeto
                        Material[] mats = ventanaRenderer.materials;

                        // Verificamos que al menos tenga 2 materiales (para asegurarnos de modificar el vidrio en el Element 1)
                        if (mats.Length > 1)
                        {
                            // Cambiamos únicamente el Element 1 (el vidrio)
                            mats[1] = materialesVidrio[indiceMaterial];

                            // Reasignamos el array modificado al renderer
                            ventanaRenderer.materials = mats;

                            // Pasamos al siguiente tono de la lista
                            indiceMaterial = (indiceMaterial + 1) % materialesVidrio.Length;
                        }
                    }
                }
            }
        }
    }
}