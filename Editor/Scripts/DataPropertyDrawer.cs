using Immel.DataCodingTest.Runtime;
using UnityEditor;
using UnityEngine;

namespace Immel.DataCodingTest.Editor
{
    /// <summary>
    /// The goal with the property drawer was to show using reflection to get the fields available dynamically, and then proceed to draw them using PropertyField in the theoretical event
    /// one of the properties had its own PropertyDrawer.
    /// </summary>
    [CustomPropertyDrawer(typeof(Data))]
    public class DataPropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var fields = typeof(Data).GetFields();

            // Calculate the width per field. This makes sure everything is nice and evenly spaced. In theory, you could probably add an attribute to each field that defines a width
            // and use that instead, but in the interest of simplicity, we'll make everything even widthed.
            var widthPerField = position.width / fields.Length;
            var initialX = position.x;

            position.height = EditorGUIUtility.singleLineHeight;
            EditorGUI.BeginProperty(position, label, property);

            // Start off simple, using the field names as a header of sorts.
            for(var i = 0; i < fields.Length; i++)
            {
                // I'm going to make the property draw in 90% of its actual allowed space, so I can create a bit of spacing between properties. There's probably a better way of doing this
                // that I'm not thinking of, but, I'm frankly exhausted right now.
                position.width = widthPerField * 0.9f;
                EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), new GUIContent(fields[i].Name));
                position.x += widthPerField;
            }

            // Reset position to a good starting point, height, and width
            position.x = initialX;
            position.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
            position.height = EditorGUIUtility.singleLineHeight;

            for(var i = 0; i < fields.Length; i++)
            {
                var serializedProperty = property.FindPropertyRelative(fields[i].Name);

                position.width = widthPerField * 0.9f;
                
                if (serializedProperty == null)
                {
                    // This case should hopefully never happen, but I always assume failure is an option and want to put something on screen for the user.
                    position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), new GUIContent("Invalid Property"));
                }
                else
                {
                    // Use PropertyField, so any nested properties with their own PropertyDrawers here should work. I could get a bit crazier with this, but, again I want to 
                    // aim for simplicty now, and have the best odds that adding a new field to Data would 'Just Work' - Todd Howard, circa ~2017
                    EditorGUI.PropertyField(position, serializedProperty, new GUIContent(""));
                    
                    position.x += widthPerField;
                }
            }

            EditorGUI.EndProperty();

            // Apply modified properties so they persist
            property.serializedObject.ApplyModifiedProperties();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var fields = typeof(Data).GetFields();
            var maxItemCount = 0;

            // Iterate over all our fields, checking that they're valid, an array, and expanded.
            foreach(var field in fields)
            {
                var subProperty = property.FindPropertyRelative(field.Name);

                if (subProperty == null || !subProperty.isArray || !subProperty.isExpanded)
                    continue;

                maxItemCount = subProperty.arraySize > maxItemCount ? subProperty.arraySize : maxItemCount;
            }

            // Base Height is our 2 line tall baseline, one line for the header, one line for the field entry for VectorValue and String Value
            var baseHeight = EditorGUIUtility.singleLineHeight * 2 + EditorGUIUtility.standardVerticalSpacing;

            // We had no items, use our base height
            if (maxItemCount == 0)
            {
                return baseHeight;
            }
            else
            {
                // We did have items to display, so we add to our base height our max item count (plus one, to account for Unity's built in +/- buttons), times a singleLineheight+standardVerticalSpace
                // so that everything is nice and nifty. Then, add a bit of spacing so the +/- buttons aren't completely flat against the bottom of the drawn property area.
                return baseHeight + (maxItemCount + 1) * (EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing) + EditorGUIUtility.standardVerticalSpacing;
            }
        }
    }
}