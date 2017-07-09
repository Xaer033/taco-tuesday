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

    [CreateAssetMenu]
    public class FontManager : ScriptableObject, IPostInit
    {
        public FontBank fontBank;

        public void PostInit()
        {
            if (fontBank == null)
            {
                fontBank = _getDefaultFontBank();
            }
        }


        public Font GetFont(FontType type)
        {
            switch (type)
            {
                case FontType.BigHeader:    return fontBank.bigHeaderFont;
                case FontType.SmallHeader:  return fontBank.smallHeaderFont;
                case FontType.NormalText:   return fontBank.normalTextFont;
            }

            Debug.LogError(string.Format("Font Type {0} not supported!", type));
            return null;
        }
        
        private FontBank _getDefaultFontBank()
        {
            return Resources.Load<FontBank>("FontBanks/DefaultFontBank");
        }
    }
}
