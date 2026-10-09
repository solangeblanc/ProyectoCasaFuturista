using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    private void Start()
    {
        
        var playerInput = GetComponent<PlayerInput>();
        playerInput.actions.Disable();
        playerInput.actions.FindActionMap("Player").Enable();

        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
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
    public void OnMove(UnityEngine.InputSystem.InputAction.CallbackContext context)
    {
        movimiento = context.ReadValue<Vector2>();
    }

    public void OnLook(UnityEngine.InputSystem.InputAction.CallbackContext context)
    {
        mouse = context.ReadValue<Vector2>();
    }


    // =========================
    // MOVIMIENTO DEL JUGADOR
    // =========================

    private void MoverJugador()
    {
        // 1. Obtenemos la dirección cruda de los inputs (WASD)
        Vector3 direccion = transform.right * movimiento.x + transform.forward * movimiento.y;

        // 2. Normalizamos la dirección para que no corra más rápido en diagonal (opcional pero recomendado)
        direccion.Normalize();

        // 3. ¡CORRECCIÓN AQUÍ! Calculamos el movimiento final multiplicando por la velocidad Y por Time.deltaTime UNA SOLA VEZ.
        // Esto hace que el movimiento sea independiente de los FPS.
        Vector3 movimientoFinal = direccion * _vel * Time.deltaTime;

        // 4. Mantenemos la velocidad vertical (la gravedad) calculada en AplicarGravedad()
        movimientoFinal.y = velocidadVertical;

        // 5. Movemos el Character Controller
        // ¡IMPORTANTE! Aquí ya NO multiplicamos por Time.deltaTime, porque ya lo hicimos en el paso 3.
        _CHC.Move(movimientoFinal);
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
        // Salimos si no tenemos cámara asignada
        if (_camara == null) return;

        // Obtenemos los valores del Input (mouseX y mouseY)
        float mouseX = mouse.x * _sensibilidad;
        float mouseY = mouse.y * _sensibilidad;

        // 1. Rotación horizontal (Gira el jugador entero sobre Y)
        transform.Rotate(Vector3.up * mouseX);

        // 2. Rotación vertical (Gira SOLO la cámara sobre X)
        // IMPORTANTE: Usamos -= para invertir el eje Y y que sea intuitivo (arriba es arriba)
        rotacionX -= mouseY;

        // Limitamos la rotación vertical
        rotacionX = Mathf.Clamp(rotacionX, -_ClampCam, _ClampCam);

        // Aplicamos la rotación a la cámara localmente
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