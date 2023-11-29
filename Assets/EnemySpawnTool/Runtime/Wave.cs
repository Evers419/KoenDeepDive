using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace EnemySpawnTool.Runtime
{
    public class Wave : MonoBehaviour
    {
        public List<SpawnPoint> spawnPoints;
        internal int EnemiesAlive;
        internal bool HardMode;
        private float _timer;
        private bool _spawnCompleted;
        internal bool WaveDefeated;

        private void Update()
        {
            if (EnemiesAlive == 0 && _spawnCompleted)
            {
                WaveDefeated = true;
            }
        }

        internal void SpawnWave()
        {
            if (_timer <= 0f)
            {
                if(HardMode)
                {
                    SpawnHardEnemies();
                }
                else
                {
                    SpawnNormalEnemies();
                }

                _spawnCompleted = true;
            }
            else
            {
                StartCoroutine(WaitForTimer(_timer));
            }
        }

        private IEnumerator WaitForTimer(float time)
        {
            yield return new WaitForSeconds(time);
            SpawnWave();
        }

        private void SpawnHardEnemies()
        {
            foreach (var spawnPoint in spawnPoints)
            {
                spawnPoint.Spawn();
            }
        }

        private void SpawnNormalEnemies()
        {
            foreach (var spawnPoint in spawnPoints.Where(spawnPoint => !spawnPoint.IsHardSpawner))
            {
                spawnPoint.Spawn();
            }
        }
        
        public void AddSpawnPoint(SpawnPoint spawnPoint)
        {
            spawnPoints.Add(spawnPoint);
        }
    }
}