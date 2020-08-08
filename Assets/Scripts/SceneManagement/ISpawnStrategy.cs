using Enemy;
using System.Collections.Generic;

namespace SceneManagement
{
    interface ISpawnStrategy
    {
        ICollection<EnemyController> SpawnEnemies();
    }
}
