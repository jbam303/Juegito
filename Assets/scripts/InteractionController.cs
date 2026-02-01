using UnityEngine;
using TMPro;

public class InteractionController : MonoBehaviour
{
    [Header("Configuración Raycast")]
    public float distanciaInteraccion = 5f;
    public LayerMask capasInteractuables;
    public KeyCode teclaInteraccion = KeyCode.E;
    
    [Header("UI")]
    public GameObject indicadorUI;
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
        // No hacer nada si hay interacción en curso o puzzle activo
        if (ObjetoInteractuable.HayInteraccionEnCurso() || popUpGame.puzzleTerminado)
        {
            LimpiarObjetoActual();
            return;
        }
        
        // Si el movimiento está bloqueado pero no hay interacción, también limpiar
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
        if (camara == null)
        {
            camara = Camera.main;
            if (camara == null) return;
        }
        
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
            
            // También buscar en el padre si no se encuentra
            if (objeto == null)
            {
                objeto = hit.collider.GetComponentInParent<ObjetoInteractuable>();
            }
            
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