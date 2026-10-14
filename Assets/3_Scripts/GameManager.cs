using System;
using UnityEngine;

public enum EstadoJuego
{
    Bienvenida,   // cartel inicial, juego congelado
    Jugando,      // el jugador se mueve libremente
    PanelAbierto  // panel de colores/ventanas/piso abierto
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Jugador")]
    [Tooltip("Arrastrá acá el objeto Player (el que tiene PlayerController).")]
    [SerializeField] private PlayerController jugador;

    public EstadoJuego Estado { get; private set; } = EstadoJuego.Jugando;

    // Otros scripts (UI, sonidos, etc.) pueden suscribirse para enterarse de los cambios.
    public static event Action<EstadoJuego> EstadoCambiado;

    private void Awake()
    {
        // Singleton: una sola instancia por escena.
        // No usamos DontDestroyOnLoad porque cada escena tiene su propio manager.
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }

    public void CambiarEstado(EstadoJuego nuevoEstado)
    {
        Estado = nuevoEstado;

        switch (nuevoEstado)
        {
            case EstadoJuego.Bienvenida:
                Time.timeScale = 0f;
                SetCursor(visible: false);
                SetJugadorActivo(false);
                break;

            case EstadoJuego.PanelAbierto:
                Time.timeScale = 0f;
                SetCursor(visible: true);
                SetJugadorActivo(false);
                break;

            case EstadoJuego.Jugando:
                Time.timeScale = 1f;
                SetCursor(visible: false);
                SetJugadorActivo(true);
                break;
        }

        EstadoCambiado?.Invoke(nuevoEstado);
    }

    public bool EstaJugando => Estado == EstadoJuego.Jugando;

    private void SetCursor(bool visible)
    {
        Cursor.visible = visible;
        Cursor.lockState = visible ? CursorLockMode.None : CursorLockMode.Locked;
    }

    private void SetJugadorActivo(bool activo)
    {
        if (jugador != null) jugador.enabled = activo;
    }
}