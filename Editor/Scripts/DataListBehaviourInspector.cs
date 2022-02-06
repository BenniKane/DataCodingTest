using Immel.DataCodingTest.Runtime;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Immel.DataCodingTest.Editor
{
    [CustomEditor(typeof(DataListBehaviour))]
    public class DataListBehaviourInspector : UnityEditor.Editor
    {
        private List<bool> _toggles = new List<bool>();
        private SerializedProperty _dataValuesProperty;

        private void OnEnable()
        {
            _toggles.Clear();
            _dataValuesProperty = serializedObject.FindProperty("DataValues");
        
            for(var i = 0; i < _dataValuesProperty.arraySize; i++)
            {
                _toggles.Add(false);
            }
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            var removedIndex = -1;
            
            if (GUILayout.Button("Add Data Element"))
            {
                _toggles.Add(true);
                _dataValuesProperty.InsertArrayElementAtIndex(_dataValuesProperty.arraySize);
            }

            var centerLabelStyle = new GUIStyle(GUI.skin.label)
            {
                alignment = TextAnchor.MiddleCenter,
            };

            if (_dataValuesProperty.arraySize == 0)
            {
                EditorGUILayout.LabelField("No data members have been added, yet.", centerLabelStyle);
            }

            var removeButtonStyle = new GUIStyle(GUI.skin.button);
            removeButtonStyle.normal.textColor = Color.red;
            removeButtonStyle.normal.background = new Texture2D(0, 0);
            
            for (var i = 0; i < _dataValuesProperty.arraySize; i++)
            {
                var property = _dataValuesProperty.GetArrayElementAtIndex(i);
                
                EditorGUILayout.BeginHorizontal();

                _toggles[i] = EditorGUILayout.Foldout(_toggles[i], string.IsNullOrEmpty(property.FindPropertyRelative("StringValue").stringValue) ? $"Element {i+1}" : property.FindPropertyRelative("StringValue").stringValue, true);
                
                if (GUILayout.Button("X", removeButtonStyle, GUILayout.Width(30), GUILayout.Height(16)))
                {
                    removedIndex = i;
                }

                EditorGUILayout.EndHorizontal();
                
                EditorGUI.indentLevel++;

                if (_toggles[i])
                {
                    EditorGUILayout.PropertyField(property.FindPropertyRelative("IntegerValues"));
                    EditorGUILayout.PropertyField(property.FindPropertyRelative("VectorValue"));
                    EditorGUILayout.PropertyField(property.FindPropertyRelative("StringValue"));
                }

                EditorGUI.indentLevel--;
            }

            if (removedIndex >= 0)
            {
                _dataValuesProperty.DeleteArrayElementAtIndex(removedIndex);
                _toggles.RemoveAt(removedIndex);
            }


            serializedObject.ApplyModifiedProperties();
        }
    }
}