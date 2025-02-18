using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class CustomHoverUnityAction : CustomButtonBase
{
    [SerializeField] private UnityEvent CustomHoverEvent;

    public override void OnPointerEnter(PointerEventData eventData)
    {
        CustomHoverEvent.Invoke();
    }
}
