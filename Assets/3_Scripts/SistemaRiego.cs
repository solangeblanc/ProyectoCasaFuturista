using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(ParticleSystem))]
public class SistemaRiego : MonoBehaviour
{
    [Header("Tecla")]
    [SerializeField] private Key teclaRiego = Key.Space;

    [Header("Canvas / Cartel de Ayuda")]
    [Tooltip("Arrastra acá tu Canvas o cartel que indica que se puede presionar la tecla")]
    [SerializeField] private GameObject canvasCartel;

    private ParticleSystem agua;
    private bool jugadorCerca = false;

    private void Awake()
    {
        agua = GetComponent<ParticleSystem>();
        agua.Stop();

        // Aseguramos que el cartel empiece apagado
        if (canvasCartel != null)
            canvasCartel.SetActive(false);
    }

    private void Update()
    {
        // Solo si está dentro del collider
        if (jugadorCerca && Keyboard.current != null && Keyboard.current[teclaRiego].wasPressedThisFrame)
        {
            if (agua.isPlaying)
                agua.Stop();
            else
                agua.Play();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            jugadorCerca = true;

            // Muestra el cartel cuando el jugador entra al área
            if (canvasCartel != null)
                canvasCartel.SetActive(true);
        }
    }



    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            jugadorCerca = false;

            // Oculta el cartel cuando el jugador sale del área
            if (canvasCartel != null)
                canvasCartel.SetActive(false);
        }
    }
}