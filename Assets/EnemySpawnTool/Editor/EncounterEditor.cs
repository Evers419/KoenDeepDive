using EnemySpawnTool.Runtime;
using UnityEditor;
using UnityEngine;

namespace EnemySpawnTool.Editor
{
    [CustomEditor(typeof(Encounter))]
    public class EncounterEditor : UnityEditor.Editor
    {
        
        //TODO bij instantiate methods voeg dingen toe aan list
        //TODO zorg dat prefabs daadwerkelijk gespawned worden
        //TODO laat de waves zien als ik op een trigger klik
        //TODO laat de spawnpoints zien als ik op een wave klik
        //TODO laat met raycast dingen geplaced worden
        
        public GameObject triggerPrefab;
        public GameObject wavePrefab;
        public GameObject spawnPointPrefab;

        private GameObject _triggerContainer;
        private GameObject _waveContainer;
        private GameObject _spawnContainer;

        private Encounter.TriggerWave _selectedTriggerWave;
        private SerializedProperty _triggerWavesProp;
        private int _selectedTriggerWaveIndex;

        private void OnEnable()
        {
            _triggerWavesProp = serializedObject.FindProperty("triggerWaves");
            SceneView.duringSceneGui += OnSceneGUI;
        }

        private void OnDisable()
        {
            SceneView.duringSceneGui -= OnSceneGUI;
        }

        // ReSharper disable once Unity.IncorrectMethodSignature
        private void OnSceneGUI(SceneView sceneView)
        {
            HandleRaycastForPrefabPlacement();
        }

        private Vector3 _prefabPlacementPosition;
        
        private void HandleRaycastForPrefabPlacement()
        {
            Event guiEvent = Event.current;

            if (guiEvent.type == EventType.MouseDown && guiEvent.button == 0)
            {
                Ray ray = HandleUtility.GUIPointToWorldRay(guiEvent.mousePosition);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit))
                {
                    // Set the prefab placement position based on the raycast hit point
                    _prefabPlacementPosition = hit.point;
                }

                guiEvent.Use();
            }
        }

        private void SpawnTriggerPrefab(Transform hitTransform)
        {
            Instantiate(triggerPrefab, hitTransform.position, hitTransform.rotation, _triggerContainer.transform);
        }

        private void SpawnWavePrefab(Transform hitTransform)
        {
            Instantiate(wavePrefab, hitTransform.position, hitTransform.rotation, _waveContainer.transform);
        }

        private void SpawnSpawnPointPrefab(Transform hitTransform)
        {
            Instantiate(spawnPointPrefab, hitTransform.position, hitTransform.rotation, _spawnContainer.transform);
        }
        
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Encounter Editor", EditorStyles.boldLabel);

            Encounter encounter = (Encounter)target;
            
            if (GUILayout.Button("Add Trigger"))
            {
                encounter.triggerWaves.Add(new Encounter.TriggerWave());
                EditorUtility.SetDirty(encounter);
            }

            for (int i = 0; i < _triggerWavesProp.arraySize; i++)
            {
                EditorGUILayout.BeginHorizontal();

                EditorGUILayout.LabelField("Trigger " + i);

                if (GUILayout.Button("Select", GUILayout.MaxWidth(60)))
                {
                    _selectedTriggerWaveIndex = i;
                }

                EditorGUILayout.EndHorizontal();

                EditorGUI.indentLevel++;

                if (i == _selectedTriggerWaveIndex)
                {
                   //DisplaySpawnPointsControls(wavesProp);
                }

                EditorGUI.indentLevel--;
            }
            serializedObject.ApplyModifiedProperties();
        }
    }
}