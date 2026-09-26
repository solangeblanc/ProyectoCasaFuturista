using UnityEngine;
using UnityEngine.InputSystem;

public class ControlParedesPorMaterial : MonoBehaviour
{
    [Header("Cámara del Jugador")]
    public Transform _camara;

    [Header("Lista de Materiales para la Pared")]
    public Material[] materialesPared;
    private int indiceMaterialActual = 0;

    public void OnCambiarColorPared(InputValue value)
    {
        if (value.isPressed)
        {
            if (materialesPared == null || materialesPared.Length == 0)
            {
                Debug.LogWarning("¡No hay materiales asignados en la lista!");
                return;
            }

            Ray ray = new Ray(_camara.position, _camara.forward);
            RaycastHit hit;

            // Lanzamos el rayo para detectar si le apuntamos a una pared
            if (Physics.Raycast(ray, out hit))
            {
                // Verificamos si lo que tocamos tiene el Tag "Paredes"
                if (hit.collider.CompareTag("Paredes"))
                {
                    // Buscamos TODOS los GameObjects en la escena que tengan el Tag "Paredes"
                    GameObject[] todasLasParedes = GameObject.FindGameObjectsWithTag("Paredes");

                    foreach (GameObject pared in todasLasParedes)
                    {
                        Renderer rendererPared = pared.GetComponent<Renderer>();
                        if (rendererPared != null)
                        {
                            // Le aplicamos el material actual a cada una de las paredes encontradas
                            rendererPared.material = materialesPared[indiceMaterialActual];
                        }
                    }

                    // Avanzamos al siguiente material para el próximo clic (con loop)
                    indiceMaterialActual = (indiceMaterialActual + 1) % materialesPared.Length;
                }
            }
        }
    }
}