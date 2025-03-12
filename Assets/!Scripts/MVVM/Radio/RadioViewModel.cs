using UnityEngine;
using System.Collections.Generic;
using UniRx;
using System;

public class RadioViewModel
{
    public ReactiveProperty<string> CurrentSong { get; private set; } = new ReactiveProperty<string>();
    public ReactiveProperty<bool> IsPlaying { get; private set; } = new ReactiveProperty<bool>(false);

    private AudioSource _audioSource;
    private List<RadioModel> _songs;
    private int _currentSongIndex = 0;

    public IObservable<string> OnSongChanged => CurrentSong;
    public IObservable<bool> OnPlaybackStateChanged => IsPlaying;

    public RadioViewModel(AudioSource audioSource, List<RadioModel> songs)
    {
        _audioSource = audioSource;
        _songs = songs;

        if (_songs.Count == 0)
        {
            Debug.LogError("No songs available in the playlist.");
        }
    }

    public void PlayPauseSong()
    {
        if (IsPlaying.Value)
        {
            _audioSource.Pause();
            IsPlaying.Value = false;
        }
        else
        {
            _audioSource.clip = _songs[_currentSongIndex].Clip;
            _audioSource.Play();
            IsPlaying.Value = true;
        }
    }

    public void NextSong()
    {
        _currentSongIndex = (_currentSongIndex + 1) % _songs.Count;
        StartNewSong();
    }

    public void PreviousSong()
    {
        _currentSongIndex = (_currentSongIndex - 1 + _songs.Count) % _songs.Count;
        StartNewSong();
    }

    private void StartNewSong()
    {
        _audioSource.clip = _songs[_currentSongIndex].Clip;
        if (IsPlaying.Value)
        {
            _audioSource.Play();
        }
        CurrentSong.Value = _songs[_currentSongIndex].Name;
    }
}
