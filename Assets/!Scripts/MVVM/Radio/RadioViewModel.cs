using UnityEngine;
using System.Collections.Generic;
using System;

public class RadioViewModel
{
    public Action<string> OnSongChanged;
    public Action<bool> OnPlaybackStateChanged; 

    private AudioSource _audioSource;
    private List<RadioModel> _songs;
    private int _currentSongIndex = 0;
    private bool _isPlaying = false;

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
        if (_isPlaying)
        {
            _audioSource.Pause();
            OnPlaybackStateChanged?.Invoke(false);
        }
        else
        {
            _audioSource.clip = _songs[_currentSongIndex].Clip;
            _audioSource.Play();
            OnPlaybackStateChanged?.Invoke(true);
        }
        _isPlaying = !_isPlaying;
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
        if (_isPlaying)
        {
            _audioSource.Play();
        }
        OnSongChanged?.Invoke(_songs[_currentSongIndex].Name);
    }
}
