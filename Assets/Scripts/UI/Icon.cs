using UnityEngine;
using UnityEngine.EventSystems;

// для обработки событий мыши, наследуются IPointerEnterHandler и IPointerExitHandler
public class Icon : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public IconDelegate onMouseOverCustom;

    public void OnPointerEnter(PointerEventData eventData)
    {
        // так как в префабе иконки есть дочернии объекты,
        // проверяется является ли объект события объектом носителем данного скрипта
        // если нет, в качестве параметра делегата используется RectTransform родителя (т.е. объекта носителя скрипта)
        if (eventData.selectedObject == gameObject)
            onMouseOverCustom(GetComponent<RectTransform>(), true);
        else
            onMouseOverCustom(GetComponentInParent<RectTransform>(), true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (eventData.selectedObject == gameObject)
            onMouseOverCustom(GetComponent<RectTransform>(), false);
        else
            onMouseOverCustom(GetComponentInParent<RectTransform>(), false);
    }
}
