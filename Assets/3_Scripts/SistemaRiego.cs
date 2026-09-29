using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(ParticleSystem))]
public class SistemaRiego : MonoBehaviour
{
    [Header("Tecla")]
    [Tooltip("Tecla que prende/apaga el riego.")]
    [SerializeField] private Key teclaRiego = Key.R;

    private ParticleSystem agua;

    private void Awake()
    {
        agua = GetComponent<ParticleSystem>();
        agua.Stop(); // Arranca apagado
    }

    private void Update()
    {
        if (Keyboard.current != null && Keyboard.current[teclaRiego].wasPressedThisFrame)
        {
            if (agua.isPlaying)
            {
                agua.Stop(); // Deja de emitir, pero las gotas que ya salieron terminan su animación
            }
            else
            {
                agua.Play();
            }
        }
    }
}