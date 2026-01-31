using GLTFast.Schema;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class popUpGame : MonoBehaviour
{
    public TextContainer textContainer;
    public GameObject panel;
    public int interactionCount;
    public BoardManager boardManager;
    public String interactTag;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        interactionCount = 0;
        interactTag = this.gameObject.tag;

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        panel.SetActive(true);
        panel.GetComponentInChildren<TextMeshProUGUI>().text = textContainer.textContainer[interactionCount];
        interactionCount++;
        Debug.Log(interactTag);
        switch (interactTag)
        {
            case "Interact_01":
                boardManager.interact_01 = true;
                break;
            case "Interact_02":
                boardManager.interact_02 = true; 
                break;
            case "interact_03":
                boardManager.interact_03 = true;
                break;
        }


    }
    private void OnTriggerExit(Collider other)
    {
        panel.SetActive(false);
        if (interactionCount >= textContainer.textContainer.Length)
        {
            interactionCount = 0;
        }

        if ((boardManager.interact_01 == true) && (boardManager.interact_02 == true))
        {
            boardManager.setup();
        }


    }
}
