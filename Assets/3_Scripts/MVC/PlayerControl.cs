using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerModel))]
[RequireComponent(typeof(PlayerView))]
public class PlayerControl : MonoBehaviour
{
    private PlayerModel _model;
    private PlayerView _view;

    private Vector2 _movimientoInput;
    private Vector2 _mouseInput;
    private float _velocidadVertical;

    private void Awake()
    {
        _model = GetComponent<PlayerModel>();
        _view = GetComponent<PlayerView>();
    }

    private void Start()
    {
        var playerInput = GetComponent<PlayerInput>();
        if (playerInput != null && playerInput.actions != null)
        {
            playerInput.actions.Disable();
            var map = playerInput.actions.FindActionMap("Player");
            if (map != null) map.Enable();
        }
    }

    private void Update()
    {
        // Si el GameManager pausa el juego o desactiva al jugador, puedes omitir la lógica aquí
        ProcesarMovimientoYGravedad();
        ProcesarCamara();
    }

    private void ProcesarMovimientoYGravedad()
    {
        // 1. Calcular gravedad / salto
        if (_view.EstaEnSuelo())
        {
            if (_velocidadVertical < 0)
            {
                _velocidadVertical = -2f;
            }

            if (_model.FuerzaSalto > 0)
            {
                _velocidadVertical = _model.FuerzaSalto;
            }
        }
        else
        {
            _velocidadVertical += _model.Gravedad * Time.deltaTime;
            _velocidadVertical = Mathf.Max(_velocidadVertical, _model.VelocidadMaximaCaida);
        }

        // 2. Calcular movimiento horizontal
        Vector3 direccion = transform.right * _movimientoInput.x + transform.forward * _movimientoInput.y;
        direccion.Normalize();

        Vector3 movimientoFinal = direccion * _model.Vel * Time.deltaTime;
        movimientoFinal.y = _velocidadVertical;

        // 3. Enviar a la Vista para que mueva el CharacterController
        _view.Mover(movimientoFinal);
    }

    private void ProcesarCamara()
    {
        _view.MoverCamara(_mouseInput.x, _mouseInput.y, _model.Sensibilidad, _model.ClampCam);
    }

    // Métodos del Input System (Old / New SendMessage)
    public void OnMove(InputValue value)
    {
        _movimientoInput = value.Get<Vector2>();
    }

    public void OnLook(InputValue value)
    {
        _mouseInput = value.Get<Vector2>();
    }

    // Por si usas CallbackContext directo
    public void OnMove(InputAction.CallbackContext context)
    {
        _movimientoInput = context.ReadValue<Vector2>();
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        _mouseInput = context.ReadValue<Vector2>();
    }
}