using System;
using System.Collections.Generic;
using UnityEngine;

namespace ScriptableObjects
{
    [CreateAssetMenu(fileName = "EnemiesSpawnerConfig", menuName = "ScriptableObjects/EnemiesSpawnerConfig", order = 1)]
    internal class EnemiesSpawnerConfig : ScriptableObject
    {
        [Serializable]
        internal class EnemyGroup
        {
            [Range(1, 7)]
            public int EnemyCount = 3;

            public GameObject EnemyPrefab;
            public EnemyParams EnemyParams;
        }

        public List<EnemyGroup> EnemyGroups;
    }
}
