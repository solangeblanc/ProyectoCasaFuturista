using UnityEngine;
using UnityEngine.AI;

public class AspiradoraRandom : MonoBehaviour
{
    private NavMeshAgent agent;
    public float rangoAleatorio = 15f;
    public float distanciaParaCambiar = 0.8f;

    [Header("Canvas")]
    public GameObject canvas; // el que dice "Presiona espacio para prender/apagar"

    private bool jugadorCerca = false;
    private bool estaPrendida = false;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.enabled = false;
        canvas.SetActive(false);
    }

    void Update()
    {
        // Solo podes prender/apagar si estas cerca
        if (jugadorCerca && Input.GetKeyDown(KeyCode.Space))
        {
            estaPrendida = !estaPrendida;
            agent.enabled = estaPrendida;

            if (estaPrendida) IrAPuntoRandom();
            else agent.ResetPath();
        }

        if (!estaPrendida) return;

        // Recorrido aleatorio
        if (!agent.pathPending && agent.remainingDistance <= distanciaParaCambiar)
        {
            IrAPuntoRandom();
        }
    }

    void IrAPuntoRandom()
    {
        agent.SetDestination(GetRandomNavMeshPoint(transform.position, rangoAleatorio));
    }

    Vector3 GetRandomNavMeshPoint(Vector3 centro, float rango)
    {
        for (int i = 0; i < 30; i++)
        {
            Vector3 randomPos = centro + Random.insideUnitSphere * rango;
            NavMeshHit hit;
            if (NavMesh.SamplePosition(randomPos, out hit, 2f, NavMesh.AllAreas))
                return hit.position;
        }
        return centro;
    }

    // Detecta cuando te acercas
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            jugadorCerca = true;
            canvas.SetActive(true);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            jugadorCerca = false;
            canvas.SetActive(false);
        }
    }
}