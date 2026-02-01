using UnityEngine;
using UnityEngine.Events;

public class ObjetoInteractuable : MonoBehaviour
{
    [Header("Configuración")]
    public string nombreObjeto = "Objeto";
    
    [Header("Visual")]
    public Color colorResaltado = Color.yellow;
    public Color colorInteractuado = Color.green;
    
    [Header("Eventos")]
    public UnityEvent alInteractuar;
    
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
    }
    
    public bool PuedeInteractuar()
    {
        return !yaInteractuado;
    }
    
    public void Resaltar()
    {
        if (objetoRenderer != null && !yaInteractuado)
        {
            objetoRenderer.material.color = colorResaltado;
        }
    }
    
    public void QuitarResaltado()
    {
        if (objetoRenderer != null && !yaInteractuado)
        {
            objetoRenderer.material.color = colorOriginal;
        }
    }
    
    public void Interactuar()
    {
        if (yaInteractuado) return;
        
        yaInteractuado = true;
        
        Debug.Log($"Interactuaste con: {nombreObjeto}");
        
        // Cambiar color a interactuado
        if (objetoRenderer != null)
        {
            objetoRenderer.material.color = colorInteractuado;
        }
        
        // Disparar evento Unity
        alInteractuar?.Invoke();
        
        // Notificar al controlador del puzzle
        OnObjetoInteractuado?.Invoke(this);
    }
    
    public void Resetear()
    {
        yaInteractuado = false;
        if (objetoRenderer != null)
        {
            objetoRenderer.material.color = colorOriginal;
        }
    }
    
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, 1f);
    }
}
