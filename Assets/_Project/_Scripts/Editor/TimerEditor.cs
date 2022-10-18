using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace MJM.HG
{
    [CustomEditor(typeof(Timer), true)]
    [CanEditMultipleObjects]
    public class TimerEditor : Editor
    {
        SerializedProperty count;

        void OnEnable()
        {
            count = serializedObject.FindProperty("_tickCount");
        }
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUILayout.PropertyField(count);
        }
    }
}
