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
    public float giro = 3f;
    public float distanciaLlegada = 2f;

    [Header("Final en la cucha")]
    public float velocidadGiroFinal = 3f;
    public float segundoPoseFinal = 0f;

    [Header("Animaciones (arrastrar los clips)")]
    public AnimationClip clipIdle;
    public AnimationClip clipWalk;
    public float velocidadAnimacionCaminar = 1f;

    PlayableGraph graph;
    AnimationMixerPlayable mixer;
    AnimationClipPlayable pIdle, pWalk;
    float pesoWalk = 0f;

    Estado estado = Estado.Esperando;
    int indice = 0;
    Quaternion rotFinal;
    bool congelado = false;

    void Start()
    {
        Animator anim = GetComponentInChildren<Animator>();
        anim.runtimeAnimatorController = null;

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

    void EsperarAlJugador()
    {
        float dist = Vector3.Distance(transform.position, jugador.position);

        if (dist < distanciaActivacion)
        {
            MostrarMensaje(false);            // el cartel desaparece
            estado = Estado.Recorriendo;      // el perro arranca
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

    void IrAlPunto()
    {
        Vector3 dir = puntos[indice].position - transform.position;
        dir.y = 0f;

        if (dir.magnitude < distanciaLlegada)
        {
            if (indice >= puntos.Length - 1)
            {
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

    void GirarEnLaCucha()
    {
        transform.rotation = Quaternion.RotateTowards(
            transform.rotation, rotFinal, velocidadGiroFinal * 60f * Time.deltaTime);

        if (Quaternion.Angle(transform.rotation, rotFinal) < 1f)
        {
            transform.rotation = rotFinal;
            estado = Estado.Terminado;
        }
    }

    void Congelar()
    {
        if (congelado) return;
        congelado = true;

        pIdle.SetTime(Mathf.Clamp(segundoPoseFinal, 0f, clipIdle.length));
        pIdle.SetSpeed(0f);
        pWalk.SetSpeed(0f);
    }

    void ActualizarAnimacion()
    {
        bool moviendo = (estado == Estado.Recorriendo || estado == Estado.Girando);
        float objetivo = moviendo ? 1f : 0f;
        pesoWalk = Mathf.MoveTowards(pesoWalk, objetivo, Time.deltaTime * 6f);
        mixer.SetInputWeight(0, 1f - pesoWalk);
        mixer.SetInputWeight(1, pesoWalk);

        if (estado == Estado.Terminado) return;

        Repetir(pIdle, clipIdle);
        Repetir(pWalk, clipWalk);
    }

    void Repetir(AnimationClipPlayable p, AnimationClip c)
    {
        if (c.length > 0f && p.GetTime() >= c.length)
            p.SetTime(p.GetTime() % c.length);
    }

    void OnDestroy()
    {
        if (graph.IsValid()) graph.Destroy();
    }
}