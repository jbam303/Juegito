using UnityEngine;
using UnityEngine.Events;

public class TextContainer : MonoBehaviour
{
    // This is just a temporary container for text lines that live within interaction volumes, and an interaction counter to move through the different lines.
    // Ideally i'll figure out a better way to move text around

    public string[] textContainer;
    public int interactCounter;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        interactCounter = 0;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
