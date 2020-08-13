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

        [SerializeField]
        private float _timeToRespawn;
        [SerializeField]
        private float _timeToStartGame;

        private IPlayerLifeController _playerLifeController;
        private IEnemyGroupLifeController _enemyGroupLifeController;
        private IInputProxy _inputProxy;
        private IEnemyTargetController _enemyTargetController;

        private GameState _currentGameState;

        private void Start()
        {
            ChangeGameState(GameState.Reset);

            StartCoroutine(StartGame());
        }

        [Inject]
        private void InitializeDI(IPlayerLifeController playerLifeController, IEnemyGroupLifeController enemyGroupLifeController, IInputProxy inputProxy, IEnemyTargetController enemyTargetController)
        {
            _playerLifeController = playerLifeController;
            _enemyGroupLifeController = enemyGroupLifeController;
            _inputProxy = inputProxy;
            _enemyTargetController = enemyTargetController;

            _playerLifeController.PlayerHit += OnPlayerHit;
            _playerLifeController.PlayerLifeReachedZero += OnPlayerLifeReachedZero;
            _enemyGroupLifeController.EnemyCountReachedZero += OnEnemyCountReachedZero;
            _enemyTargetController.EnemyReachedTarget += OnEnemyReachedTarget;
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

            StartCoroutine(StartGame());
        }

        private void OnPlayerLifeReachedZero()
        {
            ChangeGameState(GameState.PlayerKilled);
        }

        private void OnEnemyCountReachedZero(object sender, EventArgs e)
        {
            // TODO: Spawn new wave

            ChangeGameState(GameState.End);
        }

        private void OnPlayerHit(PlayerController pc)
        {
            ChangeGameState(GameState.Pause);

            StartCoroutine(Respawn());
        }

        private void OnEnemyReachedTarget()
        {
            ChangeGameState(GameState.PlayerKilled);
        }

        private void ChangeGameState(GameState newGameState)
        {
            _currentGameState = newGameState;

            GameStateChanged?.Invoke(newGameState);
        }

        private IEnumerator StartGame()
        {
            yield return new WaitForSeconds(_timeToStartGame);

            ChangeGameStateToPlaying();
        }

        private IEnumerator Respawn()
        {
            yield return new WaitForSeconds(_timeToRespawn);

            ChangeGameState(GameState.Respawning);

            yield return new WaitForSeconds(0.5f);

            ChangeGameState(GameState.Playing);
        }
    }
}