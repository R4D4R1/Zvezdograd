using UniRx;
using UnityEngine;
using Zenject;

public class SoundController : MonoBehaviour
{
    [SerializeField] private AudioClip hoverSound;
    [SerializeField] private AudioClip selectionSound;
    [SerializeField] private AudioClip buttonPressSound;
    [SerializeField] private AudioClip mainMenuSound;

    public AudioSource SFXAudioSource;
    
    [Inject] private SettingsController _settingsController;
    [Inject] private LoadLevelController _loadLevelController;
    
    private void Awake()
    {
        SFXAudioSource = GetComponent<AudioSource>();

        _loadLevelController.OnMainMenuSceneLoaded
            .Subscribe(_ => MainMenuSceneLoaded())
            .AddTo(this);
        
        _loadLevelController.OnSceneChanged
            .Subscribe(_ => StopMusic())
            .AddTo(this);
    }

    public void PlayHoverSound()
    {
        if (hoverSound && !Application.isMobilePlatform)
        {
            SFXAudioSource.PlayOneShot(hoverSound);
        }
    }

    public void PlaySelectionSound()
    {
        if (selectionSound)
        {
            SFXAudioSource.PlayOneShot(selectionSound);
        }
    }

    public void PlayButtonPressSound()
    {
        if (buttonPressSound)
        {
            SFXAudioSource.PlayOneShot(buttonPressSound);
        }
    }

    private void MainMenuSceneLoaded()
    {
        _settingsController.SettingsMusicAudioSource.clip = mainMenuSound;
        _settingsController.SettingsMusicAudioSource.Play();
    }

    private void StopMusic()
    {
        _settingsController.SettingsMusicAudioSource.Stop();
    }
    
}
