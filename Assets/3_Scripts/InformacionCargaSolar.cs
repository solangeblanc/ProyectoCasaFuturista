// Importa las clases básicas de Unity (MonoBehaviour, GameObject, Collider, etc.)
using UnityEngine;
// Importa TextMeshPro, necesario para usar el tipo TMP_Text (textos de interfaz con mejor calidad).
using TMPro;

// Script que muestra un panel informativo cuando el jugador entra a una zona
// (por ejemplo, cerca de los autos que se cargan con energía solar) y lo oculta cuando sale.
// El nombre de la clase tiene que coincidir con el nombre del archivo: InformacionCargaSolar.cs
public class InformacionCargaSolar : MonoBehaviour
{
    // Header solo agrega un título en el Inspector para ordenar las variables, no afecta la lógica.
    [Header("Panel informativo")]
    // Referencia al panel (objeto de la UI) que se muestra u oculta. Se arrastra desde la Hierarchy en el Inspector.
    public GameObject panelInformacion;

    // Otro título visual en el Inspector.
    [Header("Texto del estado de carga")]
    // Referencia al texto TextMeshPro donde se va a escribir el estado de carga. También se arrastra en el Inspector.
    public TMP_Text textoCarga;

    // Variable privada que recuerda si el jugador está dentro de la zona en este momento.
    // Empieza en false porque al comenzar el juego el jugador no está en la zona.
    private bool jugadorEnZona = false;

    // Start se ejecuta una sola vez, antes del primer frame.
    private void Start()
    {
        // Comprueba que el panel esté asignado en el Inspector, para evitar errores si se olvidó arrastrarlo.
        if (panelInformacion != null)
        {
            // Oculta el panel al empezar el juego, ya que solo debe verse cuando el jugador se acerque.
            panelInformacion.SetActive(false);
        }
    }

    // Unity llama a este método automáticamente cuando otro objeto entra al collider de este objeto.
    // Para que funcione, el collider de este objeto tiene que tener marcado "Is Trigger"
    // y alguno de los dos objetos (el jugador o la zona) tiene que tener un Rigidbody.
    private void OnTriggerEnter(Collider other)
    {
        // Verifica que quien entró sea el jugador, mirando su Tag.
        // El jugador tiene que tener el Tag "Player" asignado en el Inspector.
        if (other.CompareTag("Player"))
        {
            // Anota que el jugador ya está dentro de la zona.
            jugadorEnZona = true;

            // Comprueba que el panel esté asignado antes de usarlo.
            if (panelInformacion != null)
            {
                // Muestra el panel informativo.
                panelInformacion.SetActive(true);
            }
        }
    }

    // Unity llama a este método automáticamente cuando otro objeto sale del collider de este objeto.
    private void OnTriggerExit(Collider other)
    {
        // Igual que antes: verifica que quien salió sea el jugador.
        if (other.CompareTag("Player"))
        {
            // Anota que el jugador ya no está en la zona.
            jugadorEnZona = false;

            // Comprueba que el panel esté asignado antes de usarlo.
            if (panelInformacion != null)
            {
                // Oculta el panel otra vez.
                panelInformacion.SetActive(false);
            }
        }
    }

    // Método público, pensado para llamarse desde afuera, por ejemplo desde un botón de la UI
    // (en el evento OnClick del botón) o desde otro script.
    public void MostrarCargaActual()
    {
        // Si el jugador no está en la zona, o el texto no fue asignado en el Inspector,
        // sale del método sin hacer nada (return corta la ejecución).
        if (!jugadorEnZona || textoCarga == null)
            return;

        // Escribe el mensaje en el texto de la interfaz.
        // "\n" es un salto de línea, así que el mensaje queda en dos renglones.
        // El signo + une los dos pedazos de texto en uno solo.
        textoCarga.text =
            "Carga actual de los autos: 100 %\n" +
            "Los dos vehículos están completamente cargados.";
    }
}

