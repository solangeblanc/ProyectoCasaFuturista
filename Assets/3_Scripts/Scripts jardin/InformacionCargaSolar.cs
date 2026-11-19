using UnityEngine;
using TMPro;

// Muestra un panel informativo cuando el jugador entra a la zona (collider "Is Trigger")
// y lo oculta al salir. Marca la función como descubierta apenas el jugador ve la información.
// Sirve para los autos (id "cargaAutos") y para los paneles solares (id "paneles"):
// se pone una copia del script en cada zona y se cambia el id y el mensaje en el Inspector.
public class InformacionCargaSolar : MonoBehaviour
{
    [Header("Panel informativo")]
    public GameObject panelInformacion;

    [Header("Texto")]
    public TMP_Text textoCarga;
    [TextArea(2, 4)]
    public string mensaje = "Carga actual de los autos: 100 %\nLos dos vehículos están completamente cargados.";

    [Header("Descubrimiento (mismo id que en el registro)")]
    public string idDescubrimiento = "cargaAutos";

    private bool jugadorEnZona = false;

    private void Start()
    {
        if (panelInformacion != null) panelInformacion.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        jugadorEnZona = true;
        if (panelInformacion != null) panelInformacion.SetActive(true);

        // Escribe el mensaje y marca la función como descubierta.
        MostrarCargaActual();
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        jugadorEnZona = false;
        if (panelInformacion != null) panelInformacion.SetActive(false);
    }

    // También se puede llamar desde un botón de la UI (OnClick).
    public void MostrarCargaActual()
    {
        if (!jugadorEnZona) return;

        if (textoCarga != null) textoCarga.text = mensaje;

        RegistroDescubrimientos.Descubrir(idDescubrimiento);
    }
}

