using System;
using System.Collections.Generic;
using EnemySpawnTool.Runtime;
using UnityEditor;
using UnityEngine;

namespace EnemySpawnTool.Editor
{
    [CustomEditor(typeof(Encounter))]
    public class EncounterEditor : UnityEditor.Editor
    {
        //TODO laat de spawnpoints zien als ik op een wave klik
        //TODO laat met raycast dingen geplaced worden
        
        private string _trackingStatus;
        private bool _isPlacingPrefab;
        private GameObject _objectToPlace;
        
        public GameObject triggerPrefab;
        public GameObject wavePrefab;
        public GameObject spawnPointPrefab;

        private Encounter _encounter;
        private SerializedProperty _triggerWavesProp;
        private Encounter.TriggerWave _selectedTriggerWave;
        private int _selectedTriggerWaveIndex;
        private int _selectedWaveIndex;

        private void OnEnable()
        {
            _encounter = (Encounter)target;
            
            _triggerWavesProp = serializedObject.FindProperty("triggerWaves");
            SceneView.duringSceneGui += OnSceneGUI;

            _selectedTriggerWaveIndex = -1;
            _selectedWaveIndex = -1;
            triggerPrefab = _encounter.triggerPrefab;
            wavePrefab = _encounter.wavePrefab;
            spawnPointPrefab = _encounter.spawnPointPrefab;
        }

        private void OnDisable()
        {
            SceneView.duringSceneGui -= OnSceneGUI;

            _encounter.triggerPrefab = triggerPrefab;
            _encounter.wavePrefab = wavePrefab;
            _encounter.spawnPointPrefab = spawnPointPrefab;
        }
        
        private void OnSceneGUI(SceneView sceneView)
        {
            if (_isPlacingPrefab && Event.current.rawType == EventType.MouseDown)
            {
                if (Event.current.button == 0)
                {
                    GUIUtility.hotControl = GUIUtility.GetControlID(FocusType.Passive);
                    PlaceObject();
                }
            }
        }

        private Transform _prefabPlacementPosition;
        
        private void PlaceObject()
        {
            Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
            RaycastHit hit;
            if (_isPlacingPrefab && Physics.Raycast(ray, out hit, Mathf.Infinity))
            {
                if (_objectToPlace == triggerPrefab) SpawnTriggerPrefab(hit);
                if (_objectToPlace == spawnPointPrefab) SpawnSpawnPointPrefab(hit);
                _isPlacingPrefab = false;
            }
        }

        private void SpawnTriggerPrefab(RaycastHit hit)
        {
            GameObject triggerInstance = Instantiate(_objectToPlace, hit.point, new Quaternion());
            _encounter.AddTrigger(triggerInstance);
            
            serializedObject.Update();
            _selectedTriggerWaveIndex = _triggerWavesProp.arraySize;
            GetTriggerWaveAtIndex(_triggerWavesProp.arraySize);
            EditorUtility.SetDirty(_encounter);
        }

        private void SpawnWavePrefab()
        {
            GameObject waveInstance = Instantiate(_objectToPlace, Vector3.zero, Quaternion.identity, _selectedTriggerWave.container.transform);
            Wave newWave = waveInstance.GetComponent<Wave>();
            
            SerializedProperty wavesList = _triggerWavesProp.GetArrayElementAtIndex(_selectedTriggerWaveIndex).FindPropertyRelative("waves");
            wavesList.GetArrayElementAtIndex(wavesList.arraySize - 1).objectReferenceValue = newWave;
            
            _encounter.AddWave(_selectedTriggerWave, newWave);
            EditorUtility.SetDirty(_encounter);
        }

        private void SpawnSpawnPointPrefab(RaycastHit hit)
        {
            GameObject spawnInstance = Instantiate(_objectToPlace, hit.point, new Quaternion(), _selectedTriggerWave.waves[_selectedWaveIndex].transform);
            SpawnPoint newSpawnPoint = spawnInstance.GetComponent<SpawnPoint>();
            newSpawnPoint.wave = _selectedTriggerWave.waves[_selectedWaveIndex];
            _selectedTriggerWave.waves[_selectedWaveIndex].AddSpawnPoint(newSpawnPoint);
            EditorUtility.SetDirty(_encounter);
        }

        private void DeleteSelected()
        {
            throw new NotImplementedException();
        }
        
