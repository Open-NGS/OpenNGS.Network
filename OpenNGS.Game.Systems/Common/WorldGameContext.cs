using Microsoft.Extensions.DependencyInjection;
using OpenNGS;
using OpenNGS.Systems;
using System;
using System.Collections.Generic;
using Systems;

public class WorldGameContext : GameContext
{
    //private IApplicationBuilder 
    public override void Configure(IApplicationBuilder app)
    {
    }

    public override void ConfigureServices(IServiceCollection services)
    {
        services.Add(new ServiceDescriptor(typeof(ISaveSystem), new SaveSystem()));
        services.Add(new ServiceDescriptor(typeof(IExchangeSystem), new ExchangeSystem()));
        services.Add(new ServiceDescriptor(typeof(IItemSystem),new ItemSystem()));
        services.Add(new ServiceDescriptor(typeof(ICharacterSystem), new CharacterSystem()));
        services.Add(new ServiceDescriptor(typeof(IShopSystem), new ShopSystem()));
        services.Add(new ServiceDescriptor(typeof(ISettingSystem), new SettingSystem()));
        services.Add(new ServiceDescriptor(typeof(IDialogSystem), new DialogSystem()));
        services.Add(new ServiceDescriptor(typeof(IEquipSystem), new EquipSystem()));
        services.Add(new ServiceDescriptor(typeof(ITechnologySystem), new TechnologySystem()));
        services.Add(new ServiceDescriptor(typeof(IRankSystem), new RankSystem()));
        services.Add(new ServiceDescriptor(typeof(IStatSystem), new StatSystem()));
        services.Add(new ServiceDescriptor(typeof(IAchievementSystem), new AchievementSystem()));
    }

    protected override void OnInit()
    {
        //Logo, Splash
    }
}
