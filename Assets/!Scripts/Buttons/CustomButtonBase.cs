using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

public abstract class CustomButtonBase : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler, IPointerExitHandler
{
    public virtual void OnPointerClick(PointerEventData eventData)
    {
        Bootstrapper.Instance?.SoundController?.PlayButtonPressSound();

    }

    public virtual void OnPointerEnter(PointerEventData eventData)
    {
        Bootstrapper.Instance?.SoundController?.PlayHoverSound();

    }

    public virtual void OnPointerExit(PointerEventData eventData)
    {

    }
}
