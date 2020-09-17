using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Cli.Code.Runtime.View
{
    public class ScrollComponent : MonoBehaviour
    {
        public float ScrollValue
        {
            get => _scrollRect.verticalScrollbar.value;
            set => UpdateScrollValue(value);
        }

        private void UpdateScrollValue(float value)
        {
            var v = Mathf.Clamp01(value);
            _scrollRect.verticalScrollbar.value = v;
            _scrollRect.onValueChanged?.Invoke(new Vector2(0, v));
        }

        [SerializeField] private Transform parentContent;
        [SerializeField] private bool autoScroll = true;
        private ScrollRect _scrollRect;
        private float _prevValue;
        private float _prevSize;


        private void OnEnable()
        {
            if (!_scrollRect)
            {
                _scrollRect = GetComponent<ScrollRect>();
                _scrollRect.onValueChanged.AddListener(OnChanged);
            }
        }

        private void OnChanged(Vector2 v)
        {
            if (autoScroll)
            {
                AutoScrollDown();
            }
        }

        public void ForceUpdate()
        {
            UpdateScrollValue(ScrollValue);
        }

        private void AutoScrollDown()
        {
            var vertical = _scrollRect.verticalScrollbar;
            float size = vertical.size;


            // has changed        
            if (Math.Abs(size - _prevSize) > 0.0000005f)
            {
                if (_prevValue <= 0.05f || _prevValue >= 1f || _prevSize >= 1)
                {
                    vertical.value = 0;
                }
            }

            _prevSize = vertical.size;
            _prevValue = vertical.value;
        }

        // Todo consider viewport
        public void ScrollTo(int index)
        {
            //Canvas.ForceUpdateCanvases();
            var children = parentContent.GetComponentsInChildren<Transform>()
                .Where(t => !t.Equals(parentContent))
                .OfType<RectTransform>()
                .ToArray();

            if (children.Length <= 0)
            {
                return;
            }

            index = Mathf.Clamp(index, 0, children.Length);
            var totalHeight = children.Sum(t => t.sizeDelta.y);
            float targetY = 0;

            for (int i = 0; i < index; i++)
            {
                targetY += children[i].sizeDelta.y;
            }

            float n = targetY / totalHeight;
            ScrollValue = 1 - n;
        }
    }
}