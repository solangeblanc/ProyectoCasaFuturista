using UnityEngine;

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
    }

    public void ToggleMusica()
    {
        muteado = !muteado;
        miMusica.mute = muteado;
        PlayerPrefs.SetInt("MusicaMuteada", muteado ? 1 : 0);
    }
}