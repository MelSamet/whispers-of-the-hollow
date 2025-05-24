using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class MainMenuManager : MonoBehaviour
{
    [Header("UI Elements")]
    public Button playButton;
    public Button exitButton;
    public RawImage backgroundVideoImage;

    [Header("Audio")]
    public AudioSource audioSource;
    public string musicClipPath = "Music/main_menu_theme"; // Resources/Music/main_menu_theme.wav or .mp3

    [Header("Video Background")]
    public string videoFileName = "menu_bg_video.mp4"; // Located in StreamingAssets folder

    void Start()
    {
        SetupButtons();
        PlayMusic();
        PlayBackgroundVideo();
    }

    void SetupButtons()
    {
        if (playButton != null)
            playButton.onClick.AddListener(PlayGame);

        if (exitButton != null)
            exitButton.onClick.AddListener(ExitGame);
    }

    void PlayMusic()
    {
        if (audioSource != null)
        {
            AudioClip clip = Resources.Load<AudioClip>(musicClipPath);
            if (clip != null)
            {
                audioSource.clip = clip;
                audioSource.loop = true;
                audioSource.Play();
            }
            else
            {
                Debug.LogWarning("Main menu music not found at: Resources/" + musicClipPath);
            }
        }
    }

    void PlayBackgroundVideo()
    {
        if (backgroundVideoImage != null)
        {
            VideoPlayer videoPlayer = backgroundVideoImage.gameObject.AddComponent<VideoPlayer>();
            videoPlayer.renderMode = VideoRenderMode.APIOnly;
            videoPlayer.source = VideoSource.Url;
            videoPlayer.url = System.IO.Path.Combine(Application.streamingAssetsPath, videoFileName);
            videoPlayer.isLooping = true;
            videoPlayer.audioOutputMode = VideoAudioOutputMode.None;

            videoPlayer.prepareCompleted += (vp) =>
            {
                backgroundVideoImage.texture = vp.texture;
                vp.Play();
            };

            videoPlayer.Prepare();
        }
    }

    public void PlayGame()
    {
        SceneManager.LoadScene("SampleScene"); // Değiştirilecek sahne ismini buraya yaz
    }

    public void ExitGame()
    {
        Debug.Log("Quitting the game...");
        Application.Quit();
    }
}
