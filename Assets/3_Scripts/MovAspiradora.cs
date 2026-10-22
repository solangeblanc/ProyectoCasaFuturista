using UnityEngine;

public class MovAspiradora : MonoBehaviour
{
    public Transform[] puntos;
    public float velocidad = 1.5f;
    int actual = 0;
    float yFijo;

    [Header("Interacción")]
    public GameObject canvasAspiradora;
    public Transform jugador;
    public float distanciaActivacion = 100f;

    bool encendida = false;
    bool jugadorCerca = false;

    void Start()
    {
        yFijo = transform.position.y;
        encendida = false;
        if (canvasAspiradora != null)
            canvasAspiradora.SetActive(false);
    }

    void Update()
    {
        if (jugador == null) return;

        // chequeo distancia
        float dist = Vector3.Distance(transform.position, jugador.position);
        jugadorCerca = dist <= distanciaActivacion;

        // mostrar canvas si estás cerca
        if (canvasAspiradora != null)
            canvasAspiradora.SetActive(jugadorCerca);

        // prender / apagar con ESPACIO
        if (jugadorCerca && Input.GetKeyDown(KeyCode.Space))
        {
            encendida = !encendida;
            Debug.Log("Aspiradora: " + (encendida ? "PRENDIDA" : "APAGADA"));
        }

        // si está apagada no se mueve
        if (!encendida) return;

        if (puntos.Length == 0) return;

        Vector3 destino = puntos[actual].position;
        destino.y = yFijo;

        transform.position = Vector3.MoveTowards(transform.position, destino, velocidad * Time.deltaTime);

        Vector3 dir = destino - transform.position;
        dir.y = 0;
        if (dir.sqrMagnitude > 0.001f)
        {
            Quaternion rot = Quaternion.LookRotation(dir);
            transform.rotation = Quaternion.Euler(0, rot.eulerAngles.y, 0);
        }

        if (Vector3.Distance(transform.position, destino) < 0.2f)
            actual = (actual + 1) % puntos.Length;
    }

    void LateUpdate()
    {
        transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);
        transform.position = new Vector3(transform.position.x, yFijo, transform.position.z);
    }
}