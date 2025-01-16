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

    public bool IsActive { get; protected set; } = false;

    private void OnEnable()
    {
        _bgImage.transform.localScale = Vector3.zero;
        SetAlpha(0);
    }

    public virtual void ShowPopUp()
    {
        IsActive = true;

        _bgImage.transform.DOScale(Vector3.one, scaleDuration).OnComplete(() =>
        {
            SetAlpha(1);

            ControllersManager.Instance.mainGameUIController.InPopUp(this);
        });
    }

    public void ShowPopUp(string Label, string Description)
    {
        ControllersManager.Instance.mainGameUIController.InPopUp(this);

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
            ControllersManager.Instance.mainGameUIController.Running();

            _bgImage.transform.DOScale(Vector3.zero, scaleDownDuration).OnComplete(async () =>
            {
                IsActive = false;
                ControllersManager.Instance.mainGameUIController.InGame();

                await UniTask.Delay(1000);
                Destroy(gameObject);
            });
        }
    }


    protected void SetAlpha(float alpha)
    {
        _canvasGroup.DOFade(alpha, fadeDuration);
    }
}