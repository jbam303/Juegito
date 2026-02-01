using UnityEngine;

public class AudioController : MonoBehaviour
{
    public AudioClip[] audioClips;
    public AudioSource AudioSource;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        AudioSource = GetComponent<AudioSource>();
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void PlayFx(string sfx)
    {
        switch (sfx)
        {
            case "Door":
                AudioSource.PlayOneShot(audioClips[0],1f); 
                break;
        }
    }
}
