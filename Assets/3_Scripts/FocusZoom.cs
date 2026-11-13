using UnityEngine;

public class FocusZoom : MonoBehaviour
{
    [Header("Configuración de Zoom")]
    [Tooltip("Campo de visión normal (se detecta automáticamente al iniciar)")]
    public float normalFOV = 60f;
    [Tooltip("Campo de visión al hacer zoom (más bajo = más cerca)")]
    public float zoomedFOV = 40f;
    [Tooltip("Velocidad con la que entra y sale el zoom")]
    public float zoomSpeed = 8.0f;

    private Camera cam;

    void Start()
    {
        // Obtenemos el componente Camera de este objeto
        cam = GetComponent<Camera>();

        if (cam != null)
        {
            // Guardamos el FOV que ya tiene la cámara por defecto como el normal
            normalFOV = cam.fieldOfView;
        }
        else
        {
            Debug.LogError("No se encontró el componente Camera en este objeto.");
        }
    }

    void Update()
    {
        if (cam == null) return;

        // Definimos cuál es el FOV al que queremos llegar
        float targetFOV = normalFOV;

        // Si mantenemos presionado el Clic Derecho del mouse (índice 1)
        if (Input.GetMouseButton(1))
        {
            targetFOV = zoomedFOV;
        }

        // Interpolar suavemente el campo de visión actual hacia el objetivo
        cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, targetFOV, zoomSpeed * Time.deltaTime);
    }
}