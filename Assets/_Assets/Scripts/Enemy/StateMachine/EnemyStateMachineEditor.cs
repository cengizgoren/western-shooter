using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.TerrainTools;
using UnityEngine;
using UnityEngine.AI;

[CanEditMultipleObjects]
[CustomEditor(typeof(EnemyStateMachine))]
public class EnemyStateMachineEditor : Editor
{

    //private void Awake()
    //{

    //}

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        if (GUILayout.Button("Select all"))
        {
            Selection.objects = FindObjectsOfType<EnemyStateMachine>().Select(e => e.gameObject).ToArray();
        }
    }

    //private void OnSceneGUI()
    //{

    //}

}
