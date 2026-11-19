using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

// Interfaz de descubrimientos, construida por código al empezar:
// - Un contador arriba a la derecha ("Funciones 2/7 · Tab").
// - Una lista que se abre y cierra con una tecla (Tab por defecto).
// - Un aviso emergente abajo cuando el jugador descubre algo.
// RequireComponent agrega solo el RegistroDescubrimientos al mismo objeto.
[RequireComponent(typeof(RegistroDescubrimientos))]
public class PanelDescubrimientos : MonoBehaviour
{
    [Header("Tecla")]
    [SerializeField] private Key teclaPanel = Key.Tab;

    [Header("Colores")]
    [SerializeField] private Color colorFondo = new Color(0.06f, 0.08f, 0.12f, 0.9f);
    [SerializeField] private Color colorAcento = new Color(0.25f, 0.85f, 1f);
    [SerializeField] private Color colorDescubierto = new Color(0.4f, 0.9f, 0.5f);
    [SerializeField] private Color colorPendiente = new Color(0.6f, 0.65f, 0.72f);

    [Header("Avisos")]
    [SerializeField] private float segundosAviso = 3.5f;
    [Tooltip("Sonido corto al descubrir algo (opcional)")]
    [SerializeField] private AudioClip sonidoDescubrimiento;
    [Range(0f, 1f)]
    [SerializeField] private float volumenSonido = 0.7f;

    // Los elementos de una fila de la lista.
    private class Fila
    {
        public string id;
        public Image icono;
        public TMP_Text nombre;
        public TMP_Text pista;
    }

    private RegistroDescubrimientos registro;
    private CanvasGroup grupoRaiz, grupoPanel, grupoAviso;
    private TMP_Text textoContador, textoAviso;
    private readonly List<Fila> filas = new List<Fila>();
    private AudioSource audioAviso;
    private Sprite spriteRedondeado, spriteCirculo, spriteAnillo;
    private bool panelAbierto;
    private bool avisoInicialMostrado;
    private float tiempoAviso;

    private void Start()
    {
        registro = RegistroDescubrimientos.Instance;
        if (registro == null)
        {
            enabled = false;
            return;
        }

        // Imágenes generadas por código (no hace falta importar nada).
        spriteRedondeado = CrearSpriteRedondeado(64, 18);
        spriteCirculo = CrearSpriteCirculo(64, false);
        spriteAnillo = CrearSpriteCirculo(64, true);

        audioAviso = gameObject.AddComponent<AudioSource>();
        audioAviso.playOnAwake = false;
        audioAviso.spatialBlend = 0f;

        ConstruirInterfaz();
        registro.AlDescubrir += AlDescubrirFuncion;
        Refrescar();
    }

    private void OnDestroy()
    {
        if (registro != null) registro.AlDescubrir -= AlDescubrirFuncion;
    }

    private void Update()
    {
        // La interfaz solo se ve mientras se juega (no en el menú ni en pausa).
        GameManager gm = GameManager.Instance;
        bool jugando = gm != null && gm.EstaJugando;
        grupoRaiz.alpha = jugando ? 1f : 0f;
        if (!jugando) return;

        // Al empezar a jugar, avisa una sola vez que existe la lista.
        if (!avisoInicialMostrado)
        {
            avisoInicialMostrado = true;
            string hex = ColorUtility.ToHtmlStringRGB(colorAcento);
            MostrarAviso("Presioná <color=#" + hex + ">Tab</color> para ver las funciones de la casa");
        }

        // Abre o cierra la lista.
        if (Keyboard.current != null && Keyboard.current[teclaPanel].wasPressedThisFrame)
            panelAbierto = !panelAbierto;

        // Aparece y desaparece con un fundido suave.
        grupoPanel.alpha = Mathf.MoveTowards(grupoPanel.alpha, panelAbierto ? 1f : 0f, 6f * Time.unscaledDeltaTime);

        if (tiempoAviso > 0f) tiempoAviso -= Time.unscaledDeltaTime;
        grupoAviso.alpha = Mathf.MoveTowards(grupoAviso.alpha, tiempoAviso > 0f ? 1f : 0f, 4f * Time.unscaledDeltaTime);
    }

