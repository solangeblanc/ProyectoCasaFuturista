using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

public class MenuPrincipal : MonoBehaviour
{
    private void Start()//metodo que se ejecuta automaticamente una vez , cuando inicia el juego
    {
        var root = GetComponent<UIDocument>().rootVisualElement;//obtiene el elemento raiz del documento de interfaz de usuario UI document

        Button BotonIniciar = root.Q<Button>("BotonIniciar");// Busca el boton iniciar por su nombre exacto en el archivo UXML

        BotonIniciar.clicked += () =>//configura la accion al hacer click en iniciar 
        {
            Time.timeScale = 1f;//restaura la velocidad del jugador por si el juego estaba pausado
            SceneManager.LoadScene("1_Simulador");// cambia la escena 
        };

        Button BotonCreditos = root.Q<Button>("BotonCreditos");//busca el boton creditos

        BotonCreditos.clicked += () =>// configura la accion al hacer click en creditos
        {
            Time.timeScale = 1f;// descongela el juego
            SceneManager.LoadScene("2_Creditos");// carga la escena
        };
        Button botonSalir = root.Q<Button>("BotonSalir");

        botonSalir.clicked += () =>
        {
            Application.Quit();
        };
    }
}