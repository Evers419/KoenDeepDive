using UnityEngine;

namespace EnemySpawnTool.Runtime
{
    public class Trigger : MonoBehaviour
    {
        internal bool HasTriggered;
        private void OnTriggerEnter(Collider other)
        {
            if (HasTriggered || !other.gameObject.CompareTag("Player")) return;
            HasTriggered = true;
        }

    }
}