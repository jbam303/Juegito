using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;
using UnityEngine.UIElements;
using static UnityEngine.Rendering.DebugUI;

public class DoorTransition : MonoBehaviour
{
    // public GameObject player;
    private Vector3 initialPos;
    private Vector3 destinationPos;
    public GameObject destinationPoint;
    public TextContainer textContainer;
    public AudioController AudioController;




    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

        destinationPos = destinationPoint.transform.position;
        AudioController = GameObject.Find("AudioController").GetComponent<AudioController>();
        

    }

    // Update is called once per frame
    void Update()
    {
       

    }

    private void OnTriggerEnter(Collider other)
    {

        

        if (other.gameObject.CompareTag("Player"))
        {
            popUpGame.movimientoBloqueado = true;
            GameObject panel = other.transform.Find("Canvas").Find("Panel").gameObject;
            panel.SetActive(true);
            panel.GetComponentInChildren<TextMeshProUGUI>().text = textContainer.textContainer[0];

                
            }
        
    
    }
    private void OnTriggerStay(Collider other)
    {
        Vector3 newpos = new Vector3(10.0f, 11.0f, 100.0f);
        GameObject panel = other.transform.Find("Canvas").Find("Panel").gameObject;
        if (Input.GetKeyDown(KeyCode.U))
        {
            popUpGame.movimientoBloqueado = false;
            panel.SetActive(false);

        }
        if (Input.GetKeyDown(KeyCode.Return))
        {
            // player.GetComponent<CharacterController>().enabled = false;
            other.GetComponent<CharacterController>().enabled = false;
            other.transform.SetPositionAndRotation(destinationPos, Quaternion.identity);
            popUpGame.movimientoBloqueado = false;
            panel.SetActive(false);
            AudioController.PlayFx("Door");
            other.GetComponent<CharacterController>().enabled = true;
        }
    }
}
