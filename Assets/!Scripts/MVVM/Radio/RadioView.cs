using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UniRx;
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

    private RadioViewModel _viewModel;

    void Start()
    {
        var songList = new List<RadioModel>();
        foreach (var clip in songs)
        {
            songList.Add(new RadioModel(clip.name, clip));
        }

        _viewModel = new RadioViewModel(audioSource, songList);

        _viewModel.OnSongChanged.Subscribe(UpdateSongTitle).AddTo(this);

        _viewModel.OnPlaybackStateChanged.Subscribe(UpdatePlaybackButtons).AddTo(this);

        playButton.onClick.AddListener(() => _viewModel.PlayPauseSong());
        pauseButton.onClick.AddListener(() => _viewModel.PlayPauseSong());
        nextButton.onClick.AddListener(() => _viewModel.NextSong());
        previousButton.onClick.AddListener(() => _viewModel.PreviousSong());

        UpdateSongTitle(songList[0].Name);
        UpdatePlaybackButtons(false);
    }

    private void UpdateSongTitle(string songTitle)
    {
        songTitleText.text = songTitle;
    }

    private void UpdatePlaybackButtons(bool isPlaying)
    {
        playButton.gameObject.SetActive(!isPlaying);
        pauseButton.gameObject.SetActive(isPlaying);
    }
}
