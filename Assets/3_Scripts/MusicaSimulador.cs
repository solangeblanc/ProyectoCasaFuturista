using UnityEngine;
public class MusicaSimulador : MonoBehaviour
{
    public AudioClip musicaDeEstaEscena;
    void Start()
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.ReproducirMusica(musicaDeEstaEscena);
        else
        {
            var a = GetComponent<AudioSource>(); if (a == null) a = gameObject.AddComponent<AudioSource>();
            a.clip = musicaDeEstaEscena; a.loop = true; a.Play();
        }
    }
}