namespace Player
{
    public delegate void PlayerLifeReachedZero();
    public delegate void PlayerHit(PlayerController pc);

    public interface IPlayerLifeController
    {
        int LifesLeft { get; }

        event PlayerHit PlayerHit;
        event PlayerLifeReachedZero PlayerLifeReachedZero;

        void Respawn();
    }
}
