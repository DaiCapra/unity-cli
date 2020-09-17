using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Cli.Code.Runtime.View
{
    public class DragComponent : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
    {
        public Action<Vector2> CallbackPosition;
        private Canvas _canvas;
        private RectTransform _canvasRect;
        private Vector2 _pointerOffset;
        private RectTransform _rectTransform;
        private bool _initialized;


        public void OnEnable()
        {
            if (!_initialized)
            {
                _initialized = true;
                _canvas = GetComponentInParent<Canvas>();
                _canvasRect = _canvas.GetComponent<RectTransform>();
                _rectTransform = GetComponent<RectTransform>();
            }
        }

        public void OnDrag(PointerEventData data)
        {
            if (_rectTransform == null)
                return;

            Vector2 pointerPostion = ClampToWindow(data);

            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
                _canvasRect.GetComponent<RectTransform>(), pointerPostion, data.pressEventCamera,
                out var localPointerPosition
            ))
            {
                var p = localPointerPosition - _pointerOffset;
                SetPosition(p);
            }
        }

        public void SetPosition(Vector2 pos)
        {
            _rectTransform.localPosition = pos;
        }

        public virtual void OnPointerDown(PointerEventData data)
        {
            _rectTransform.SetAsLastSibling();
            RectTransformUtility.ScreenPointToLocalPointInRectangle(_rectTransform, data.position,
                data.pressEventCamera,
                out _pointerOffset);
        }

        private Vector2 ClampToWindow(PointerEventData data)
        {
            Vector2 rawPointerPosition = data.position;

            Vector3[] canvasCorners = new Vector3[4];
            _canvasRect.GetComponent<RectTransform>().GetWorldCorners(canvasCorners);

            float clampedX = Mathf.Clamp(rawPointerPosition.x, canvasCorners[0].x, canvasCorners[2].x);
            float clampedY = Mathf.Clamp(rawPointerPosition.y, canvasCorners[0].y, canvasCorners[2].y);

            Vector2 newPointerPosition = new Vector2(clampedX, clampedY);
            return newPointerPosition;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            CallbackPosition?.Invoke(_rectTransform.localPosition);
        }
    }
}