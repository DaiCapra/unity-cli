using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Cli.Code.Runtime.View
{
    public class ResizePanel : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
    {
        public Action<Vector2> CallbackResize { get; set; }

        public Vector2 minSize = new Vector2(100, 100);
        public Vector2 maxSize = new Vector2(400, 400);

        private RectTransform panelRectTransform;
        private Vector2 originalLocalPointerPosition;
        private Vector2 originalSizeDelta;
        
        private bool initialized;

        void OnEnable()
        {
            Init();
        }

        public void Init()
        {
            if (initialized)
            {
                return;
            }

            initialized = true;
            panelRectTransform = transform.parent.GetComponent<RectTransform>();
        }

        public void OnPointerDown(PointerEventData data)
        {
            originalSizeDelta = panelRectTransform.sizeDelta;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(panelRectTransform, data.position,
                data.pressEventCamera, out originalLocalPointerPosition);
        }

        public void OnDrag(PointerEventData data)
        {
            if (panelRectTransform == null)
                return;

            Vector2 localPointerPosition;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(panelRectTransform, data.position,
                data.pressEventCamera, out localPointerPosition);
            Vector3 offsetToOriginal = localPointerPosition - originalLocalPointerPosition;

            Vector2 sizeDelta = originalSizeDelta + new Vector2(offsetToOriginal.x, -offsetToOriginal.y);
            sizeDelta = new Vector2(
                Mathf.Clamp(sizeDelta.x, minSize.x, maxSize.x),
                Mathf.Clamp(sizeDelta.y, minSize.y, maxSize.y)
            );

            SetSize(sizeDelta);
        }


        public void SetSize(Vector2 sizeDelta)
        {
            if (panelRectTransform == null)
            {
                return;
            }

            panelRectTransform.sizeDelta = sizeDelta;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            OnResize();
        }

        public void OnResize()
        {
            if (panelRectTransform == null)
            {
                return;
            }

            CallbackResize?.Invoke(panelRectTransform.sizeDelta);
        }
    }
}