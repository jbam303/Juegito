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
    
    [Header("Referencias")]
    public Transform camaraTransform; // Asigna la cámara aquí
    
    private CharacterController controller;
    private float rotacionX = 0f;
    private Vector3 velocidadVertical;
    private bool cursorBloqueado = true;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        
        // Si no se asignó cámara, buscar la principal
        if (camaraTransform == null)
        {
            camaraTransform = Camera.main?.transform;
        }
        
        BloquearCursor(true);
    }

    void Update()
    {
        MoverPersonaje();
        RotarConMouse();
        AplicarGravedad();
        ManejarSalto();
        
        // Alternar bloqueo del cursor con Escape
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            BloquearCursor(!cursorBloqueado);
        }
    }

    void MoverPersonaje()
    {
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
        if (!cursorBloqueado) return;
        
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
}