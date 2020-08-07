using System;
using UnityEngine;

namespace Enemy
{
    public delegate void EnemyKilled(GameObject enemyGO, EnemyKilledEventArgs args);

    public class EnemyKilledEventArgs : EventArgs
    {
        public int Points { get; set; }
    }
}