using System;
using TMPro;
using UnityEngine;

namespace Cli.Code.Runtime.View
{
    [ExecuteInEditMode]
    public class FontController : MonoBehaviour
    {
        [SerializeField] private int fontSizeSmall = 14;
        [SerializeField] private int fontSizeNormal = 16;

        private void OnValidate()
        {
            var fontComponents = GetComponentsInChildren<FontComponent>();
            foreach (var fontComponent in fontComponents)
            {
                var t = fontComponent?.GetComponent<TMP_Text>();
                if (t == null)
                {
                    continue;
                }

                switch (fontComponent.fontSize)
                {
                    case FontSize.Small:
                        SetFontSize(t, fontSizeSmall);
                        break;
                    case FontSize.Normal:
                        SetFontSize(t, fontSizeNormal);
                        break;
                }
            }
        }

        private void SetFontSize(TMP_Text text, int size)
        {
            if (text.fontSize != size)
            {
                text.fontSize = size;
            }
        }
    }
}