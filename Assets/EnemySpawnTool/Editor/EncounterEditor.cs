using System;
using EnemySpawnTool.Runtime;
using UnityEditor;
using UnityEngine;

namespace EnemySpawnTool.Editor
{
    [CustomEditor(typeof(Encounter))]
    public class EncounterEditor : UnityEditor.Editor
    {
        private GameObject _waveContainer;
        private GameObject _triggerContainer;
        
        private Trigger _selectedTrigger;
        private Wave _selectedWave;

        private SerializedProperty _triggersProp;
        private SerializedProperty _wavesProp;
        private SerializedProperty _triggerWaveMapProp;

        private void OnEnable()
        {
            _triggersProp = serializedObject.FindProperty("triggers");
            _wavesProp = serializedObject.FindProperty("waves");
            _triggerWaveMapProp = serializedObject.FindProperty("TriggerWaveMap");
        }

        public override void OnInspectorGUI()
        {
            Encounter encounter = (Encounter)target;

            // Display default inspector property editor
            DrawDefaultInspector();

            // Add buttons to add triggers and waves
            if (GUILayout.Button("Add Trigger"))
            {
                if (_triggerContainer == null)
                {
                    _triggerContainer = new GameObject("TriggerContainer");
                    _triggerContainer.transform.SetParent(encounter.transform);
                }

                GameObject newTriggerObject = new GameObject("Trigger");
                newTriggerObject.transform.SetParent(_triggerContainer.transform);
                
                encounter.AddTrigger(newTriggerObject.GetComponent<Trigger>());
            }

            if (GUILayout.Button("Add Wave"))
            {
                if (_waveContainer == null)
                {
                    _waveContainer = new GameObject("WaveContainer");
                    _waveContainer.transform.SetParent(encounter.transform);
                }
                
                encounter.AddWave(_waveContainer.GetComponent<Wave>(), _selectedTrigger);
            }

            // Display triggers
            EditorGUILayout.LabelField("Triggers");
            for (int i = 0; i < encounter.triggers.Count; i++)
            {
                EditorGUILayout.LabelField($"Trigger {i + 1}");
                // Display trigger properties
            }

            // Display waves
            EditorGUILayout.LabelField("Waves");
            for (int i = 0; i < encounter.waves.Count; i++)
            {
                EditorGUILayout.LabelField($"Wave {i + 1}");

                // Display wave properties
                EditorGUI.indentLevel++;
                for (int j = 0; j < encounter.waves[i].spawnPoints.Count; j++)
                {
                    EditorGUILayout.LabelField($"Spawn Point {j + 1}");
                    // Display spawn point properties
                }
                EditorGUI.indentLevel--;
            }
        }
    }
}