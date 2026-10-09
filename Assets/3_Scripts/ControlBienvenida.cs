using UnityEngine;

public class ControlBienvenida : MonoBehaviour
{
    public GameObject panelBienvenida;
    private bool juegoPausado = false;

    void Start()
    {
        // Forzamos la pausa inmediatamente
        PausarJuego();
    }

    void Update()
    {
        // FORZAMOS LA PAUSA CADA FRAME para anular cualquier otro script que quiera reactivar el tiempo
        if (juegoPausado)
        {
            Time.timeScale = 0f;

            // Bloqueamos y ocultamos el cursor para que no se mueva por la pantalla mientras lee
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }

        // Si el juego está pausado y pulsa Espacio
        if (juegoPausado && Input.GetKeyDown(KeyCode.Space))
        {
            ReanudarJuego();
        }
    }

    void PausarJuego()
    {
        juegoPausado = true;
        Time.timeScale = 0f; // Tiempo congelado

        if (panelBienvenida != null)
        {
            panelBienvenida.SetActive(true);
        }
    }

    void ReanudarJuego()
    {
        juegoPausado = false;
        Time.timeScale = 1f; // Tiempo normal

        if (panelBienvenida != null)
        {
            panelBienvenida.SetActive(false);
        }

        // Restauramos el cursor para jugar
        Cursor.visible = false; // O true, depende de tu juego
        Cursor.lockState = CursorLockMode.Locked;
    }
}