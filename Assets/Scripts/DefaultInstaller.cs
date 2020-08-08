using Enemy;
using SceneManagement;
using UserInput;
using Zenject;

public class DefaultInstaller : MonoInstaller<DefaultInstaller>
{
    public override void InstallBindings()
    {
        Container.Bind<IInputProxy>().To<KeyboardInput>().AsSingle();
        Container.Bind<IEnemyLifeController>().To<EnemyGroupController>().AsSingle();
    }
}