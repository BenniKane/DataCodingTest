using Immel.DataCodingTest.Runtime;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Immel.DataCodingTest.Editor
{
    [CustomEditor(typeof(DataListBehaviour))]
    public class DataListBehaviourInspector : UnityEditor.Editor
    {
        private SerializedProperty _dataValuesProperty;

        // Cache our data value members/properties
        private void OnEnable()
        {
            _dataValuesProperty = serializedObject.FindProperty("DataValues");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            var removedIndex = -1;
            
            // Putting the add data element button at the top, because I hate it when clicking buttons to add items moves in the event I want to add multiple. Keeping it at the top
            // will prevent it from shifting
            if (GUILayout.Button("Add Data Element"))
            {
                _dataValuesProperty.InsertArrayElementAtIndex(_dataValuesProperty.arraySize);
            }

            // I like centered text
            var centerLabelStyle = new GUIStyle(GUI.skin.label)
            {
                alignment = TextAnchor.MiddleCenter,
            };

            // If we have no data members yet, let the user know, so they don't think something just failed to load/draw
            if (_dataValuesProperty.arraySize == 0)
            {
                EditorGUILayout.LabelField("No data members have been added, yet.", centerLabelStyle);
            }

            // Style up our remove button a little
            var removeButtonStyle = new GUIStyle(GUI.skin.button);
            removeButtonStyle.normal.textColor = Color.red;
            removeButtonStyle.normal.background = new Texture2D(0, 0);
            
            for (var i = 0; i < _dataValuesProperty.arraySize; i++)
            {
                var property = _dataValuesProperty.GetArrayElementAtIndex(i);
                
                // Horizontal group, to make sure that the element main line and remove button are in-line with eachother
                EditorGUILayout.BeginHorizontal();

                property.isExpanded = EditorGUILayout.Foldout(property.isExpanded, string.IsNullOrEmpty(property.FindPropertyRelative("StringValue").stringValue) ? $"Element {i+1}" : property.FindPropertyRelative("StringValue").stringValue, true);
                
                if (GUILayout.Button("X", removeButtonStyle, GUILayout.Width(30), GUILayout.Height(16)))
                {
                    removedIndex = i;
                }

                EditorGUILayout.EndHorizontal();
                
                EditorGUI.indentLevel++;

                if (property.isExpanded)
                {
                    EditorGUILayout.PropertyField(property.FindPropertyRelative("IntegerValues"));
                    EditorGUILayout.PropertyField(property.FindPropertyRelative("VectorValue"));
                    EditorGUILayout.PropertyField(property.FindPropertyRelative("StringValue"));
                }

                EditorGUI.indentLevel--;
            }

            // Remove at the end, so we don't cause any iteration errors for modifying it in a loop.
            if (removedIndex >= 0)
            {
                _dataValuesProperty.DeleteArrayElementAtIndex(removedIndex);
            }

            // Apply modified properties so they persist
            serializedObject.ApplyModifiedProperties();
        }
    }
}