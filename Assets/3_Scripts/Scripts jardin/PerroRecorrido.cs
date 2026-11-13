// Importa las clases básicas de Unity (MonoBehaviour, Transform, Vector3, Quaternion, AudioSource, etc.)
using UnityEngine;
// Importa AnimationPlayableOutput y AnimationClipPlayable, para reproducir animaciones por código sin Animator Controller.
using UnityEngine.Animations;
// Importa PlayableGraph y los tipos base del sistema Playables (el "grafo" que mezcla animaciones).
using UnityEngine.Playables;

// Script del perro que recorre el jardín: espera a que el jugador se acerque, muestra un cartel de aviso,
// y cuando el jugador llega lo bastante cerca camina por una lista de puntos hasta su cucha,
// gira 180° y se queda quieto. Mezcla animaciones de idle y caminar por código y puede ladrar.
// El nombre de la clase tiene que coincidir con el nombre del archivo: PerroRecorrido.cs
public class PerroRecorrido : MonoBehaviour
{
    // Enumeración con los estados posibles del perro. Solo se usa dentro de este script (por eso no es public).
    // Esperando: quieto hasta que llegue el jugador. Recorriendo: camina por los puntos.
    // Girando: da la vuelta final en la cucha. Terminado: quedó congelado en su pose final.
    enum Estado { Esperando, Recorriendo, Girando, Terminado }

    // Header solo agrega un título en el Inspector para ordenar las variables, no afecta la lógica.
    [Header("Jugador")]
    // Transform del jugador, para medir la distancia entre el perro y él. Se arrastra desde la Hierarchy.
    public Transform jugador;

    [Header("Mensaje previo")]
    // Cartel (Canvas) que aparece antes de que el perro arranque.
    public GameObject canvasMensaje;
    public float distanciaMensaje = 40f;       // aparece el cartel
    public float distanciaActivacion = 15f;    // el perro arranca

    [Header("Recorrido (el último punto es la cucha)")]
    // Lista de puntos por los que pasa el perro, en orden. El último punto es la cucha.
    public Transform[] puntos;
    // Velocidad de avance, en unidades de Unity por segundo.
    public float velocidad = 10f;
    // Qué tan rápido gira el perro para mirar hacia el siguiente punto. Más alto = giro más brusco.
    public float giro = 6f;
    // Distancia a la que se considera que el perro "llegó" a un punto y pasa al siguiente.
    public float distanciaLlegada = 2f;

    [Header("Final en la cucha")]
    // Velocidad del giro de 180° que hace al llegar a la cucha.
    public float velocidadGiroFinal = 3f;
    // Segundo de la animación idle en el que el perro queda congelado al terminar.
    public float segundoPoseFinal = 0f;

    [Header("Animaciones (arrastrar los clips)")]
    // Clip de animación del perro quieto. Se arrastra desde el Project (están dentro del .fbx del perro).
    public AnimationClip clipIdle;
    // Clip de animación del perro caminando.
    public AnimationClip clipWalk;
    // Velocidad de reproducción de la animación de caminar (1 = normal). Sirve para sincronizar las patas con la velocidad real.
    public float velocidadAnimacionCaminar = 1f;

    [Header("Sonido (se busca solo si queda vacío)")]
    // AudioSource con el sonido del ladrido. Si se deja vacío, Start lo busca en el perro.
    public AudioSource audioPerro;
    // Ladra al empezar a caminar.
    public bool ladrarAlArrancar = true;
    // Ladra al llegar a la cucha.
    public bool ladrarEnLaCucha = true;
    // Ladra cada cierto tiempo mientras camina.
    public bool ladrarCaminando = false;
    // Segundos entre ladridos mientras camina (solo se usa si ladrarCaminando está activado).
    public float intervaloLadrido = 5f;

    // El "grafo" de animación: el contenedor que maneja todas las animaciones que se reproducen por código.
    PlayableGraph graph;
    // Mezclador que combina dos animaciones (idle y walk) según el peso de cada una.
    AnimationMixerPlayable mixer;
    // Reproductores individuales de cada clip.
    AnimationClipPlayable pIdle, pWalk;
    // Cuánto pesa la animación de caminar en la mezcla: 0 = solo idle, 1 = solo walk.
    float pesoWalk = 0f;

    // Estado actual del perro. Empieza esperando al jugador.
    Estado estado = Estado.Esperando;
    // Índice del punto del recorrido hacia el que camina el perro en este momento.
    int indice = 0;
    // Rotación objetivo del giro final en la cucha.
    Quaternion rotFinal;
    // Indica si ya se congeló la pose final, para hacerlo una sola vez.
    bool congelado = false;
    // Temporizador para los ladridos mientras camina.
    float tLadrido = 0f;

