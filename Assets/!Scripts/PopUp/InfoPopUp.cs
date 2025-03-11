using UnityEngine;
using TMPro;
using DG.Tweening;
using Cysharp.Threading.Tasks;
using Zenject;

public class InfoPopUp : MonoBehaviour
{
    [SerializeField] protected float scaleDuration = 0.2f;
    [SerializeField] protected float fadeDuration = 0.2f;
    [SerializeField] protected float scaleDownDuration = 0.1f;

    public TextMeshProUGUI LabelText;
    public TextMeshProUGUI DescriptionText;
    [SerializeField] private CanvasGroup _canvasGroup;

    protected bool _isDestroyable = true;

    public bool IsActive { get; protected set; } = false;

    protected ControllersManager _controllersManager;
    protected ResourceViewModel _resourceViewModel;

    [Inject]
    public void Construct(ControllersManager controllersManager, ResourceViewModel resourceViewModel)
    {
        _controllersManager = controllersManager;
        _resourceViewModel = resourceViewModel;
    }

    private void OnEnable()
    {
        IsActive = false;

        transform.localScale = Vector3.zero;
        SetAlpha(0);
    }

    public virtual void ShowPopUp()
    {
        IsActive = true;

        transform.DOScale(Vector3.one, scaleDuration).OnComplete(() =>
        {
            SetAlpha(1);
        });
    }

    public void ShowPopUp(string Label, string Description)
    {
        IsActive = true;

        LabelText.text = "";
        DescriptionText.text = "";

        transform.DOScale(Vector3.one, scaleDuration).OnComplete(() =>
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
            if (_isDestroyable)
            {

                transform.DOScale(Vector3.zero, scaleDownDuration).OnComplete(async () =>
                {
                    IsActive = false;

                    await UniTask.Delay(1000);
                    Destroy(gameObject);
                });
            }
            else
            {
                transform.DOScale(Vector3.zero, scaleDownDuration).OnComplete(() =>
                {
                    IsActive = false;
                });

                _controllersManager.MainGameUIController.EnableEscapeMenuToggle();
                _controllersManager.MainGameUIController.TurnOnUI();

                SetAlpha(0);
            }
        }
    }


    protected void SetAlpha(float alpha)
    {
        _canvasGroup.DOFade(alpha, fadeDuration);
    }
}