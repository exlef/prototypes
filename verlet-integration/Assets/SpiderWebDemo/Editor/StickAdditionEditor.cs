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
                e.Use(); // Consume the event
                // get all the currentl selected items
                var selections = Selection.gameObjects;
                if(selections.Length != 2) return;
                var p1 = selections[0].GetComponent<Point>();
                var p2 = selections[1].GetComponent<Point>();
                if(p1 == null || p2 == null) return;
                string sticksParentName = "Sticks";
                Transform sticks = GameObject.Find(sticksParentName).GetComponent<Transform>();
                if (!sticks) Debug.LogWarning($"there is no game object called as {sticksParentName} in the hierarchy.");
                if (!sticks) return;
                var go = LevelTool.InstantiatePrefab("Assets/SpiderWebDemo/Stick.prefab", sticks);
                if (!go) return;
                var stick = go.GetComponent<Stick>();
                stick.pointA = p1;
                stick.pointB = p2;
                stick.ForceToRefresh();
            }
        }
    }
}
