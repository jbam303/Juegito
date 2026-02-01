using UnityEngine;

public class AudioVolumeTrigger : MonoBehaviour
{

    public AudioController AudioController;
    public string audioKeyword;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
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
            AudioController.PlayFx(audioKeyword);

        }
    }
}