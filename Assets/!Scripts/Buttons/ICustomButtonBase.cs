using UnityEngine.EventSystems;

public interface ICustomButtonBase
{
    bool OnPointerClick(PointerEventData eventData);
    bool OnPointerEnter(PointerEventData eventData);
    bool OnPointerExit(PointerEventData eventData);
}