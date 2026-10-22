using UnityEngine;

public class CartelProximidad : MonoBehaviour
{
    [Header("Dónde aparece")]
    [Tooltip("Objeto del cuarto (por ejemplo sistemaSolar o una batería)")]
    [SerializeField] private Transform punto;
    [SerializeField] private float radio = 4f;
    [Tooltip("Diferencia de altura máxima (para no activarse desde otro piso)")]
    [SerializeField] private float alturaMaxima = 3f;
    [Tooltip("Si queda vacío usa la cámara con tag MainCamera")]
    [SerializeField] private Transform jugador;

    [Header("Cartel")]
    [SerializeField] private GameObject cartel;
    [Tooltip("Segundos visible. 0 = mientras estés cerca")]
    [SerializeField] private float segundosVisible = 0f;

    private bool estabaCerca = false;
    private bool agotado = false;
    private float tiempoMostrado = 0f;

    private void Start()
    {
        if (jugador == null)
        {
            if (Camera.main != null) jugador = Camera.main.transform;
        }

        Mostrar(false);
    }

    private void Update()
    {
        GameManager gm = GameManager.Instance;

        if (gm == null || !gm.EstaJugando || punto == null || jugador == null)
        {
            Mostrar(false);
            return;
        }

        Vector3 a = jugador.position;
        Vector3 b = punto.position;
        float dy = Mathf.Abs(a.y - b.y);
        a.y = 0f;
        b.y = 0f;

        bool cerca = Vector3.Distance(a, b) <= radio && dy <= alturaMaxima;

        if (cerca && !estabaCerca)
        {
            tiempoMostrado = 0f;
            agotado = false;
        }
        estabaCerca = cerca;

        bool visible = cerca && !agotado;

        if (visible && segundosVisible > 0f)
        {
            tiempoMostrado += Time.unscaledDeltaTime;
            if (tiempoMostrado >= segundosVisible)
            {
                agotado = true;
                visible = false;
            }
        }

        Mostrar(visible);
    }

    private void Mostrar(bool mostrar)
    {
        if (cartel == null) return;
        if (cartel.activeSelf == mostrar) return;
        cartel.SetActive(mostrar);
    }

    private void OnDrawGizmosSelected()
    {
        if (punto == null) return;
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(punto.position, radio);
    }
}
