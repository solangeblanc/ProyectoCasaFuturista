using UnityEngine;
using UnityEngine.InputSystem;

public class CambioColorPiso : MonoBehaviour
{
    [SerializeField] private Camera camara;
    [SerializeField] private Material[] materialesPiso;
    private int indiceMaterial = 0;

    public void OnCambiarPiso(InputValue value)
   {
        if (value.isPressed)
        {
            Ray ray = new Ray(camara.transform.position, camara.transform.forward);
            RaycastHit hit;

           if (Physics.Raycast(ray, out hit))
            {
               Renderer pisoRenderer = hit.collider.GetComponent<Renderer>();
               if (pisoRenderer != null)
                {
                  pisoRenderer.material = materialesPiso[indiceMaterial];
                    indiceMaterial = (indiceMaterial + 1) % materialesPiso.Length;
               }
           }
       }
   }
}



