using UnityEngine;

public class MovimientoNPC : MonoBehaviour
{
    public Transform[] puntosRuta;
    public float velocidad = 2f;
    [Tooltip("Al llegar al último punto vuelve al primero y sigue")]
    public bool repetir = true;

    private int puntoActual = 1;
    private bool enMovimiento = false;
    private float alturaInicial;

    public bool EnMovimiento => enMovimiento;

    private void Start()
    {
        alturaInicial = transform.position.y;

        if (puntosRuta != null && puntosRuta.Length > 0 && puntosRuta[0] != null)
            puntosRuta[0].position = transform.position;
    }

    public void IniciarRecorrido()
    {
        if (puntosRuta == null || puntosRuta.Length < 2) return;
        if (puntoActual >= puntosRuta.Length) puntoActual = 1;
        enMovimiento = true;
    }

    public void Detener()
    {
        enMovimiento = false;
    }

    private void Update()
    {
        if (!enMovimiento || puntosRuta == null || puntoActual >= puntosRuta.Length)
            return;

        if (puntosRuta[puntoActual] == null) return;

        Vector3 destino = puntosRuta[puntoActual].position;
        destino.y = alturaInicial;

        transform.position = Vector3.MoveTowards(transform.position, destino,
                                                 velocidad * Time.deltaTime);

        Vector3 direccion = destino - transform.position;
        direccion.y = 0f;

        if (direccion.sqrMagnitude > 0.001f)
            transform.forward = direccion.normalized;

        if (Vector3.Distance(transform.position, destino) < 0.1f)
        {
            puntoActual++;

            if (puntoActual >= puntosRuta.Length)
            {
                if (repetir) puntoActual = 0;
                else enMovimiento = false;
            }
        }
    }
}


