using UnityEngine;
using TMPro;

public class InteractionController : MonoBehaviour
{
    [Header("Configuración Raycast")]
    public float distanciaInteraccion = 5f;
    public LayerMask capasInteractuables; // Opcional: filtrar por layers
    public KeyCode teclaInteraccion = KeyCode.E;
    
    [Header("UI")]
    public GameObject indicadorUI; // Texto "Presiona E para interactuar"
    public TextMeshProUGUI textoIndicador;
    
    [Header("Debug")]
    public bool mostrarRaycast = true;
    
    private Camera camara;
    private ObjetoInteractuable objetoActual;
    
    void Start()
    {
        camara = Camera.main;
        
        if (indicadorUI != null)
        {
            indicadorUI.SetActive(false);
        }
    }
    
    void Update()
    {
        // No hacer nada si el movimiento está bloqueado
        if (popUpGame.movimientoBloqueado) 
        {
            LimpiarObjetoActual();
            return;
        }
        
        RealizarRaycast();
        
        // Interactuar con E
        if (objetoActual != null && Input.GetKeyDown(teclaInteraccion))
        {
            objetoActual.Interactuar();
            LimpiarObjetoActual();
        }
    }
    
    void RealizarRaycast()
    {
        if (camara == null) return;
        
        // Crear ray desde el centro de la pantalla (crosshair)
        Ray ray = camara.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;
        
        // Debug visual
        if (mostrarRaycast)
        {
            Debug.DrawRay(ray.origin, ray.direction * distanciaInteraccion, Color.green);
        }
        
        // Lanzar raycast
        bool hayHit;
        if (capasInteractuables != 0)
        {
            hayHit = Physics.Raycast(ray, out hit, distanciaInteraccion, capasInteractuables);
        }
        else
        {
            hayHit = Physics.Raycast(ray, out hit, distanciaInteraccion);
        }
        
        if (hayHit)
        {
            // Intentar obtener componente ObjetoInteractuable
            ObjetoInteractuable objeto = hit.collider.GetComponent<ObjetoInteractuable>();
            
            if (objeto != null && objeto.PuedeInteractuar())
            {
                // Nuevo objeto detectado
                if (objeto != objetoActual)
                {
                    // Quitar resaltado del anterior
                    if (objetoActual != null)
                    {
                        objetoActual.QuitarResaltado();
                    }
                    
                    // Resaltar nuevo objeto
                    objetoActual = objeto;
                    objetoActual.Resaltar();
                    MostrarIndicador(objetoActual.nombreObjeto);
                }
                return;
            }
        }
        
        // No hay objeto válido - limpiar
        LimpiarObjetoActual();
    }
    
    void LimpiarObjetoActual()
    {
        if (objetoActual != null)
        {
            objetoActual.QuitarResaltado();
            objetoActual = null;
        }
        OcultarIndicador();
    }
    
    void MostrarIndicador(string nombreObjeto)
    {
        if (indicadorUI != null)
        {
            indicadorUI.SetActive(true);
            
            if (textoIndicador != null)
            {
                textoIndicador.text = $"Presiona E - {nombreObjeto}";
            }
        }
    }
    
    void OcultarIndicador()
    {
        if (indicadorUI != null)
        {
            indicadorUI.SetActive(false);
        }
    }
}