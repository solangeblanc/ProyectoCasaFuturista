using UnityEngine;
using TMPro;

public class RiegoControlJardin : MonoBehaviour
{
    public GameObject canvasAviso;
    public TMP_Text texto;
    public ParticleSystem[] chorros;
    public GirarAspersor brazo;

    bool regando = false;
    bool jugadorDentro = false;

    void Start()
    {
        canvasAviso.SetActive(false);
        AplicarRiego(false);
    }

    void Update()
    {
        if (jugadorDentro && Input.GetKeyDown(KeyCode.Space))
        {
            AplicarRiego(!regando);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            jugadorDentro = true;
            canvasAviso.SetActive(true);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            jugadorDentro = false;
            canvasAviso.SetActive(false);
        }
    }

    void AplicarRiego(bool encender)
    {
        regando = encender;
        brazo.enabled = encender;

        foreach (ParticleSystem c in chorros)
        {
            if (encender) c.Play();
            else c.Stop(true, ParticleSystemStopBehavior.StopEmitting);
        }

        texto.text = regando
            ? "Presioná ESPACIO para detener el riego"
            : "Presioná ESPACIO para comenzar el riego programado";
    }
}
