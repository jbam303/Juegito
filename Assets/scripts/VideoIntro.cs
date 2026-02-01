using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class VideoIntro : MonoBehaviour
{
    [Header("Video")]
    public VideoPlayer videoPlayer;
    public RawImage pantalla;
    public string rutaVideo;
    public VideoClip videoClip;

    [Header("Configuración")]
    public bool saltarConTecla = true;
    public KeyCode teclaSaltar = KeyCode.Space;
    public KeyCode teclaSaltarAlternativa = KeyCode.Escape;
    public float tiempoEsperaInicio = 0.5f;

    [Header("Al Terminar")]
    public bool cargarEscena = true;
    public string nombreEscenaSiguiente = "Game";
    public int indiceEscenaSiguiente = 1;
    public bool usarNombreEscena = true;

    [Header("Fade")]
    public bool usarFade = true;
    public Image panelFade;
    public float duracionFade = 0.5f;

    private bool videoTerminado = false;
    private bool saltando = false;

    void Awake()
    {
        // Crear cámara dummy si no existe ninguna
        if (Camera.main == null)
        {
            GameObject camObj = new GameObject("IntroCamera");
            Camera cam = camObj.AddComponent<Camera>();
            cam.clearFlags = CameraClearFlags.SolidColor;
            cam.backgroundColor = Color.black;
            cam.cullingMask = 0; // No renderiza nada
            cam.depth = -100; // Prioridad más baja
        }
    }

    void Start()
    {
        if (videoPlayer == null)
        {
            videoPlayer = gameObject.AddComponent<VideoPlayer>();
        }

        if (videoClip != null)
        {
            videoPlayer.source = VideoSource.VideoClip;
            videoPlayer.clip = videoClip;
        }
        else if (!string.IsNullOrEmpty(rutaVideo))
        {
            videoPlayer.source = VideoSource.Url;
            videoPlayer.url = System.IO.Path.Combine(Application.streamingAssetsPath, rutaVideo);
        }

        if (pantalla != null)
        {
            videoPlayer.renderMode = VideoRenderMode.APIOnly;
            videoPlayer.prepareCompleted += OnVideoPrepared;
        }

        videoPlayer.loopPointReached += OnVideoTerminado;

        // Ocultar cursor durante el video
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        // Iniciar
        StartCoroutine(IniciarVideo());
    }

    IEnumerator IniciarVideo()
    {
        // Fade in desde negro
        if (usarFade && panelFade != null)
        {
            panelFade.color = Color.black;
        }

        yield return new WaitForSeconds(tiempoEsperaInicio);

        videoPlayer.Prepare();

        while (!videoPlayer.isPrepared)
        {
            yield return null;
        }

        // Fade out del panel negro
        if (usarFade && panelFade != null)
        {
            yield return StartCoroutine(Fade(1f, 0f));
        }

        videoPlayer.Play();
    }

    void OnVideoPrepared(VideoPlayer vp)
    {
        if (pantalla != null)
        {
            pantalla.texture = vp.texture;
        }
    }

    void Update()
    {
        // Actualizar textura cada frame
        if (videoPlayer.isPlaying && pantalla != null && videoPlayer.texture != null)
        {
            pantalla.texture = videoPlayer.texture;
        }

        // Saltar video
        if (saltarConTecla && !saltando)
        {
            if (Input.GetKeyDown(teclaSaltar) || Input.GetKeyDown(teclaSaltarAlternativa))
            {
                SaltarVideo();
            }
        }
    }

    void OnVideoTerminado(VideoPlayer vp)
    {
        if (!videoTerminado)
        {
            videoTerminado = true;
            StartCoroutine(TerminarYCargarEscena());
        }
    }

    public void SaltarVideo()
    {
        if (saltando) return;

        saltando = true;
        videoPlayer.Stop();
        StartCoroutine(TerminarYCargarEscena());
    }

    IEnumerator TerminarYCargarEscena()
    {
        // Fade a negro
        if (usarFade && panelFade != null)
        {
            yield return StartCoroutine(Fade(0f, 1f));
        }

        // Restaurar cursor
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        // Cargar siguiente escena
        if (cargarEscena)
        {
            if (usarNombreEscena)
            {
                SceneManager.LoadScene(nombreEscenaSiguiente);
            }
            else
            {
                SceneManager.LoadScene(indiceEscenaSiguiente);
            }
        }
    }

    IEnumerator Fade(float desde, float hasta)
    {
        float tiempo = 0f;
        Color color = panelFade.color;

        while (tiempo < duracionFade)
        {
            tiempo += Time.deltaTime;
            float alpha = Mathf.Lerp(desde, hasta, tiempo / duracionFade);
            color.a = alpha;
            panelFade.color = color;
            yield return null;
        }

        color.a = hasta;
        panelFade.color = color;
    }
}