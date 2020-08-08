using System;

namespace Enemy
{
    public interface IEnemyLifeController
    {
        event EnemyKilled EnemyKilled;
    }

    public delegate void EnemyKilled(EnemyController enemyController, EnemyKilledEventArgs args);

    public class EnemyKilledEventArgs : EventArgs
    {
        public int Points { get; set; }
    }
}