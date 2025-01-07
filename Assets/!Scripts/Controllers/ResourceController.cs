using System;
using UnityEngine;
using UnityEngine.UI;

public class ResourceController : MonoBehaviour
{
    // Приватные переменные для ресурсов
    [Range(0f, 10f)]
    [SerializeField] private int provision = 0;

    [Range(0f, 10f)]
    [SerializeField] private int medicine = 0;

    [Range(0f, 10f)]
    [SerializeField] private int rawMaterials = 0;

    [Range(0f, 10f)]
    [SerializeField] private int buildingMaterials = 0;

    private int _maxProvision;
    private int _maxMedicine;
    private int _maxRawMaterials;
    private int _maxBuildingMaterials;
    private int _maxStability;

    // Приватная переменная для стабильности
    [Range(0f, 100f)]
    [SerializeField] private int stability = 100;

    // Цвета для минимального и максимального значений
    [SerializeField] private Color minColor = Color.red;    // Красный цвет
    [SerializeField] private Color maxColor = Color.blue;   // Синий цвет

    // Ссылка на Image заливки слайдера стабильности
    [SerializeField] private Image stabilityFillImage;



    // Слайдеры для ресурсов и стабильности
    public Slider provisionSlider;
    public Slider medicineSlider;
    public Slider rawMaterialsSlider;
    public Slider buildingMaterialsSlider;
    public Slider stabilitySlider;


    void Start()
    {
        // Инициализация слайдеров и привязка значений
        InitializeSliders();

        _maxProvision = provision;
        _maxMedicine = medicine;
        _maxBuildingMaterials = buildingMaterials;
        _maxRawMaterials = 10;
        _maxStability = stability;

        ControllersManager.Instance.timeController.OnNextTurnBtnPressed += NextTurnBtnPressed;
    }

    private void OnDestroy()
    {
        ControllersManager.Instance.timeController.OnNextTurnBtnPressed -= NextTurnBtnPressed;
    }

    private void NextTurnBtnPressed()
    {
        // За каждое обычное сломманое здание стабильность батает на 2 с каждым ходом
        foreach (var building in ControllersManager.Instance.buildingBombingController.RegularBuildings)
        {
            if (building.CurrentState == RepairableBuilding.State.Damaged)
            {
                AddOrRemoveStability(-2);
            }
        }
    }

    // Инициализация слайдеров
    private void InitializeSliders()
    {
        // Слайдеры для ресурсов
        provisionSlider.maxValue = 10;
        provisionSlider.minValue = 0;
        provisionSlider.value = provision;
        provisionSlider.interactable = false;

        medicineSlider.maxValue = 10;
        medicineSlider.minValue = 0;
        medicineSlider.value = medicine;
        medicineSlider.interactable = false;

        rawMaterialsSlider.maxValue = 10;
        rawMaterialsSlider.minValue = 0;
        rawMaterialsSlider.value = rawMaterials;
        rawMaterialsSlider.interactable = false;

        buildingMaterialsSlider.maxValue = 10;
        buildingMaterialsSlider.minValue = 0;
        buildingMaterialsSlider.value = buildingMaterials;
        buildingMaterialsSlider.interactable = false;

        // Слайдер для стабильности
        stabilitySlider.maxValue = 100;
        stabilitySlider.minValue = 0;
        stabilitySlider.value = stability;
        stabilitySlider.interactable = false;
    }

    public int GetProvision()
    {
        return provision;
    }
    public int GetMedicine()
    {
        return medicine;
    }
    public int GetRawMaterials()
    {
        return rawMaterials;
    }
    public int GetBuildingMaterials()
    {
        return buildingMaterials;
    }
    public int GetStability()
    {
        return stability;
    }

    public int GetMaxProvision()
    {
        return _maxProvision;
    }
    public int GetMaxMedicine()
    {
        return _maxMedicine;
    }
    public int GetMaxRawMaterials()
    {
        return _maxRawMaterials;
    }
    public int GetMaxBuildingMaterials()
    {
        return _maxBuildingMaterials;
    }
    public int GetMaxStability()
    {
        return _maxStability;
    }

    // Методы обновления значений ресурсов

    public void AddOrRemoveProvision(int value)
    {
        provision = Mathf.Clamp(provision + value, 0, 10);
        UpdateProvisionSlider();
    }

    public void AddOrRemoveMedicine(int value)
    {
        medicine = Mathf.Clamp(medicine + value, 0, 10);
        UpdateMedicineSlider();
    }

    public void AddOrRemoveRawMaterials(int value)
    {
        rawMaterials = Mathf.Clamp(rawMaterials + value, 0, 10);
        UpdateRawMaterialsSlider();
    }

    public void AddOrRemoveBuildingMaterials(int value)
    {
        buildingMaterials = Mathf.Clamp(buildingMaterials + value, 0, 10);
        UpdateBuildingMaterialsSlider();
    }

    public void AddOrRemoveStability(int value)
    {
        stability = Mathf.Clamp(stability + value, 0, 100);
        UpdateStabilitySlider();
    }

    // Методы обновления значений слайдеров

    private void UpdateProvisionSlider()
    {
        provisionSlider.value = provision;
    }

    private void UpdateMedicineSlider()
    {
        medicineSlider.value = medicine;
    }

    private void UpdateRawMaterialsSlider()
    {
        rawMaterialsSlider.value = rawMaterials;
    }

    private void UpdateBuildingMaterialsSlider()
    {
        buildingMaterialsSlider.value = buildingMaterials;
    }

    private void UpdateStabilitySlider()
    {
        stabilitySlider.value = stability;
        UpdateStabilityFillColor(); // Обновление цвета заливки
    }

    private void UpdateStabilityFillColor()
    {
        if (stabilityFillImage != null)
        {
            // Интерполяция цвета в зависимости от текущего значения стабильности
            float t = stability / 100f; // Нормализация значения (от 0 до 1)
            stabilityFillImage.color = Color.Lerp(minColor, maxColor, t);
        }
    }
}