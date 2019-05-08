using UnityEngine;
using UnityEditor;

namespace GhostGen
{
    static class CreateCardResourceBank
    {
        [MenuItem("Assets/Create/Create Card Resource Bank")]
        public static void CreateCardBank()
        {
            ScriptableObjectUtility.CreateAsset<CardResourceBank>();
        }
    }
}