using UnityEngine;
using UnityEngine.Events;

public class ObjetoInteractuable : MonoBehaviour
{
    [Header("Configuración")]
    public string nombreObjeto = "Objeto";
    public float distanciaInteraccion = 3f;
    public KeyCode teclaInteraccion = KeyCode.E;
    
    [Header("Visual")]
    public GameObject indicadorUI; // Opcional: texto "Presiona E"
    public Color colorResaltado = Color.yellow;
    
    [Header("Eventos")]
    public UnityEvent alInteractuar;
    
    private bool jugadorCerca = false;
    private bool yaInteractuado = false;
    private Renderer objetoRenderer;
    private Color colorOriginal;
    
    // Evento estático para notificar al controlador del puzzle
    public static event System.Action<ObjetoInteractuable> OnObjetoInteractuado;

    void Start()
    {
        objetoRenderer = GetComponent<Renderer>();
        if (objetoRenderer != null)
        {
            colorOriginal = objetoRenderer.material.color;
        }
        
        if (indicadorUI != null)
        {
            indicadorUI.SetActive(false);
        }
    }

    void Update()
    {
        if (yaInteractuado) return;
        
        VerificarDistanciaJugador();
        
        if (jugadorCerca && Input.GetKeyDown(teclaInteraccion))
        {
            Interactuar();
        }
    }

    void VerificarDistanciaJugador()
    {
        GameObject jugador = GameObject.FindGameObjectWithTag("Player");
        if (jugador == null) return;
        
        float distancia = Vector3.Distance(transform.position, jugador.transform.position);
        bool estabaCerca = jugadorCerca;
        jugadorCerca = distancia <= distanciaInteraccion;
        
        // Cambió el estado
        if (jugadorCerca != estabaCerca)
        {
            if (jugadorCerca)
            {
                MostrarIndicador(true);
                ResaltarObjeto(true);
            }
            else
            {
                MostrarIndicador(false);
                ResaltarObjeto(false);
            }
        }
    }

    void Interactuar()
    {
        yaInteractuado = true;
        
        Debug.Log($"Interactuaste con: {nombreObjeto}");
        
        // Ocultar indicadores
        MostrarIndicador(false);
        ResaltarObjeto(false);
        
        // Efecto visual de interacción completada
        if (objetoRenderer != null)
        {
            objetoRenderer.material.color = Color.green;
        }
        
        // Disparar evento Unity (para acciones personalizadas en el Inspector)
        alInteractuar?.Invoke();
        
        // Notificar al controlador del puzzle
        OnObjetoInteractuado?.Invoke(this);
    }

    void MostrarIndicador(bool mostrar)
    {
        if (indicadorUI != null)
        {
            indicadorUI.SetActive(mostrar);
        }
    }

    void ResaltarObjeto(bool resaltar)
    {
        if (objetoRenderer != null && !yaInteractuado)
        {
            objetoRenderer.material.color = resaltar ? colorResaltado : colorOriginal;
        }
    }

    // Para resetear el objeto si es necesario
    public void Resetear()
    {
        yaInteractuado = false;
        if (objetoRenderer != null)
        {
            objetoRenderer.material.color = colorOriginal;
        }
    }

    // Visualizar el rango de interacción en el editor
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, distanciaInteraccion);
    }
}
