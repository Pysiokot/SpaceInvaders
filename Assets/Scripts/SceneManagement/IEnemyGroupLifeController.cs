using System;

namespace SceneManagement
{
    interface IEnemyGroupLifeController
    {
        event EventHandler<EventArgs> EnemyCountReachedZero;
    }
}