    // ---------- Eventos ----------
    private void AlDescubrirFuncion(string id)
    {
        Refrescar();

        // Busca la función para mostrar su nombre.
        RegistroDescubrimientos.Funcion funcion = null;
        foreach (RegistroDescubrimientos.Funcion f in registro.Funciones)
        {
            if (f.id == id) { funcion = f; break; }
        }
        if (funcion == null) return;

        string hex = ColorUtility.ToHtmlStringRGB(colorAcento);
        if (registro.CantidadDescubiertas >= registro.Total)
            MostrarAviso("¡Descubriste todas las funciones de la casa!");
        else
            MostrarAviso("¡Descubriste: <color=#" + hex + ">" + funcion.nombre + "</color>!");

        if (sonidoDescubrimiento != null)
            audioAviso.PlayOneShot(sonidoDescubrimiento, volumenSonido);
    }

    private void MostrarAviso(string mensaje)
    {
        textoAviso.text = mensaje;
        tiempoAviso = segundosAviso;
    }

    // Actualiza los íconos, textos y el contador según lo descubierto.
    private void Refrescar()
    {
        foreach (Fila f in filas)
        {
            bool d = registro.EstaDescubierta(f.id);
            f.icono.sprite = d ? spriteCirculo : spriteAnillo;
            f.icono.color = d ? colorDescubierto : colorPendiente;
            f.pista.color = d ? colorDescubierto : colorPendiente;
            if (d) f.pista.text = "Descubierta";
        }

        int hechas = registro.CantidadDescubiertas;
        int total = registro.Total;
        bool completo = hechas >= total;
        textoContador.color = completo ? colorDescubierto : colorAcento;
        textoContador.text = completo ? "¡Todo descubierto!" : "Funciones " + hechas + "/" + total + "  ·  Tab";
    }

