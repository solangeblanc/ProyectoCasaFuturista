
using UnityEngine;

public class GirarAspersor : MonoBehaviour  // Script que hace girar un objeto de forma continua
{
 
   
    public float velocidad = 120f; //variable en el ispector para ajustar la velocidad del giro( para no cambiar desde el codigo)- una vuelta cada 3 segundos

    
    void Update()// Update se ejecuta en cada frame mientras el juego corre.
    {
  
        transform.Rotate(0f, 0f, velocidad * Time.deltaTime); // transfor. Rotate gira el objeto sumando una rotacion a la que ya tiene, gira en x, y, z es igual
        // a la velocidad multiplicada Time.deltatime---tiene valores en cero en x e y, solo gira en z--
        // Time.deltaTime es el tiempo que pasó desde el frame anterior.
        // Multiplicar por Time.deltaTime hace que la velocidad sea la misma en cualquier computadora,
        // sin importar los fps: así gira 120 grados por segundo reales, no 120 grados por frame.
    }
}