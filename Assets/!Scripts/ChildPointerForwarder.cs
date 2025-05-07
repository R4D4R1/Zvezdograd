using UnityEngine;
using UnityEngine.EventSystems;

public class ChildPointerForwarder : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public void OnPointerEnter(PointerEventData eventData)
    {
        transform.parent.GetComponent<SelectableBuilding>().OnPointerEnter(eventData);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        transform.parent.GetComponent<SelectableBuilding>().OnPointerExit(eventData);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        transform.parent.GetComponent<SelectableBuilding>().OnPointerClick(eventData);
    }
}
