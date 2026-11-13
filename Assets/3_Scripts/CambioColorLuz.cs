using UnityEngine;

public class CambioColorLuz : MonoBehaviour
{
    private Light luz;

    [Header("Lista de colores para la luz")]
    [SerializeField]
    private Color[] colores = new Color[]
    {
        new Color(0f, 1f, 0.3f), // Verde neón inicial
        new Color(0f, 0.6f, 1f), // Azul cian
        new Color(0.7f, 0f, 1f)  // Violeta futurista
    };

    [Header("Velocidad de transición entre colores")]
    [SerializeField] private float velocidadCambio = 0.5f;

    private int indiceActual = 0;
    private float tiempoTransicion = 0f;

    void Start()
    {
        luz = GetComponent<Light>();
        if (luz != null && colores.Length > 0)
        {
            luz.color = colores[0];
        }
    }

    void Update()
    {
        if (luz == null || colores.Length == 0) return;

        // Avanza el tiempo de transición
        tiempoTransicion += Time.deltaTime * velocidadCambio;

        // Si llega al final, pasa al siguiente color de la lista de forma circular
        if (tiempoTransicion >= 1f)
        {
            tiempoTransicion = 0f;
            indiceActual = (indiceActual + 1) % colores.Length;
        }

        // Calcula el color actual y el siguiente objetivo
        Color colorOrigen = colores[indiceActual];
        Color colorDestino = colores[(indiceActual + 1) % colores.Length];

        // Mezcla suavemente ambos colores en tiempo real
        luz.color = Color.Lerp(colorOrigen, colorDestino, tiempoTransicion);
    }
}