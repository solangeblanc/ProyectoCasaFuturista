using UnityEngine;

public class MovAspiradora : MonoBehaviour
{
    public Transform[] puntos;
    public float velocidad = 1.5f;
    int actual = 0;
    float yFijo;

    void Start()
    {
        yFijo = transform.position.y;
    }

    void Update()
    {
        if (puntos.Length == 0) return;

        Vector3 destino = puntos[actual].position;
        destino.y = yFijo; // que los puntos no la tiren abajo

        // mover
        transform.position = Vector3.MoveTowards(transform.position, destino, velocidad * Time.deltaTime);

        // girar SOLO en Y
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
        // candado final, por si algo la quiere volcar
        transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);
        transform.position = new Vector3(transform.position.x, yFijo, transform.position.z);
    }
}