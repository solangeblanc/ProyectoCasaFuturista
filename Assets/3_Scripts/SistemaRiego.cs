using UnityEngine;
using UnityEngine.InputSystem;

public class SistemaRiego : MonoBehaviour
{
    [Header("Tecla")]
    [SerializeField] private Key teclaRiego = Key.Space;

    [Header("Rayo")]
    [Tooltip("Cámara del jugador. Si queda vacío usa la cámara con tag MainCamera.")]
    [SerializeField] private Transform camara;
    [SerializeField] private float distanciaMaxima = 3f;
    [SerializeField] private LayerMask capas = ~0;

    [Header("Agua")]
    [Tooltip("Arrastrá acá el Particle System del regador")]
    [SerializeField] private ParticleSystem agua;

    [Header("Cartel (opcional)")]
    [Tooltip("Canvas vivero: se muestra solo mientras el jugador mira el botón")]
    [SerializeField] private GameObject canvasCartel;

    private bool regando = false;

    private void Start()
    {
        if (camara == null && Camera.main != null)
            camara = Camera.main.transform;

        if (agua != null)
            agua.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);

        MostrarCartel(false);
    }

    private void Update()
    {
        if (GameManager.Instance == null || !GameManager.Instance.EstaJugando || camara == null)
        {
            MostrarCartel(false);
            return;
        }

        bool mirandoBoton = MirandoAlBoton();
        MostrarCartel(mirandoBoton);

        if (mirandoBoton && Keyboard.current != null && Keyboard.current[teclaRiego].wasPressedThisFrame)
        {
            AlternarRiego();
        }
    }

    private bool MirandoAlBoton()
    {
        RaycastHit[] hits = Physics.RaycastAll(camara.position, camara.forward,
                                               distanciaMaxima, capas,
                                               QueryTriggerInteraction.Ignore);

        foreach (RaycastHit hit in hits)
        {
            Debug.Log("Rayo pasó por: " + hit.collider.name);
            if (hit.collider.transform.IsChildOf(transform))
                return true;
        }
        return false;
    }

    public void AlternarRiego()
    {
        if (agua == null) return;

        regando = !regando;

        if (regando) agua.Play();
        else agua.Stop();
    }

    private void MostrarCartel(bool mostrar)
    {
        if (canvasCartel == null) return;
        if (canvasCartel.activeSelf == mostrar) return;

        canvasCartel.SetActive(mostrar);
    }
}
