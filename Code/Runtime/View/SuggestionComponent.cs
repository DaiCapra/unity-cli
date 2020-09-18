using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Cli.Code.Runtime.View
{
    public class SuggestionComponent : MonoBehaviour
    {
        public Action<string> CallbackSelection { get; set; }

        [SerializeField] private GameObject prefabLine;
        [SerializeField] private Transform parentLines;
        [SerializeField] private ScrollComponent scrollComponent;

        private bool _initialized;


        private int index;
        private List<SuggestionLine> _lines;

        private void OnEnable()
        {
            if (!_initialized)
            {
                Init();
            }
        }

        private void Init()
        {
            _initialized = true;
            _lines = new List<SuggestionLine>();
            prefabLine.gameObject.SetActive(false);
            Clear();
            ResetIndex();
        }

        public void Clear()
        {
            if (!_initialized)
            {
                return;
            }
            _lines.Clear();
            if (parentLines == null)
            {
                return;
            }

            foreach (var child in parentLines.GetComponentsInChildren<Transform>())
            {
                if (child.Equals(parentLines))
                {
                    continue;
                }

                if (child.gameObject.activeSelf)
                {
                    Destroy(child.gameObject);
                }
            }
        }

        public void ResetIndex()
        {
            index = -1;
        }

        public void Add(string text)
        {
            if (parentLines == null)
            {
                return;
            }

            var g = Instantiate(prefabLine, parentLines.transform);
            g.SetActive(true);
            g.GetComponentInChildren<TMP_Text>()?.SetText(text);
            g.name = $"T: {text}";
            var line = g.GetComponentInChildren<SuggestionLine>();
            line.text = text;
            _lines.Add(line);
        }

        public void Next()
        {
            index++;
            if (index >= _lines.Count)
            {
                index = 0;
            }

            Select(index);
        }

        private void Select(int i)
        {
            index = (int) Mathf.Clamp(i, 0, _lines.Count - 1);
            if (_lines.Count == 0)
            {
                return;
            }

            var line = _lines[index];
            scrollComponent.ScrollTo(index);
            CallbackSelection?.Invoke(line.text);
        }

        public void HighlightSelected()
        {
            _lines.ForEach(t => t.Highlight(false));

            if (index >= 0 && index < _lines.Count)
            {
                var line = _lines[index];
                line.Highlight(true);
            }

        }

        public void Prev()
        {
            index--;
            if (index < 0)
            {
                index = _lines.Count;
            }

            Select(index);
        }

        public string GetSelected()
        {
            if (index < 0 || index >= _lines.Count)
            {
                return string.Empty;
            }

            return _lines[index].text;
        }

        public void Toggle(bool isEnabled)
        {
            transform.GetChild(0).gameObject.SetActive(isEnabled);
        }
    }
}