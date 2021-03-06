﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GhostGen
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(Text))]
    public class TextHelper : MonoBehaviour
    {

        [SerializeField]
        public FontType fontType = FontType.NormalText;

        private Text _text;
        private FontType _dirtyFontType = FontType.None;
        private FontManager _fontManager;

        // Use this for initialization
        void Awake()
        {
            _fontManager = Singleton.instance.fontManager;
            _text = GetComponent<Text>();
            _text.verticalOverflow = VerticalWrapMode.Overflow;
            _setTextParameters(_fontManager.GetFont(fontType));
        }

        private void _setTextParameters(Font font)
        {
            if(font == null)
            {
                return;
            }
            _text.font = font;
            _text.fontSize = font.fontSize;
        }

#if UNITY_EDITOR
        public void Update()
        {
            if (fontType != _dirtyFontType)
            {
                _setTextParameters(_fontManager.GetFont(fontType));
            }
            _dirtyFontType = fontType;
        }
#endif
    }
}
