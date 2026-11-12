using UnityEngine;

[RequireComponent(typeof(CharacterController))]//asegura que este game object tenga obligatoriamente el caracter controler
public class PlayerView : MonoBehaviour
{
    private CharacterController _CHC;//etiqueta para recordar el componente que mueve al jugador 
    // caracter controller lo que le permite moverse y avanzar sin caer al vacio 
    [SerializeField] private Transform _camara;// se guarda la camara para poder moverla 

    private float rotacionX; //variable para anotar cuanto se mueve la cabeza de arriba hacia abajo 

    private void Awake() // metodo que comienza apenas inicia el juego 
    {
        _CHC = GetComponent<CharacterController>();
        //buscamos y guardamos el componente que permite caminar
        if (_camara == null)
        {
            // Intenta buscar la cámara en los hijos si no está asignada
            Camera cam = GetComponentInChildren<Camera>();
            if (cam != null) _camara = cam.transform;
        }
    }

    public void Mover(Vector3 movimientoFinal)
        // funcion para caminar, recibe la direccion y hace que el personaje se mueva
    {
        if (_CHC != null)
        {
            _CHC.Move(movimientoFinal);
        }
    }

    public bool EstaEnSuelo()
    {
        return _CHC != null && _CHC.isGrounded;
        // revisa si los pies del jugador estan tocando el suelo deuelve verdadero o falso 
    }

    public void MoverCamara(float mouseX, float mouseY, float sensibilidad, float clampCam)
        //controla hacia donde mira el jugador cuando se mueve el raton
    {
        if (_camara == null) return;

        // 1. Rotación horizontal (Gira el jugador entero sobre Y)
        transform.Rotate(Vector3.up * (mouseX * sensibilidad));

        // 2. Rotación vertical (Gira SOLO la cámara sobre X)
        rotacionX -= mouseY * sensibilidad;
        rotacionX = Mathf.Clamp(rotacionX, -clampCam, clampCam);

        _camara.localRotation = Quaternion.Euler(rotacionX, 0, 0);
    }
}