using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class UnityEventsButton : CustomButtonBase
{
    [SerializeField] private UnityEvent OnHoverEnter;
    [SerializeField] private UnityEvent OnExitEvent;
    [SerializeField] private UnityEvent OnClickEvent;

    public override void OnPointerEnter(PointerEventData eventData)
    {
        base.OnPointerEnter(eventData);
        OnHoverEnter?.Invoke();
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        base.OnPointerExit(eventData);
        OnExitEvent?.Invoke();
    }

    public override void OnPointerClick(PointerEventData eventData)
    {
        base.OnPointerClick(eventData);
        OnClickEvent?.Invoke();
    }
}
