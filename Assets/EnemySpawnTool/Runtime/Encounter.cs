using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace EnemySpawnTool.Runtime
{
    public class Encounter : MonoBehaviour
    {
        public List<TriggerWave> triggerWaves = new List<TriggerWave>();
        
        [Serializable]
        public struct TriggerWave
        {
            public Trigger trigger;
            public List<Wave> waves;
        }
        private bool _hardMode;

        public static TriggerWave NewTriggerWave(Trigger trigger, List<Wave> waves)
        {
            TriggerWave triggerWave = new TriggerWave
            {
                trigger = trigger,
                waves = waves
            };
            return triggerWave;
        }

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
                if (!triggerWave.trigger.HasTriggered) continue;
                foreach (var wave in triggerWave.waves)
                {
                    if (wave.WaveDefeated) continue;
                    wave.SpawnWave();
                    break;
                }
            }
        }

        public void AddTrigger(Trigger trigger)
        {
            triggerWaves.Add(NewTriggerWave(trigger, new List<Wave>()));
        }

        public void AddWave(TriggerWave triggerWave, Wave wave)
        {
            triggerWave.waves.Add(wave);
        }
    }
}