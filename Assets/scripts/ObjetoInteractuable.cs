using UnityEngine;
using UnityEngine.Events;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class ObjetoInteractuable : MonoBehaviour
{
    public enum TipoInteraccion
    {
        Interact_01,
        Interact_02,
        Interact_03,
        Interact_04,
        Interact_Especial
    }
    
    [Header("Configuración")]
    public string nombreObjeto = "Objeto";
    public KeyCode teclaInteraccion = KeyCode.E;
    public TipoInteraccion tipoInteraccion = TipoInteraccion.Interact_01;
    
    [Header("Visual")]
    public Color colorResaltado = Color.yellow;
    
    [Header("UI Indicador (Tecla)")]
    public GameObject canvasIndicador;
    public TextMeshProUGUI textoTecla;
    
    [Header("UI Mensaje al Interactuar")]
    public GameObject canvasMensaje;
    public TextMeshProUGUI textoMensaje;
    [TextArea(2, 4)]
    public string[] mensajesInteraccion;
    public float duracionMensaje = 2f;
    
    [Header("Referencias Puzzle")]
    public BoardManager boardManager;
    
    [Header("Configuración Puzzle")]
    public int interaccionesNecesarias = 3;
    
    [Header("Eventos")]
    public UnityEvent alInteractuar;
    public UnityEvent alDesbloquearse; // Evento cuando el especial se desbloquea
    
    private int indiceActual = 0;
    private bool estaResaltado = false;
    private Renderer objetoRenderer;
    private Color colorOriginal;
    private Coroutine coroutinaMensaje;
    
    // Para objetos especiales - guardar estado de renderizado
    private bool objetoVisible = true;
    private Collider objetoCollider;
    
    // Sistema de tracking
    private static HashSet<TipoInteraccion> objetosUnicos = new HashSet<TipoInteraccion>();
    private static bool interaccionEnCurso = false;
    private static BoardManager boardManagerRef;
    private static List<ObjetoInteractuable> objetosEspeciales = new List<ObjetoInteractuable>();
    
    public static event System.Action<ObjetoInteractuable> OnObjetoInteractuado;

    void Start()
    {
        objetoRenderer = GetComponent<Renderer>();
        objetoCollider = GetComponent<Collider>();
        
        if (objetoRenderer != null)
        {
            colorOriginal = objetoRenderer.material.color;
        }
        
        if (canvasIndicador != null)
        {
            canvasIndicador.SetActive(false);
        }
        
        if (canvasMensaje != null)
        {
            canvasMensaje.SetActive(false);
        }
        
        if (textoTecla != null)
        {
            textoTecla.text = teclaInteraccion.ToString();
        }
        
        if (boardManager != null && boardManagerRef == null)
        {
            boardManagerRef = boardManager;
        }
        
        // Si es especial, ocultarlo y registrarlo
        if (tipoInteraccion == TipoInteraccion.Interact_Especial)
        {
            objetosEspeciales.Add(this);
            OcultarObjeto();
            Debug.Log($"[INIT] {nombreObjeto} (ESPECIAL) - Oculto hasta desbloqueo");
        }
        else
        {
            Debug.Log($"[INIT] {nombreObjeto} configurado como: {tipoInteraccion}");
        }
    }
    
    void OnDestroy()
    {
        // Limpiar de la lista cuando se destruya
        if (objetosEspeciales.Contains(this))
        {
            objetosEspeciales.Remove(this);
        }
    }
    
    void OcultarObjeto()
    {
        objetoVisible = false;
        
        if (objetoRenderer != null)
        {
            objetoRenderer.enabled = false;
        }
        
        if (objetoCollider != null)
        {
            objetoCollider.enabled = false;
        }
        
        // Ocultar hijos también (por si tiene efectos visuales)
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(false);
        }
    }
    
    void MostrarObjeto()
    {
        if (objetoVisible) return;
        
        objetoVisible = true;
        
        if (objetoRenderer != null)
        {
            objetoRenderer.enabled = true;
        }
        
        if (objetoCollider != null)
        {
            objetoCollider.enabled = true;
        }
        
        // Mostrar hijos
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(true);
        }
        
        Debug.Log($"[ESPECIAL] ¡{nombreObjeto} ahora es visible!");
        
        // Disparar evento de desbloqueo
        alDesbloquearse?.Invoke();
    }
    
    void LateUpdate()
    {
        if (canvasIndicador != null && canvasIndicador.activeSelf)
        {
            MirarHaciaCamara(canvasIndicador.transform);
        }
        
        if (canvasMensaje != null && canvasMensaje.activeSelf)
        {
            MirarHaciaCamara(canvasMensaje.transform);
        }
    }
    
    void MirarHaciaCamara(Transform objetivo)
    {
        Camera cam = Camera.main;
        if (cam != null)
        {
            objetivo.LookAt(objetivo.position + cam.transform.forward);
        }
    }
    
    static bool EspecialDesbloqueado(int necesarias)
    {
        int contadorNormales = 0;
        foreach (var tipo in objetosUnicos)
        {
            if (tipo != TipoInteraccion.Interact_Especial)
            {
                contadorNormales++;
            }
        }
        return contadorNormales >= necesarias;
    }
    
    // Método estático para mostrar todos los objetos especiales
    static void VerificarYMostrarEspeciales()
    {
        foreach (var especial in objetosEspeciales)
        {
            if (especial != null && !especial.objetoVisible)
            {
                if (EspecialDesbloqueado(especial.interaccionesNecesarias))
                {
                    especial.MostrarObjeto();
                }
            }
        }
    }
    
    public bool PuedeInteractuar()
    {
        if (popUpGame.puzzleTerminado || interaccionEnCurso) return false;
        
        // Si es especial y no está visible, no puede interactuar
        if (tipoInteraccion == TipoInteraccion.Interact_Especial && !objetoVisible)
        {
            return false;
        }
        
        return true;
    }
    
    public void Resaltar()
    {
        if (popUpGame.puzzleTerminado || interaccionEnCurso) return;
        if (!objetoVisible) return;
        
        estaResaltado = true;
        
        if (objetoRenderer != null)
        {
            objetoRenderer.material.color = colorResaltado;
        }
        
        if (canvasIndicador != null)
        {
            canvasIndicador.SetActive(true);
        }
    }
    
    public void QuitarResaltado()
    {
        estaResaltado = false;
        
        if (objetoRenderer != null)
        {
            objetoRenderer.material.color = colorOriginal;
        }
        
        if (canvasIndicador != null)
        {
            canvasIndicador.SetActive(false);
        }
    }
    
    public void Interactuar()
    {
        if (popUpGame.puzzleTerminado || interaccionEnCurso) return;
        if (!objetoVisible) return;
        
        Debug.Log($"=== INTERACCIÓN: {nombreObjeto} (Tipo: {tipoInteraccion}) ===");
        
        interaccionEnCurso = true;
        popUpGame.movimientoBloqueado = true;
        
        QuitarResaltado();
        
        RegistrarInteraccion();
        MostrarMensaje();
        
        alInteractuar?.Invoke();
        OnObjetoInteractuado?.Invoke(this);
    }
    
    void RegistrarInteraccion()
    {
        if (!objetosUnicos.Contains(tipoInteraccion))
        {
            objetosUnicos.Add(tipoInteraccion);
            
            Debug.Log($"[REGISTRO] Nuevo tipo: {tipoInteraccion}");
            Debug.Log($"[REGISTRO] Progreso: {ObtenerProgresoNormal()}/{interaccionesNecesarias}");
            
            BoardManager bm = boardManager != null ? boardManager : boardManagerRef;
            
            if (bm != null)
            {
                switch (tipoInteraccion)
                {
                    case TipoInteraccion.Interact_01:
                        bm.interact_01 = true;
                        break;
                    case TipoInteraccion.Interact_02:
                        bm.interact_02 = true;
                        break;
                    case TipoInteraccion.Interact_03:
                        bm.interact_03 = true;
                        break;
                }
            }
            
            // Verificar si hay que mostrar objetos especiales
            VerificarYMostrarEspeciales();
        }
    }
    
    void VerificarActivacionPuzzle()
    {
        if (tipoInteraccion == TipoInteraccion.Interact_Especial)
        {
            BoardManager bm = boardManager != null ? boardManager : boardManagerRef;
            
            if (bm != null)
            {
                Debug.Log("[PUZZLE] ¡Interact_Especial activado! Abriendo puzzle...");
                StartCoroutine(AbrirPuzzleConDelay(bm));
                return;
            }
        }
        
        DesbloquearInteraccion();
    }
    
    void DesbloquearInteraccion()
    {
        interaccionEnCurso = false;
        popUpGame.movimientoBloqueado = false;
        Debug.Log("[DESBLOQUEO] Liberado");
    }
    
    IEnumerator AbrirPuzzleConDelay(BoardManager bm)
    {
        yield return new WaitForSeconds(0.5f);
        
        Debug.Log("[PUZZLE] Ejecutando boardManager.setup()");
        interaccionEnCurso = false;
        bm.setup();
    }
    
    void MostrarMensaje()
    {
        if (canvasMensaje == null)
        {
            StartCoroutine(VerificarPuzzleSinMensaje());
            return;
        }
        
        if (textoMensaje != null && mensajesInteraccion != null && mensajesInteraccion.Length > 0)
        {
            textoMensaje.text = mensajesInteraccion[indiceActual];
            
            indiceActual++;
            if (indiceActual >= mensajesInteraccion.Length)
            {
                indiceActual = 0;
            }
        }
        
        canvasMensaje.SetActive(true);
        
        if (coroutinaMensaje != null)
        {
            StopCoroutine(coroutinaMensaje);
        }
        coroutinaMensaje = StartCoroutine(OcultarMensajeDespuesDeTiempo());
    }
    
    IEnumerator VerificarPuzzleSinMensaje()
    {
        yield return new WaitForSeconds(0.5f);
        VerificarActivacionPuzzle();
    }
    
    IEnumerator OcultarMensajeDespuesDeTiempo()
    {
        yield return new WaitForSeconds(duracionMensaje);
        
        if (canvasMensaje != null)
        {
            canvasMensaje.SetActive(false);
        }
        
        VerificarActivacionPuzzle();
    }
    
    public void Resetear()
    {
        indiceActual = 0;
        estaResaltado = false;
        
        if (objetoRenderer != null)
        {
            objetoRenderer.material.color = colorOriginal;
        }
        
        if (canvasIndicador != null)
        {
            canvasIndicador.SetActive(false);
        }
        
        if (canvasMensaje != null)
        {
            canvasMensaje.SetActive(false);
        }
        
        // Si es especial, volver a ocultar
        if (tipoInteraccion == TipoInteraccion.Interact_Especial)
        {
            OcultarObjeto();
        }
    }
    
    public static void ResetearSistema()
    {
        objetosUnicos.Clear();
        interaccionEnCurso = false;
        
        // Ocultar todos los especiales de nuevo
        foreach (var especial in objetosEspeciales)
        {
            if (especial != null)
            {
                especial.OcultarObjeto();
            }
        }
    }
    
    public static int ObtenerProgresoNormal()
    {
        int contador = 0;
        foreach (var tipo in objetosUnicos)
        {
            if (tipo != TipoInteraccion.Interact_Especial)
            {
                contador++;
            }
        }
        return contador;
    }
    
    public static int ObtenerProgreso()
    {
        return objetosUnicos.Count;
    }
    
    public static bool HayInteraccionEnCurso()
    {
        return interaccionEnCurso;
    }
    
    void OnDrawGizmosSelected()
    {
        Gizmos.color = tipoInteraccion == TipoInteraccion.Interact_Especial ? Color.magenta : Color.cyan;
        Gizmos.DrawWireSphere(transform.position, 1f);
    }
}