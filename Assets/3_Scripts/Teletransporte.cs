using UnityEngine;
using UnityEngine.InputSystem;

public class Teletransporte : MonoBehaviour
{
    public Transform destino;

    [Header("Tecla")]
    [SerializeField] private Key teclaTeletransporte = Key.Space;

    [Header("Cartel de la escalera")]
    [SerializeField] private GameObject cartelEscalera;

    private bool jugadorCerca = false;
    private GameObject jugador;
    private CharacterController controllerJugador;

    private void Update()
    {
        if (jugadorCerca && Keyboard.current != null && Keyboard.current[teclaTeletransporte].wasPressedThisFrame)
        {
            Teletransportar();
        }
    }

    private void Teletransportar()
    {
        if (jugador == null || destino == null) return;

        if (controllerJugador != null)
            controllerJugador.enabled = false;

        jugador.transform.position = destino.position;
        jugador.transform.rotation = destino.rotation;

        if (controllerJugador != null)
            controllerJugador.enabled = true;

        if (cartelEscalera != null)
            cartelEscalera.SetActive(false);

        jugadorCerca = false; // para que no se teletransporte 2 veces seguidas
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            jugadorCerca = true;
            jugador = other.gameObject;
            controllerJugador = other.GetComponent<CharacterController>();

            if (cartelEscalera != null)
                cartelEscalera.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            jugadorCerca = false;
            jugador = null;

            if (cartelEscalera != null)
                cartelEscalera.SetActive(false);
        }
    }
}