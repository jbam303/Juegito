using UnityEngine;
using UnityEngine.Events;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class ObjetoInteractuable : MonoBehaviour
{
    // Enum para evitar errores de escritura
    public enum TipoInteraccion
    {
        Interact_01,
        Interact_02,
        Interact_03
    }
    
    [Header("Configuración")]
    public string nombreObjeto = "Objeto";
    public KeyCode teclaInteraccion = KeyCode.E;
    public TipoInteraccion tipoInteraccion = TipoInteraccion.Interact_01; // DROPDOWN en el Inspector
    
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
    
    [Header("Eventos")]
    public UnityEvent alInteractuar;
    
    private int indiceActual = 0;
    private bool estaResaltado = false;
    private Renderer objetoRenderer;
    private Color colorOriginal;
    private Coroutine coroutinaMensaje;
    
    private static HashSet<TipoInteraccion> objetosUnicos = new HashSet<TipoInteraccion>();
    private static bool interaccionEnCurso = false;
    private static BoardManager boardManagerRef;
    
    public static event System.Action<ObjetoInteractuable> OnObjetoInteractuado;

    void Start()
    {
        objetoRenderer = GetComponent<Renderer>();
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
        
        if (boardManager == null)
        {
            Debug.LogWarning($"[{nombreObjeto}] BoardManager no asignado!");
        }
        
        // Log para verificar configuración
        Debug.Log($"[INIT] {nombreObjeto} configurado con tipo: {tipoInteraccion}");
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
    
    public bool PuedeInteractuar()
    {
        return !popUpGame.puzzleTerminado && !interaccionEnCurso;
    }
    
    public void Resaltar()
    {
        if (popUpGame.puzzleTerminado || interaccionEnCurso) return;
        
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
            
            Debug.Log($"[REGISTRO] Nuevo tipo único: {tipoInteraccion}");
            Debug.Log($"[REGISTRO] Total: {objetosUnicos.Count}/3");
            
            BoardManager bm = boardManager != null ? boardManager : boardManagerRef;
            
            if (bm != null)
            {
                switch (tipoInteraccion)
                {
                    case TipoInteraccion.Interact_01:
                        bm.interact_01 = true;
                        Debug.Log("[BOARDMANAGER] interact_01 = true");
                        break;
                    case TipoInteraccion.Interact_02:
                        bm.interact_02 = true;
                        Debug.Log("[BOARDMANAGER] interact_02 = true");
                        break;
                    case TipoInteraccion.Interact_03:
                        bm.interact_03 = true;
                        Debug.Log("[BOARDMANAGER] interact_03 = true");
                        break;
                }
                
                Debug.Log($"[BOARDMANAGER] Estado: 01={bm.interact_01}, 02={bm.interact_02}, 03={bm.interact_03}");
            }
            else
            {
                Debug.LogError("[ERROR] No hay BoardManager disponible!");
            }
        }
        else
        {
            Debug.Log($"[REGISTRO] Tipo {tipoInteraccion} ya fue registrado");
        }
    }
    
    void VerificarActivacionPuzzle()
    {
        BoardManager bm = boardManager != null ? boardManager : boardManagerRef;
        
        if (bm == null)
        {
            Debug.LogError("[ERROR] No se puede verificar puzzle - BoardManager es NULL");
            DesbloquearInteraccion();
            return;
        }
        
        Debug.Log($"[VERIFICAR] 01={bm.interact_01}, 02={bm.interact_02}, 03={bm.interact_03}");
        
        if (bm.interact_01 && bm.interact_02 && bm.interact_03)
        {
            Debug.Log("[PUZZLE] ¡3 objetos únicos! Abriendo puzzle...");
            StartCoroutine(AbrirPuzzleConDelay(bm));
        }
        else
        {
            Debug.Log($"[PUZZLE] Faltan objetos ({objetosUnicos.Count}/3). Desbloqueando...");
            DesbloquearInteraccion();
        }
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
    }
    
    public static void ResetearSistema()
    {
        objetosUnicos.Clear();
        interaccionEnCurso = false;
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
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, 1f);
    }
}