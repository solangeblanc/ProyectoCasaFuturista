using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerView : MonoBehaviour
{
    private CharacterController _CHC;
    [SerializeField] private Transform _camara;

    private float rotacionX;

    private void Awake()
    {
        _CHC = GetComponent<CharacterController>();
        if (_camara == null)
        {
            // Intenta buscar la cámara en los hijos si no está asignada
            Camera cam = GetComponentInChildren<Camera>();
            if (cam != null) _camara = cam.transform;
        }
    }

    public void Mover(Vector3 movimientoFinal)
    {
        if (_CHC != null)
        {
            _CHC.Move(movimientoFinal);
        }
    }

    public bool EstaEnSuelo()
    {
        return _CHC != null && _CHC.isGrounded;
    }

    public void MoverCamara(float mouseX, float mouseY, float sensibilidad, float clampCam)
    {
        if (_camara == null) return;

        // 1. Rotación horizontal (Gira el jugador entero sobre Y)
        transform.Rotate(Vector3.up * (mouseX * sensibilidad));

        // 2. Rotación vertical (Gira SOLO la cámara sobre X)
        rotacionX -= mouseY * sensibilidad;
        rotacionX = Mathf.Clamp(rotacionX, -clampCam, clampCam);

        _camara.localRotation = Quaternion.Euler(rotacionX, 0, 0);
    }
}