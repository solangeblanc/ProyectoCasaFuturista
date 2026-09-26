using UnityEngine;
using UnityEngine.InputSystem;

public class ControlLuces : MonoBehaviour
{
    [Header("Cámara del Jugador")]
    public Transform _camara;

    public void OnIntercambiarLuz(InputValue value)
    {
        if (value.isPressed)
        {
            Ray ray = new Ray(_camara.position, _camara.forward);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                // Verificamos si le apuntamos a la lámpara
                if (hit.collider.CompareTag("Luz"))
                {
                    // Buscamos el componente Light dentro de los objetos hijos de esta lámpara
                    Light luzComponente = hit.collider.GetComponentInChildren<Light>();

                    if (luzComponente != null)
                    {
                        // Alternamos el estado de la luz
                        luzComponente.enabled = !luzComponente.enabled;
                    }
                }
            }
        }
    }
}
