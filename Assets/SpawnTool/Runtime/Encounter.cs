using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace EnemySpawnTool.Runtime
{
    public class Encounter : MonoBehaviour
    {
        public List<TriggerWave> triggerWaves = new List<TriggerWave>();
        private bool _hardMode;
        
        [Serializable]
        public struct TriggerWave
        {
            public GameObject container;
            public GameObject trigger;
            public List<Wave> waves;
        }
        
        private TriggerWave NewTriggerWave(GameObject trigger, List<Wave> waves)
        {
            TriggerWave triggerWave = new TriggerWave
            {
                trigger = trigger,
                waves = waves,
                container = new GameObject("Trigger" + triggerWaves.Count),
            };
            triggerWave.container.transform.parent = transform;
            triggerWave.trigger.transform.parent = triggerWave.container.transform;
            return triggerWave;
        }
        
        public GameObject triggerPrefab;
        public GameObject wavePrefab;
        public GameObject spawnPointPrefab;

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
            foreach (var wave in triggerWaves.SelectMany(tw => tw.waves))
            {
                wave.HardMode = _hardMode;
            }
        }
        
        private void CheckTriggers()
        {
            foreach (TriggerWave triggerWave in triggerWaves)
            {
                if (!triggerWave.trigger.GetComponent<Trigger>().HasTriggered) continue;
                foreach (var wave in triggerWave.waves)
                {
                    if (wave.WaveDefeated) continue;
                    wave.SpawnWave();
                    break;
                }
            }
        }

        public TriggerWave AddTrigger(GameObject trigger)
        {
            TriggerWave newTriggerWave = NewTriggerWave(trigger, new List<Wave>());
            triggerWaves.Add(newTriggerWave);
            return newTriggerWave;
        }

        public void AddWave(TriggerWave triggerWave, Wave wave)
        {
            triggerWave.waves.Add(wave);
        }
    }
}