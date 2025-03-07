using UnityEngine;
using UnityEngine.UI;
using TMPro; // Импортируем TextMeshPro
using System.Collections.Generic;

public class RadioController : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private TMP_Text songTitleText;
    [SerializeField] private Button playButton;
    [SerializeField] private Button pauseButton;
    [SerializeField] private Button nextButton;
    [SerializeField] private Button previousButton;
    [SerializeField] private List<AudioClip> songs;

    private int currentSongIndex = 0;
    private bool isPlaying = false;

    void Start()
    {
        if (songs.Count == 0)
        {
            Debug.LogError("No songs available in the playlist.");
            return;
        }

        // Initial setup
        UpdateSongTitle();
        playButton.onClick.AddListener(TogglePlayPause);
        pauseButton.onClick.AddListener(TogglePlayPause);
        nextButton.onClick.AddListener(PlayNextSong);
        previousButton.onClick.AddListener(PlayPreviousSong);

        audioSource.loop = false;
        audioSource.playOnAwake = false;

        TogglePlayPause();
    }

    void Update()
    {
        // Проверяем, закончилась ли песня
        if (!audioSource.isPlaying && isPlaying)
        {
            PlayNextSong();
        }
    }

    void UpdateSongTitle()
    {
        songTitleText.text = songs[currentSongIndex].name;
    }

    void TogglePlayPause()
    {
        if (isPlaying)
        {
            audioSource.Pause();

            playButton.gameObject.SetActive(true);
            pauseButton.gameObject.SetActive(false);
        }
        else
        {
            audioSource.clip = songs[currentSongIndex];
            audioSource.Play();

            playButton.gameObject.SetActive(false);
            pauseButton.gameObject.SetActive(true);
        }
        isPlaying = !isPlaying;
    }

    void PlayNextSong()
    {
        currentSongIndex = (currentSongIndex + 1) % songs.Count;
        StartNewSong();
    }

    void PlayPreviousSong()
    {
        currentSongIndex = (currentSongIndex - 1 + songs.Count) % songs.Count;
        StartNewSong();
    }

    void StartNewSong()
    {
        audioSource.clip = songs[currentSongIndex];
        if (isPlaying)
        {
            audioSource.Play();
        }
        UpdateSongTitle();
    }
}