    // ---------- Construcción de la interfaz ----------
    private void ConstruirInterfaz()
    {
        // Canvas propio, por encima de la escena.
        GameObject goCanvas = new GameObject("CanvasDescubrimientos");
        goCanvas.transform.SetParent(transform, false);
        Canvas canvas = goCanvas.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 5;
        CanvasScaler escalador = goCanvas.AddComponent<CanvasScaler>();
        escalador.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        escalador.referenceResolution = new Vector2(1920f, 1080f);
        escalador.matchWidthOrHeight = 0.5f;
        grupoRaiz = goCanvas.AddComponent<CanvasGroup>();
        grupoRaiz.interactable = false;
        grupoRaiz.blocksRaycasts = false;
        grupoRaiz.alpha = 0f;

        // Contador arriba a la derecha.
        RectTransform chip = CrearImagen("Contador", goCanvas.transform, spriteRedondeado, colorFondo);
        Anclar(chip, new Vector2(1f, 1f), new Vector2(1f, 1f), new Vector2(1f, 1f), new Vector2(-30f, -30f), new Vector2(360f, 64f));
        textoContador = CrearTexto("Texto", chip, 26f, FontStyles.Bold, colorAcento, TextAlignmentOptions.Center);
        Estirar(textoContador.rectTransform);

        // Panel de la lista, a la derecha y centrado en altura.
        int cantidad = registro.Funciones.Count;
        float anchoPanel = 640f, altoCabecera = 120f, margen = 28f;
        // Con muchas funciones achica las filas para que el panel siempre entre en la pantalla.
        float altoFila = Mathf.Min(78f, (960f - altoCabecera - margen) / cantidad);
        // Proporción para achicar también los textos de cada fila.
        float escala = altoFila / 78f;
        float altoPanel = altoCabecera + cantidad * altoFila + margen;

        RectTransform panel = CrearImagen("PanelFunciones", goCanvas.transform, spriteRedondeado, colorFondo);
        Anclar(panel, new Vector2(1f, 0.5f), new Vector2(1f, 0.5f), new Vector2(1f, 0.5f), new Vector2(-30f, 0f), new Vector2(anchoPanel, altoPanel));
        grupoPanel = panel.gameObject.AddComponent<CanvasGroup>();
        grupoPanel.alpha = 0f;

        TMP_Text titulo = CrearTexto("Titulo", panel, 38f, FontStyles.Bold, Color.white, TextAlignmentOptions.TopLeft);
        titulo.text = "Funciones de la casa";
        Anclar(titulo.rectTransform, new Vector2(0f, 1f), new Vector2(1f, 1f), new Vector2(0f, 1f), new Vector2(margen, -22f), new Vector2(-2f * margen, 50f));

        TMP_Text subtitulo = CrearTexto("Subtitulo", panel, 22f, FontStyles.Normal, colorPendiente, TextAlignmentOptions.TopLeft);
        subtitulo.text = "Probá todo para completar la lista";
        Anclar(subtitulo.rectTransform, new Vector2(0f, 1f), new Vector2(1f, 1f), new Vector2(0f, 1f), new Vector2(margen, -70f), new Vector2(-2f * margen, 34f));

        // Una fila por cada función.
        for (int i = 0; i < cantidad; i++)
        {
            RegistroDescubrimientos.Funcion f = registro.Funciones[i];

            RectTransform fila = new GameObject("Fila_" + f.id, typeof(RectTransform)).GetComponent<RectTransform>();
            fila.SetParent(panel, false);
            Anclar(fila, new Vector2(0f, 1f), new Vector2(1f, 1f), new Vector2(0f, 1f),
                   new Vector2(margen, -(altoCabecera + i * altoFila)), new Vector2(-2f * margen, altoFila - 10f));

            RectTransform icono = CrearImagen("Icono", fila, spriteAnillo, colorPendiente);
            Anclar(icono, new Vector2(0f, 0.5f), new Vector2(0f, 0.5f), new Vector2(0f, 0.5f), Vector2.zero, new Vector2(34f, 34f));

            // Nombre en la mitad de arriba y pista en la de abajo.
            TMP_Text nombre = CrearTexto("Nombre", fila, 30f * escala, FontStyles.Bold, Color.white, TextAlignmentOptions.BottomLeft);
            nombre.text = f.nombre;
            nombre.rectTransform.anchorMin = new Vector2(0f, 0.5f);
            nombre.rectTransform.anchorMax = new Vector2(1f, 1f);
            nombre.rectTransform.offsetMin = new Vector2(56f, 0f);
            nombre.rectTransform.offsetMax = Vector2.zero;

            TMP_Text pista = CrearTexto("Pista", fila, 21f * escala, FontStyles.Normal, colorPendiente, TextAlignmentOptions.TopLeft);
            pista.text = f.pista;
            pista.rectTransform.anchorMin = new Vector2(0f, 0f);
            pista.rectTransform.anchorMax = new Vector2(1f, 0.5f);
            pista.rectTransform.offsetMin = new Vector2(56f, 0f);
            pista.rectTransform.offsetMax = Vector2.zero;

            filas.Add(new Fila { id = f.id, icono = icono.GetComponent<Image>(), nombre = nombre, pista = pista });
        }

        // Aviso emergente abajo al centro.
        RectTransform aviso = CrearImagen("Aviso", goCanvas.transform, spriteRedondeado, colorFondo);
        Anclar(aviso, new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(0f, 90f), new Vector2(820f, 84f));
        grupoAviso = aviso.gameObject.AddComponent<CanvasGroup>();
        grupoAviso.alpha = 0f;
        textoAviso = CrearTexto("Texto", aviso, 30f, FontStyles.Normal, Color.white, TextAlignmentOptions.Center);
        Estirar(textoAviso.rectTransform);
    }

