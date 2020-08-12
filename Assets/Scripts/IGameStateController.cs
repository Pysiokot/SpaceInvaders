namespace Utils
{
    public delegate void GameStateChanged(GameState newState);

    public enum GameState
    {
        Reset,
        Playing,
        Pause,
        Respawning,
        PauseMenu,
        PlayerKilled,
        End
    }

    public interface IGameStateController
    {
        event GameStateChanged GameStateChanged;

        void ChangeGameStateToMenuPause();
        void ChangeGameStateToPlaying();
        void ResetGame();
    }
}
