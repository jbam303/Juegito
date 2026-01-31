using UnityEngine;

public class PC_Movements : MonoBehaviour
{
    [Header("Movimiento")]
    public float velocidad = 5f;
    public float velocidadSprint = 8f;
    
    [Header("Rotación con Mouse")]
    public float sensibilidadMouse = 2f;
    public float limiteVertical = 80f;
    
    [Header("Gravedad y Salto")]
    public float fuerzaSalto = 5f;
    public float gravedad = -9.81f;
    
    [Header("Sonidos de Pasos")]
    public AudioClip[] sonidosPasos; // Array de sonidos para variedad
    public AudioClip sonidoSalto;
    public AudioClip sonidoAterrizar;
    [Range(0f, 1f)] public float volumenPasos = 0.5f;
    public float intervaloPasosCaminando = 0.5f;
    public float intervaloPasosCorriendo = 0.3f;
    [Range(0f, 0.3f)] public float variacionTono = 0.1f; // Variación aleatoria del pitch
    
    [Header("Referencias")]
    public Transform camaraTransform; // Asigna la cámara aquí
    
    private CharacterController controller;
    private float rotacionX = 0f;
    private Vector3 velocidadVertical;
    private bool cursorBloqueado = true;
    
    // Variables para sonido de pasos
    private AudioSource audioSource;
    private float tiempoSiguientePaso = 0f;
    private bool estabaEnSuelo = true;
    
    // Variable para detectar cambio de estado del puzzle
    private bool puzzleActivoAnterior = false;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        
        // Si no se asignó cámara, buscar la principal
        if (camaraTransform == null)
        {
            camaraTransform = Camera.main?.transform;
        }
        
        // Configurar AudioSource
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 0f; // 2D para el jugador local
        
        BloquearCursor(true);
    }

    void Update()
    {
        // Detectar si el puzzle se activó/desactivó para mostrar/ocultar cursor
        if (popUpGame.movimientoBloqueado != puzzleActivoAnterior)
        {
            puzzleActivoAnterior = popUpGame.movimientoBloqueado;
            BloquearCursor(!popUpGame.movimientoBloqueado);
        }
        
        MoverPersonaje();
        RotarConMouse();
        AplicarGravedad();
        ManejarSalto();
        ManejarSonidoPasos();
        DetectarAterrizaje();
        
        // Alternar bloqueo del cursor con Escape (solo si no está en puzzle)
        if (Input.GetKeyDown(KeyCode.Escape) && !popUpGame.movimientoBloqueado)
        {
            BloquearCursor(!cursorBloqueado);
        }
    }

    void MoverPersonaje()
    {
        // Si el movimiento está bloqueado, no procesamos input
        if (popUpGame.movimientoBloqueado) return;

        // Obtener input WASD
        float horizontal = Input.GetAxisRaw("Horizontal"); // A/D
        float vertical = Input.GetAxisRaw("Vertical");     // W/S
        
        // Calcular dirección de movimiento relativa al personaje
        Vector3 direccion = transform.right * horizontal + transform.forward * vertical;
        direccion = direccion.normalized;
        
        // Verificar si está corriendo (Shift)
        float velocidadActual = Input.GetKey(KeyCode.LeftShift) ? velocidadSprint : velocidad;
        
        // Aplicar movimiento
        controller.Move(direccion * velocidadActual * Time.deltaTime);
    }

    void RotarConMouse()
    {
        // Si el puzzle está activo o el cursor no está bloqueado, no rotar
        if (!cursorBloqueado || popUpGame.movimientoBloqueado) return;
        
        // Obtener movimiento del mouse
        float mouseX = Input.GetAxis("Mouse X") * sensibilidadMouse;
        float mouseY = Input.GetAxis("Mouse Y") * sensibilidadMouse;
        
        // Rotación horizontal (cuerpo completo)
        transform.Rotate(Vector3.up * mouseX);
        
        // Rotación vertical (solo cámara, con límites)
        rotacionX -= mouseY;
        rotacionX = Mathf.Clamp(rotacionX, -limiteVertical, limiteVertical);
        
        if (camaraTransform != null)
        {
            camaraTransform.localRotation = Quaternion.Euler(rotacionX, 0f, 0f);
        }
    }

    void AplicarGravedad()
    {
        if (controller.isGrounded && velocidadVertical.y < 0)
        {
            velocidadVertical.y = -2f; // Pequeña fuerza hacia abajo
        }
        
        velocidadVertical.y += gravedad * Time.deltaTime;
        controller.Move(velocidadVertical * Time.deltaTime);
    }

    void ManejarSalto()
    {
        if (Input.GetKeyDown(KeyCode.Space) && controller.isGrounded)
        {
            velocidadVertical.y = Mathf.Sqrt(fuerzaSalto * -2f * gravedad);
        }
    }

    void BloquearCursor(bool bloquear)
    {
        cursorBloqueado = bloquear;
        Cursor.lockState = bloquear ? CursorLockMode.Locked : CursorLockMode.None;
        Cursor.visible = !bloquear;
    }
    
    void ManejarSonidoPasos()
    {
        // Solo reproducir si está en el suelo y moviéndose
        if (!controller.isGrounded) return;
        if (sonidosPasos == null || sonidosPasos.Length == 0) return;
        
        // Verificar si hay input de movimiento
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        bool seEstMoviendo = Mathf.Abs(horizontal) > 0.1f || Mathf.Abs(vertical) > 0.1f;
        
        if (!seEstMoviendo) return;
        
        // Verificar intervalo de tiempo
        if (Time.time >= tiempoSiguientePaso)
        {
            ReproducirPaso();
            
            // Calcular siguiente intervalo según velocidad
            bool corriendo = Input.GetKey(KeyCode.LeftShift);
            float intervalo = corriendo ? intervaloPasosCorriendo : intervaloPasosCaminando;
            tiempoSiguientePaso = Time.time + intervalo;
        }
    }
    
    void ReproducirPaso()
    {
        if (sonidosPasos.Length == 0) return;
        
        // Seleccionar sonido aleatorio
        int indice = Random.Range(0, sonidosPasos.Length);
        AudioClip clip = sonidosPasos[indice];
        
        if (clip == null) return;
        
        // Variar el pitch para más naturalidad
        audioSource.pitch = 1f + Random.Range(-variacionTono, variacionTono);
        audioSource.PlayOneShot(clip, volumenPasos);
    }
    
    void DetectarAterrizaje()
    {
        // Detectar cuando aterriza
        if (controller.isGrounded && !estabaEnSuelo)
        {
            ReproducirSonidoAterrizaje();
        }
        
        estabaEnSuelo = controller.isGrounded;
    }
    
    void ReproducirSonidoAterrizaje()
    {
        if (sonidoAterrizar != null)
        {
            audioSource.pitch = 1f;
            audioSource.PlayOneShot(sonidoAterrizar, volumenPasos * 1.2f);
        }
        else if (sonidosPasos != null && sonidosPasos.Length > 0)
        {
            // Usar sonido de paso como fallback
            audioSource.pitch = 0.8f; // Más grave para aterrizaje
            audioSource.PlayOneShot(sonidosPasos[0], volumenPasos * 1.2f);
        }
    }
}