    // Start se ejecuta una sola vez, antes del primer frame.
    void Start()
    {
        // Busca el Audio Source solo, sin arrastrar nada
        // Primero en este mismo objeto, y si no está, en sus hijos.
        if (audioPerro == null) audioPerro = GetComponent<AudioSource>();
        if (audioPerro == null) audioPerro = GetComponentInChildren<AudioSource>();

        // Busca el Animator del perro (en este objeto o en sus hijos).
        Animator anim = GetComponentInChildren<Animator>();
        // Le quita el Animator Controller para que no pise las animaciones que se reproducen por código.
        anim.runtimeAnimatorController = null;   // ignora el controller

        // Crea el grafo de animación, con el nombre "Perro" (solo es una etiqueta).
        graph = PlayableGraph.Create("Perro");
        // Crea la salida del grafo: conecta lo que se reproduzca en el grafo con el Animator del perro.
        var output = AnimationPlayableOutput.Create(graph, "Anim", anim);
        // Crea el mezclador con 2 entradas (idle y walk).
        mixer = AnimationMixerPlayable.Create(graph, 2);

        // Crea un reproductor para cada clip.
        pIdle = AnimationClipPlayable.Create(graph, clipIdle);
        pWalk = AnimationClipPlayable.Create(graph, clipWalk);
        // Ajusta la velocidad de la animación de caminar.
        pWalk.SetSpeed(velocidadAnimacionCaminar);

        // Conecta cada reproductor a una entrada del mezclador: idle a la 0 y walk a la 1.
        graph.Connect(pIdle, 0, mixer, 0);
        graph.Connect(pWalk, 0, mixer, 1);
        // Al empezar se ve solo el idle (peso 1) y nada de walk (peso 0).
        mixer.SetInputWeight(0, 1f);
        mixer.SetInputWeight(1, 0f);

        // Le dice a la salida que lo que se muestra es lo que sale del mezclador, y pone a funcionar el grafo.
        output.SetSourcePlayable(mixer);
        graph.Play();

        // El cartel arranca oculto.
        MostrarMensaje(false);
    }

    // Update se ejecuta en cada frame mientras el juego corre.
    void Update()
    {
        // Si falta el jugador o no hay puntos de recorrido, no hace nada.
        if (jugador == null || puntos.Length == 0) return;

        // Ejecuta la lógica que corresponde al estado actual.
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

        // Después de la lógica de movimiento, actualiza la mezcla de animaciones.
        ActualizarAnimacion();
    }

    // ---------- Espera y cartel ----------
    // Estado Esperando: el perro está quieto y vigila la distancia al jugador.
    void EsperarAlJugador()
    {
        // Distancia entre el perro y el jugador (en 3D, o sea, incluye la diferencia de altura).
        float dist = Vector3.Distance(transform.position, jugador.position);

        // Si el jugador se acercó lo suficiente, el perro arranca.
        if (dist < distanciaActivacion)
        {
            MostrarMensaje(false);                 // el cartel desaparece
            if (ladrarAlArrancar) Ladrar();
            estado = Estado.Recorriendo;           // el perro arranca
        }
        else
        {
            // El cartel se ve solo entre las dos distancias
            // Mathf.Max evita problemas si por error se configura la distancia del mensaje menor que la de activación.
            MostrarMensaje(dist < Mathf.Max(distanciaMensaje, distanciaActivacion));
        }
    }

    // Muestra u oculta el cartel, sin llamar a SetActive si ya está en el estado pedido.
    void MostrarMensaje(bool visible)
    {
        if (canvasMensaje != null && canvasMensaje.activeSelf != visible)
            canvasMensaje.SetActive(visible);
    }

