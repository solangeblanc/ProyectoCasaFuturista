using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum EstadoJuego
{
    Bienvenida,
    Jugando,
    PanelAbierto,
    Pausa
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Jugador")]
    [Tooltip("Arrastrá acá el objeto Player (el que tiene PlayerController).")]
    [SerializeField] private PlayerController jugador;
    [SerializeField] private GameObject botonPausa;
    [SerializeField] private GameObject panelMenuPausa;
    [SerializeField] private string nombreEscenaMenu = "MenuPrincipal";

    public EstadoJuego Estado { get; private set; } = EstadoJuego.Jugando;
    public static event Action<EstadoJuego> EstadoCambiado;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    private void OnDestroy() { if (Instance == this) Instance = null; }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (Estado == EstadoJuego.Pausa) Reanudar();
            else if (Estado == EstadoJuego.Jugando) Pausar();
        }
    }

    public void CambiarEstado(EstadoJuego nuevoEstado)
    {
        Estado = nuevoEstado;
        switch (nuevoEstado)
        {
            case EstadoJuego.Bienvenida:
                Time.timeScale = 0f;
                SetCursor(false);
                SetJugadorActivo(false);
                if (botonPausa != null) botonPausa.SetActive(false);
                if (panelMenuPausa != null) panelMenuPausa.SetActive(false);
                break;

            case EstadoJuego.PanelAbierto:
                Time.timeScale = 0f;
                SetCursor(true);
                SetJugadorActivo(false);
                if (botonPausa != null) botonPausa.SetActive(false);
                break;

            case EstadoJuego.Pausa:
                Time.timeScale = 0f;
                SetCursor(true);
                SetJugadorActivo(false);
                if (panelMenuPausa != null) panelMenuPausa.SetActive(true);
                break;

            case EstadoJuego.Jugando:
                Time.timeScale = 1f;
                SetCursor(false);
                SetJugadorActivo(true);
                if (botonPausa != null) botonPausa.SetActive(true);
                if (panelMenuPausa != null) panelMenuPausa.SetActive(false);
                break;
        }
        EstadoCambiado?.Invoke(nuevoEstado);
    }

    public void Pausar() => CambiarEstado(EstadoJuego.Pausa);
    public void Reanudar() => CambiarEstado(EstadoJuego.Jugando);
    public void VolverAlMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("0_Bienvenida");
    }

    public bool EstaJugando => Estado == EstadoJuego.Jugando;

    private void SetCursor(bool visible)
    {
        Cursor.visible = visible;
        Cursor.lockState = visible ? CursorLockMode.None : CursorLockMode.Locked;
    }
    private void SetJugadorActivo(bool activo) { if (jugador != null) jugador.enabled = activo; }
}