using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

public abstract class CustomButtonBase : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler, IPointerExitHandler
{
    private SoundController _soundController;

    [Inject]
    public void Construct(SoundController soundController)
    {
        _soundController = soundController;
    }

    public virtual void OnPointerClick(PointerEventData eventData)
    {
        _soundController?.PlayButtonPressSound();
    }

    public virtual void OnPointerEnter(PointerEventData eventData)
    {
        _soundController?.PlayHoverSound();
    }

    public virtual void OnPointerExit(PointerEventData eventData)
    {
    }
}