        private void ShowWavesSection()
        {
            // Ensure _triggerWavesProp is not null and _selectedTriggerWaveIndex is valid
            if (_triggerWavesProp == null || _selectedTriggerWaveIndex < 0 || _selectedTriggerWaveIndex >= _triggerWavesProp.arraySize)
            {
                EditorGUILayout.LabelField("Invalid selection");
                return;
            }

            SerializedProperty triggerWaveElement = _triggerWavesProp.GetArrayElementAtIndex(_selectedTriggerWaveIndex);
            SerializedProperty wavesList = triggerWaveElement.FindPropertyRelative("waves");

            if (wavesList != null)
            {
                EditorGUILayout.Space();
                for (int i = 0; i < wavesList.arraySize; i++)
                {
                    SerializedProperty waveElement = wavesList.GetArrayElementAtIndex(i);
                    Wave wave = waveElement.objectReferenceValue as Wave;
                    
                    EditorGUILayout.LabelField("Wave " + i, EditorStyles.boldLabel);
                    EditorGUI.indentLevel++;
                    EditorGUI.BeginChangeCheck();
                    
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("Wave Timer");
                    float modifiedTimer = EditorGUILayout.FloatField(wave!.timer);
                    EditorGUILayout.EndHorizontal();
                    
                    for (int j = 0; j < wave.spawnPoints.Count; j++)
                    {
                        EditorGUI.indentLevel++;
                        
                        EditorGUILayout.BeginVertical();
                        EditorGUILayout.LabelField("Spawner" + j, EditorStyles.boldLabel, GUILayout.MaxWidth(300));
                        EditorGUILayout.EndVertical();
                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField("enemyPrefab: ", GUILayout.MaxWidth(125));
                        wave.spawnPoints[j].enemyPrefab = EditorGUILayout.ObjectField(wave.spawnPoints[j].enemyPrefab, typeof(GameObject), true) as GameObject;
                        EditorGUILayout.EndHorizontal();
                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField("Hardmode Spawner: ");
                        wave.spawnPoints[j].isHardSpawner = EditorGUILayout.Toggle(wave.spawnPoints[j].isHardSpawner);
                        EditorGUILayout.EndHorizontal();
                        EditorGUI.indentLevel--;
                    }
                    
                    if (GUILayout.Button("Add Spawn Point"))
                    {
                        _objectToPlace = spawnPointPrefab;
                        _isPlacingPrefab = true;
                        _selectedWaveIndex = i;
                        serializedObject.ApplyModifiedProperties();
                    }
                    
                    
                    if (EditorGUI.EndChangeCheck())
                    {
                        wave!.timer = modifiedTimer;
                        serializedObject.ApplyModifiedProperties();
                    }
                    EditorGUI.indentLevel--;
                }
                
                if (GUILayout.Button("Add Wave"))
                {
                    wavesList.arraySize++;
                    _objectToPlace = wavePrefab;
                    SpawnWavePrefab();
                    serializedObject.ApplyModifiedProperties();
                }
            }
            else
            {
                EditorGUILayout.LabelField("TriggerWave elements not found for Trigger " + _selectedTriggerWaveIndex);
            }
        }
        
        private Encounter.TriggerWave GetTriggerWaveAtIndex(int index)
        {
            Encounter.TriggerWave triggerWave = new Encounter.TriggerWave();

            if (index >= 0 && index < _triggerWavesProp.arraySize)
            {
                SerializedProperty triggerWaveElement = _triggerWavesProp.GetArrayElementAtIndex(index);

                // Assuming the 'container' and 'trigger' fields are of type GameObject
                triggerWave.container = triggerWaveElement.FindPropertyRelative("container").objectReferenceValue as GameObject;
                triggerWave.trigger = triggerWaveElement.FindPropertyRelative("trigger").objectReferenceValue as GameObject;

                triggerWave.waves = new List<Wave>();
                SerializedProperty wavesList = triggerWaveElement.FindPropertyRelative("waves");
                for (int i = 0; i < wavesList.arraySize; i++)
                {
                    SerializedProperty waveElement = wavesList.GetArrayElementAtIndex(i);
                    Wave wave = waveElement.objectReferenceValue as Wave;
                    triggerWave.waves.Add(wave);
                }
            }
            return triggerWave;
        }
        
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Encounter Editor", EditorStyles.boldLabel);

            if (_isPlacingPrefab)
            {
                EditorGUILayout.LabelField("Click on the scene to place object");
            }
            
            if (GUILayout.Button("Add Trigger"))
            {
                _objectToPlace = triggerPrefab;
                _isPlacingPrefab = true;
            }

            for (int i = 0; i < _triggerWavesProp.arraySize; i++)
            {
                EditorGUILayout.BeginHorizontal();

                EditorGUILayout.LabelField("Trigger " + i);

                if (GUILayout.Button("Select", GUILayout.MaxWidth(60)))
                {
                    _selectedTriggerWaveIndex = i;
                    _selectedTriggerWave = GetTriggerWaveAtIndex(i);
                }

                if (GUILayout.Button("Delete", GUILayout.MaxWidth(60)))
                {
                    DeleteSelected();
                }

                EditorGUILayout.EndHorizontal();

                EditorGUI.indentLevel++;

                if (i == _selectedTriggerWaveIndex)
                {
                    ShowWavesSection();
                }

                EditorGUI.indentLevel--;
            }
            serializedObject.ApplyModifiedProperties();
        }
    }
}