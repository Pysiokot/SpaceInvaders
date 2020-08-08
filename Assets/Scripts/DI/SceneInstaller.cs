using Enemy;
using Player;
using UnityEngine;
using Zenject;

namespace DI
{
    public class SceneInstaller : MonoInstaller
    {
        [SerializeField] private GameObject _enemyLifeController;
        [SerializeField] private GameObject _playerController;

        public override void InstallBindings()
        {
            Container.Bind<IEnemyLifeController>().FromInstance(_enemyLifeController.GetComponent<IEnemyLifeController>()).AsSingle();
            Container.Bind<IPlayerLifeController>().FromInstance(_playerController.GetComponent<IPlayerLifeController>()).AsSingle();
        }
    }
}