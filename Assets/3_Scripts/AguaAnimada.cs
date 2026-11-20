using UnityEngine;

// Hace que el agua parezca moverse: desplaza despacio las texturas del material (las ondas del normal map).
// Va en el objeto del agua (el plano o cubo). Funciona con el material URP/Lit.
// Usa una copia propia del material, así que no cambia el material original del proyecto.
public class AguaAnimada : MonoBehaviour
{
    [Header("Velocidad de las ondas (ciclos por segundo)")]
    [Tooltip("Valores chicos, por ejemplo 0.01 a 0.05. Cuanto más alto, más rápido corre el agua.")]
    [SerializeField] private Vector2 velocidad = new Vector2(0.02f, 0.015f);

    [Header("Tamaño de las ondas")]
    [Tooltip("Cuántas veces se repite la textura sobre el agua. Más alto = ondas más chicas.")]
    [SerializeField] private Vector2 repeticiones = new Vector2(8f, 8f);

    private Material material;

    private void Start()
    {
        Renderer r = GetComponent<Renderer>();
        if (r == null)
        {
            Debug.LogWarning("AguaAnimada: este objeto no tiene Renderer. Ponelo en el objeto del agua.", this);
            enabled = false;
            return;
        }

        // .material crea una copia propia para este objeto.
        material = r.material;
        material.SetTextureScale("_BaseMap", repeticiones);
    }

    private void Update()
    {
        // Mathf.Repeat mantiene el valor entre 0 y 1 para que no pierda precisión con el tiempo.
        Vector2 desplazamiento = new Vector2(
            Mathf.Repeat(Time.time * velocidad.x, 1f),
            Mathf.Repeat(Time.time * velocidad.y, 1f));

        // En URP/Lit, el desplazamiento de _BaseMap se aplica también al normal map.
        material.SetTextureOffset("_BaseMap", desplazamiento);
    }
}
