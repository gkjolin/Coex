using UnityEngine;
using UnityEditor;

namespace IFGame.Lix
{
    [CustomEditor(typeof(CoexBehaviour), true)]
    class CoexBehaviourInspector : Editor
    {
        bool foldoutList;

        public override bool RequiresConstantRepaint()
        {
            return foldoutList;
        }

        public override void OnInspectorGUI()
        {
            CoexBehaviour script = (CoexBehaviour)target;
            float labelWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = 95f;
            EditorGUILayout.Toggle("addToEngine", script.addToEngineI);
            if (foldoutList = EditorGUILayout.Foldout(foldoutList, "coroutines"))
            {
                ++EditorGUI.indentLevel;
                for (int i = 0; i < script.coexsI.Count; ++i)
                {
                    Coex c = script.coexsI[i];
                    if (c.foldout = EditorGUILayout.Foldout(c.foldout, c.routineNameI))
                    {
                        if (null != c.returnValueTypeI)
                            EditorGUILayout.LabelField("return type:", c.returnValueTypeI.FullName);
                        if (null != c.returnValueI)
                            EditorGUILayout.LabelField("last return:", c.returnValueI.ToString());
                        EditorGUILayout.LabelField("yield count:", c.yieldCount.ToString());
                    }
                }
                --EditorGUI.indentLevel;
            }
            EditorGUIUtility.labelWidth = labelWidth;
        }
    }
}