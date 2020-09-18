using System;
using UnityEngine;

namespace Cli.Code.Runtime.View
{
    public class FontComponent : MonoBehaviour
    {
        public FontSize fontSize;
    }

    public enum FontSize
    {
        Small,
        Normal,
    }
}