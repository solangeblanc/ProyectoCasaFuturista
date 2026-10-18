using UnityEngine;
using UnityEngine.InputSystem;

public class SistemaRiego : MonoBehaviour
{
    [Header("Tecla")]
    [SerializeField] private Key teclaRiego = Key.Space;

    [Header("Rayo")]
    [SerializeField] private Transform camara;
    [SerializeField] private float distanciaMaxima = 4f;
    [SerializeField] private LayerMask capas = ~0;

    [Header("Agua")]
    [SerializeField] private ParticleSystem agua;

    [Header("Cartel")]
    [SerializeField] private GameObject canvasCartel;

    [Header("Diagnóstico (dejar apagado cuando funcione)")]
    [SerializeField] private bool mostrarLogs = false;

    private bool regando = false;

    private void Start()
    {
        if (camara == null)
        {
            if (Camera.main != null) camara = Camera.main.transform;
        }

        if (agua != null)
            agua.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);

        MostrarCartel(false);
    }

    private void Update()
    {
        GameManager gm = GameManager.Instance;

        if (gm == null || !gm.EstaJugando || camara == null)
        {
            MostrarCartel(false);
            return;
        }

        bool mirando = MirandoAlBoton();
        MostrarCartel(mirando);

        if (!mirando) return;
        if (Keyboard.current == null) return;

        if (Keyboard.current[teclaRiego].wasPressedThisFrame)
            AlternarRiego();
    }

    private bool MirandoAlBoton()
    {
        RaycastHit[] hits = Physics.RaycastAll(camara.position, camara.forward,
                                               distanciaMaxima, capas,
                                               QueryTriggerInteraction.Ignore);

        foreach (RaycastHit hit in hits)
        {
            if (mostrarLogs) Debug.Log("El rayo pasó por: " + hit.collider.name);

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

        if (mostrar)
        {
            // Gira el cartel para que mire al jugador (sin inclinarse)
            Vector3 dir = canvasCartel.transform.position - camara.position;
            dir.y = 0f;
            if (dir.sqrMagnitude > 0.001f)
                canvasCartel.transform.rotation = Quaternion.LookRotation(dir);
        }

        if (canvasCartel.activeSelf != mostrar)
            canvasCartel.SetActive(mostrar);
    }
}
