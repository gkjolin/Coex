using UnityEngine;
using UnityEditor;

namespace IFGame.Lix
{
    [CustomEditor(typeof(CoexEngine))]
    class CoexEngineInspector : Editor
    {
        public override void OnInspectorGUI()
        {
            CoexEngine script = (CoexEngine)target;
            var bvrs = script.behavioursI;
            for (int i = 0; i < bvrs.Count; ++i)
            {
                EditorGUILayout.ObjectField(
                    string.Format("{0}.", i),
                    bvrs[i],
                    typeof(CoexBehaviour),
                    false);
            }
        }
    }
}