    // ---------- Recorrido ----------
    // Estado Recorriendo: el perro camina hacia el punto actual de la lista.
    void IrAlPunto()
    {
        // Ladrido opcional mientras camina
        if (ladrarCaminando)
        {
            // Suma el tiempo transcurrido y ladra cuando se cumple el intervalo.
            tLadrido += Time.deltaTime;
            if (tLadrido >= intervaloLadrido)
            {
                tLadrido = 0f;
                Ladrar();
            }
        }

        // Dirección hacia el punto actual. La Y se pone en 0 para que el perro no se incline hacia arriba o abajo.
        Vector3 dir = puntos[indice].position - transform.position;
        dir.y = 0f;

        // Si ya está lo bastante cerca del punto, se considera que llegó.
        if (dir.magnitude < distanciaLlegada)
        {
            // Si era el último punto (la cucha)...
            if (indice >= puntos.Length - 1)
            {
                // Llegó a la cucha: prepara el giro de 180°
                // La rotación objetivo es la actual más 180° sobre el eje Y.
                rotFinal = Quaternion.Euler(0f, transform.eulerAngles.y + 180f, 0f);
                estado = Estado.Girando;
            }
            // Si no era el último, pasa al siguiente punto.
            else indice++;
            // Corta acá: no avanza en este frame.
            return;
        }

        // Gira suavemente hacia el punto. Slerp interpola entre la rotación actual y la deseada.
        Quaternion rot = Quaternion.LookRotation(dir);
        transform.rotation = Quaternion.Slerp(transform.rotation, rot, Time.deltaTime * giro);
        // Avanza hacia adelante (según hacia dónde mira). Time.deltaTime lo hace independiente de los FPS.
        transform.position += transform.forward * velocidad * Time.deltaTime;
    }

    // ---------- Final en la cucha ----------
    // Estado Girando: el perro da la vuelta de 180° en la cucha.
    void GirarEnLaCucha()
    {
        // Gira hacia la rotación final a velocidad constante. RotateTowards avanza como máximo
        // "velocidadGiroFinal * 60" grados por segundo, sin pasarse del objetivo.
        transform.rotation = Quaternion.RotateTowards(
            transform.rotation, rotFinal, velocidadGiroFinal * 60f * Time.deltaTime);

        // Cuando falta menos de 1 grado, termina el giro.
        if (Quaternion.Angle(transform.rotation, rotFinal) < 1f)
        {
            // Lo deja exactamente en la rotación final, sin el pequeño error restante.
            transform.rotation = rotFinal;
            if (ladrarEnLaCucha) Ladrar();
            estado = Estado.Terminado;
        }
    }

    // Estado Terminado: congela al perro en una pose final.
    void Congelar()
    {
        // Solo se hace una vez.
        if (congelado) return;
        congelado = true;

        // Deja el idle en el frame elegido y detiene todo
        // Mathf.Clamp evita que el segundo elegido se pase de la duración del clip.
        pIdle.SetTime(Mathf.Clamp(segundoPoseFinal, 0f, clipIdle.length));
        // Velocidad 0 en ambos clips: las animaciones se quedan paradas.
        pIdle.SetSpeed(0f);
        pWalk.SetSpeed(0f);
    }

    // ---------- Animación ----------
    // Mezcla las animaciones de idle y caminar según lo que esté haciendo el perro.
    void ActualizarAnimacion()
    {
        // Patas en movimiento mientras recorre o gira
        bool moviendo = (estado == Estado.Recorriendo || estado == Estado.Girando);
        // Si se mueve, la meta es 100% walk; si no, 100% idle.
        float objetivo = moviendo ? 1f : 0f;
        // Acerca el peso actual a la meta de forma gradual (transición suave de unos 0,17 segundos).
        pesoWalk = Mathf.MoveTowards(pesoWalk, objetivo, Time.deltaTime * 6f);
        // Aplica los pesos al mezclador: lo que sube uno, baja el otro.
        mixer.SetInputWeight(0, 1f - pesoWalk);
        mixer.SetInputWeight(1, pesoWalk);

        if (estado == Estado.Terminado) return;   // congelado: no repetir

        // Reinicia cada clip cuando llega al final, para que se repitan en bucle.
        Repetir(pIdle, clipIdle);
        Repetir(pWalk, clipWalk);
    }

    // Hace que un clip vuelva a empezar cuando llega a su final (bucle manual).
    void Repetir(AnimationClipPlayable p, AnimationClip c)
    {
        // El operador % (resto) devuelve el tiempo dentro de la duración del clip, o sea, vuelve al principio.
        if (c.length > 0f && p.GetTime() >= c.length)
            p.SetTime(p.GetTime() % c.length);
    }

    // ---------- Sonido ----------
    // Reproduce el ladrido, si hay un AudioSource asignado.
    void Ladrar()
    {
        if (audioPerro != null)
            audioPerro.Play();
    }

    // Unity llama a este método cuando el objeto se destruye (por ejemplo, al cambiar de escena).
    void OnDestroy()
    {
        // Libera el grafo de animación. Si no se destruye a mano, queda en memoria y Unity avisa de una fuga.
        if (graph.IsValid()) graph.Destroy();
    }
}