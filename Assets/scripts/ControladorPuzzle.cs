using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

public class ControladorPuzzle : MonoBehaviour
{
    [Header("Configuración del Puzzle")]
    [Tooltip("Cantidad de objetos que se deben interactuar para activar el puzzle")]
    public int objetosRequeridos = 2;
    
    [Header("Puzzle a Desplegar")]
    [Tooltip("El GameObject del puzzle que se activará")]
    public GameObject puzzleObject;
    
    [Tooltip("Animación de despliegue (opcional)")]
    public Animator puzzleAnimator;
    public string triggerAnimacion = "Desplegar";
    
    [Header("Configuración de Aparición")]
    public bool aparicionAnimada = true;
    public float duracionAparicion = 1f;
    public AnimationCurve curvaAparicion = AnimationCurve.EaseInOut(0, 0, 1, 1);
    
    [Header("Audio")]
    public AudioClip sonidoDespliegue;
    private AudioSource audioSource;
    
    [Header("Eventos")]
    public UnityEvent alCompletarInteracciones;
    public UnityEvent alDesplegarPuzzle;
    
    [Header("Debug")]
    public bool mostrarProgreso = true;
    
    private int objetosInteractuados = 0;
    private List<ObjetoInteractuable> objetosRegistrados = new List<ObjetoInteractuable>();
    private bool puzzleDesplegado = false;

    void Awake()
    {
        // Configurar audio
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null && sonidoDespliegue != null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    void OnEnable()
    {
        // Suscribirse al evento de interacción
        ObjetoInteractuable.OnObjetoInteractuado += OnObjetoInteractuado;
    }

    void OnDisable()
    {
        // Desuscribirse para evitar memory leaks
        ObjetoInteractuable.OnObjetoInteractuado -= OnObjetoInteractuado;
    }

    void Start()
    {
        // Asegurarse de que el puzzle esté oculto al inicio
        if (puzzleObject != null)
        {
            puzzleObject.SetActive(false);
        }
        
        if (mostrarProgreso)
        {
            Debug.Log($"[Puzzle] Esperando {objetosRequeridos} interacciones para desplegar el puzzle.");
        }
    }

    void OnObjetoInteractuado(ObjetoInteractuable objeto)
    {
        if (puzzleDesplegado) return;
        
        // Evitar contar el mismo objeto dos veces
        if (objetosRegistrados.Contains(objeto)) return;
        
        objetosRegistrados.Add(objeto);
        objetosInteractuados++;
        
        if (mostrarProgreso)
        {
            Debug.Log($"[Puzzle] Progreso: {objetosInteractuados}/{objetosRequeridos} - Interactuado: {objeto.nombreObjeto}");
        }
        
        // Verificar si se completaron todas las interacciones
        if (objetosInteractuados >= objetosRequeridos)
        {
            CompletarInteracciones();
        }
    }

    void CompletarInteracciones()
    {
        Debug.Log("[Puzzle] ¡Todas las interacciones completadas! Desplegando puzzle...");
        
        alCompletarInteracciones?.Invoke();
        
        StartCoroutine(DesplegarPuzzle());
    }

    IEnumerator DesplegarPuzzle()
    {
        puzzleDesplegado = true;
        
        // Pequeña pausa dramática
        yield return new WaitForSeconds(0.5f);
        
        // Reproducir sonido
        if (audioSource != null && sonidoDespliegue != null)
        {
            audioSource.PlayOneShot(sonidoDespliegue);
        }
        
        // Activar el puzzle
        if (puzzleObject != null)
        {
            puzzleObject.SetActive(true);
            
            if (aparicionAnimada)
            {
                yield return StartCoroutine(AnimarAparicion());
            }
            
            // Activar animación si existe
            if (puzzleAnimator != null && !string.IsNullOrEmpty(triggerAnimacion))
            {
                puzzleAnimator.SetTrigger(triggerAnimacion);
            }
        }
        
        alDesplegarPuzzle?.Invoke();
        
        Debug.Log("[Puzzle] ¡Puzzle desplegado exitosamente!");
    }

    IEnumerator AnimarAparicion()
    {
        if (puzzleObject == null) yield break;
        
        Vector3 escalaFinal = puzzleObject.transform.localScale;
        float tiempo = 0f;
        
        while (tiempo < duracionAparicion)
        {
            tiempo += Time.deltaTime;
            float progreso = curvaAparicion.Evaluate(tiempo / duracionAparicion);
            puzzleObject.transform.localScale = escalaFinal * progreso;
            yield return null;
        }
        
        puzzleObject.transform.localScale = escalaFinal;
    }

    // Método público para resetear el sistema
    public void ResetearPuzzle()
    {
        puzzleDesplegado = false;
        objetosInteractuados = 0;
        
        // Resetear objetos interactuables
        foreach (var objeto in objetosRegistrados)
        {
            if (objeto != null)
            {
                objeto.Resetear();
            }
        }
        objetosRegistrados.Clear();
        
        // Ocultar puzzle
        if (puzzleObject != null)
        {
            puzzleObject.SetActive(false);
        }
        
        Debug.Log("[Puzzle] Sistema reseteado.");
    }

    // Método para forzar el despliegue (útil para testing)
    [ContextMenu("Forzar Despliegue")]
    public void ForzarDespliegue()
    {
        if (!puzzleDesplegado)
        {
            StartCoroutine(DesplegarPuzzle());
        }
    }

    // Obtener progreso actual
    public float ObtenerProgreso()
    {
        return (float)objetosInteractuados / objetosRequeridos;
    }

    public string ObtenerTextoProgreso()
    {
        return $"{objetosInteractuados}/{objetosRequeridos}";
    }
}
