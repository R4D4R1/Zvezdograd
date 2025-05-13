using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;
using UniRx;

public class SpeedTreeColorChanger : MonoBehaviour
{
    // Shader property names
    private const string MAIN_COLOR_PROPERTY = "_Color";
    private const string HUE_VARIATION_PROPERTY = "_HueVariation";

    [Header("Material Reference")]
    [SerializeField] private Material speedTreeMaterial;

    [Header("Regular Colors")]
    [SerializeField] private Color regularMainColor = Color.green;
    [SerializeField] private Color regularHueVariation = new Color(0.5f, 0.5f, 0.5f);

    [Header("Snow Colors")]
    [SerializeField] private Color snowMainColor = Color.white;
    [SerializeField] private Color snowHueVariation = new Color(0.8f, 0.8f, 0.8f);

    [Inject] private EventController _eventController;


    private void Start()
    {
        _eventController.OnSnowStarted
            .Subscribe(_ => SetSnowTree(true))
            .AddTo(this);

        var saveLoadController = FindFirstObjectByType<SaveLoadController>();
        saveLoadController.OnSnowChangeState
            .Subscribe(SetSnowTree)
            .AddTo(this);

        saveLoadController.StartNewGame();
    }

    /// <summary>
    /// Applies foliage colors to the SpeedTree material based on season.
    /// </summary>
    /// <param name="isSnow">If true, apply snow colors; otherwise, apply regular colors.</param>
    private void SetSnowTree(bool isSnow)
    {
        if (speedTreeMaterial == null)
        {
            Debug.LogWarning("SpeedTree material not assigned.");
            return;
        }

        Color mainColor = isSnow ? snowMainColor : regularMainColor;
        Color hueVariation = isSnow ? snowHueVariation : regularHueVariation;

        speedTreeMaterial.SetColor(MAIN_COLOR_PROPERTY, mainColor);
        speedTreeMaterial.SetColor(HUE_VARIATION_PROPERTY, hueVariation);
    }
}
