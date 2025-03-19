using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Linq;
using Zenject;

public class SettingsController : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown _resolutionDropdown;
    [SerializeField] private TMP_Dropdown _graphicsDropdown;
    [SerializeField] private Toggle _fullscreenToggle;
    [SerializeField] private Slider _musicVolumeSlider;
    [SerializeField] private Slider _SFXVolumeSlider;
    [SerializeField] private Button _applyButton;
    [SerializeField] private Button _resetButton;
    [SerializeField] private AudioSource _musicAudioSource;

    private Resolution[] _resolutions;
    private List<Resolution> _filteredResolutions;
    private int _currentResolutionIndex;
    private bool _isFullscreen;
    private float _musicVolume;
    private float _soundVolume;
    private int _graphicsQualityIndex;

    private const string _MusicVolumeKey = "MusicVolume";
    private const string _SoundVolumeKey = "SoundVolume";
    private const string _FullscreenKey = "Fullscreen";
    private const string _ResolutionKey = "Resolution";
    private const string _GraphicsQualityKey = "GraphicsQuality";

    private SoundController _soundController;

    [Inject]
    public void Construct(SoundController soundController)
    {
        _soundController = soundController;
    }

    private void Start()
    {
        // Получение и фильтрация доступных разрешений экрана
        _resolutions = Screen.resolutions;
        _filteredResolutions = FilterResolutions(_resolutions);
        _resolutionDropdown.ClearOptions();

        List<string> options = new List<string>();
        _currentResolutionIndex = 0;
        for (int i = 0; i < _filteredResolutions.Count; i++)
        {
            string option = _filteredResolutions[i].width + " x " + _filteredResolutions[i].height;
            options.Add(option);

            if (_filteredResolutions[i].width == Screen.currentResolution.width &&
                _filteredResolutions[i].height == Screen.currentResolution.height)
            {
                _currentResolutionIndex = i;
            }
        }
        _resolutionDropdown.AddOptions(options);

        // Загрузка сохраненных настроек
        LoadSettings();

        _resolutionDropdown.value = _currentResolutionIndex;
        _resolutionDropdown.RefreshShownValue();

        // Настройка выпадающего меню качества графики
        _graphicsDropdown.ClearOptions();
        List<string> graphicsOptions = new List<string> { "НИЗКОЕ", "ВЫСОКОЕ" };
        _graphicsDropdown.AddOptions(graphicsOptions);

        _fullscreenToggle.isOn = _isFullscreen;
        _musicVolumeSlider.value = _musicVolume;
        _SFXVolumeSlider.value = _soundVolume;

        _graphicsDropdown.value = _graphicsQualityIndex;
        _graphicsDropdown.RefreshShownValue();

        // Добавление слушателей для изменений настроек
        _resolutionDropdown.onValueChanged.AddListener(_ => OnSettingsChanged());
        _fullscreenToggle.onValueChanged.AddListener(_ => OnSettingsChanged());
        _musicVolumeSlider.onValueChanged.AddListener(_ => OnMusicVolumeChanged());
        _SFXVolumeSlider.onValueChanged.AddListener(_ => OnSoundVolumeChanged());
        _graphicsDropdown.onValueChanged.AddListener(_ => OnSettingsChanged());
        _applyButton.onClick.AddListener(ApplySettings);
        _resetButton.onClick.AddListener(ResetSettings);

        _applyButton.interactable = false;
        _resetButton.interactable = false;
    }

    private List<Resolution> FilterResolutions(Resolution[] resolutions)
    {
        return resolutions
            .GroupBy(r => new { r.width, r.height })
            .Select(g => g.First())
            .OrderBy(r => r.width)
            .ThenBy(r => r.height)
            .ToList();
    }

    private void OnSettingsChanged()
    {
        _applyButton.interactable = true;
        _resetButton.interactable = true;
    }

    private void OnMusicVolumeChanged()
    {
        _musicAudioSource.volume = _musicVolumeSlider.value;
        OnSettingsChanged();
    }

    private void OnSoundVolumeChanged()
    {
        _soundController.SFXAudioSource.volume = _SFXVolumeSlider.value;
        OnSettingsChanged();
    }

    public void ApplySettings()
    {
        int resolutionIndex = _resolutionDropdown.value;
        Resolution resolution = _filteredResolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, _fullscreenToggle.isOn);

        Screen.fullScreen = _fullscreenToggle.isOn;
        _musicVolume = _musicVolumeSlider.value;
        _soundVolume = _SFXVolumeSlider.value;
        QualitySettings.SetQualityLevel(_graphicsDropdown.value);


        _currentResolutionIndex = resolutionIndex;
        _isFullscreen = _fullscreenToggle.isOn;
        _graphicsQualityIndex = _graphicsDropdown.value;

        _applyButton.interactable = false;
        _resetButton.interactable = false;

        SaveSettings();
    }

    public void ResetSettings()
    {
        _resolutionDropdown.value = _currentResolutionIndex;
        _fullscreenToggle.isOn = _isFullscreen;
        _musicVolumeSlider.value = _musicVolume;
        _SFXVolumeSlider.value = _soundVolume;
        _graphicsDropdown.value = _graphicsQualityIndex;

        _resolutionDropdown.RefreshShownValue();
        _graphicsDropdown.RefreshShownValue();

        _applyButton.interactable = false;
        _resetButton.interactable = false;
    }

    private void SaveSettings()
    {
        PlayerPrefs.SetInt(_ResolutionKey, _currentResolutionIndex);
        PlayerPrefs.SetInt(_FullscreenKey, _isFullscreen ? 1 : 0);
        PlayerPrefs.SetFloat(_MusicVolumeKey, _musicVolume);
        PlayerPrefs.SetFloat(_SoundVolumeKey, _soundVolume);
        PlayerPrefs.SetInt(_GraphicsQualityKey, _graphicsQualityIndex);
        PlayerPrefs.Save();
    }

    private void LoadSettings()
    {
        _currentResolutionIndex = PlayerPrefs.GetInt(_ResolutionKey, _currentResolutionIndex);
        _isFullscreen = PlayerPrefs.GetInt(_FullscreenKey, 1) == 1;
        _musicVolume = PlayerPrefs.GetFloat(_MusicVolumeKey, 1f);
        _soundVolume = PlayerPrefs.GetFloat(_SoundVolumeKey, 1f);
        _graphicsQualityIndex = PlayerPrefs.GetInt(_GraphicsQualityKey, 1);

        if (_musicAudioSource != null)
        {
            _musicAudioSource.volume = _musicVolume;
        }

        if (_soundController.SFXAudioSource != null)
        {
            _soundController.SFXAudioSource.volume = _soundVolume;
        }
    }
}
