using UnityEngine;

public class CamaraVigila : MonoBehaviour
{
    public float angulo = 20f;    // cuántos grados se mueve a cada lado
    public float velocidad = 1f;  // más alto = más rápido

    Quaternion rotInicial;

    void Start()
    {
        rotInicial = transform.localRotation;
    }

    void Update()
    {
        float giro = Mathf.Sin(Time.time * velocidad) * angulo;
        transform.localRotation = rotInicial * Quaternion.Euler(0f, giro, 0f);
    }
}
