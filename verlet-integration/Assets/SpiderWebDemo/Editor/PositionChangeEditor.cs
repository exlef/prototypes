using System.Drawing;
using UnityEditor;
using UnityEngine;

namespace SpiderWeb
{

    [CustomEditor(typeof(Transform))]
    public class PositionChangeEditor : Editor
    {
        private Vector3 lastPosition;

        void OnEnable()
        {
            lastPosition = ((Transform)target).position;
            EditorApplication.update += Update;
        }

        void OnDisable()
        {
            EditorApplication.update -= Update;
        }

        void Update()
        {
            Transform t = (Transform)target;
            if(t == null) return;
            if (t.position != lastPosition)
            {
                lastPosition = t.position;
                Point point = t.GetComponent<Point>();
                if (point == null) return;
                OnPositionChanged();
            }
        }

        private void OnPositionChanged()
        {
            var sticks = FindObjectsByType<Stick>(FindObjectsSortMode.None);
            foreach (var stick in sticks)
            {
                stick.ForceToRefresh();
            }
        }
    }
}
