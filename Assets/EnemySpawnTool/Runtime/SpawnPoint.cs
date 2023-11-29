using UnityEngine;

namespace EnemySpawnTool.Runtime
{
    public class SpawnPoint : MonoBehaviour
    {
        private GameObject _enemyPrefab;
        private Wave _wave;
        internal bool IsHardSpawner;
        private bool _hasSpawned;

        private void Update()
        {
            SpawnDefeated();
        }

        internal void Spawn()
        {
            var spawnTransform = transform;
            Instantiate(_enemyPrefab, spawnTransform.position, spawnTransform.rotation, spawnTransform);
            _hasSpawned = true;
            _wave.EnemiesAlive++;
        }

        private void SpawnDefeated()
        {
            if (!_hasSpawned || transform.childCount != 0) return;
            _wave.EnemiesAlive--;
            gameObject.SetActive(false);
        }
    }
}