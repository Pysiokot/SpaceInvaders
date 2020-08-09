using Player;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace UI
{
    internal class LifeIndicatorController : MonoBehaviour
    {
        [SerializeField] private GameObject _lifeIconPrefab;

        private IPlayerLifeController _playerLifeController;

        private List<GameObject> _playerLifesIcons = new List<GameObject>();

        [Inject]
        private void InitializeDI(IPlayerLifeController playerLifeController)
        {
            _playerLifeController = playerLifeController;

            SpawnPlayerLifesIcons(_playerLifeController.LifesLeft - 1);

            _playerLifeController.PlayerHit += OnPlayerHit;
        }

        private void SpawnPlayerLifesIcons(int lifesLeft)
        {
            for (int i = 0; i < lifesLeft; i++)
            {
                var iconGo = Instantiate(_lifeIconPrefab, transform, false);
                _playerLifesIcons.Add(iconGo);
            }
        }

        private void OnPlayerHit(PlayerController pc)
        {
            if (_playerLifesIcons.Count == 0)
                return;

            var iconGo = _playerLifesIcons[0];
            _playerLifesIcons.RemoveAt(0);

            Destroy(iconGo);
        }
    }
}
