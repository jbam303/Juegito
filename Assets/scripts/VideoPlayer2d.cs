using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;

public class VideoPlayer2D : MonoBehaviour
{
    [Header("Video")]
    public VideoPlayer videoPlayer;
    public RawImage pantalla;
    public VideoClip videoClip;

    [Header("Configuraci�n")]
    public bool reproducirAlIniciar = false;
    public bool loop = false;
    public bool pausarJuegoMientras = false;

    [Header("Controles")]
    public bool permitirSaltar = true;
    public KeyCode teclaSaltar = KeyCode.Space;

    [Header("Eventos")]
    public UnityEvent alIniciar;
    public UnityEvent alTerminar;
    public UnityEvent alSaltar;

    private bool reproduciendo = false;
    private float timeScaleOriginal;

    void Start()
    {
        if (videoPlayer == null)
        {
            videoPlayer = GetComponent<VideoPlayer>();
            if (videoPlayer == null)
            {
                videoPlayer = gameObject.AddComponent<VideoPlayer>();
            }
        }

        // Configurar
        videoPlayer.playOnAwake = false;
        videoPlayer.renderMode = VideoRenderMode.APIOnly;
        videoPlayer.loopPointReached += OnVideoTerminado;
        videoPlayer.prepareCompleted += OnVideoPrepared;

        if (videoClip != null)
        {
            videoPlayer.clip = videoClip;
        }

        videoPlayer.isLooping = loop;

        // Ocultar pantalla inicialmente
        if (pantalla != null && !reproducirAlIniciar)
        {
            pantalla.gameObject.SetActive(false);
        }

        if (reproducirAlIniciar)
        {
            Reproducir();
        }
    }

    void Update()
    {
        // Actualizar textura
        if (reproduciendo && pantalla != null && videoPlayer.texture != null)
        {
            pantalla.texture = videoPlayer.texture;
        }

        // Saltar
        if (reproduciendo && permitirSaltar && Input.GetKeyDown(teclaSaltar))
        {
            Saltar();
        }
    }

    void OnVideoPrepared(VideoPlayer vp)
    {
        if (pantalla != null)
        {
            pantalla.texture = vp.texture;
        }
    }

    void OnVideoTerminado(VideoPlayer vp)
    {
        if (!loop)
        {
            Detener();
            alTerminar?.Invoke();
        }
    }

    /// <summary>
    /// Reproduce el video asignado
    /// </summary>
    public void Reproducir()
    {
        if (reproduciendo) return;

        StartCoroutine(ReproducirCoroutine());
    }

    /// <summary>
    /// Reproduce un VideoClip espec�fico
    /// </summary>
    public void Reproducir(VideoClip clip)
    {
        videoClip = clip;
        videoPlayer.clip = clip;
        Reproducir();
    }

    IEnumerator ReproducirCoroutine()
    {
        reproduciendo = true;

        if (pausarJuegoMientras)
        {
            timeScaleOriginal = Time.timeScale;
            Time.timeScale = 0f;
            videoPlayer.timeUpdateMode = VideoTimeUpdateMode.DSPTime;
        }

        if (pantalla != null)
        {
            pantalla.gameObject.SetActive(true);
        }

        videoPlayer.Prepare();

        while (!videoPlayer.isPrepared)
        {
            yield return null;
        }

        videoPlayer.Play();
        alIniciar?.Invoke();
    }

    /// <summary>
    /// Salta el video actual
    /// </summary>
    public void Saltar()
    {
        alSaltar?.Invoke();
        Detener();
        alTerminar?.Invoke();
    }

    /// <summary>
    /// Detiene el video
    /// </summary>
    public void Detener()
    {
        videoPlayer.Stop();
        reproduciendo = false;

        if (pausarJuegoMientras)
        {
            Time.timeScale = timeScaleOriginal;
        }

        if (pantalla != null)
        {
            pantalla.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// Pausa el video
    /// </summary>
    public void Pausar()
    {
        if (reproduciendo)
        {
            videoPlayer.Pause();
        }
    }

    /// <summary>
    /// Contin�a el video pausado
    /// </summary>
    public void Continuar()
    {
        if (reproduciendo)
        {
            videoPlayer.Play();
        }
    }

    public bool EstaReproduciendo()
    {
        return reproduciendo && videoPlayer.isPlaying;
    }

    public float ObtenerProgreso()
    {
        if (videoPlayer.frameCount > 0)
        {
            return (float)videoPlayer.frame / (float)videoPlayer.frameCount;
        }
        return 0f;
    }
}