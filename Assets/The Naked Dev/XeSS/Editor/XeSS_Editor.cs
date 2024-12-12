using UnityEngine;
using UnityEditor;

namespace TND.XeSS
{
    [CustomEditor(typeof(XeSS_Base), editorForChildClasses: true)]
    public class XeSS_Editor : Editor
    {
        public override void OnInspectorGUI() {
#if !TND_HDRP_EDITEDSOURCE && UNITY_HDRP
            EditorGUILayout.LabelField("----- HDRP Upscaling requires Source File edits. Please read the 'Quick Start: HDRP' chapter in the documentation. ------", EditorStyles.boldLabel);
            if (GUILayout.Button("I have edited the source files!"))
            {
                PipelineDefines.AddDefine("TND_HDRP_EDITEDSOURCE");
                AssetDatabase.Refresh();
            }
#else

#if !UNITY_STANDALONE_WIN || !UNITY_64
            EditorGUILayout.LabelField("----- XeSS is not supported on this platform ------", EditorStyles.boldLabel);
#endif
            XeSS_Base xessScript = target as XeSS_Base;

            EditorGUI.BeginChangeCheck();

            EditorGUILayout.LabelField("XeSS Settings", EditorStyles.boldLabel);
            XeSS_Quality xessQuality = (XeSS_Quality)EditorGUILayout.EnumPopup(Styles.qualityText, xessScript.OnGetQuality());

            float antiGhosting = EditorGUILayout.Slider(Styles.antiGhostingText, xessScript.OnGetAntiGhosting(), 0.0f, 1.0f);

            bool sharpening = EditorGUILayout.Toggle(Styles.sharpeningText, xessScript.sharpening);
            float sharpness = xessScript.sharpness;
            if (xessScript.sharpening)
            {
                EditorGUI.indentLevel++;
                sharpness = EditorGUILayout.Slider(Styles.sharpnessText, xessScript.sharpness, 0.0f, 1.0f);
                EditorGUI.indentLevel--;
            }

            EditorGUILayout.Space();
            EditorGUILayout.Space();

            if(EditorGUI.EndChangeCheck()) {
                EditorUtility.SetDirty(xessScript);

                Undo.RecordObject(target, "Changed Area Of Effect");
                xessScript.OnSetQuality(xessQuality);
                xessScript.OnSetAntiGhosting(antiGhosting);
                xessScript.sharpening = sharpening;
                xessScript.sharpness = sharpness;
            }
#endif
        }

        private static class Styles
        {
            public static GUIContent qualityText = new GUIContent("Quality", "Quality 1.5, Balanced 1.7, Performance 2, Ultra Performance 3");
            public static GUIContent antiGhostingText = new GUIContent("Anti-Ghosting", "The Anti-Ghosting value between 0 and 1, where 0 is no additional anti-ghosting and 1 is maximum additional Anti-Ghosting.");
            public static GUIContent sharpeningText = new GUIContent("Sharpening", "Enable an additional sharpening in the XeSS algorithm.");
            public static GUIContent sharpnessText = new GUIContent("Sharpness", "The sharpness value between 0 and 1, where 0 is no additional sharpness and 1 is maximum additional sharpness.");

        }
    }
}
