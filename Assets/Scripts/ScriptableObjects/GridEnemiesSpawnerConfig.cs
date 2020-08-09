using System;
using System.Collections.Generic;
using UnityEngine;

namespace ScriptableObjects
{
    [CreateAssetMenu(fileName = "EnemiesSpawnerConfig", menuName = "ScriptableObjects/EnemiesSpawnerConfig", order = 1)]
    internal class GridEnemiesSpawnerConfig : ScriptableObject, ISpawnerConfig
    {
        [Serializable]
        internal class EnemyGroup
        {
            [Range(1, 12)]
            public int EnemyCount = 3;

            public GameObject EnemyPrefab;
            public EnemyParams EnemyParams;
        }

        public List<EnemyGroup> EnemyGroups;
    }
}
