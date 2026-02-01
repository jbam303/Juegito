using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DoorTransition : MonoBehaviour
{
    private Vector3 destinationPos;
    public GameObject destinationPoint;
    public TextContainer textContainer;
    public AudioController AudioController;
    
    [Header("Mensaje cuando está bloqueado")]
    [TextArea(2, 4)]
    public string mensajeBloqueado = "Creo que tengo que explorar un poco mas aqui.";
    public float duracionMensajeBloqueado = 2f;
    
    private bool jugadorEnZona = false;
    private GameObject panelActual;

    void Start()
    {
        destinationPos = destinationPoint.transform.position;
        AudioController = GameObject.Find("AudioController").GetComponent<AudioController>();
    }

    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.gameObject.CompareTag("Player")) return;
        
        jugadorEnZona = true;
        panelActual = other.transform.Find("Canvas").Find("Panel").gameObject;
        
        // Verificar si el puzzle está completado
        if (popUpGame.puzzleTerminado)
        {
            // Puerta desbloqueada - mostrar opción de transición
            popUpGame.movimientoBloqueado = true;
            panelActual.SetActive(true);
            panelActual.GetComponentInChildren<TextMeshProUGUI>().text = textContainer.textContainer[0];
        }
        else
        {
            // Puerta bloqueada - mostrar mensaje de bloqueo
            StartCoroutine(MostrarMensajeBloqueado(other));
        }
    }
    
    IEnumerator MostrarMensajeBloqueado(Collider other)
    {
        popUpGame.movimientoBloqueado = true;
        panelActual.SetActive(true);
        panelActual.GetComponentInChildren<TextMeshProUGUI>().text = mensajeBloqueado;
        
        yield return new WaitForSeconds(duracionMensajeBloqueado);
        
        // Solo cerrar si sigue en la zona
        if (panelActual != null)
        {
            panelActual.SetActive(false);
        }
        popUpGame.movimientoBloqueado = false;
    }
    
    private void OnTriggerStay(Collider other)
    {
        if (!other.gameObject.CompareTag("Player")) return;
        
        // Solo permitir interacción si el puzzle está completado
        if (!popUpGame.puzzleTerminado) return;
        
        GameObject panel = other.transform.Find("Canvas").Find("Panel").gameObject;
        
        // Cancelar con U
        if (Input.GetKeyDown(KeyCode.U))
        {
            popUpGame.movimientoBloqueado = false;
            panel.SetActive(false);
        }
        
        // Confirmar transición con Enter
        if (Input.GetKeyDown(KeyCode.Return))
        {
            other.GetComponent<CharacterController>().enabled = false;
            other.transform.SetPositionAndRotation(destinationPos, Quaternion.identity);
            popUpGame.movimientoBloqueado = false;
            panel.SetActive(false);
            AudioController.PlayFx("Door");
            other.GetComponent<CharacterController>().enabled = true;
        }
    }
    
    private void OnTriggerExit(Collider other)
    {
        if (!other.gameObject.CompareTag("Player")) return;
        
        jugadorEnZona = false;
        
        // Cerrar panel al salir
        if (panelActual != null)
        {
            panelActual.SetActive(false);
        }
        popUpGame.movimientoBloqueado = false;
    }
}
