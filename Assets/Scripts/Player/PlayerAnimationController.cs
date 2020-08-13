using System;
using UnityEngine;
using Utils;
using Zenject;

namespace Player
{
    public class PlayerAnimationController : MonoBehaviour
    {
        private static readonly int Idle = Animator.StringToHash("Idle");
        private static readonly int Explosion = Animator.StringToHash("Explosion");
        private static readonly int Spawn = Animator.StringToHash("Spawn");

        [SerializeField]
        private Animator _animator;

        private IPlayerLifeController _playerLifeController;
        private IGameStateController _gameStateController;

        [Inject]
        private void InitializeDI(IGameStateController gameStateController, IPlayerLifeController playerLifeController)
        {
            _gameStateController = gameStateController;
            _playerLifeController = playerLifeController;

            _gameStateController.GameStateChanged += OnGameStateChanged;
            _playerLifeController.PlayerHit += OnPlayerHit;
            _playerLifeController.PlayerLifeReachedZero += () => OnPlayerHit(null);
        }

#if UNITY_EDITOR
        private void Update()
        {
            if(Input.GetKeyDown(KeyCode.V))
            {
                OnPlayerHit(null);
            }
        }
#endif

        private void OnPlayerHit(PlayerController pc)
        {
            _animator.ResetTrigger(Explosion);
            _animator.ResetTrigger(Idle);
            _animator.SetTrigger(Explosion);
        }

        private void OnGameStateChanged(GameState newState)
        {
            if(newState == GameState.Playing)
            {
                _animator.ResetTrigger(Explosion);
                _animator.SetTrigger(Idle);
            }
            else if(newState == GameState.Respawning || newState == GameState.Reset)
            {
                _animator.ResetTrigger(Explosion);
                _animator.ResetTrigger(Idle);
                _animator.SetTrigger(Spawn);
            }
        }

        private void OnDestroy()
        {
            if(_gameStateController != null)
            {
                _gameStateController.GameStateChanged -= OnGameStateChanged;
            }

            if(_playerLifeController != null)
            {
                _playerLifeController.PlayerHit -= OnPlayerHit;
            }
        }
    }
}