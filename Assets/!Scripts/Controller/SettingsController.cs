using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Linq;

public class SettingsController : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown resolutionDropdown;
    [SerializeField] private Toggle fullscreenToggle;
    [SerializeField] private Slider volumeSlider;
    [SerializeField] private TMP_Dropdown graphicsDropdown;
    [SerializeField] private Button applyButton;
    [SerializeField] private Button resetButton;

    private Resolution[] resolutions;
    private List<Resolution> filteredResolutions;
    private int currentResolutionIndex;
    private bool isFullscreen;
    private float volume;
    private int graphicsQualityIndex;

    private void Start()
    {
        // Получение и фильтрация доступных разрешений экрана
        resolutions = Screen.resolutions;
        filteredResolutions = FilterResolutions(resolutions);
        resolutionDropdown.ClearOptions();

        List<string> options = new List<string>();
        currentResolutionIndex = 0;
        for (int i = 0; i < filteredResolutions.Count; i++)
        {
            // Форматирование разрешений в виде строки
            string option = filteredResolutions[i].width + " x " + filteredResolutions[i].height;
            options.Add(option);

            // Установка текущего разрешения
            if (filteredResolutions[i].width == Screen.currentResolution.width &&
                filteredResolutions[i].height == Screen.currentResolution.height)
            {
                currentResolutionIndex = i;
            }
        }
        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();

        // Установка начальных значений для элементов UI
        isFullscreen = Screen.fullScreen;
        fullscreenToggle.isOn = isFullscreen;

        volume = AudioListener.volume;
        volumeSlider.value = volume;

        graphicsDropdown.ClearOptions();
        List<string> graphicsOptions = new List<string> { "НИЗКОЕ", "ВЫСОКОЕ" };
        graphicsDropdown.AddOptions(graphicsOptions);
        graphicsQualityIndex = QualitySettings.GetQualityLevel() > 0 ? 1 : 0;
        graphicsDropdown.value = graphicsQualityIndex;
        graphicsDropdown.RefreshShownValue();

        // Добавление слушателей для изменений настроек
        resolutionDropdown.onValueChanged.AddListener(_ => OnSettingsChanged());
        fullscreenToggle.onValueChanged.AddListener(_ => OnSettingsChanged());
        volumeSlider.onValueChanged.AddListener(_ => OnSettingsChanged());
        graphicsDropdown.onValueChanged.AddListener(_ => OnSettingsChanged());
        applyButton.onClick.AddListener(ApplySettings);
        resetButton.onClick.AddListener(ResetSettings);

        // Отключение кнопок "Применить" и "Сброс" в начале
        applyButton.interactable = false;
        resetButton.interactable = false;
    }

    // Фильтрация разрешений для удаления дубликатов
    private List<Resolution> FilterResolutions(Resolution[] resolutions)
    {
        return resolutions
            .GroupBy(r => new { r.width, r.height })
            .Select(g => g.First())
            .OrderBy(r => r.width)
            .ThenBy(r => r.height)
            .ToList();
    }

    // Включение кнопок "Применить" и "Сброс" при изменении настроек
    private void OnSettingsChanged()
    {
        applyButton.interactable = true;
        resetButton.interactable = true;
    }

    // Применение новых настроек
    public void ApplySettings()
    {
        int resolutionIndex = resolutionDropdown.value;
        Resolution resolution = filteredResolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, fullscreenToggle.isOn);

        Screen.fullScreen = fullscreenToggle.isOn;
        AudioListener.volume = volumeSlider.value;
        QualitySettings.SetQualityLevel(graphicsDropdown.value == 0 ? 0 : 1);

        // Обновление текущих значений
        currentResolutionIndex = resolutionIndex;
        isFullscreen = fullscreenToggle.isOn;
        volume = volumeSlider.value;
        graphicsQualityIndex = graphicsDropdown.value;

        applyButton.interactable = false;
        resetButton.interactable = false;
    }

    // Сброс настроек к текущим значениям
    public void ResetSettings()
    {
        resolutionDropdown.value = currentResolutionIndex;
        fullscreenToggle.isOn = isFullscreen;
        volumeSlider.value = volume;
        graphicsDropdown.value = graphicsQualityIndex;

        resolutionDropdown.RefreshShownValue();
        fullscreenToggle.isOn = isFullscreen;
        volumeSlider.value = volume;
        graphicsDropdown.RefreshShownValue();

        applyButton.interactable = false;
        resetButton.interactable = false;
    }
}
