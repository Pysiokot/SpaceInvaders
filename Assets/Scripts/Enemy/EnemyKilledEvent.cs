using System;

namespace Enemy
{
    public delegate void EnemyKilled(EnemyController enemyController, EnemyKilledEventArgs args);

    public class EnemyKilledEventArgs : EventArgs
    {
        public int Points { get; set; }
    }
}