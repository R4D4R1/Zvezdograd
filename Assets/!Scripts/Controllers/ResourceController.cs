using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ResourceController : MonoBehaviour
{
    public int GetProvision() => provision;
    public int GetMedicine() => medicine;
    public int GetRawMaterials() => rawMaterials;
    public int GetReadyMaterials() => readyMaterials;
    public int GetStability() => stability;

    public int GetMaxProvision() => maxProvision;
    public int GetMaxMedicine() => maxMedicine;
    public int GetMaxRawMaterials() => maxRawMaterials;
    public int GetMaxReadyMaterials() => maxReadyMaterials;
    public int GetMaxStability() => maxStability;

    [SerializeField] private TextMeshProUGUI provisionText;
    [SerializeField] private TextMeshProUGUI medicineText;
    [SerializeField] private TextMeshProUGUI rawMaterialsText;
    [SerializeField] private TextMeshProUGUI readyMaterialsText;
    [SerializeField] private TextMeshProUGUI stabilityText;

    [SerializeField, Range(0, 10)] private int provision = 0;
    [SerializeField, Range(0, 10)] private int medicine = 0;
    [SerializeField, Range(0, 10)] private int rawMaterials = 0;
    [SerializeField, Range(0, 10)] private int readyMaterials = 0;
    [SerializeField, Range(0, 100)] private int stability = 100;

    [SerializeField, Range(0, 10)] private int maxProvision = 10;
    [SerializeField, Range(0, 10)] private int maxMedicine = 10;
    [SerializeField, Range(0, 10)] private int maxRawMaterials = 10;
    [SerializeField, Range(0, 10)] private int maxReadyMaterials = 10;
    [SerializeField, Range(0, 100)] private int maxStability = 100;

    [SerializeField] private Color stabilityColor0to25 = Color.red;
    [SerializeField] private Color stabilityColor26to75 = Color.green;
    [SerializeField] private Color stabilityColor76to100 = Color.blue;

    [SerializeField] private Image stabilityFillImage;

    [SerializeField] private Slider provisionSlider;
    [SerializeField] private Slider medicineSlider;
    [SerializeField] private Slider rawMaterialsSlider;
    [SerializeField] private Slider readyMaterialsSlider;
    [SerializeField] private Slider stabilitySlider;

    [SerializeField] private GameObject notificationPrefab;
    [SerializeField] private Transform notificationParent;

    private bool _isStabilityZero = false;

    public enum ResourceType
    {
        Provision,
        Medicine,
        RawMaterials,
        ReadyMaterials,
        Stability,
    }

    void Start()
    {
        InitializeSliders();
        UpdateAllUI();
        ControllersManager.Instance.timeController.OnNextTurnBtnPressed += NextTurnBtnPressed;
    }

    private void OnDestroy()
    {
        ControllersManager.Instance.timeController.OnNextTurnBtnPressed -= NextTurnBtnPressed;
    }

    private void NextTurnBtnPressed()
    {
        foreach (var building in ControllersManager.Instance.buildingController.RegularBuildings)
        {
            if (building.CurrentState == RepairableBuilding.State.Damaged && !_isStabilityZero)
            {
                ModifyResource(ResourceController.ResourceType.Stability, -2);
            }
        }
    }

    private void InitializeSliders()
    {
        SetupSlider(provisionSlider, maxProvision);
        SetupSlider(medicineSlider, maxMedicine);
        SetupSlider(rawMaterialsSlider, maxRawMaterials);
        SetupSlider(readyMaterialsSlider, maxReadyMaterials);
        SetupSlider(stabilitySlider, maxStability);
    }

    private void SetupSlider(Slider slider, int maxValue)
    {
        slider.maxValue = maxValue;
        slider.minValue = 0;
        slider.interactable = false;
    }

    private void UpdateAllUI()
    {
        UpdateSliderUI(provisionSlider, provision, maxProvision, provisionText, "œ–Œ¬»«»ﬂ");
        UpdateSliderUI(medicineSlider, medicine, maxMedicine, medicineText, "Ã≈ƒ» ¿Ã≈Õ“€");
        UpdateSliderUI(rawMaterialsSlider, rawMaterials, maxRawMaterials, rawMaterialsText, "—€–‹≈");
        UpdateSliderUI(readyMaterialsSlider, readyMaterials, maxReadyMaterials, readyMaterialsText, "—“–Œ…Ã¿“≈–»¿À€");
        UpdateStabilityUI();
    }

    private void UpdateSliderUI(Slider slider, int value, int maxValue, TextMeshProUGUI text, string label)
    {
        slider.value = value;
        text.text = $"{label}  {value}/{maxValue}";
    }

    private void UpdateStabilityUI()
    {
        stabilitySlider.value = stability;
        stabilityText.text = $"—“¿¡»À‹ÕŒ—“‹  {stability}/{maxStability}";
        UpdateStabilityFillColor();
    }

    private void UpdateStabilityFillColor()
    {
        if (stabilityFillImage != null)
        {
            if (stability <= 25)
            {
                stabilityFillImage.color = stabilityColor0to25;
            }
            else if (stability <= 75)
            {
                stabilityFillImage.color = stabilityColor26to75;
            }
            else
            {
                stabilityFillImage.color = stabilityColor76to100;
            }
        }
    }

    public void ModifyResource(ResourceType type, int changeValue)
    {
        ref int resource = ref provision;
        int maxValue = maxProvision;
        Slider slider = provisionSlider;
        TextMeshProUGUI text = provisionText;
        string label = "ÔÓ‚ËÁËˇ";
        bool isStability = false;

        switch (type)
        {
            case ResourceType.Provision:
                resource = ref provision;
                maxValue = maxProvision;
                slider = provisionSlider;
                text = provisionText;
                label = "ÔÓ‚ËÁËˇ";
                break;
            case ResourceType.Medicine:
                resource = ref medicine;
                maxValue = maxMedicine;
                slider = medicineSlider;
                text = medicineText;
                label = "ÏÂ‰ËÍ‡ÏÂÌÚ˚";
                break;
            case ResourceType.RawMaterials:
                resource = ref rawMaterials;
                maxValue = maxRawMaterials;
                slider = rawMaterialsSlider;
                text = rawMaterialsText;
                label = "Ò˚¸Â";
                break;
            case ResourceType.ReadyMaterials:
                resource = ref readyMaterials;
                maxValue = maxReadyMaterials;
                slider = readyMaterialsSlider;
                text = readyMaterialsText;
                label = "ÒÚÓÈÏ‡ÚÂË‡Î˚";
                break;
            case ResourceType.Stability:
                resource = ref stability;
                maxValue = maxStability;
                slider = stabilitySlider;
                text = stabilityText;
                label = "ÒÚ‡·ËÎ¸ÌÓÒÚ¸";
                isStability = true;
                break;
            default:
                return;
        }

        int oldValue = resource;
        resource = Mathf.Clamp(resource + changeValue, 0, maxValue);

        if (resource != oldValue)
        {
            string operation = changeValue > 0 ? "ƒÓ·‡‚ÎÂÌÓ" : "”·‡‚ÎÂÌÓ";
            CreateNotification($"{operation} {label} {Mathf.Abs(changeValue)}", changeValue > 0);
        }

        if (isStability)
        {
            UpdateStabilityUI();
            _isStabilityZero = (resource == 0);
            if (_isStabilityZero)
            {
                ControllersManager.Instance.mainGameController.GameLost();
            }
        }
        else
        {
            UpdateSliderUI(slider, resource, maxValue, text, label.ToUpper());
        }
    }


    private void CreateNotification(string message, bool isIncrease)
    {
        if (notificationPrefab == null || notificationParent == null) return;

        GameObject notification = Instantiate(notificationPrefab, notificationParent);
        TextMeshProUGUI notificationText = notification.GetComponentInChildren<TextMeshProUGUI>();

        if (notificationText != null)
        {
            notificationText.text = message;
            notificationText.color = isIncrease ? Color.green : Color.red;
        }
    }
}
