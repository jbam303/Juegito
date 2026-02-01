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


    // HOWTO: cuando necesitas reproducir un sonide llama a PlayFx() usando una keyword como parametro
    // La keyword corresponde al case que reproduce alguno de los audioclips del array de AudioClip[] que se encuentra en el GameObject "Audio Controller"
    public void PlayFx(string sfx)
    {
        switch (sfx)
        {
            case "Door":
                AudioSource.PlayOneShot(audioClips[0],1f); 
                break;
            case "ropa":
                AudioSource.PlayOneShot(audioClips[5], 1f);
                break;
            case "closet":
                AudioSource.PlayOneShot(audioClips[4], 1f);
                break;
            case "planta":
                AudioSource.PlayOneShot(audioClips[6], 1f);
                break;
            case "foto":
                AudioSource.clip = audioClips[7];
                AudioSource.loop = true;
                AudioSource.Play(0);
                break;
            case "room":
                AudioSource.clip = audioClips[8];
                AudioSource.loop = true;
                AudioSource.Play(0);
                break;
            case "creepy":
                AudioSource.clip = audioClips[2];
                AudioSource.loop = true;
                AudioSource.Play(0);
                break;
            case "studio":
                AudioSource.clip = audioClips[9];
                AudioSource.loop = true;
                AudioSource.Play(0);
                break;
        }
    }
}
