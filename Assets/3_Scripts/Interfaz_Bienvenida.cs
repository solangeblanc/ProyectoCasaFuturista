using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

public class MenuPrincipal : MonoBehaviour
{
    private void Start()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;

        Button BotonIniciar = root.Q<Button>("BotonIniciar");

        BotonIniciar.clicked += () =>
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene("1_Simulador");
        };

        Button BotonCreditos = root.Q<Button>("BotonCreditos");

        BotonCreditos.clicked += () =>
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene("2_Creditos");
        };
        Button botonSalir = root.Q<Button>("BotonSalir");

        botonSalir.clicked += () =>
        {
            Application.Quit();
        };
    }
}