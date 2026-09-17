using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("Movimiento")]
    [Tooltip("Character Controller")]
    private CharacterController _CHC;

    [Tooltip("Velocidad de movimiento del jugador.")]
    [SerializeField] private float _vel = 5f;


    [Header("Gravedad")]
    [Tooltip("Fuerza de gravedad aplicada al jugador. Valores más negativos hacen que caiga más rápido.")]
    [SerializeField] private float _gravedad = -9.81f;

    [Tooltip("Velocidad máxima a la que puede caer el jugador. Evita que alcance velocidades excesivas.")]
    [SerializeField] private float _velocidadMaximaCaida = -20f;

    [Tooltip("Fuerza con la que el jugador salta. En 0 el salto queda desactivado.")]
    [SerializeField] private float _fuerzaSalto = 0f;


    [Header("Cámara")]
    [Tooltip("Transform de la cámara que se utilizará para visualizar el juego.")]
    [SerializeField] private Transform _camara;

    [Tooltip("Sensibilidad de la cámara al mover el mouse o el control de visión.")][SerializeField] private float _sensibilidad = 2f;

    [Tooltip("Ángulo máximo de rotación vertical que puede realizar la cámara."), SerializeField, Range(0, 120)] private float _ClampCam = 80f;



    [Tooltip("Rotación vertical actual de la cámara.")] private float rotacionX;

    [Tooltip("Utilizada para calcular la gravedad y el salto.")] private float velocidadVertical;

    [Header("INPUTS")]
    [Tooltip("Entrada de movimiento recibida desde el Input System.")]
    private Vector2 movimiento;

    [Tooltip("Entrada utilizada para controlar la rotación de la cámara.")]
    private Vector2 mouse;

    private void Awake()
    {
        _CHC = GetComponent<CharacterController>();
    }
   

    private void Update()
    {
        MoverJugador();
        AplicarGravedad();
        MoverCamara();
    }

    // =========================
    // MOVIMIENTO DEL JUGADOR
    // =========================

    private void MoverJugador()
    {
        // Usamos la variable 'movimiento' que alimentan los mensajes del Input System
        Vector3 direccion = transform.right * movimiento.x + transform.forward * movimiento.y;

        Vector3 movimientoFinal = direccion * _vel;

        movimientoFinal.y = velocidadVertical;

        _CHC.Move(movimientoFinal * Time.deltaTime);
    }

    #region Gravedad

    private void AplicarGravedad()
    {
        //Si esta en el piso no le agrego gavedad pero si se cae o esta en altura si
        if (_CHC.isGrounded)
        {
            // Evita que el jugador quede "flotando" sobre el suelo
            if (velocidadVertical < 0)
            {
                velocidadVertical = -2f;
            }

            // Salto opcional
            if (_fuerzaSalto > 0)
            {
                velocidadVertical = _fuerzaSalto;
            }
        }
        else
        {
            // Aplicamos gravedad
            velocidadVertical += _gravedad * Time.deltaTime;

            // Limitar velocidad de caída para que vaya de 10 al maximo que querramos
            velocidadVertical = Mathf.Max(velocidadVertical, _velocidadMaximaCaida);
        }
    }
    #endregion
    // =========================
    // CÁMARA
    // =========================

    private void MoverCamara()
    {
        // Rotación horizontal del jugador
        transform.Rotate(Vector3.up * mouse.x * _sensibilidad);

        // Rotación vertical de la cámara
        rotacionX -= mouse.y * _sensibilidad;

        // Clampeams la camara en un valor para que no rote de mas y se rompa
        rotacionX = Mathf.Clamp(rotacionX, -_ClampCam, _ClampCam);

        _camara.localRotation = Quaternion.Euler(rotacionX, 0, 0);
    }

    // =========================
    // INPUT SYSTEM Esta zona es donde vamos a llamar todos los inputs del input manager para que cuando apretemos las teclas
    // =========================

    public void OnMove(InputValue value)
    {
        //WASD
        movimiento = value.Get<Vector2>();
    }

    public void OnLook(InputValue value)
    {
        //Mouse
        mouse = value.Get<Vector2>();
    }
}
