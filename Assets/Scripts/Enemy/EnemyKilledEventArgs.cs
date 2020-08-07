using System;

namespace Enemy
{
    internal class EnemyKilledEventArgs : EventArgs
    {
        public int Points { get; set; }
    }
}