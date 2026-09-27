using UnityEngine;
using UnityEngine.InputSystem;

public class ControlPiso : MonoBehaviour
{
    [Header("Cámara del Jugador")]
    public Transform _camara;

    [Header("Lista de Materiales para el Piso")]
    public Material[] materialesPiso; // Aquí tienes tus 10 materiales puestos en el Inspector
    private int indiceMaterialActual = 0;

    // Se ejecuta al presionar la tecla 'C' configurada en el Input System
    public void OnControlPiso(InputValue value)
    {
        if (value.isPressed)
        {
            if (materialesPiso == null || materialesPiso.Length == 0 || materialesPiso[0] == null)
            {
                Debug.LogWarning("¡Faltan materiales asignados en la lista de ControlPiso del Inspector!");
                return;
            }

            if (_camara == null)
            {
                Debug.LogWarning("¡Falta asignar la Cámara en el script de ControlPiso!");
                return;
            }

            Ray ray = new Ray(_camara.position, _camara.forward);
            RaycastHit hit;

            // Lanzamos el rayo
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

                    // Pasamos al siguiente material de forma cíclica entre tus 10 opciones
                    indiceMaterialActual = (indiceMaterialActual + 1) % materialesPiso.Length;
                }
            }
        }
    }
}