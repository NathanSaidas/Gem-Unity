using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(ObjectFieldAttribute))]
public class ObjectFieldDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        property.objectReferenceValue = EditorGUI.ObjectField(position,label,property.objectReferenceValue,typeof(Object),true);
    }
}
