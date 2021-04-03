using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Proba
{
    /// <summary>listening to button OnClick</summary>
    public class ProbaButtonListener : MonoBehaviour, IMoveHandler, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler, ISelectHandler, IDeselectHandler, IEventSystemHandler

    {

        private void Clicked()
        {
            //PROBA.TapEvent(TapTypes.Tap, gameObject.name);
        }

        public void OnMove(AxisEventData eventData)
        {
            throw new System.NotImplementedException();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            throw new System.NotImplementedException();
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            throw new System.NotImplementedException();
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            throw new System.NotImplementedException();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            throw new System.NotImplementedException();
        }

        public void OnSelect(BaseEventData eventData)
        {
            throw new System.NotImplementedException();
        }

        public void OnDeselect(BaseEventData eventData)
        {
            throw new System.NotImplementedException();
        }
    }
}
