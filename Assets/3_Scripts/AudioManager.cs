using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;
    private AudioSource miMusica;
    private bool muteado = false;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        miMusica = GetComponent<AudioSource>();
        muteado = PlayerPrefs.GetInt("MusicaMuteada", 0) == 1;
        miMusica.mute = muteado;
        miMusica.playOnAwake = false;
        miMusica.loop = true;
        miMusica.clip = null;
        miMusica.Stop();
        SceneManager.sceneLoaded += AlCargarEscena;
    }

    void AlCargarEscena(Scene escena, LoadSceneMode modo)
    {
        if (escena.name == "0_bienvenida" || escena.name == "Creditos" || escena.buildIndex == 0)
            DetenerMusica();
    }

    public void ReproducirMusica(AudioClip clip)
    {
        if (clip == null) return;
        if (SceneManager.GetActiveScene().name == "0_bienvenida" || SceneManager.GetActiveScene().name == "Creditos") return;
        miMusica.clip = clip;
        miMusica.loop = true;
        miMusica.mute = muteado;
        if (!muteado) miMusica.Play();
    }

    public void ToggleMusica()
    {
        muteado = !muteado;
        miMusica.mute = muteado;
        PlayerPrefs.SetInt("MusicaMuteada", muteado ? 1 : 0);
        PlayerPrefs.Save();
        if (!muteado && miMusica.clip != null && !miMusica.isPlaying) miMusica.Play();
    }

    public void DetenerMusica() { miMusica.Stop(); miMusica.clip = null; }
    void OnDestroy() { SceneManager.sceneLoaded -= AlCargarEscena; }
}