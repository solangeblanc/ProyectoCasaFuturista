using UnityEngine;

public class PlayerModel : MonoBehaviour
{
    [Header("Movimiento")]//crea un titulo para organizar parametros 
    [SerializeField] private float _vel = 5f;//campos para arrastrar en el inspector
    [SerializeField] private float _gravedad = -9.81f;
    [SerializeField] private float _velocidadMaximaCaida = -20f;
    [SerializeField] private float _fuerzaSalto = 0f;

    [Header("Cámara")]
    [SerializeField] private float _sensibilidad = 2f;
    [SerializeField] private float _ClampCam = 80f;

    // Getters y Setters públicos para que el Controlador y la Vista consulten los valores
    //usan la flecha para permitir que el player control y el player view vean los valores sin modificarlos
    public float Vel => _vel;
    public float Gravedad => _gravedad;
    public float VelocidadMaximaCaida => _velocidadMaximaCaida;
    public float FuerzaSalto => _fuerzaSalto;
    public float Sensibilidad => _sensibilidad;
    public float ClampCam => _ClampCam;
}