    // ---------- Ayudas para construir la interfaz ----------
    private RectTransform CrearImagen(string nombre, Transform padre, Sprite sprite, Color color)
    {
        GameObject go = new GameObject(nombre, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        go.transform.SetParent(padre, false);
        Image img = go.GetComponent<Image>();
        img.sprite = sprite;
        img.color = color;
        img.raycastTarget = false;
        img.type = (sprite != null && sprite.border != Vector4.zero) ? Image.Type.Sliced : Image.Type.Simple;
        return go.GetComponent<RectTransform>();
    }

    private TMP_Text CrearTexto(string nombre, Transform padre, float tamano, FontStyles estilo, Color color, TextAlignmentOptions alineacion)
    {
        GameObject go = new GameObject(nombre, typeof(RectTransform));
        go.transform.SetParent(padre, false);
        TextMeshProUGUI t = go.AddComponent<TextMeshProUGUI>();
        t.fontSize = tamano;
        t.fontStyle = estilo;
        t.color = color;
        t.alignment = alineacion;
        t.raycastTarget = false;
        return t;
    }

    private void Anclar(RectTransform r, Vector2 anclaMin, Vector2 anclaMax, Vector2 pivote, Vector2 posicion, Vector2 tamano)
    {
        r.anchorMin = anclaMin;
        r.anchorMax = anclaMax;
        r.pivot = pivote;
        r.anchoredPosition = posicion;
        r.sizeDelta = tamano;
    }

    private void Estirar(RectTransform r)
    {
        r.anchorMin = Vector2.zero;
        r.anchorMax = Vector2.one;
        r.offsetMin = Vector2.zero;
        r.offsetMax = Vector2.zero;
    }

    // Rectángulo blanco con esquinas redondeadas (se estira en 9 partes sin deformar las esquinas).
    private Sprite CrearSpriteRedondeado(int tam, int radio)
    {
        Texture2D tex = new Texture2D(tam, tam, TextureFormat.RGBA32, false);
        tex.wrapMode = TextureWrapMode.Clamp;
        Color[] px = new Color[tam * tam];
        for (int y = 0; y < tam; y++)
        {
            for (int x = 0; x < tam; x++)
            {
                float cx = Mathf.Clamp(x + 0.5f, radio, tam - radio);
                float cy = Mathf.Clamp(y + 0.5f, radio, tam - radio);
                float d = Vector2.Distance(new Vector2(x + 0.5f, y + 0.5f), new Vector2(cx, cy));
                px[y * tam + x] = new Color(1f, 1f, 1f, Mathf.Clamp01(radio - d + 0.5f));
            }
        }
        tex.SetPixels(px);
        tex.Apply();
        return Sprite.Create(tex, new Rect(0, 0, tam, tam), new Vector2(0.5f, 0.5f), 100f, 0,
                             SpriteMeshType.FullRect, new Vector4(radio, radio, radio, radio));
    }

    // Círculo lleno o anillo blanco.
    private Sprite CrearSpriteCirculo(int tam, bool anillo)
    {
        Texture2D tex = new Texture2D(tam, tam, TextureFormat.RGBA32, false);
        tex.wrapMode = TextureWrapMode.Clamp;
        Color[] px = new Color[tam * tam];
        float r = tam * 0.5f - 1f;
        float grosor = tam * 0.14f;
        for (int y = 0; y < tam; y++)
        {
            for (int x = 0; x < tam; x++)
            {
                float d = Vector2.Distance(new Vector2(x + 0.5f, y + 0.5f), new Vector2(tam * 0.5f, tam * 0.5f));
                float a = Mathf.Clamp01(r - d + 0.5f);
                if (anillo) a *= Mathf.Clamp01(d - (r - grosor) + 0.5f);
                px[y * tam + x] = new Color(1f, 1f, 1f, a);
            }
        }
        tex.SetPixels(px);
        tex.Apply();
        return Sprite.Create(tex, new Rect(0, 0, tam, tam), new Vector2(0.5f, 0.5f), 100f);
    }
}