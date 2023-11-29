using System.Collections.Generic;
using UnityEngine;

namespace EnemySpawnTool.Runtime
{
    public class Encounter : MonoBehaviour
    {
        public List<Trigger> triggers;
        public List<Wave> waves;
        public Dictionary<Trigger, List<Wave>> TriggerWaveMap = new Dictionary<Trigger, List<Wave>>();
        
        private int _currentWaveIndex;
        private bool _hardMode;

        private void Awake()
        {
            SetHardMode();
        }

        private void Update()
        {
            CheckTriggers();
        }

        private void SetHardMode()
        {
            _hardMode = true;
            foreach (var wave in waves)
            {
                wave.HardMode = _hardMode;
            }
        }

        private void CheckTriggers()
        {
            foreach (var (trigger, waveList) in TriggerWaveMap)
            {
                if (!trigger.HasTriggered) continue;
                foreach (var wave in waveList)
                {
                    if (wave.WaveDefeated) continue;
                    wave.SpawnWave();
                    break;
                }
            }
        }

        public void AddTrigger(Trigger trigger)
        {
            TriggerWaveMap[trigger] = new List<Wave>();
        }

        public void AddWave(Wave wave, Trigger trigger)
        {
            TriggerWaveMap[trigger].Add(wave);
        }
    }
}