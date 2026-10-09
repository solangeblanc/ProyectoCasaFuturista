using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

public class Creditos : MonoBehaviour
{
    private void Start()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;

        Button BotonVolver = root.Q<Button>("BotonVolver");

        BotonVolver.clicked += () =>
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene("0_Bienvenida");
        };
    }
}