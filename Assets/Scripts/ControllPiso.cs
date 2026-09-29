using UnityEngine;
using UnityEngine.InputSystem;

public class ControlPiso : MonoBehaviour
{
    [Header("Cámara del Jugador")]
    public Transform _camara;

    [Header("Lista de Materiales para el Piso")]
    public Material[] materialesPiso; // Poner en lista los materiales de piso intercambiables
    private int indiceMaterialActual = 0;

    // Se ejecuta al presionar la tecla 'C' configurada en el Input System--configurada en keyboard C---
    public void OnControlPiso(InputValue value)
    {
        if (value.isPressed)
        {
            if (materialesPiso == null || materialesPiso.Length == 0 || materialesPiso[0] == null)
            {
                Debug.LogWarning("¡Faltan materiales asignados en la lista de ControlPiso del Inspector!");//mensaje si faltan los materiales en la lista
                return;
            }

            if (_camara == null)
            {
                Debug.LogWarning("¡Falta asignar la Cámara en el script de ControlPiso!");//mensaje si falta asignar la camara en el script no va a funcionar
                return;
            }

            Ray ray = new Ray(_camara.position, _camara.forward);
            RaycastHit hit;

            // Lanza el rayo cuando mira al piso y apretamos C----condicional----
            if (Physics.Raycast(ray, out hit))
            {
                // Obtenemos el Renderer del objeto al que le apuntamos
                Renderer rendererPiso = hit.collider.GetComponent<Renderer>();

                if (rendererPiso != null)
                {
                    // Asignamos el material actual de tu lista
                    rendererPiso.material = materialesPiso[indiceMaterialActual];

                    // Forzamos la escala (Tiling) para que la textura no se rompa
                    rendererPiso.material.SetTextureScale("_BaseMap", new Vector2(5f, 5f));
                    rendererPiso.material.SetTextureScale("_MainTex", new Vector2(5f, 5f));

                    // Pasamos al siguiente material. Ponemos entre 8 y 10 materiales.
                    indiceMaterialActual = (indiceMaterialActual + 1) % materialesPiso.Length;
                }
            }
        }
    }
}