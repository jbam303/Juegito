using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WinPanel : MonoBehaviour
{
    [Header("Referencias UI")]
    public GameObject panel;
    public TextMeshProUGUI mensajeTexto;
    public string mensajeVictoria = "¡GANASTE!";
    
    [Header("Botón Cerrar")]
    public Button botonCerrar;
    public float delayMostrarBoton = 1f;
    
    [Header("Crosshair")]
    public GameObject crosshair;
    
    [Header("Configuración de Apariencia")]
    public float tiempoAnimacion = 0.5f;
    public AnimationCurve curvaAparicion = AnimationCurve.EaseInOut(0, 0, 1, 1);
    
    [Header("Posición frente a cámara (para Sprite/3D)")]
    public bool usarPosicionCamara = false;
    public float distanciaCamara = 5f;
    public Camera camaraObjetivo;
    
    [Header("Testing")]
    public bool mostrarAlIniciar = false; // Desactivado por defecto
    public float delayInicial = 0.5f;
    
    private Vector3 escalaOriginal;
    private bool panelActivo = false;
    
    void Start()
    {
        if (panel != null)
        {
            escalaOriginal = panel.transform.localScale;
            panel.SetActive(false);
        }
        
        if (botonCerrar != null)
        {
            botonCerrar.gameObject.SetActive(false);
            botonCerrar.onClick.AddListener(CerrarYContinuar);
        }
        
        if (mostrarAlIniciar)
        {
            Invoke(nameof(MostrarPanelVictoria), delayInicial);
        }
    }
    
    void Update()
    {
        // Solo para testing - quitar en producción
        if (Input.GetKeyDown(KeyCode.G))
        {
            MostrarPanelVictoria();
        }
    }
    
    public void MostrarPanelVictoria()
    {
        if (panel == null || panelActivo) return;
        
        panelActivo = true;
        
        Debug.Log("[WINPANEL] Mostrando panel de victoria");
        
        // Bloquear movimiento y mostrar cursor
        popUpGame.movimientoBloqueado = true;
        
        // El crosshair ya debería estar oculto por el BoardManager
        // Pero por seguridad, lo ocultamos aquí también
        OcultarCrosshair();
        
        if (usarPosicionCamara && camaraObjetivo != null)
        {
            PosicionarFrenteCamara();
        }
        
        if (mensajeTexto != null)
        {
            mensajeTexto.text = mensajeVictoria;
        }
        
        panel.SetActive(true);
        StartCoroutine(AnimarAparicion());
    }
    
    public void MostrarPanelVictoria(string mensaje)
    {
        mensajeVictoria = mensaje;
        MostrarPanelVictoria();
    }
    
    private void OcultarCrosshair()
    {
        if (crosshair != null)
        {
            crosshair.SetActive(false);
        }
    }
    
    private void MostrarCrosshair()
    {
        if (crosshair != null)
        {
            crosshair.SetActive(true);
        }
    }
    
    private void PosicionarFrenteCamara()
    {
        Vector3 posicionCamara = camaraObjetivo.transform.position;
        Vector3 direccion = camaraObjetivo.transform.forward;
        
        panel.transform.position = posicionCamara + direccion * distanciaCamara;
        panel.transform.LookAt(camaraObjetivo.transform);
        panel.transform.Rotate(0, 180, 0);
    }
    
    private System.Collections.IEnumerator AnimarAparicion()
    {
        float tiempo = 0f;
        panel.transform.localScale = Vector3.zero;
        
        if (botonCerrar != null)
        {
            botonCerrar.gameObject.SetActive(false);
        }
        
        while (tiempo < tiempoAnimacion)
        {
            tiempo += Time.deltaTime;
            float progreso = curvaAparicion.Evaluate(tiempo / tiempoAnimacion);
            panel.transform.localScale = escalaOriginal * progreso;
            yield return null;
        }
        
        panel.transform.localScale = escalaOriginal;
        
        yield return new WaitForSeconds(delayMostrarBoton);
        
        if (botonCerrar != null)
        {
            botonCerrar.gameObject.SetActive(true);
        }
    }
    
    public void OcultarPanel()
    {
        if (panel != null)
        {
            panel.SetActive(false);
        }
        
        panelActivo = false;
        
        // Restaurar crosshair al cerrar
        MostrarCrosshair();
        
        Debug.Log("[WINPANEL] Panel cerrado, crosshair restaurado");
    }
    
    public void CerrarYContinuar()
    {
        OcultarPanel();
        popUpGame.movimientoBloqueado = false;
        
        Debug.Log("[WINPANEL] Juego desbloqueado");
    }
}
