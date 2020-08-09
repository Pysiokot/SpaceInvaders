using Enemy;
using Player;
using Projectiles;
using SceneManagement;
using UnityEngine;
using Utils;
using Zenject;

namespace DI
{
    public class SceneInstaller : MonoInstaller
    {
        [SerializeField] private GameObject _enemyLifeController;
        [SerializeField] private GameObject _playerController;
        [SerializeField] private GameObject _gameStateController;
        [SerializeField] private GameObject _projectilesContainter;


        public override void InstallBindings()
        {
            Container.Bind<IEnemyLifeController>().FromInstance(_enemyLifeController.GetComponent<IEnemyLifeController>()).AsSingle();
            Container.Bind<IEnemyGroupLifeController>().FromInstance(_enemyLifeController.GetComponent<IEnemyGroupLifeController>()).AsSingle();
            Container.Bind<IPlayerLifeController>().FromInstance(_playerController.GetComponent<IPlayerLifeController>()).AsSingle();
            Container.Bind<IGameStateController>().FromInstance(_gameStateController.GetComponent<IGameStateController>()).AsSingle();
            Container.Bind<IProjectileContainerController>().FromInstance(_projectilesContainter.GetComponent<IProjectileContainerController>()).AsSingle();

            Container.Bind<ISpawnStrategy>().FromComponentInChildren().AsSingle();
        }
    }
}