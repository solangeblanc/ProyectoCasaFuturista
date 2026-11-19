using System;
using System.Collections.Generic;
using UnityEngine;

// Registro central de las funciones de la casa. Guarda cuáles ya descubrió el jugador.
// Cualquier script puede avisar con: RegistroDescubrimientos.Descubrir("id");
public class RegistroDescubrimientos : MonoBehaviour
{
    // Una función descubrible. Aparece como una lista editable en el Inspector.
    [Serializable]
    public class Funcion
    {
        public string id;      // identificador corto, sin espacios (el que se usa en el código)
        public string nombre;  // nombre que se muestra al jugador
        public string pista;   // pista de dónde encontrarla (máximo ~45 caracteres)
    }

    // Una sola instancia accesible desde cualquier script.
    public static RegistroDescubrimientos Instance { get; private set; }

    // Lista de funciones. Se edita ACÁ, en el código (no en el Inspector), para que lo que
    // dice acá sea siempre lo que se ve en el juego. Sin [SerializeField] Unity ignora lo viejo guardado en la escena.
    private List<Funcion> funciones = new List<Funcion>
    {
        new Funcion { id = "luces",         nombre = "Luces",                       pista = "Buscá los interruptores de cada ambiente" },
        new Funcion { id = "puertas",       nombre = "Puertas",                     pista = "Acercate a una puerta y abrila" },
        new Funcion { id = "aspiradora",    nombre = "Aspiradora robot",            pista = "Acercate a la aspiradora y activala" },
        new Funcion { id = "aire",          nombre = "Aire acondicionado",          pista = "Acercate a un aire acondicionado" },
        new Funcion { id = "escalera",      nombre = "Escalera",                    pista = "Usá la escalera para subir al primer piso" },
        new Funcion { id = "tv",            nombre = "TV planta baja",              pista = "Cambiá pisos, paredes, marcos y vidrios" },
        new Funcion { id = "riegoVivero",   nombre = "Sistema de riego del vivero", pista = "Mirá el botón del riego en el vivero" },
        new Funcion { id = "regadorJardin", nombre = "Regador del jardín",          pista = "Acercate al regador del jardín" },
        new Funcion { id = "cortadora",     nombre = "Robot cortacésped",           pista = "Activá el robot del patio" },
        new Funcion { id = "perro",         nombre = "Perro Firulais",              pista = "Acercate al perro en el patio" },
        new Funcion { id = "cargaAutos",    nombre = "Carga solar de los autos",    pista = "Visitá la zona de carga de los autos" },
        new Funcion { id = "paneles",       nombre = "Paneles solares",             pista = "Mirá la información de los paneles" },
    };

    // Ids ya descubiertos (no se guarda: en cada partida se empieza de cero).
    private readonly HashSet<string> descubiertas = new HashSet<string>();

    // Se dispara cada vez que se descubre algo nuevo. Lo usa el panel para actualizarse.
    public event Action<string> AlDescubrir;

    public IReadOnlyList<Funcion> Funciones => funciones;
    public int Total => funciones.Count;

    // Cuántas funciones de la lista ya se descubrieron.
    public int CantidadDescubiertas
    {
        get
        {
            int n = 0;
            foreach (Funcion f in funciones)
                if (descubiertas.Contains(f.id)) n++;
            return n;
        }
    }

    private void Awake()
    {
        // Si ya hay otro registro, este sobra.
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }
        Instance = this;
    }

    private void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }

    private bool ExisteId(string id)
    {
        foreach (Funcion f in funciones)
            if (f.id == id) return true;
        return false;
    }

    public bool EstaDescubierta(string id)
    {
        return descubiertas.Contains(id);
    }

    // Se llama desde los demás scripts. Si no hay registro en la escena, no hace nada (no da error).
    public static void Descubrir(string id)
    {
        if (Instance == null || string.IsNullOrEmpty(id)) return;
        Instance.Registrar(id);
    }

    private void Registrar(string id)
    {
        // Si el id no está en la lista, avisa en la consola (así se detecta un id mal escrito).
        if (!ExisteId(id))
        {
            Debug.LogWarning("RegistroDescubrimientos: el id \"" + id + "\" no está en la lista de funciones.");
            return;
        }

        // Add devuelve false si ya estaba descubierta: en ese caso no repite el aviso.
        if (!descubiertas.Add(id)) return;
        AlDescubrir?.Invoke(id);
    }
}
