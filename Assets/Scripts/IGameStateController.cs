﻿namespace Utils
{
    public delegate void GameStateChanged(GameState newState);

    public enum GameState
    {
        Playing,
        Pause,
        PauseMenu,
        PlayerKilled,
        End
    }

    public interface IGameStateController
    {
        event GameStateChanged GameStateChanged;

        void ChangeGameStateToMenuPause();
        void ChangeGameStateToPlaying();
    }
}