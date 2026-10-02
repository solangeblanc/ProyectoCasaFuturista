using UnityEngine;

public class CartelEscalera : MonoBehaviour
{
    [Header("Canvas")]
    [SerializeField] private GameObject canvasCartel;

    [Header("Seguridad de Distancia")]
    [SerializeField] private Transform jugadorTransform; // Arrastra aquí al jugador o se busca solo
    [SerializeField] private float distanciaMaxima = 3f;  // Si te alejas más de esto, el cartel se apaga a la fuerza

    private bool jugadorCerca = false;

    private void Start()
    {
        if (canvasCartel != null)
            canvasCartel.SetActive(false);

        // Si no asignaste al jugador, lo busca automáticamente por la etiqueta "Player"
        if (jugadorTransform == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
                jugadorTransform = playerObj.transform;
        }
    }

    private void Update()
    {
        // Si el cartel está activo o el jugador estaba cerca, revisamos la distancia constantemente
        if (jugadorTransform != null)
        {
            float distancia = Vector3.Distance(transform.position, jugadorTransform.position);

            // Si el jugador se teletransportó o se alejó más de la distancia permitida
            if (distancia > distanciaMaxima && jugadorCerca)
            {
                jugadorCerca = false;
                if (canvasCartel != null)
                    canvasCartel.SetActive(false);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            jugadorCerca = true;
            jugadorTransform = other.transform;

            if (canvasCartel != null)
                canvasCartel.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            jugadorCerca = false;

            if (canvasCartel != null)
                canvasCartel.SetActive(false);
        }
    }
}