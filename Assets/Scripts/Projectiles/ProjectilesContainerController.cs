using System.Collections.Generic;
using UnityEngine;
using Utils;
using Zenject;

namespace Projectiles
{
    public interface IProjectileContainerController
    {
        void InstanitateNewProjectile(GameObject prefab, Vector3 position, Quaternion rotation);
    }

    public class ProjectilesContainerController : MonoBehaviour, IProjectileContainerController
    {
        private IList<ProjectileController> _spawnedProjectiles = new List<ProjectileController>();

        private IGameStateController _gameStateController;

        [Inject]
        private void InitializeDI(IGameStateController gameStateController)
        {
            _gameStateController = gameStateController;

            _gameStateController.GameStateChanged += OnGameStateChanged;
        }

        public void InstanitateNewProjectile(GameObject prefab, Vector3 position, Quaternion rotation)
        {
            var spawnedProjectile = Instantiate(prefab, position, rotation, this.transform);

            var projectileSctipt = spawnedProjectile.GetComponent<ProjectileController>();

            projectileSctipt.AllowMoving = true;
            _spawnedProjectiles.Add(projectileSctipt);
        }

        private void OnGameStateChanged(GameState newState)
        {
            if (newState == GameState.Pause || newState == GameState.Reset)
            {
                DestroyProjectiles();
            }
            else if(newState == GameState.PauseMenu)
            {
                StopAllProjectiles();
            }
            else if(newState == GameState.Playing)
            {
                ResumeAllProjectiles();
            }
        }

        private void ResumeAllProjectiles()
        {
            foreach (var projectile in _spawnedProjectiles)
            {
                projectile.AllowMoving = true;
            }
        }

        private void StopAllProjectiles()
        {
            foreach (var projectile in _spawnedProjectiles)
            {
                projectile.AllowMoving = false;
            }
        }

        private void DestroyProjectiles()
        {
            for (int i = _spawnedProjectiles.Count - 1; i >= 0; i--)
            {
                if(_spawnedProjectiles[i] != null)
                {
                    Destroy(_spawnedProjectiles[i].gameObject);
                }

                _spawnedProjectiles.RemoveAt(i);
            }
        }
    }
}