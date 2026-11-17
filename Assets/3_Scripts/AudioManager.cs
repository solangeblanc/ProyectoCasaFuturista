using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;
    private AudioSource miMusica;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        miMusica = GetComponent<AudioSource>();
        miMusica.loop = true;
        miMusica.mute = PlayerPrefs.GetInt("MusicaMuteada", 0) == 1;
    }

    public void ReproducirMusica(AudioClip clip)
    {
        if (clip == null) return;
        if (miMusica.clip == clip && miMusica.isPlaying) return;
        miMusica.clip = clip;
        miMusica.Play();
    }

    public void DetenerMusica()
    {
        miMusica.Stop();
    }

    public void ToggleMusica()
    {
        miMusica.mute = !miMusica.mute;
        PlayerPrefs.SetInt("MusicaMuteada", miMusica.mute ? 1 : 0);
        PlayerPrefs.Save();
    }
}