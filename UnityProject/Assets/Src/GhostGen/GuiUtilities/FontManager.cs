using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GhostGen
{
    
    [SerializeField]
    public enum FontType
    {
        None = 0,
        BigHeader,
        SmallHeader,
        NormalText,
    }
    

    [DisallowMultipleComponent]
    public class FontManager : MonoBehaviour
    {
        public FontBank fontBank;
        public static FontBank sFontBank { get; private set; }

        public void Awake()
        {
            _setupFonts();
        }


        public static Font GetFont(FontType type)
        {
            if(sFontBank == null)
            {
                sFontBank = _getOrLoadDefaultFontBank();
            }
            switch (type)
            {
                case FontType.BigHeader:    return sFontBank.bigHeaderFont;
                case FontType.SmallHeader:  return sFontBank.smallHeaderFont;
                case FontType.NormalText:   return sFontBank.normalTextFont;
            }

            Debug.LogError(string.Format("Font Type {0} not supported!", type));
            return null;
        }

        private void _setupFonts()
        {
            fontBank = _getOrLoadDefaultFontBank();
            Debug.Assert(fontBank != null, "Font Bank could not be found!");
            sFontBank = fontBank;
        }

        private static FontBank _getOrLoadDefaultFontBank()
        {
            return Resources.Load<FontBank>("FontBanks/DefaultFontBank");
        }
    }
}
