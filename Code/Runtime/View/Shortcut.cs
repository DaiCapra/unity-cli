using System;
using System.Linq;
using UnityEngine;

namespace Cli.Code.Runtime.View
{
    public class Shortcut
    {
        public Action Callback { get; set; }

        private readonly KeyCode[] _keys;

        public Shortcut(KeyCode key)
        {
            _keys = new[] {key};
        }

        public Shortcut(KeyCode[] keys)
        {
            _keys = keys;
        }

        public bool IsPressed()
        {
            return _keys != null && _keys.Any(Input.GetKeyDown);
        }
    }
}