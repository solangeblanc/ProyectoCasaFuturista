// Importa las clases básicas de Unity (MonoBehaviour, AudioSource, PlayerPrefs, etc.)
using UnityEngine;

// Script que administra la música de fondo del juego: permite silenciarla y des-silenciarla,
// recuerda esa elección entre sesiones y evita que haya dos reproductores de música a la vez.
// El nombre de la clase tiene que coincidir con el nombre del archivo: AudioManager.cs
public class AudioManager : MonoBehaviour
{
    // Variable estática: pertenece a la clase y no a un objeto en particular, así que hay una sola
    // para todo el juego. Guarda la referencia a "el" AudioManager activo (patrón Singleton).
    // Al ser pública, cualquier otro script puede llamar a AudioManager.Instance sin buscar el objeto.
    public static AudioManager Instance;
    // Referencia al componente AudioSource que reproduce la música. Es privada porque solo se usa acá adentro.
    private AudioSource miMusica;
    // Recuerda si la música está silenciada en este momento. Empieza en false (con sonido).
    private bool muteado = false;

    // Awake se ejecuta apenas se crea el objeto, antes que Start. Es el lugar ideal para inicializar cosas.
    void Awake()
    {
        // Si ya existe un AudioManager y no es este, significa que es un duplicado
        // (por ejemplo, al volver a cargar la escena). Se destruye este objeto y se corta con return,
        // para que no suene la música dos veces superpuesta.
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        // Si llegó hasta acá, este es el único AudioManager: se guarda como la instancia oficial.
        Instance = this;
        // Le dice a Unity que no destruya este objeto cuando se cambie de escena,
        // así la música sigue sonando sin cortarse entre escenas.
        DontDestroyOnLoad(gameObject);

        // Busca el componente AudioSource que está en este mismo objeto y lo guarda.
        // Por eso el objeto tiene que tener un AudioSource agregado en el Inspector.
        miMusica = GetComponent<AudioSource>();
        // Lee de PlayerPrefs (memoria guardada en la computadora) si la música estaba muteada en una sesión anterior.
        // "MusicaMuteada" es la clave con la que se guardó; el 0 es el valor por defecto si nunca se guardó nada.
        // Si el valor guardado es 1, muteado queda en true; si es 0, queda en false.
        muteado = PlayerPrefs.GetInt("MusicaMuteada", 0) == 1;
        // Aplica ese estado al AudioSource: si muteado es true, la música queda en silencio (sigue reproduciéndose, pero sin sonido).
        miMusica.mute = muteado;
    }

    // Método público, pensado para llamarse desde un botón de la UI (evento OnClick)
    // o desde otro script con AudioManager.Instance.ToggleMusica().
    public void ToggleMusica()
    {
        // Invierte el estado: si estaba con sonido pasa a silenciado, y viceversa.
        muteado = !muteado;
        // Aplica el nuevo estado al AudioSource.
        miMusica.mute = muteado;
        // Guarda la elección en PlayerPrefs para recordarla la próxima vez que se abra el juego.
        // PlayerPrefs no guarda booleanos, así que se convierte: si muteado es true guarda 1, si es false guarda 0.
        // El operador "? :" funciona como un if corto: condición ? valor_si_true : valor_si_false.
        PlayerPrefs.SetInt("MusicaMuteada", muteado ? 1 : 0);
    }
}