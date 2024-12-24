using UnityEngine;
using UnityEditor;

namespace SpiderWeb
{
    [InitializeOnLoad]
    public class StickAdditionEditor
    {
        static StickAdditionEditor() 
        {
            SceneView.duringSceneGui += OnSceneGUI;
        }

        static void OnSceneGUI(SceneView sceneView)
        {
            Event e = Event.current;
            if (e.type == EventType.KeyDown && e.keyCode == KeyCode.Space)
            {
                Debug.Log("spaceeeeee");
                e.Use(); // Consume the event
            }
        }
    }
}
