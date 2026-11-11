// Importa las clases básicas de Unity (MonoBehaviour, GameObject, Collider, etc.)
using UnityEngine;

// Script que muestra un cartel (un Canvas) cuando el jugador entra a una zona, en este caso cerca de la escalera,
// y lo oculta cuando sale.
// El nombre de la clase tiene que coincidir con el nombre del archivo: CartelEscalera.cs
public class CartelEscalera : MonoBehaviour
{
    // Referencia al Canvas (o al objeto) del cartel que se muestra u oculta.
    // Al ser pública aparece en el Inspector, donde se arrastra el objeto desde la Hierarchy.
    public GameObject canvasCartel;

    // Start se ejecuta una sola vez, antes del primer frame.
    // SetActive(false) desactiva el objeto, así que el cartel arranca invisible.
    void Start() { canvasCartel.SetActive(false); } //oculta el cartel al iniciar

    // Unity llama a este método automáticamente cuando otro objeto entra al collider de este objeto.
    // Para que funcione, el collider de este objeto tiene que tener marcado "Is Trigger"
    // y alguno de los dos objetos (el jugador o la zona) tiene que tener un Rigidbody.
    void OnTriggerEnter(Collider other)
    {
        // Comprueba que quien entró tenga el Tag "Player" (asignado en el Inspector del jugador).
        // Si es así, SetActive(true) activa el objeto y el cartel se vuelve visible.
        if (other.CompareTag("Player")) canvasCartel.SetActive(true); // si el player entra en zona muestra el cartel
    }
    // Unity llama a este método automáticamente cuando otro objeto sale del collider de este objeto.
    void OnTriggerExit(Collider other)
    {
        // Si quien salió es el jugador, desactiva el cartel para que vuelva a ocultarse.
        if (other.CompareTag("Player")) canvasCartel.SetActive(false);
    }
}