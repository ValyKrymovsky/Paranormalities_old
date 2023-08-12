using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.TerrainTools;

namespace MyCode.Enemy.Systems
{
    [CustomEditor(typeof(NavigationPath))]
    public class NavigationPathEditor : Editor
    {
        SerializedProperty _pathPoints;
        SerializedProperty _pathMode;

        private void OnEnable()
        {
            _pathPoints = serializedObject.FindProperty("_navigationPoints");
            _pathMode = serializedObject.FindProperty("_pathMode");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            NavigationPath navigation = (NavigationPath)target;

            EditorGUILayout.PropertyField(_pathPoints, true);
            EditorGUILayout.Space(5);
            if (GUILayout.Button("Get Path Points"))
            {
                navigation.NavigationPoints = navigation.gameObject.GetComponentsInChildren<NavigationPathPoint>();
                PrefabUtility.RecordPrefabInstancePropertyModifications(navigation);
            }
            EditorGUILayout.Space(5);
            EditorGUILayout.PropertyField(_pathMode, true);

            serializedObject.ApplyModifiedProperties();
        }

        public void OnSceneGUI()
        {
            NavigationPath navigation = (NavigationPath)target;

            if (navigation.NavigationPoints.Length == 0) return;
            Handles.color = Color.blue;

            for (int i = 0; i < navigation.NavigationPoints.Length; i++)
            {
                if (i == navigation.NavigationPoints.Length - 1) break;

                for (int j = i + 1; i < navigation.NavigationPoints.Length; j++)
                {
                    Handles.DrawLine(navigation.NavigationPoints[i].gameObject.transform.position, navigation.NavigationPoints[j].gameObject.transform.position);
                    break;
                }
            }
        }
    }
}
