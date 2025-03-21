using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class ResourceView : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI provisionText;
    [SerializeField] private TextMeshProUGUI medicineText;
    [SerializeField] private TextMeshProUGUI rawMaterialsText;
    [SerializeField] private TextMeshProUGUI readyMaterialsText;
    [SerializeField] private TextMeshProUGUI stabilityText;
    [SerializeField] private Slider provisionSlider;
    [SerializeField] private Slider medicineSlider;
    [SerializeField] private Slider rawMaterialsSlider;
    [SerializeField] private Slider readyMaterialsSlider;
    [SerializeField] private Slider stabilitySlider;

    private ResourceViewModel _viewModel;
    private PopUpFactory _popUpFactory;
    private ResourcesConfig _resourcesConfig;

    [Inject]
    public void Construct(ResourceViewModel viewModel, PopUpFactory popUpFactory, ResourcesConfig resourcesConfig)
    {
        _viewModel = viewModel;
        _popUpFactory = popUpFactory;
        _resourcesConfig = resourcesConfig;
        BindUI();
    }

    private void BindUI()
    {
        provisionSlider.maxValue = _viewModel.Model.MaxProvision;
        medicineSlider.maxValue = _viewModel.Model.MaxMedicine;
        rawMaterialsSlider.maxValue = _viewModel.Model.MaxRawMaterials;
        readyMaterialsSlider.maxValue = _viewModel.Model.MaxReadyMaterials;
        stabilitySlider.maxValue = _viewModel.Model.MaxStability;

        SubscribeToResource(_viewModel.Provision, provisionSlider, provisionText, "Провизия", _viewModel.Model.MaxProvision);
        SubscribeToResource(_viewModel.Medicine, medicineSlider, medicineText, "Медикаменты", _viewModel.Model.MaxMedicine);
        SubscribeToResource(_viewModel.RawMaterials, rawMaterialsSlider, rawMaterialsText, "Сырье", _viewModel.Model.MaxRawMaterials);
        SubscribeToResource(_viewModel.ReadyMaterials, readyMaterialsSlider, readyMaterialsText, "Стройматериалы", _viewModel.Model.MaxReadyMaterials);
        SubscribeToResource(_viewModel.Stability, stabilitySlider, stabilityText, "Стабильность", _viewModel.Model.MaxStability);
    }

    private void SubscribeToResource(IReadOnlyReactiveProperty<int> resource, Slider slider, TextMeshProUGUI text, string name, int maxValue)
    {
        resource.Subscribe(newValue =>
        {
            int previousValue = (int)slider.value;

            slider.value = newValue;
            text.text = $"{name} {newValue}/{maxValue}";

            int difference = newValue - previousValue;
            if (difference != 0)
            {
                if (slider == stabilitySlider)
                {
                    Color newColor = _resourcesConfig.GetStabilityColor(newValue);
                    stabilitySlider.fillRect.GetComponent<Image>().color = newColor;
                }

                string action = difference > 0 ? "Добавлено" : "Убавлено";
                string message = $"{action} {name.ToLower()} {Mathf.Abs(difference)}";
                _popUpFactory.CreateNotification(message, difference > 0);
            }
        }).AddTo(this);
    }
}