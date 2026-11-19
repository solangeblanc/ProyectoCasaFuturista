using UnityEngine;

// Ícono flotante "!" que señala un objeto con el que el jugador todavía no interactuó.
// Mira siempre a la cámara, pulsa suavemente, y se desvanece cuando la función ya fue descubierta.
// Se pone en un objeto vacío ubicado sobre lo que se quiere señalar.
public class MarcadorDescubrimiento : MonoBehaviour
{
    [Header("Qué función señala (mismo id que en el registro)")]
    [SerializeField] private string idDescubrimiento = "luces";

    [Header("Aspecto")]
    [Tooltip("Tamaño del ícono en unidades de Unity")]
    [SerializeField] private float tamano = 14f;
    [SerializeField] private Color color = new Color(0.25f, 0.85f, 1f);
    [SerializeField] private float velocidadPulso = 2.5f;
    [Tooltip("Cuánto sube y baja flotando")]
    [SerializeField] private float amplitudFlote = 2f;

    [Header("Cuándo se ve")]
    [Tooltip("Más lejos que esto, el ícono se desvanece")]
    [SerializeField] private float distanciaVisible = 150f;
    [Tooltip("Más cerca que esto, el ícono se desvanece para no tapar la pantalla")]
    [SerializeField] private float distanciaMinima = 25f;

    private static Sprite spriteCompartido;
    private Transform icono;
    private SpriteRenderer render;
    private Transform camara;
    private float visibilidad;
    private float escalaPadre = 1f;

    private void Start()
    {
        // Crea el ícono como hijo de este objeto.
        GameObject go = new GameObject("Icono");
        go.transform.SetParent(transform, false);
        icono = go.transform;
        render = go.AddComponent<SpriteRenderer>();
        render.sprite = ObtenerSprite();
        render.sortingOrder = 100;
        render.enabled = false;

        // Compensa la escala del padre para que el tamaño sea el indicado.
        escalaPadre = Mathf.Max(0.0001f, transform.lossyScale.x);
    }

    private void LateUpdate()
    {
        if (camara == null)
        {
            if (Camera.main == null) return;
            camara = Camera.main.transform;
        }

        GameManager gm = GameManager.Instance;
        bool jugando = gm != null && gm.EstaJugando;
        bool descubierto = RegistroDescubrimientos.Instance != null
                           && RegistroDescubrimientos.Instance.EstaDescubierta(idDescubrimiento);

        // Visibilidad objetivo: 0 si no se juega o ya se descubrió; si no, depende de la distancia.
        float objetivo = 0f;
        if (jugando && !descubierto)
        {
            float dist = Vector3.Distance(camara.position, transform.position);
            float lejos = 1f - Mathf.InverseLerp(distanciaVisible * 0.7f, distanciaVisible, dist);
            float cerca = Mathf.InverseLerp(distanciaMinima * 0.5f, distanciaMinima, dist);
            objetivo = Mathf.Clamp01(Mathf.Min(lejos, cerca));
        }

        // Aparece y desaparece con suavidad.
        visibilidad = Mathf.MoveTowards(visibilidad, objetivo, 2f * Time.unscaledDeltaTime);
        render.enabled = visibilidad > 0.01f;
        if (!render.enabled) return;

        // Siempre de frente a la cámara.
        icono.rotation = camara.rotation;

        // Pulso de tamaño y leve flote vertical.
        float pulso = 1f + 0.12f * Mathf.Sin(Time.time * velocidadPulso);
        icono.localScale = Vector3.one * (tamano / escalaPadre) * pulso * Mathf.Lerp(0.6f, 1f, visibilidad);
        icono.position = transform.position + Vector3.up * (Mathf.Sin(Time.time * velocidadPulso * 0.5f) * amplitudFlote);

        Color c = color;
        c.a = visibilidad * 0.95f;
        render.color = c;
    }

    // Dibuja el ícono (círculo con un "!") por código, una sola vez para todos los marcadores.
    // La imagen es gris para poder teñirla con el color elegido.
    private static Sprite ObtenerSprite()
    {
        if (spriteCompartido != null) return spriteCompartido;

        int n = 128;
        Texture2D tex = new Texture2D(n, n, TextureFormat.RGBA32, true);
        tex.wrapMode = TextureWrapMode.Clamp;
        Color[] px = new Color[n * n];
        for (int y = 0; y < n; y++)
        {
            for (int x = 0; x < n; x++)
            {
                Vector2 p = new Vector2((x + 0.5f) / n - 0.5f, (y + 0.5f) / n - 0.5f);
                float d = p.magnitude;

                float disco = Mathf.Clamp01((0.48f - d) * n + 0.5f);                 // círculo
                float borde = Mathf.Clamp01((d - 0.40f) * n + 0.5f);                 // aro claro en el borde
                float barra = Mathf.Clamp01(0.5f - DistRectRedondeado(p, new Vector2(0f, 0.08f), new Vector2(0.05f, 0.19f), 0.05f) * n);
                float punto = Mathf.Clamp01((0.065f - Vector2.Distance(p, new Vector2(0f, -0.2f))) * n + 0.5f);
                float signo = Mathf.Max(barra, punto);

                float v = Mathf.Lerp(0.92f, 1f, borde);   // fondo del círculo
                v = Mathf.Lerp(v, 0.12f, signo);          // "!" oscuro
                px[y * n + x] = new Color(v, v, v, disco);
            }
        }
        tex.SetPixels(px);
        tex.Apply();
        spriteCompartido = Sprite.Create(tex, new Rect(0, 0, n, n), new Vector2(0.5f, 0.5f), n);
        return spriteCompartido;
    }

    // Distancia con signo a un rectángulo de esquinas redondeadas (negativa adentro).
    private static float DistRectRedondeado(Vector2 p, Vector2 centro, Vector2 mitad, float radio)
    {
        Vector2 q = new Vector2(Mathf.Abs(p.x - centro.x), Mathf.Abs(p.y - centro.y)) - (mitad - new Vector2(radio, radio));
        return Vector2.Max(q, Vector2.zero).magnitude + Mathf.Min(Mathf.Max(q.x, q.y), 0f) - radio;
    }

    // Solo en el Editor, con el objeto seleccionado: muestra hasta dónde se ve el marcador.
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, distanciaVisible);
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, distanciaMinima);
    }
}