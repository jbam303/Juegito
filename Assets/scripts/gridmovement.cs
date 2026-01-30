using System.Collections;
using UnityEngine;

public class GridMovement : MonoBehaviour
{
    [Header("Configuración de Movimiento")]
    public float moveSpeed = 5f; // Velocidad de desplazamiento visual
    public Transform movePoint;   // El "puntero" invisible al que seguimos
    
    [Header("Colisiones")]
    public LayerMask obstaclesLayer; // ¿Qué objetos me bloquean el paso?

    void Start()
    {
        // Desvinculamos el punto de destino del jugador.
        // Si fuera hijo, se movería CON el jugador y se rompería la lógica.
        movePoint.parent = null; 
    }

    void Update()
    {
        // 1. Interpolación (El movimiento visual suave)
        // Movemos al personaje actual hacia el punto destino
        transform.position = Vector3.MoveTowards(transform.position, movePoint.position, moveSpeed * Time.deltaTime);

        // 2. Comprobación: ¿Estamos cerca del destino? (Grid Locking)
        // Solo aceptamos nuevo input si ya "casi" llegamos a la baldosa anterior.
        if (Vector3.Distance(transform.position, movePoint.position) <= .05f)
        {
            // Input Horizontal (A/D o Flechas)
            if (Mathf.Abs(Input.GetAxisRaw("Horizontal")) == 1f)
            {
                // Calculamos dónde caeríamos
                Vector3 target = movePoint.position + new Vector3(Input.GetAxisRaw("Horizontal"), 0f, 0f);
                
                // Si la baldosa es válida, movemos el puntero
                if (IsWalkable(target))
                {
                    movePoint.position = target;
                }
            }
            // Input Vertical (W/S o Flechas)
            else if (Mathf.Abs(Input.GetAxisRaw("Vertical")) == 1f)
            {
                Vector3 target = movePoint.position + new Vector3(0f, 0f, Input.GetAxisRaw("Vertical"));
                
                if (IsWalkable(target))
                {
                    movePoint.position = target;
                }
            }
        }
    }

    // Función de Ingeniería: Sensor de proximidad
    private bool IsWalkable(Vector3 targetPos)
    {
        // Lanza una esfera invisible de radio 0.2 en el centro de la baldosa destino.
        // Si toca algo con la capa "Obstacles", devuelve false (No caminar).
        return !Physics.CheckSphere(targetPos, 0.2f, obstaclesLayer);
    }
    // Dibuja una esfera roja en el editor para ver qué está detectando
    void OnDrawGizmos()
    {
        if (movePoint != null)
        {
            Gizmos.color = Color.red;
            // Dibuja la esfera de detección en el destino (movePoint)
            Gizmos.DrawWireSphere(movePoint.position, 0.2f);
        }
    }
}