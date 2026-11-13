
//El efecto se llama Head Bobbing (también conocido como balanceo de cámara o cámara en mano en primera persona).
//Simula el movimiento natural y orgánico que hace la cabeza de una persona al caminar (el sutil vaivén hacia arriba, abajo y los costados).




using UnityEngine;

public class HeadBobbing : MonoBehaviour
{
    [Header("Configuración de Movimiento")]
    [Tooltip("Qué tan alto/bajo se mueve la cámara")]
    public float bobbingAmount = 0.03f;
    [Tooltip("Velocidad de la oscilación")]
    public float bobbingSpeed = 10.0f;

    private Vector3 startPosition;
    private float timer = 0.0f;

    void Start()
    {
        // Guardamos la posición local original de la cámara
        startPosition = transform.localPosition;
    }

    void Update()
    {
        // Detectamos si el usuario está presionando alguna tecla de movimiento (WASD / Flechas)
        bool isMoving = Mathf.Abs(Input.GetAxis("Horizontal")) > 0.1f || Mathf.Abs(Input.GetAxis("Vertical")) > 0.1f;

        if (isMoving)
        {
            // Si se mueve, generamos el vaivén usando una onda senoidal
            timer += Time.deltaTime * bobbingSpeed;

            // Movimiento vertical (arriba y abajo) y horizontal leve
            float targetY = startPosition.y + Mathf.Sin(timer) * bobbingAmount;
            float targetX = startPosition.x + Mathf.Cos(timer / 2f) * (bobbingAmount / 2f);

            transform.localPosition = new Vector3(targetX, targetY, startPosition.z);
        }
        else
        {
            // Si está quieto, devolvemos la cámara suavemente a su posición inicial
            timer = 0.0f;
            transform.localPosition = Vector3.Lerp(transform.localPosition, startPosition, Time.deltaTime * 5f);
        }
    }
}