using UnityEngine;
using UnityEngine.UI;

public class ResourceController : MonoBehaviour
{
    // Приватные переменные для ресурсов
    [SerializeField] private int provision = 0;
    [SerializeField] private int medicine = 0;
    [SerializeField] private int rawMaterials = 0;
    [SerializeField] private int buildingMaterials = 0;

    // Приватная переменная для стабильности
    [SerializeField] private int stability = 100;

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
    }

    // Инициализация слайдеров
    private void InitializeSliders()
    {
        // Слайдеры для ресурсов
        provisionSlider.maxValue = 5;
        provisionSlider.minValue = 0;
        provisionSlider.value = provision;
        provisionSlider.onValueChanged.AddListener(UpdateProvision);

        medicineSlider.maxValue = 5;
        medicineSlider.minValue = 0;
        medicineSlider.value = medicine;
        medicineSlider.onValueChanged.AddListener(UpdateMedicine);

        rawMaterialsSlider.maxValue = 5;
        rawMaterialsSlider.minValue = 0;
        rawMaterialsSlider.value = rawMaterials;
        rawMaterialsSlider.onValueChanged.AddListener(UpdateRawMaterials);

        buildingMaterialsSlider.maxValue = 5;
        buildingMaterialsSlider.minValue = 0;
        buildingMaterialsSlider.value = buildingMaterials;
        buildingMaterialsSlider.onValueChanged.AddListener(UpdateBuildingMaterials);

        // Слайдер для стабильности
        stabilitySlider.maxValue = 100;
        stabilitySlider.minValue = 0;
        stabilitySlider.value = stability;
        stabilitySlider.onValueChanged.AddListener(UpdateStability);
    }

    // Методы обновления значений ресурсов
    private void UpdateProvision(float value)
    {
        provision = Mathf.Clamp(Mathf.RoundToInt(value), 0, 5); // Ограничиваем значение от 0 до 5
    }

    private void UpdateMedicine(float value)
    {
        medicine = Mathf.Clamp(Mathf.RoundToInt(value), 0, 5); // Ограничиваем значение от 0 до 5
    }

    private void UpdateRawMaterials(float value)
    {
        rawMaterials = Mathf.Clamp(Mathf.RoundToInt(value), 0, 5); // Ограничиваем значение от 0 до 5
    }

    private void UpdateBuildingMaterials(float value)
    {
        buildingMaterials = Mathf.Clamp(Mathf.RoundToInt(value), 0, 5); // Ограничиваем значение от 0 до 5
    }

    // Метод обновления стабильности
    private void UpdateStability(float value)
    {
        stability = Mathf.Clamp(Mathf.RoundToInt(value), 0, 100); // Ограничиваем значение от 0 до 100
    }

    // Методы для получения значений ресурсов
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

    // Метод для получения значения стабильности
    public int GetStability()
    {
        return stability;
    }
}
