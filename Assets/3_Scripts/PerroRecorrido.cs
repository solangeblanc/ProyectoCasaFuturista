using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

public class PerroRecorrido : MonoBehaviour
{
    enum Estado { Esperando, Recorriendo, Girando, Terminado }

    [Header("Jugador")]
    public Transform jugador;

    [Header("Mensaje previo")]
    public GameObject canvasMensaje;
    public float distanciaMensaje = 40f;       // aparece el cartel
    public float distanciaActivacion = 15f;    // el perro arranca

    [Header("Recorrido (el último punto es la cucha)")]
    public Transform[] puntos;
    public float velocidad = 10f;
    public float giro = 6f;
    public float distanciaLlegada = 2f;

    [Header("Final en la cucha")]
    public float velocidadGiroFinal = 3f;
    public float segundoPoseFinal = 0f;

    [Header("Animaciones (arrastrar los clips)")]
    public AnimationClip clipIdle;
    public AnimationClip clipWalk;
    public float velocidadAnimacionCaminar = 1f;

    [Header("Sonido (se busca solo si queda vacío)")]
    public AudioSource audioPerro;
    public bool ladrarAlArrancar = true;
    public bool ladrarEnLaCucha = true;
    public bool ladrarCaminando = false;
    public float intervaloLadrido = 5f;

    PlayableGraph graph;
    AnimationMixerPlayable mixer;
    AnimationClipPlayable pIdle, pWalk;
    float pesoWalk = 0f;

    Estado estado = Estado.Esperando;
    int indice = 0;
    Quaternion rotFinal;
    bool congelado = false;
    float tLadrido = 0f;

    void Start()
    {
        // Busca el Audio Source solo, sin arrastrar nada
        if (audioPerro == null) audioPerro = GetComponent<AudioSource>();
        if (audioPerro == null) audioPerro = GetComponentInChildren<AudioSource>();
        if (audioPerro == null) Debug.LogWarning("No hay Audio Source en el perro");

        Animator anim = GetComponentInChildren<Animator>();
        anim.runtimeAnimatorController = null;   // ignora el controller

        graph = PlayableGraph.Create("Perro");
        var output = AnimationPlayableOutput.Create(graph, "Anim", anim);
        mixer = AnimationMixerPlayable.Create(graph, 2);

        pIdle = AnimationClipPlayable.Create(graph, clipIdle);
        pWalk = AnimationClipPlayable.Create(graph, clipWalk);
        pWalk.SetSpeed(velocidadAnimacionCaminar);

        graph.Connect(pIdle, 0, mixer, 0);
        graph.Connect(pWalk, 0, mixer, 1);
        mixer.SetInputWeight(0, 1f);
        mixer.SetInputWeight(1, 0f);

        output.SetSourcePlayable(mixer);
        graph.Play();

        MostrarMensaje(false);

        if (jugador == null) Debug.LogWarning("Falta asignar el Jugador en PerroRecorrido");
        if (canvasMensaje == null) Debug.LogWarning("Falta asignar el Canvas Mensaje en PerroRecorrido");
        if (puntos.Length == 0) Debug.LogWarning("Faltan los puntos en PerroRecorrido");
        if (clipIdle == null || clipWalk == null) Debug.LogWarning("Faltan los clips Idle y Walk en PerroRecorrido");
    }

    void Update()
    {
        if (jugador == null || puntos.Length == 0) return;

        switch (estado)
        {
            case Estado.Esperando:
                EsperarAlJugador();
                break;

            case Estado.Recorriendo:
                IrAlPunto();
                break;

            case Estado.Girando:
                GirarEnLaCucha();
                break;

            case Estado.Terminado:
                Congelar();
                break;
        }

        ActualizarAnimacion();
    }

    // ---------- Espera y cartel ----------
    void EsperarAlJugador()
    {
        float dist = Vector3.Distance(transform.position, jugador.position);

        if (dist < distanciaActivacion)
        {
            MostrarMensaje(false);                 // el cartel desaparece
            if (ladrarAlArrancar) Ladrar();
            estado = Estado.Recorriendo;           // el perro arranca
        }
        else
        {
            // El cartel se ve solo entre las dos distancias
            MostrarMensaje(dist < Mathf.Max(distanciaMensaje, distanciaActivacion));
        }
    }

    void MostrarMensaje(bool visible)
    {
        if (canvasMensaje != null && canvasMensaje.activeSelf != visible)
            canvasMensaje.SetActive(visible);
    }

    // ---------- Recorrido ----------
    void IrAlPunto()
    {
        // Ladrido opcional mientras camina
        if (ladrarCaminando)
        {
            tLadrido += Time.deltaTime;
            if (tLadrido >= intervaloLadrido)
            {
                tLadrido = 0f;
                Ladrar();
            }
        }

        Vector3 dir = puntos[indice].position - transform.position;
        dir.y = 0f;

        if (dir.magnitude < distanciaLlegada)
        {
            if (indice >= puntos.Length - 1)
            {
                // Llegó a la cucha: prepara el giro de 180°
                rotFinal = Quaternion.Euler(0f, transform.eulerAngles.y + 180f, 0f);
                estado = Estado.Girando;
            }
            else indice++;
            return;
        }

        Quaternion rot = Quaternion.LookRotation(dir);
        transform.rotation = Quaternion.Slerp(transform.rotation, rot, Time.deltaTime * giro);
        transform.position += transform.forward * velocidad * Time.deltaTime;
    }

    // ---------- Final en la cucha ----------
    void GirarEnLaCucha()
    {
        transform.rotation = Quaternion.RotateTowards(
            transform.rotation, rotFinal, velocidadGiroFinal * 60f * Time.deltaTime);

        if (Quaternion.Angle(transform.rotation, rotFinal) < 1f)
        {
            transform.rotation = rotFinal;
            if (ladrarEnLaCucha) Ladrar();
            estado = Estado.Terminado;
        }
    }

    void Congelar()
    {
        if (congelado) return;
        congelado = true;

        // Deja el idle en el frame elegido y detiene todo
        pIdle.SetTime(Mathf.Clamp(segundoPoseFinal, 0f, clipIdle.length));
        pIdle.SetSpeed(0f);
        pWalk.SetSpeed(0f);
    }

    // ---------- Animación ----------
    void ActualizarAnimacion()
    {
        // Patas en movimiento mientras recorre o gira
        bool moviendo = (estado == Estado.Recorriendo || estado == Estado.Girando);
        float objetivo = moviendo ? 1f : 0f;
        pesoWalk = Mathf.MoveTowards(pesoWalk, objetivo, Time.deltaTime * 6f);
        mixer.SetInputWeight(0, 1f - pesoWalk);
        mixer.SetInputWeight(1, pesoWalk);

        if (estado == Estado.Terminado) return;   // congelado: no repetir

        Repetir(pIdle, clipIdle);
        Repetir(pWalk, clipWalk);
    }

    void Repetir(AnimationClipPlayable p, AnimationClip c)
    {
        if (c.length > 0f && p.GetTime() >= c.length)
            p.SetTime(p.GetTime() % c.length);
    }

    // ---------- Sonido ----------
    void Ladrar()
    {
        if (audioPerro != null)
            audioPerro.Play();
    }

    void OnDestroy()
    {
        if (graph.IsValid()) graph.Destroy();
    }
}