
using UnityEngine;
using TMPro;

public class InformacionCargaSolar : MonoBehaviour
{
    [Header("Panel informativo")]
    public GameObject panelInformacion;

    [Header("Texto del estado de carga")]
    public TMP_Text textoCarga;

    private bool jugadorEnZona = false;

    private void Start()
    {
        if (panelInformacion != null)
        {
            panelInformacion.SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            jugadorEnZona = true;

            if (panelInformacion != null)
            {
                panelInformacion.SetActive(true);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            jugadorEnZona = false;

            if (panelInformacion != null)
            {
                panelInformacion.SetActive(false);
            }
        }
    }

    public void MostrarCargaActual()
    {
        if (!jugadorEnZona || textoCarga == null)
            return;

        textoCarga.text =
            "Carga actual de los autos: 100 %\n" +
            "Los dos vehículos están completamente cargados.";
    }
}

