using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Cli.Code.Runtime.Controller;
using Cli.Code.Runtime.Model;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Cli.Code.Runtime.View
{
    public class CliView : MonoBehaviour
    {
        public Action CallbackButtonExit { get; set; }
        public Action CallbackToggleOn { get; set; }
        public Action CallbackToggleOff { get; set; }

        private CliService _cliService;

        public Shortcut shortCutSubmit;
        public Shortcut shortCutEscape;
        public Shortcut shortCutPrevious;
        public Shortcut shortCutNext;

        [SerializeField] private GameObject prefabLine;
        [SerializeField] private Transform parentLines;
        [SerializeField] private Transform parentContent;

        [Header("Components")] [SerializeField]
        private InputComponent input;

        [SerializeField] private ScrollComponent scrollComponent;
        [SerializeField] private SuggestionComponent suggestionComponent;
        [Space(10)] [SerializeField] private Button btnExit;
        [SerializeField] private Button btnSubmit;

        private bool _initializedCli;
        private bool _initializedComponents;
        private Shortcut[] _shortcuts;

        private string _suggestCache;
        private bool _ignoreCacheUpdates;
        private int _indexHistory;
        private bool _isBrowsingHistory;
        private bool _ignoreSuggestions;


        private void OnEnable()
        {
            InitComponents();
        }

        public void Init(CliService cli)
        {
            if (cli == null)
            {
                return;
            }

            _cliService = cli;
            _cliService.DelMessageAdded += OnMessageAdded;
            _cliService.DelMessagesCleared += OnMessagesCleared;

            _initializedCli = true;
        }

        private void InitComponents()
        {
            if (_initializedComponents)
            {
                return;
            }

            suggestionComponent.CallbackSelection = OnSuggestionSelected;
            prefabLine.gameObject.SetActive(false);

            input.DelTextChanged += OnInputChanged;
            btnExit.onClick.AddListener(OnExit);
            btnSubmit.onClick.AddListener(OnSubmit);

            shortCutEscape = new Shortcut(KeyCode.Escape) {Callback = OnEscape};
            shortCutSubmit = new Shortcut(new[] {KeyCode.Return, KeyCode.KeypadEnter}) {Callback = OnSubmit};
            shortCutPrevious = new Shortcut(KeyCode.UpArrow) {Callback = OnPrevious};
            shortCutNext = new Shortcut(new[] {KeyCode.Tab, KeyCode.DownArrow}) {Callback = OnNext};
            _shortcuts = new[] {shortCutEscape, shortCutSubmit, shortCutPrevious, shortCutNext};

            ClearInput();
            ClearMessages();
            ClearSuggestions();
            _initializedComponents = true;
        }


        private void OnDestroy()
        {
            if (_cliService == null)
            {
                return;
            }

            _cliService.DelMessagesCleared -= OnMessagesCleared;
            _cliService.DelMessageAdded -= OnMessageAdded;
        }

        public void Update()
        {
            if (_shortcuts == null)
            {
                return;
            }

            foreach (var shortcut in _shortcuts)
            {
                if (shortcut?.Callback == null)
                {
                    continue;
                }

                if (shortcut.IsPressed())
                {
                    shortcut.Callback.Invoke();
                }
            }
        }

        private void OnInputChanged()
        {
            if (!_ignoreCacheUpdates)
            {
                _suggestCache = input.Text;
                suggestionComponent.ResetIndex();
            }

            if (!_ignoreSuggestions)
            {
                ClearSuggestions();
                PopulateSuggestions();
            }
        }

        private void OnSuggestionSelected(string s)
        {
            _ignoreCacheUpdates = true;
            SelectSuggestion(s);
            _ignoreCacheUpdates = false;
        }


        public void OnSubmit()
        {
            var text = input.Text;
            if (string.IsNullOrEmpty(text))
            {
                return;
            }

            AddMessage(text, false);
            _cliService?.Execute(text);

            input.Clear();
            Focus();
        }

        public void Toggle(bool isEnabled)
        {
            if (parentContent == null)
            {
                return;
            }

            parentContent.gameObject.SetActive(isEnabled);
            if (isEnabled)
            {
                CallbackToggleOn?.Invoke();
            }
            else
            {
                CallbackToggleOff?.Invoke();
            }
        }

        public void Toggle()
        {
            if (parentContent == null)
            {
                return;
            }

            var isActive = parentContent.gameObject.activeSelf;
            Toggle(!isActive);
            if (!isActive)
            {
                Focus();
            }
        }

        private void Focus()
        {
            input.Select();
        }

        private void OnExit()
        {
            CallbackButtonExit?.Invoke();
        }

        private void OnNext()
        {
            NextSuggestion();
        }

        private void OnPrevious()
        {
            PrevSuggestion();
        }

        private void OnEscape()
        {
            ClearInput();
            ClearHistory();
        }

        private void ClearInput()
        {
            ClearSuggestions();
            input.Clear();
            Focus();

            _suggestCache = string.Empty;

            ClearHistory();
        }

        private void ClearHistory()
        {
            _isBrowsingHistory = false;
            _indexHistory = 0;
        }

        public void ClearMessages()
        {
            _cliService?.Clear();
        }

        private void OnMessagesCleared(Message message)
        {
            var lines = GetLines();
            foreach (var line in lines)
            {
                if (line != null && line.gameObject.activeSelf)
                {
                    Destroy(line.gameObject);
                }
            }
        }

        private void OnMessageAdded(Message message)
        {
            var g = Instantiate(prefabLine, parentLines);
            g.SetActive(true);

            var pre = message.Outbound ? $">" : $"<";
            var text = $"{pre} {message.Text}";

            g.GetComponent<TMP_Text>()?.SetText(text);
            scrollComponent.ForceUpdate();

            g.name = $"T: {text}";
        }

        private void SelectSuggestion(string suggestion)
        {
            var text = input.Text.ToLower();
            var tokens = TextService.GetTokens(text).ToList();
            var last = tokens.LastOrDefault();

            if (last != null)
            {
                tokens.RemoveAt(tokens.Count - 1);
            }

            var s = string.Join("", tokens).Trim();
            s = s.Length <= 0 ? $"{suggestion}" : $"{s} {suggestion}";
            input.SetText(s);
        }


        public void Write(string text)
        {
            input.SetText(text);
        }

        public void PopulateSuggestions()
        {
            if (!_initializedCli)
            {
                return;
            }

            var suggestions = _cliService.Suggest(_suggestCache);
            var hasSuggestions = suggestions.Count > 0;

            suggestionComponent.Toggle(hasSuggestions);
            foreach (var suggestion in suggestions)
            {
                suggestionComponent.Add(suggestion);
            }

            suggestionComponent.HighlightSelected();
        }

        public void ClearSuggestions()
        {
            suggestionComponent.Clear();
            suggestionComponent.Toggle(false);
        }

        public string GetText()
        {
            return input?.Text;
        }

        public void AddMessage(string text, bool outbound = true)
        {
            if (!_initializedCli)
            {
                return;
            }

            _cliService.AddMessage(new Message()
            {
                Text = text,
                Outbound = outbound
            });
        }

        public TMP_Text[] GetLines()
        {
            return parentLines.GetComponentsInChildren<Transform>()
                .Where(t => !t.Equals(parentLines))
                .Select(t => t.GetComponent<TMP_Text>())
                .Where(t => t != null)
                .ToArray();
        }

        public float Scroll
        {
            get => scrollComponent.ScrollValue;
            set => scrollComponent.ScrollValue = value;
        }

        public void NextSuggestion()
        {
            if (_isBrowsingHistory || string.IsNullOrEmpty(input.Text))
            {
                CycleHistory(-1);
            }
            else
            {
                suggestionComponent.Next();
            }
        }

        public void PrevSuggestion()
        {
            if (_isBrowsingHistory || string.IsNullOrEmpty(input.Text))
            {
                CycleHistory(1);
            }
            else
            {
                suggestionComponent.Prev();
            }
        }

        private void CycleHistory(int value)
        {
            if (!_initializedCli)
            {
                return;
            }

            if (_isBrowsingHistory)
            {
                _indexHistory += value;
            }

            _isBrowsingHistory = true;
            var history = _cliService.GetHistory();

            if (_indexHistory < 0)
            {
                _indexHistory = history.Count - 1;
            }
            else if (_indexHistory >= history.Count)
            {
                _indexHistory = 0;
            }

            var i = history.Count - 1 - _indexHistory;
            if (i < 0 || i >= history.Count)
            {
                return;
            }

            var m = history[i];

            _ignoreSuggestions = true;
            Write(m.Text);
            _ignoreSuggestions = false;
        }
    }
}