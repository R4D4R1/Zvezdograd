using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Unity.VisualScripting;
using Zenject;

public class SettingsController : MonoInit
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

    private const string SettingsFile = "settings.json";
    private SettingsData _settingsData;

    private SoundController _soundController;

    [Inject]
    public void Construct(SoundController soundController)
    {
        _soundController = soundController;
        Init();
    }

    public override void Init()
    {
        base.Init();
        _resolutions = Screen.resolutions;
        _filteredResolutions = FilterResolutions(_resolutions);
        _resolutionDropdown.ClearOptions();

        List<string> resolutionOptions = _filteredResolutions
            .Select(r => r.width + " x " + r.height)
            .ToList();
        _resolutionDropdown.AddOptions(resolutionOptions);

        _graphicsDropdown.ClearOptions();
        List<string> presetNames = new List<string> { "Низкое", "Среднее", "Высокое" };
        _graphicsDropdown.AddOptions(presetNames);

        LoadSettings();

        QualitySettings.SetQualityLevel(_graphicsQualityIndex, false);
        
        _resolutionDropdown.value = _currentResolutionIndex;
        _fullscreenToggle.isOn = _isFullscreen;
        _musicVolumeSlider.value = _musicVolume;
        _SFXVolumeSlider.value = _soundVolume;
        _graphicsDropdown.value = _graphicsQualityIndex;

        _resolutionDropdown.RefreshShownValue();
        _graphicsDropdown.RefreshShownValue();

        _resolutionDropdown.onValueChanged.AddListener(_ => OnSettingsChanged());
        _fullscreenToggle.onValueChanged.AddListener(_ => OnSettingsChanged());
        _musicVolumeSlider.onValueChanged.AddListener(_ => OnSettingsChanged());
        _SFXVolumeSlider.onValueChanged.AddListener(_ => OnSettingsChanged());
        _graphicsDropdown.onValueChanged.AddListener(_ => OnSettingsChanged());
        _applyButton.onClick.AddListener(ApplySettings);
        _resetButton.onClick.AddListener(ResetSettings);

        _applyButton.interactable = false;
        _resetButton.interactable = false;
        
        if (Application.isMobilePlatform)
        {
            _resolutionDropdown.gameObject.SetActive(false);
            _fullscreenToggle.gameObject.SetActive(false);
        }
    }

    // private void Start()
    // {
    //     _resolutions = Screen.resolutions;
    //     _filteredResolutions = FilterResolutions(_resolutions);
    //     _resolutionDropdown.ClearOptions();
    //
    //     List<string> resolutionOptions = _filteredResolutions
    //         .Select(r => r.width + " x " + r.height)
    //         .ToList();
    //     _resolutionDropdown.AddOptions(resolutionOptions);
    //
    //     _graphicsDropdown.ClearOptions();
    //     List<string> presetNames = new List<string> { "Низкое", "Среднее", "Высокое" };
    //     _graphicsDropdown.AddOptions(presetNames);
    //
    //     LoadSettings();
    //
    //     QualitySettings.SetQualityLevel(_graphicsQualityIndex, false);
    //     
    //     _resolutionDropdown.value = _currentResolutionIndex;
    //     _fullscreenToggle.isOn = _isFullscreen;
    //     _musicVolumeSlider.value = _musicVolume;
    //     _SFXVolumeSlider.value = _soundVolume;
    //     _graphicsDropdown.value = _graphicsQualityIndex;
    //
    //     _resolutionDropdown.RefreshShownValue();
    //     _graphicsDropdown.RefreshShownValue();
    //
    //     _resolutionDropdown.onValueChanged.AddListener(_ => OnSettingsChanged());
    //     _fullscreenToggle.onValueChanged.AddListener(_ => OnSettingsChanged());
    //     _musicVolumeSlider.onValueChanged.AddListener(_ => OnSettingsChanged());
    //     _SFXVolumeSlider.onValueChanged.AddListener(_ => OnSettingsChanged());
    //     _graphicsDropdown.onValueChanged.AddListener(_ => OnSettingsChanged());
    //     _applyButton.onClick.AddListener(ApplySettings);
    //     _resetButton.onClick.AddListener(ResetSettings);
    //
    //     _applyButton.interactable = false;
    //     _resetButton.interactable = false;
    //     
    //     if (Application.isMobilePlatform)
    //     {
    //         _resolutionDropdown.gameObject.SetActive(false);
    //         _fullscreenToggle.gameObject.SetActive(false);
    //     }
    // }

    private List<Resolution> FilterResolutions(Resolution[] resolutions)
    {
        return resolutions.DistinctBy(r => new { r.width, r.height })
            .OrderBy(r => r.width)
            .ThenBy(r => r.height)
            .ToList();
    }

    private void OnSettingsChanged()
    {
        _applyButton.interactable = true;
        _resetButton.interactable = true;
        
        _musicVolume = _musicVolumeSlider.value;
        _soundVolume = _SFXVolumeSlider.value;
        _graphicsQualityIndex = _graphicsDropdown.value;
        
        _currentResolutionIndex = _resolutionDropdown.value;
        _isFullscreen = _fullscreenToggle.isOn;
    }

    private void ApplySettings()
    {
        Resolution resolution = _filteredResolutions[_currentResolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, _isFullscreen);

        QualitySettings.SetQualityLevel(_graphicsQualityIndex, false);

        _musicAudioSource.volume = _musicVolume;
        _soundController.SFXAudioSource.volume = _soundVolume;

        SaveSettings();
        
        _applyButton.interactable = false;
        _resetButton.interactable = false;
    }

    private void ResetSettings()
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
        _settingsData = new SettingsData
        {
            ResolutionIndex = _currentResolutionIndex,
            Fullscreen = _isFullscreen,
            MusicVolume = _musicVolume,
            SoundVolume = _soundVolume,
            GraphicsQualityIndex = _graphicsQualityIndex
        };

        File.WriteAllText(SettingsFile, JsonUtility.ToJson(_settingsData, true));
    }

    private void LoadSettings()
    {
        if (File.Exists(SettingsFile))
        {
            _settingsData = JsonUtility.FromJson<SettingsData>(File.ReadAllText(SettingsFile));
        }
        else
        {
            _settingsData = new SettingsData();
        }

        _currentResolutionIndex = _settingsData.ResolutionIndex;
        _isFullscreen = _settingsData.Fullscreen;
        _musicVolume = _settingsData.MusicVolume;
        _soundVolume = _settingsData.SoundVolume;
        _graphicsQualityIndex = _settingsData.GraphicsQualityIndex;
    }

    public void Activate()
    {
        gameObject.SetActive(true);
    }
    
    public void Deactivate()
    {
        gameObject.SetActive(false);
    }
}

[System.Serializable]
public class SettingsData
{
    public int ResolutionIndex;
    public bool Fullscreen;
    public float MusicVolume = 1f;
    public float SoundVolume = 1f;
    public int GraphicsQualityIndex = 1;
}
