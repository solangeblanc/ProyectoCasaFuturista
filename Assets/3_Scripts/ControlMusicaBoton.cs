using UnityEngine;
public class ControlMusicaBoton : MonoBehaviour
{
    public void BotonMusicaPresionado()
    {
        if (AudioManager.Instance != null) AudioManager.Instance.ToggleMusica();
        else { var a = FindAnyObjectByType<AudioSource>(); if (a != null) a.mute = !a.mute; }
    }
}