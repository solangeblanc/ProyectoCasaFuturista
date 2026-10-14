using UnityEngine;

public class ControlBienvenida : MonoBehaviour
{
    public GameObject panelBienvenida;

    void Start()
    {
        // Arranca en estado Bienvenida: juego congelado y panel visible
        GameManager.Instance.CambiarEstado(EstadoJuego.Bienvenida);

        if (panelBienvenida != null)
        {
            panelBienvenida.SetActive(true);
        }
    }

    void Update()
    {
        // Mientras está la bienvenida, Espacio empieza el juego
        if (GameManager.Instance.Estado == EstadoJuego.Bienvenida && Input.GetKeyDown(KeyCode.Space))
        {
            if (panelBienvenida != null)
            {
                panelBienvenida.SetActive(false);
            }

            GameManager.Instance.CambiarEstado(EstadoJuego.Jugando);
        }
    }
}