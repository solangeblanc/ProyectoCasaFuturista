using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerModel))]// asegura que este game object tenga los componentes player model y player view
[RequireComponent(typeof(PlayerView))]
public class PlayerControl : MonoBehaviour
{
    private PlayerModel _model;// referencias a los componentes de modelo y vista
    private PlayerView _view;

    private Vector2 _movimientoInput;// variables para almacenar las entradas (inputs) mouse y teclado
    private Vector2 _mouseInput;
    private float _velocidadVertical;// controla la fisica de caida y de salto

    private void Awake()
    {
        _model = GetComponent<PlayerModel>();//obtiene las referencias del modelo y de la vista al despertar el objeto 
        _view = GetComponent<PlayerView>();
    }

    private void Start()// metodo que se ejecuta automaticamente al iniciar la partida
    {
        var playerInput = GetComponent<PlayerInput>();
        if (playerInput != null && playerInput.actions != null)// configuracion inicial del input system de unity 
        {
            playerInput.actions.Disable();// desactiva todas las acciones por defecto para evitar conflictos
            var map = playerInput.actions.FindActionMap("Player");//busca y habilita unicamente el mapa de acciones del player
            if (map != null) map.Enable();
        }
    }

    private void Update()
    {
        // Se ejecuta en cada fotograma: procesa la física/desplazamiento y el movimiento de la cámara
        ProcesarMovimientoYGravedad();
        ProcesarCamara();
    }

    private void ProcesarMovimientoYGravedad()
    {
        // 1. Calcular gravedad / salto
        if (_view.EstaEnSuelo())
        // Si está en el suelo y cae levemente, fija una pequeña fuerza hacia abajo para mantenerlo pegado al piso
        {
            if (_velocidadVertical < 0)
            {
                _velocidadVertical = -2f;
            }

            if (_model.FuerzaSalto > 0)
       //la fuerza de salto se ejecuta automaticamente si es mayor a cero
            {
                _velocidadVertical = _model.FuerzaSalto;
            }
        }
        else
        {
            _velocidadVertical += _model.Gravedad * Time.deltaTime;
            // si esta en el aire aplica la gravedad acumulativa por el tiempo transcurrido
            _velocidadVertical = Mathf.Max(_velocidadVertical, _model.VelocidadMaximaCaida);
            //limita la velocidad maxima de caida para que no acelere infinitamente 
        }

        // 2. Calcular movimiento horizontal
        Vector3 direccion = transform.right * _movimientoInput.x + transform.forward * _movimientoInput.y;
        direccion.Normalize();

        Vector3 movimientoFinal = direccion * _model.Vel * Time.deltaTime;
        movimientoFinal.y = _velocidadVertical;//añade el componente vertical velocidad salto

        // 3. Enviar a la Vista para que mueva el CharacterController
        _view.Mover(movimientoFinal);
    }

    private void ProcesarCamara()
    {//envia los valores del raton a la camara para poder rotarla
        _view.MoverCamara(_mouseInput.x, _mouseInput.y, _model.Sensibilidad, _model.ClampCam);
    }

    // Métodos del Input System 
    public void OnMove(InputValue value)//captura las teclas para el movimiento del player
    {
        _movimientoInput = value.Get<Vector2>();
    }

    public void OnLook(InputValue value)//captura el movimiento del raton 
    {
        _mouseInput = value.Get<Vector2>();
    }

    // Por si usas CallbackContext directo metodos alternativos por si se usa imput assets 
    public void OnMove(InputAction.CallbackContext context)
    {
        _movimientoInput = context.ReadValue<Vector2>();
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        _mouseInput = context.ReadValue<Vector2>();
    }
}