using UnityEngine;
using TMPro;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections;
using Cysharp.Threading.Tasks;
using System;

public class InfoPopUp : MonoBehaviour
{
    [SerializeField] protected Image _bgImage;
    [SerializeField] protected float scaleDuration = 0.2f;
    [SerializeField] protected float fadeDuration = 0.2f;
    [SerializeField] protected float scaleDownDuration = 0.1f;

    public TextMeshProUGUI LabelText;
    public TextMeshProUGUI DescriptionText;
    [SerializeField] private CanvasGroup _canvasGroup;

    protected bool _isDestroyable = true;

    public bool IsActive { get; protected set; } = false;

    private void OnEnable()
    {
        IsActive = false;

        _bgImage.transform.localScale = Vector3.zero;
        SetAlpha(0);
    }

    public virtual void ShowPopUp()
    {
        IsActive = true;

        _bgImage.transform.DOScale(Vector3.one, scaleDuration).OnComplete(() =>
        {
            SetAlpha(1);
        });
    }

    public void ShowPopUp(string Label, string Description)
    {
        IsActive = true;

        LabelText.text = "";
        DescriptionText.text = "";

        _bgImage.transform.DOScale(Vector3.one, scaleDuration).OnComplete(() =>
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

                _bgImage.transform.DOScale(Vector3.zero, scaleDownDuration).OnComplete(async () =>
                {
                    IsActive = false;

                    await UniTask.Delay(1000);
                    Destroy(gameObject);
                });
            }
            else
            {
                _bgImage.transform.DOScale(Vector3.zero, scaleDownDuration).OnComplete(() =>
                {
                    IsActive = false;
                });

                ControllersManager.Instance.mainGameUIController.EnableEscapeMenuToggle();
                ControllersManager.Instance.mainGameUIController.TurnOnUI();

                SetAlpha(0);
            }
        }
    }


    protected void SetAlpha(float alpha)
    {
        _canvasGroup.DOFade(alpha, fadeDuration);
    }
}