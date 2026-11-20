using UnityEngine;

// Rana que salta sin parar por la pileta. NO usa NavMesh ni NavMeshAgent: mueve el objeto directamente.
// Elige un punto al azar dentro del agua, avanza hacia él con saltos (sube, avanza y aterriza) y descansa
// un momento entre salto y salto. Cuando llega, elige otro punto.
//
// Va en la rana (el objeto raíz que tiene el Animator). Las distancias se multiplican por la escala de la rana.
// Usa el tiempo del juego: mientras el juego está en la pantalla de bienvenida (tiempo en 0) la rana espera.
public class RanaSalto : MonoBehaviour
{
    [Header("Zona donde puede saltar")]
    [Tooltip("El objeto del agua de la pileta (cubo o plano con Renderer o Collider). Se usa su tamaño como zona.")]
    [SerializeField] private Transform agua;
    [Tooltip("Distancia al borde del agua donde la rana no salta (se multiplica por la escala de la rana).")]
    [SerializeField] private float margenBorde = 0.3f;
    [Tooltip("Solo se usa si no se asigna el agua: radio del círculo, alrededor de donde la dejaste, donde salta.")]
    [SerializeField] private float radioSinAgua = 5f;

    [Header("Saltos")]
    [Tooltip("Cuánto avanza en cada salto (se multiplica por la escala de la rana).")]
    [SerializeField] private float distanciaSalto = 0.5f;
    [Tooltip("Qué tan alto sube en cada salto (se multiplica por la escala de la rana).")]
    [SerializeField] private float alturaSalto = 0.2f;
    [Tooltip("Duración de cada salto, en segundos.")]
    [SerializeField] private float duracionSalto = 0.5f;
    [Tooltip("Descanso entre un salto y otro (mínimo y máximo, en segundos).")]
    [SerializeField] private float pausaMin = 0.1f;
    [SerializeField] private float pausaMax = 0.5f;

    [Header("Orientación")]
    [Tooltip("Si la rana salta de costado o de espaldas, probá 90, -90 o 180 hasta que mire hacia donde salta.")]
    [SerializeField] private float giroOffset = 0f;

    [Header("Animación (nombres de los estados en el Animator)")]
    [SerializeField] private string estadoSalto = "run";
    [Tooltip("Opcional: si se deja vacío, la rana queda congelada en la pose de aterrizaje entre saltos.")]
    [SerializeField] private string estadoQuieta = "idle";

    private enum Fase { Descansando, Saltando }
    private Fase fase = Fase.Descansando;

    private Animator animator;
    private Bounds limites;
    private bool hayLimites;
    private Vector3 centroInicial;

    private float escala = 1f;
    private float distanciaReal;
    private float alturaReal;
    private float margenReal;
    // Altura (Y) a la que está apoyada la rana sobre el agua: la de la posición en que la dejaste.
    private float alturaBase;

    private Vector3 destino;
    private Vector3 inicioSalto;
    private Vector3 finSalto;
    private float progreso;
    private float temporizador;
    private bool avisoPrimerSalto;

    private void Start()
    {
        animator = GetComponentInChildren<Animator>();

        // Si quedó un NavMeshAgent de antes en este objeto, lo apaga para que no interfiera.
        UnityEngine.AI.NavMeshAgent agente = GetComponent<UnityEngine.AI.NavMeshAgent>();
        if (agente != null) agente.enabled = false;

        // Adapta las distancias al tamaño de la rana (en tu escena está muy agrandada).
        escala = Mathf.Max(0.001f, Mathf.Abs(transform.lossyScale.y));
        distanciaReal = distanciaSalto * escala;
        alturaReal = alturaSalto * escala;
        margenReal = margenBorde * escala;
        duracionSalto = Mathf.Max(0.1f, duracionSalto);

        centroInicial = transform.position;
        alturaBase = transform.position.y;

        // Calcula el rectángulo del agua a partir de su Renderer o su Collider.
        if (agua != null)
        {
            Renderer r = agua.GetComponentInChildren<Renderer>();
            Collider c = agua.GetComponentInChildren<Collider>();
            if (r != null) { limites = r.bounds; hayLimites = true; }
            else if (c != null) { limites = c.bounds; hayLimites = true; }
        }
        if (!hayLimites)
            Debug.LogWarning("RanaSalto: no hay agua asignada (o no tiene Renderer/Collider). " +
                             "Salta en un círculo alrededor de donde está. Arrastrá el agua en 'Agua'.", this);

        Debug.Log("RanaSalto: lista. Escala = " + escala + ", cada salto avanza " + distanciaReal +
                  " unidades. Zona del agua encontrada = " + hayLimites + ".", this);

        ElegirDestino();
        EmpezarDescanso();
    }

