

using UnityEngine;   // Importa las clases básicas de Unity (MonoBehaviour, Mathf, Time, Quaternion, etc.)


public class CamaraVigila : MonoBehaviour   // clase publica para hacer girar la camara
{
    
    public float angulo = 20f;    // cuántos grados se mueve a cada lado----variable publica angulo (inspector)
    
    public float velocidad = 1f;  // variable para poner velocidad----más alto = más rápido

    // Variable privada donde se guarda la rotación con la que el objeto empieza.
    // Quaternion es el tipo de dato que usa Unity para representar rotaciones.
    Quaternion rotInicial;

    // Start se ejecuta una sola vez, justo antes del primer frame.
    void Start()
    {
       
        rotInicial = transform.localRotation;  // Guarda la rotación local inicial del objeto (respecto a su padre).
    }

    // Update se ejecuta en cada frame mientras el juego corre.
    void Update()
    {
        // Time.time son los segundos desde que arrancó el juego.
        // Time.time * velocidad ajusta qué tan rápido avanza el tiempo para la onda.
        // Mathf.Sin devuelve un valor que oscila suavemente entre -1 y 1.
        // Multiplicado por angulo, da un valor que va entre -angulo y +angulo.
        // Resultado: "giro" es el ángulo actual en grados (por ejemplo, de -20 a +20).
        float giro = Mathf.Sin(Time.time * velocidad) * angulo;

        // Quaternion.Euler(0f, giro, 0f) crea una rotación de "giro" grados solo sobre el eje Y
        // (izquierda y derecha), sin tocar los ejes X ni Z.
        // Multiplicarla por rotInicial combina ambas: primero la rotación original y encima el barrido.
        // Se asigna a localRotation, o sea que gira relativo al padre del objeto.
        transform.localRotation = rotInicial * Quaternion.Euler(0f, giro, 0f);
    }
}