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

    [Header("Cartel 'ESPACIO para regar'")]
    [SerializeField] private GameObject canvasCartel;

    [Header("Canvas informativo de especies")]
    [SerializeField] private GameObject canvasInfo;

    [Header("Diagnóstico")]
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

        Mostrar(canvasCartel, false);
        Mostrar(canvasInfo, false);
    }

    private void Update()
    {
        GameManager gm = GameManager.Instance;

        if (gm == null || !gm.EstaJugando || camara == null)
        {
            Mostrar(canvasCartel, false);
            Mostrar(canvasInfo, false);
            return;
        }

        bool mirando = MirandoAlBoton();

        // Antes de regar: cartel "ESPACIO". Regando: canvas con las especies.
        Mostrar(canvasCartel, mirando && !regando);
        Mostrar(canvasInfo, mirando && regando);

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
            if (mostrarLogs) Debug.Log("[Riego] El rayo toca: " + hit.collider.name);

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

    private void Mostrar(GameObject objeto, bool mostrar)
    {
        if (objeto == null) return;
        if (objeto.activeSelf == mostrar) return;
        objeto.SetActive(mostrar);
    }
}
