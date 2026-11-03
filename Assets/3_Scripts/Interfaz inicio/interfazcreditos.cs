using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

public class Creditos : MonoBehaviour
{
    private void Start()// metodo que se ejecuta una sola vez automaticamente inicia el juego
    {
        var root = GetComponent<UIDocument>().rootVisualElement;//obtiene el elemento raiz del documento de interfaz de usuario

        Button BotonVolver = root.Q<Button>("BotonVolver");// busca el boton por su nombre 

        BotonVolver.clicked += () =>//configura la accion al hacer click en volver 
        {
            Time.timeScale = 1f;// descongela el juego
            SceneManager.LoadScene("0_Bienvenida");//cambia de escena 
        };
    }
}