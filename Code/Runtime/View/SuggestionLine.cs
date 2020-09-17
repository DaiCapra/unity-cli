using System;
using UnityEngine;
using UnityEngine.UI;

namespace Cli.Code.Runtime.View
{
    public class SuggestionLine : MonoBehaviour
    {
        public string text;

        [SerializeField] private Image image;

        public void OnEnable()
        {
            Highlight(false);
        }

        public void Highlight(bool active)
        {
            image.gameObject.SetActive(active);
        }
    }
}