using System;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using System.Reflection;
#endif

[System.AttributeUsage(System.AttributeTargets.Method)]
public class ButtonAttribute : PropertyAttribute
{
    public string label;

    public ButtonAttribute(string label = "")
    {
        this.label = label;
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(MonoBehaviour), true)]
public class ButtonEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        MonoBehaviour script = (MonoBehaviour)target;
        MethodInfo[] methods = script.GetType().GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

        foreach (MethodInfo method in methods)
        {
            ButtonAttribute attribute = (ButtonAttribute)Attribute.GetCustomAttribute(method, typeof(ButtonAttribute));
            if (attribute != null)
            {
                if (GUILayout.Button(string.IsNullOrEmpty(attribute.label) ? method.Name : attribute.label))
                {
                    method.Invoke(script, null);
                }
            }
        }
    }
}
#endif