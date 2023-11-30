using UnityEngine;

namespace EnemySpawnTool.Runtime
{
    public class SpawnPoint : MonoBehaviour
    {
        [SerializeField]
        public GameObject enemyPrefab;
        [SerializeField]
        public Wave wave;
        [SerializeField]
        public bool isHardSpawner;
        private bool _hasSpawned;

        private void Update()
        {
            SpawnDefeated();
        }

        internal void Spawn()
        {
            if (_hasSpawned) return;
            var spawnTransform = transform;
            float prefabHeight = enemyPrefab.GetComponentInChildren<MeshRenderer>().bounds.size.y;
            var position = spawnTransform.position;
            Vector3 spawnPosition = new Vector3(position.x, position.y + .5f*prefabHeight, position.z);
            Instantiate(enemyPrefab, spawnPosition, spawnTransform.rotation, spawnTransform);
            _hasSpawned = true;
            wave.EnemiesAlive++;
        }

        private void SpawnDefeated()
        {
            if (!_hasSpawned || transform.childCount != 0) return;
            wave.EnemiesAlive--;
            gameObject.SetActive(false);
        }
    }
}