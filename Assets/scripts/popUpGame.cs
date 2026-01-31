using GLTFast.Schema;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class popUpGame : MonoBehaviour
{
    // All these components and variables live within the interaction collider
    public TextContainer textContainer;
    public GameObject panel;
    public int interactionCount;
    public String interactTag;

    // This is a reference to the mini-game controller. For now, all the references are manually done. It can be improved by making them dynamic at game-gen.
    public BoardManager boardManager;

    // Variable estática para bloquear el movimiento del jugador
    public static bool movimientoBloqueado = false;
    
    // Variable estática para indicar que el puzzle fue completado permanentemente
    public static bool puzzleTerminado = false;
    
    // Variable para detectar si el jugador está en la zona de interacción
    private bool jugadorEnZona = false;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        interactionCount = 0;
        interactTag = this.gameObject.tag;

    }

    // Update is called once per frame
    void Update()
    {
        // Si el puzzle ya terminó, no permitir más interacciones
        if (puzzleTerminado) return;
        
        // Detectar si el jugador presiona E mientras está en la zona
        if (jugadorEnZona && Input.GetKeyDown(KeyCode.E))
        {
            Interactuar();
        }
    }
    
    // Método para manejar la interacción
    private void Interactuar()
    {
        // Activar el panel y mostrar texto
        panel.SetActive(true);
        panel.GetComponentInChildren<TextMeshProUGUI>().text = textContainer.textContainer[interactionCount];
        interactionCount++;
        
        // Desactivar el panel después de 2 segundos
        StartCoroutine(DesactivarPanelConDelay());
        
        // Reset si llegamos al final del array
        if (interactionCount >= textContainer.textContainer.Length)
        {
            interactionCount = 0;
        }
        
        // Marcar este objeto como interactuado
        Debug.Log("Interactuando con: " + interactTag);
        switch (interactTag)
        {
            case "Interact_01":
                boardManager.interact_01 = true;
                Debug.Log("Interact_01 activado");
                break;
            case "Interact_02":
                boardManager.interact_02 = true;
                Debug.Log("Interact_02 activado");
                break;
            case "Interact_03":
                boardManager.interact_03 = true;
                Debug.Log("Interact_03 activado");
                break;
        }
        
        Debug.Log("Estado: interact_01=" + boardManager.interact_01 + ", interact_02=" + boardManager.interact_02 + ", interact_03=" + boardManager.interact_03);
        
        // Verificar si los 3 objetos distintos fueron interactuados
        if (boardManager.interact_01 && boardManager.interact_02 && boardManager.interact_03)
        {
            Debug.Log("¡Abriendo puzzle en 1 segundo!");
            // Bloqueamos el movimiento del jugador
            movimientoBloqueado = true;
            // Iniciar corrutina para abrir puzzle después de 1 segundo
            StartCoroutine(AbrirPuzzleConDelay());
        }
    }
    
    private IEnumerator AbrirPuzzleConDelay()
    {
        yield return new WaitForSeconds(2f);
        Debug.Log("¡Puzzle abierto!");
        boardManager.setup();
    }
    
    private IEnumerator DesactivarPanelConDelay()
    {
        yield return new WaitForSeconds(2f);
        panel.SetActive(false);
    }

    // This detects when the player enters the interaction collider.
    private void OnTriggerEnter(Collider other)
    {
        jugadorEnZona = true;
    }

    // This checks when an entity has exited the collider
    private void OnTriggerExit(Collider other)
    {
        jugadorEnZona = false;
        panel.SetActive(false);
    }
}
