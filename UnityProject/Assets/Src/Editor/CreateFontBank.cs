using UnityEngine;
using UnityEditor;

namespace GhostGen
{

    static class CreateFontBank
    {
        [MenuItem("Assets/Create/Create Font Bank")]
        public static void CreateYourScriptableObject()
        {
            ScriptableObjectUtility.CreateAsset<FontBank>();
        }

    }

}