using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Cli.Code.Runtime.View
{
    public class InputComponent : MonoBehaviour
    {
        public delegate void InputHandler();

        public InputHandler DelTextChanged;

        private TMP_InputField _input;
        public string Text => _input != null ? _input.text : string.Empty;

        private bool ignoreUpdates;

        private void OnEnable()
        {
            if (!_input)
            {
                _input = GetComponent<TMP_InputField>();
                _input.onValueChanged.AddListener(OnChanged);
            }
        }

        private void OnChanged(string arg)
        {
            if (ignoreUpdates)
            {
                return;
            }

            DelTextChanged?.Invoke();
        }

        public void Clear()
        {
            if (_input != null)
            {
                _input.text = "";
                _input.caretPosition = 0;
            }
        }

        public void Select()
        {
            if (_input == null)
            {
                return;
            }

            var eventSystem = EventSystem.current;
            if (eventSystem != null)
            {
                if (!eventSystem.alreadySelecting)
                {
                    eventSystem.SetSelectedGameObject(null);
                }

                eventSystem.SetSelectedGameObject(_input.gameObject);
                _input?.Select();
            }
        }

        public void SetText(string s)
        {
            ignoreUpdates = true;
            Clear();
            ignoreUpdates = false;

            if (_input != null)
            {
                _input.text = s;
                _input.caretPosition = s.Length;
            }
        }
    }
}