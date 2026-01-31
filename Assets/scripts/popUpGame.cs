using GLTFast.Schema;
using System;
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

    // This detects when the player enters the interaction collider.
    private void OnTriggerEnter(Collider other)
    {

        // This section activates de UI Panel and updates the display text depending on what's in the Text Container Array. It then increases the InteractionCount.
        panel.SetActive(true);
        panel.GetComponentInChildren<TextMeshProUGUI>().text = textContainer.textContainer[interactionCount];
        interactionCount++;
        
        // This checks the Tag of the collider and sets the mini-game controller Bool to TRUE. This can probably be made better by only triggering the first time someone enters.
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

    // This checks when an entity has exited the collider
    private void OnTriggerExit(Collider other)
    {

        // We turn off the UI text
        panel.SetActive(false);
        // If our interactionCount is at the end of the array, we reset it to 0
        if (interactionCount >= textContainer.textContainer.Length)
        {
            interactionCount = 0;
        }

        // And if all the mini-game controller bools are TRUE we send a message to turn on the mini-game. This can be improved by timing and stopping it from triggering more than once.
        // We need to double check if it works in-game, once exported. Im not 100% sure clicking on the UI will work once exported.
        if ((boardManager.interact_01 == true) && (boardManager.interact_02 == true))
        {
            boardManager.setup();
        }


    }
}
