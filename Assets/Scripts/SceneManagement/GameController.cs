using Player;
using UnityEngine;
using Utils;
using Zenject;

namespace SceneManagement
{
    public class GameController : MonoBehaviour, IGameStateController
    {
        public event GameStateChanged GameStateChanged;

        [Inject]
        private IPlayerLifeController _playerLifeController;

        private GameState _prevGameState;
        private GameState _currentGameState;

        private void Start()
        {
            _currentGameState = GameState.Pause;

            _playerLifeController.PlayerHit += OnPlayerHit;
            _playerLifeController.PlayerLifeReachedZero += OnPlayerLifeReachedZero;
        }

        public void ChangeGameStateToMenuPause()
        {
            ChangeGameState(GameState.PauseMenu);
        }

        public void ChangeGameStateToPlaying()
        {
            ChangeGameState(GameState.Playing);
        }

        private void OnPlayerLifeReachedZero()
        {
            ChangeGameState(GameState.End);
        }

        private void OnPlayerHit(PlayerController pc)
        {
            ChangeGameState(GameState.Pause);
        }

        private void ChangeGameState(GameState newGameState)
        {
            _prevGameState = _currentGameState;
            _currentGameState = newGameState;

            GameStateChanged?.Invoke(newGameState);
        }
    }
}