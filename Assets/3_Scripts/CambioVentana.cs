using UnityEngine;
using UnityEngine.InputSystem;

public class CambioVentanas : MonoBehaviour
{
    [Header("Tecla")]
    [SerializeField] private Key teclaCambio = Key.Space;

    [Header("Lista de materiales/tonos para el vidrio")]
    public Material[] materialesVidrio;
    private int indiceMaterial = 0;

    [Header("Rayo")]
    [Tooltip("Cámara del jugador. Si queda vacío usa la cámara con tag MainCamera.")]
    public Transform _camara;
    [SerializeField] private float distanciaMaxima = 4f;
    [SerializeField] private LayerMask capas = ~0;

    private void Start()
    {
        if (_camara == null && Camera.main != null)
            _camara = Camera.main.transform;
    }

    private void Update()
    {
        if (GameManager.Instance == null || !GameManager.Instance.EstaJugando || _camara == null)
            return;

        if (Keyboard.current != null && Keyboard.current[teclaCambio].wasPressedThisFrame)
            IntentarCambiarVidrio();
    }

    private void IntentarCambiarVidrio()
    {
        Debug.Log("Tecla apretada, lanzando rayo...");

        if (materialesVidrio == null || materialesVidrio.Length == 0)
        {
            Debug.Log("No hay materiales en la lista");
            return;
        }

        RaycastHit[] hits = Physics.RaycastAll(_camara.position, _camara.forward,
                                               distanciaMaxima, capas,
                                               QueryTriggerInteraction.Ignore);
        System.Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));

        Debug.Log("Colliders en el camino: " + hits.Length);

        foreach (RaycastHit hit in hits)
        {
            Debug.Log("Rayo pasó por: " + hit.collider.name + " | tag: " + hit.collider.tag);

            if (!hit.collider.CompareTag("Ventana")) continue;

            Renderer ventanaRenderer = hit.collider.GetComponentInChildren<Renderer>();
            if (ventanaRenderer == null)
            {
                Debug.Log("La ventana no tiene Renderer");
                continue;
            }

            Material[] mats = ventanaRenderer.materials;
            Debug.Log("Materiales de la ventana: " + mats.Length);

            if (mats.Length > 1)
            {
                mats[1] = materialesVidrio[indiceMaterial];
                ventanaRenderer.materials = mats;
                indiceMaterial = (indiceMaterial + 1) % materialesVidrio.Length;
            }
            return;
        }
    }
}