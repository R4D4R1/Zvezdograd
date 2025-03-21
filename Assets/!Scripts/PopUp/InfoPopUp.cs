using UnityEngine;
using TMPro;
using DG.Tweening;
using Zenject;

public class InfoPopUp : MonoBehaviour
{
    protected const float SCALE_DURATION = 0.25f;
    private const float FADE_DURATION = 0.25f;

    public TextMeshProUGUI LabelText;
    public TextMeshProUGUI DescriptionText;
    [SerializeField] private CanvasGroup _canvasGroup;

    public bool IsActive { get; protected set; } = false;

    protected ControllersManager _controllersManager;
    protected ResourceViewModel _resourceViewModel;
    protected PopUpFactory _popUpFactory;
    
    [Inject]
    public void Construct(ControllersManager controllersManager, ResourceViewModel resourceViewModel,PopUpFactory popUpFactory)
    {
        _controllersManager = controllersManager;
        _resourceViewModel = resourceViewModel;
        _popUpFactory = popUpFactory;
    }

    private void OnEnable()
    {
        IsActive = false;

        transform.localScale = Vector3.zero;
        SetAlpha(0);
    }

    public virtual void ShowPopUp()
    {

        transform.DOScale(Vector3.one, SCALE_DURATION).OnComplete(() =>
        {
            IsActive = true;

            SetAlpha(1);
        });
    }

    public void ShowPopUp(string Label, string Description)
    {
        IsActive = true;

        LabelText.text = "";
        DescriptionText.text = "";

        transform.DOScale(Vector3.one, SCALE_DURATION).OnComplete(() =>
        {
            LabelText.text = Label;
            DescriptionText.text = Description;
            SetAlpha(1);
        });
    }

    public virtual void HidePopUp()
    {
        if (IsActive)
        {
            transform.DOScale(Vector3.zero, SCALE_DURATION).OnComplete(() =>
            {
                IsActive = false;
            });

            _controllersManager.MainGameUIController.TurnOnUI();

            SetAlpha(0);
        }
    }

    public void HideStartInfoPopUpPopUp()
    {
        if (IsActive)
        {
            transform.DOScale(Vector3.zero, SCALE_DURATION).OnComplete(() =>
            {
                IsActive = false;
            });

            SetAlpha(0);
        }
    }

    protected void SetAlpha(float alpha)
    {
        _canvasGroup.DOFade(alpha, FADE_DURATION);
    }
}