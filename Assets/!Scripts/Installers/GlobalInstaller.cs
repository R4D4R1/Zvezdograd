using UnityEngine;
using Zenject;

public class GlobalInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        Container.Bind<SoundController>().FromComponentInHierarchy().AsSingle().NonLazy();
        Container.Bind<LoadLevelController>().FromComponentInHierarchy().AsSingle().NonLazy();
        Container.Bind<SettingsController>().FromComponentInHierarchy().AsSingle().NonLazy();
    }
}