    // Se usa LateUpdate para que la posición que escribe este script sea la última del frame.
    private void LateUpdate()
    {
        if (fase == Fase.Descansando)
        {
            temporizador -= Time.deltaTime;
            if (temporizador <= 0f) EmpezarSalto();
        }
        else
        {
            progreso += Time.deltaTime / duracionSalto;
            float k = Mathf.Clamp01(progreso);

            // Avanza en línea recta y sube y baja con un arco.
            Vector3 pos = Vector3.Lerp(inicioSalto, finSalto, k);
            pos.y = alturaBase + Mathf.Sin(k * Mathf.PI) * alturaReal;
            transform.position = pos;

            if (progreso >= 1f)
            {
                transform.position = new Vector3(finSalto.x, alturaBase, finSalto.z);
                EmpezarDescanso();
            }
        }
    }

    private void EmpezarDescanso()
    {
        fase = Fase.Descansando;
        temporizador = Random.Range(pausaMin, pausaMax);
        Animar(false);
    }

    private void EmpezarSalto()
    {
        Vector3 pos = transform.position;
        Vector3 direccion = destino - pos;
        direccion.y = 0f;

        // Si ya llegó al destino, elige uno nuevo.
        if (direccion.magnitude <= 0.05f * escala)
        {
            ElegirDestino();
            direccion = destino - pos;
            direccion.y = 0f;
        }
        if (direccion.sqrMagnitude < 0.0001f)
        {
            EmpezarDescanso();
            return;
        }

        // Cada salto avanza "distanciaReal", o lo que falte si el destino está más cerca.
        float paso = Mathf.Min(distanciaReal, direccion.magnitude);
        inicioSalto = pos;
        finSalto = pos + direccion.normalized * paso;

        // Gira hacia donde va a saltar.
        transform.rotation = Quaternion.LookRotation(direccion.normalized, Vector3.up) * Quaternion.Euler(0f, giroOffset, 0f);

        progreso = 0f;
        fase = Fase.Saltando;
        Animar(true);

        if (!avisoPrimerSalto)
        {
            avisoPrimerSalto = true;
            Debug.Log("RanaSalto: primer salto. Si ves este mensaje pero la rana no se mueve, avisame.", this);
        }
    }

    // Elige un punto al azar dentro del agua (o dentro del círculo si no hay agua asignada).
    private void ElegirDestino()
    {
        if (hayLimites)
        {
            float minX = limites.min.x + margenReal;
            float maxX = limites.max.x - margenReal;
            float minZ = limites.min.z + margenReal;
            float maxZ = limites.max.z - margenReal;
            if (minX > maxX) minX = maxX = limites.center.x;
            if (minZ > maxZ) minZ = maxZ = limites.center.z;

            destino = new Vector3(Random.Range(minX, maxX), alturaBase, Random.Range(minZ, maxZ));
        }
        else
        {
            Vector2 p = Random.insideUnitCircle * (radioSinAgua * escala);
            destino = new Vector3(centroInicial.x + p.x, alturaBase, centroInicial.z + p.y);
        }
    }

    // Controla la animación: salto o reposo.
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

    // Con la rana seleccionada, dibuja en la vista Scene el rectángulo donde puede saltar.
    private void OnDrawGizmosSelected()
    {
        if (agua == null) return;
        Renderer r = agua.GetComponentInChildren<Renderer>();
        if (r == null) return;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(r.bounds.center, r.bounds.size);
    }
}