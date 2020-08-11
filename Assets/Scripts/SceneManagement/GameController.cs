using Player;
using System;
using System.Collections;
using UnityEngine;
using UserInput;
using Utils;
using Zenject;

namespace SceneManagement
{
    public class GameController : MonoBehaviour, IGameStateController
    {
        public event GameStateChanged GameStateChanged;

        private IPlayerLifeController _playerLifeController;
        private IEnemyGroupLifeController _enemyGroupLifeController;
        private IInputProxy _inputProxy;

        private GameState _currentGameState;

        private void Start()
        {
            ChangeGameState(GameState.Reset);

            StartCoroutine(StartGameAfterTwoSec());
        }

        [Inject]
        private void InitializeDI(IPlayerLifeController playerLifeController, IEnemyGroupLifeController enemyGroupLifeController, IInputProxy inputProxy)
        {
            _playerLifeController = playerLifeController;
            _enemyGroupLifeController = enemyGroupLifeController;
            _inputProxy = inputProxy;

            _playerLifeController.PlayerHit += OnPlayerHit;
            _playerLifeController.PlayerLifeReachedZero += OnPlayerLifeReachedZero;
            _enemyGroupLifeController.EnemyCountReachedZero += OnEnemyCountReachedZero;
        }

        private void Update()
        {
            if(_inputProxy.GetButtonDown("Cancel"))
            {
                if (_currentGameState == GameState.Playing)
                {
                    ChangeGameState(GameState.PauseMenu);
                }
                else if(_currentGameState == GameState.PauseMenu)
                {
                    ChangeGameState(GameState.Playing);
                }
            }
            else if(_inputProxy.GetButtonDown("Reset"))
            {
                ResetGame();
            }
        }

        private IEnumerator StartGameAfterTwoSec()
        {
            yield return new WaitForSeconds(2);

            ChangeGameStateToPlaying();
        }

        public void ChangeGameStateToMenuPause()
        {
            ChangeGameState(GameState.PauseMenu);
        }

        public void ChangeGameStateToPlaying()
        {
            ChangeGameState(GameState.Playing);
        }

        public void ResetGame()
        {
            ChangeGameState(GameState.Reset);

            StartCoroutine(StartGameAfterTwoSec());
        }

        private void OnPlayerLifeReachedZero()
        {
            ChangeGameState(GameState.PlayerKilled);
        }

        private void OnEnemyCountReachedZero(object sender, EventArgs e)
        {
            ChangeGameState(GameState.End);
        }

        private void OnPlayerHit(PlayerController pc)
        {
            ChangeGameState(GameState.Pause);

            StartCoroutine(StartGameAfterTwoSec());
        }

        private void ChangeGameState(GameState newGameState)
        {
            _currentGameState = newGameState;

            GameStateChanged?.Invoke(newGameState);
        }
    }
}