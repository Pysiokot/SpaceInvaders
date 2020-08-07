using System;
using UnityEngine;

namespace Enemy
{
    internal delegate void EnemyKilled(GameObject enemyGO, EnemyKilledEventArgs args);
    
    internal class EnemyKilledEventArgs : EventArgs
    {
        public int Points { get; set; }
    }
}