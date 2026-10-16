using UnityEngine;
using UnityEngine.InputSystem;

public class CambioVentanas : MonoBehaviour
{
    [Header("Tecla")]
    [SerializeField] private Key teclaCambio = Key.Space;

    [Header("Lista de materiales/tonos para el vidrio")]
    public Material[] materialesVidrio;
    private int indiceMaterial = 0;

    [Header("Ventanas")]
    [Tooltip("Posición del material del vidrio dentro del Renderer (Element 1)")]
    [SerializeField] private int indiceVidrio = 1;

    [Header("Rayo")]
    [Tooltip("Cámara del jugador. Si queda vacío usa la cámara con tag MainCamera.")]
    public Transform _camara;
    [SerializeField] private float distanciaMaxima = 4f;
    [SerializeField] private LayerMask capas = ~0;

    private GameObject[] ventanas;

    private void Start()
    {
        if (_camara == null && Camera.main != null)
            _camara = Camera.main.transform;

        ventanas = GameObject.FindGameObjectsWithTag("Ventana");
    }

    private void Update()
    {
        if (GameManager.Instance == null || !GameManager.Instance.EstaJugando || _camara == null)
            return;

        if (Keyboard.current != null && Keyboard.current[teclaCambio].wasPressedThisFrame)
        {
            if (MirandoUnaVentana())
                CambiarTodasLasVentanas();
        }
    }

    private bool MirandoUnaVentana()
    {
        RaycastHit[] hits = Physics.RaycastAll(_camara.position, _camara.forward,
                                               distanciaMaxima, capas,
                                               QueryTriggerInteraction.Ignore);

        foreach (RaycastHit hit in hits)
        {
            if (hit.collider.CompareTag("Ventana"))
                return true;
        }
        return false;
    }

    private void CambiarTodasLasVentanas()
    {
        if (materialesVidrio == null || materialesVidrio.Length == 0) return;

        Material nuevo = materialesVidrio[indiceMaterial];

        foreach (GameObject ventana in ventanas)
        {
            if (ventana == null) continue;

            Renderer[] renderers = ventana.GetComponentsInChildren<Renderer>();

            foreach (Renderer r in renderers)
            {
                Material[] mats = r.materials;

                if (mats.Length > indiceVidrio)
                {
                    mats[indiceVidrio] = nuevo;
                    r.materials = mats;
                }
            }
        }

        indiceMaterial = (indiceMaterial + 1) % materialesVidrio.Length;
    }
}