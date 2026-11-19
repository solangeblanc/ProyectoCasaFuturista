using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class RanaSaltarina : MonoBehaviour
{
    [Header("Zona donde puede saltar")]
    [SerializeField] private Transform agua;
    [SerializeField] private float margenBorde = 0.3f;

    [Header("Saltos")]
    [SerializeField] private float distanciaSalto = 0.8f;
    [SerializeField] private float duracionSalto = 0.5f;
    [SerializeField] private AnimationClip clipSalto;
    [SerializeField] private float pausaMin = 0.1f;
    [SerializeField] private float pausaMax = 0.5f;

    [Header("Animación (nombres de los estados en el Animator)")]
    [SerializeField] private string estadoSalto = "run";
    [SerializeField] private string estadoQuieta = "idle";

    private NavMeshAgent agente;
    private Animator animator;
    private Bounds limites;
    private bool hayLimites;
    private NavMeshQueryFilter filtro;

    private void Start()
    {
        agente = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();

        if (clipSalto != null) duracionSalto = clipSalto.length;
        duracionSalto = Mathf.Max(0.1f, duracionSalto);

        agente.autoBraking = false;
        agente.acceleration = 200f;
        agente.angularSpeed = 540f;
        agente.speed = distanciaSalto / duracionSalto;

        filtro = new NavMeshQueryFilter { agentTypeID = agente.agentTypeID, areaMask = NavMesh.AllAreas };

        if (agua != null)
        {
            Renderer r = agua.GetComponentInChildren<Renderer>();
            Collider c = agua.GetComponentInChildren<Collider>();
            if (r != null) { limites = r.bounds; hayLimites = true; }
            else if (c != null) { limites = c.bounds; hayLimites = true; }
        }
        if (!hayLimites)
            Debug.LogWarning("RanaSaltarina: falta arrastrar el agua en 'Agua' (tiene que tener Renderer o Collider).", this);

        if (!agente.isOnNavMesh)
        {
            Debug.LogWarning("RanaSaltarina: la rana no está sobre el NavMesh. Ponela sobre el agua y revisá que el " +
                             "NavMeshSurface del agua esté bakeado con el tipo de agente de la rana.", this);
            enabled = false;
            return;
        }

        StartCoroutine(Saltar());
    }

    private IEnumerator Saltar()
    {
        while (true)
        {
            agente.SetDestination(PuntoAleatorio());
            yield return null;

            while (!HaLlegado())
            {
                agente.isStopped = false;
                agente.speed = distanciaSalto / duracionSalto;
                Animar(true);
                yield return new WaitForSeconds(duracionSalto);

                agente.isStopped = true;
                agente.velocity = Vector3.zero;
                Animar(false);
                yield return new WaitForSeconds(Random.Range(pausaMin, pausaMax));
            }
        }
    }

    private bool HaLlegado()
    {
        if (agente.pathPending) return false;
        if (!agente.hasPath) return true;
        return agente.remainingDistance <= agente.stoppingDistance + 0.1f;
    }

    private Vector3 PuntoAleatorio()
    {
        if (hayLimites)
        {
            for (int i = 0; i < 10; i++)
            {
                Vector3 p = new Vector3(
                    Random.Range(limites.min.x + margenBorde, limites.max.x - margenBorde),
                    limites.max.y,
                    Random.Range(limites.min.z + margenBorde, limites.max.z - margenBorde));

                if (NavMesh.SamplePosition(p, out NavMeshHit hit, 1.5f, filtro))
                    return hit.position;
            }
        }
        return transform.position;
    }

    private void Animar(bool saltando)
    {
        if (animator == null) return;

        animator.speed = 1f;

        if (saltando)
        {
            if (!string.IsNullOrEmpty(estadoSalto)) animator.Play(estadoSalto, 0, 0f);
        }
        else
        {
            if (!string.IsNullOrEmpty(estadoQuieta)) animator.Play(estadoQuieta, 0, 0f);
            else animator.speed = 0f;
        }
    }
}
