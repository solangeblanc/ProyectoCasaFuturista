using UnityEngine;

public class PerroRecorrido : MonoBehaviour
{
    [Header("Activación")]
    public Transform jugador;               // arrastrá acá el objeto Jugador
    public float distanciaActivacion = 8f;  // a qué distancia se activa
    public bool apagarAlAlejarse = false;   // true = vuelve a idle si el jugador se va

    [Header("Recorrido")]
    public Transform[] puntos;
    public float velocidadCaminar = 3f;
    public float velocidadCorrer = 8f;
    [Range(0f, 1f)] public float probabilidadCorrer = 0.3f;
    public float giro = 5f;
    public float distanciaLlegada = 0.3f;
    public float tiempoQuieto = 2f;

    Animator anim;
    int indice = 0;
    float espera = 0f;
    float velocidadActual;
    bool activado = false;

    void Start()
    {
        anim = GetComponentInChildren<Animator>();
        velocidadActual = velocidadCaminar;
    }

    void Update()
    {
        if (puntos.Length == 0 || jugador == null) return;

        // Detectar al jugador
        float distJugador = Vector3.Distance(transform.position, jugador.position);

        if (!activado && distJugador <= distanciaActivacion)
            activado = true;
        else if (activado && apagarAlAlejarse && distJugador > distanciaActivacion * 1.5f)
            activado = false;

        // Si no está activado, queda quieto en idle
        if (!activado)
        {
            anim.SetFloat("Speed", 0f, 0.1f, Time.deltaTime);
            return;
        }

        // Pausa en el punto
        if (espera > 0f)
        {
            espera -= Time.deltaTime;
            anim.SetFloat("Speed", 0f, 0.1f, Time.deltaTime);
            return;
        }

        Vector3 destino = puntos[indice].position;
        destino.y = transform.position.y;
        Vector3 dir = destino - transform.position;

        // Llegó al punto
        if (dir.magnitude < distanciaLlegada)
        {
            indice = (indice + 1) % puntos.Length;
            velocidadActual = Random.value < probabilidadCorrer ? velocidadCorrer : velocidadCaminar;
            espera = tiempoQuieto;
            return;
        }

        // Girar y avanzar
        Quaternion rot = Quaternion.LookRotation(dir);
        transform.rotation = Quaternion.Slerp(transform.rotation, rot, Time.deltaTime * giro);
        transform.position += transform.forward * velocidadActual * Time.deltaTime;

        anim.SetFloat("Speed", velocidadActual, 0.1f, Time.deltaTime);
    }
}