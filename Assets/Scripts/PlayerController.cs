using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("Movimiento")]
    [Tooltip("Character Controller")]
    private CharacterController _CHC;

    [Tooltip("Velocidad de movimiento del jugador.")]
    [SerializeField] private float _vel = 7f;


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

    [Tooltip("Sensibilidad de la cámara al mover el mouse o el control de visión.")]
    [SerializeField] private float _sensibilidad = 1f;

    [Tooltip("Ángulo máximo de rotación vertical que puede realizar la cámara."), SerializeField, Range(0, 120)] private float _ClampCam = 80f;

    [Tooltip("Rotación vertical actual de la cámara.")] private float rotacionX;
    [Tooltip("Utilizada para calcular la gravedad y el salto.")] private float velocidadVertical;


    [Header("INPUTS")]
    [Tooltip("Entrada de movimiento recibida desde el Input System.")]
    private Vector2 movimiento;

    [Tooltip("Entrada utilizada para controlar la rotación de la cámara.")]
    private Vector2 mouse;


    [Header("Cambio de Color de Piso")]
    [SerializeField] private Material[] materialesPiso;
    private int indiceMaterial = 0;


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
    // CAMBIO DE COLOR DE PISO
    // =========================

    public void OnCambiarPiso(InputValue value)
    {
        if (value.isPressed)
        {
            Ray ray = new Ray(_camara.position, _camara.forward);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                Renderer pisoRenderer = hit.collider.GetComponent<Renderer>();
                if (pisoRenderer != null)
                {
                    // Asignamos el material
                    pisoRenderer.material = materialesPiso[indiceMaterial];

                    // Forzamos el Tiling (escala) y Offset en los shaders modernos de Unity
                    pisoRenderer.material.SetTextureScale("_BaseMap", new Vector2(5f, 5f));
                    pisoRenderer.material.SetTextureScale("_MainTex", new Vector2(5f, 5f));

                    indiceMaterial = (indiceMaterial + 1) % materialesPiso.Length;
                }
            }
        }
    }


    // =========================
    // MOVIMIENTO DEL JUGADOR
    // =========================

    private void MoverJugador()
    {
        Vector3 direccion = transform.right * movimiento.x + transform.forward * movimiento.y;
        Vector3 movimientoFinal = direccion * _vel;
        movimientoFinal.y = velocidadVertical;

        _CHC.Move(movimientoFinal * Time.deltaTime);
    }


    #region Gravedad

    private void AplicarGravedad()
    {
        if (_CHC.isGrounded)
        {
            if (velocidadVertical < 0)
            {
                velocidadVertical = -2f;
            }

            if (_fuerzaSalto > 0)
            {
                velocidadVertical = _fuerzaSalto;
            }
        }
        else
        {
            velocidadVertical += _gravedad * Time.deltaTime;
            velocidadVertical = Mathf.Max(velocidadVertical, _velocidadMaximaCaida);
        }
    }
    #endregion


    // =========================
    // CÁMARA
    // =========================

    private void MoverCamara()
    {
        transform.Rotate(Vector3.up * mouse.x * _sensibilidad);

        rotacionX -= mouse.y * _sensibilidad;
        rotacionX = Mathf.Clamp(rotacionX, -_ClampCam, _ClampCam);

        _camara.localRotation = Quaternion.Euler(rotacionX, 0, 0);
    }


    // =========================
    // INPUT SYSTEM
    // =========================

    public void OnMove(InputValue value)
    {
        movimiento = value.Get<Vector2>();
    }

    public void OnLook(InputValue value)
    {
        mouse = value.Get<Vector2>();
    }
}