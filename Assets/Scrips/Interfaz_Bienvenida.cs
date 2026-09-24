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
            SceneManager.LoadScene("1_Simulador");
        };
    }
}