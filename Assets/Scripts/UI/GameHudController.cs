using UnityEngine;
using Utils;
using Zenject;

namespace UI
{
    public class GameHudController : MonoBehaviour
    {
        [SerializeField] private GameObject _gameOverLabel;
        [SerializeField] private GameObject _endLabel;
        [SerializeField] private GameObject _pauseMenuLabel;

        private IGameStateController _gameStateController;

        [Inject]
        private void InitializeDI(IGameStateController gameStateController)
        {
            _gameStateController = gameStateController;

            _gameStateController.GameStateChanged += OnGameStateChanged;
        }

        private void OnGameStateChanged(GameState newState)
        {
            switch (newState)
            {
                case GameState.Playing:
                    _pauseMenuLabel.SetActive(false);
                    _gameOverLabel.SetActive(false);
                    _endLabel.SetActive(false);
                    break;
                case GameState.Pause:
                    break;
                case GameState.PauseMenu:
                    _pauseMenuLabel.SetActive(true);
                    break;
                case GameState.PlayerKilled:
                    _gameOverLabel.SetActive(true);
                    break;
                case GameState.End:
                    _endLabel.SetActive(true);
                    break;
                default:
                    break;
            }
        }
    }
}