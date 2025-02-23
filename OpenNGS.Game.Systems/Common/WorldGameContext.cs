using Microsoft.Extensions.DependencyInjection;
using OpenNGS;
using OpenNGS.Systems;

public class WorldGameContext : GameContext
{
    //private IApplicationBuilder 
    public override void Configure(IApplicationBuilder app)
    {
        
    }

    public override void ConfigureServices(IServiceCollection services)
    {
        services.Add(new ServiceDescriptor(typeof(ICharacterSystem), new CharacterSystem()));
        services.Add(new ServiceDescriptor(typeof(IMakeSystem), new MakeSystem()));
        services.Add(new ServiceDescriptor(typeof(IRecordSystem), new RecordSystem()));
        services.Add(new ServiceDescriptor(typeof(IEquipSystem), new EquipSystem()));
        services.Add(new ServiceDescriptor(typeof(ITechnologySystem), new TechnologySystem()));
        services.Add(new ServiceDescriptor(typeof(IRankSystem), new RankSystem()));
#if UNITY_5_3_OR_NEWER
        services.Add(new ServiceDescriptor(typeof(INotificationSystem), new NotificationSystem()));
#endif
        services.Add(new ServiceDescriptor(typeof(IRewardSystem), new RewardSystem()));

        services.Add(new ServiceDescriptor(typeof(INgStatisticSystem), new NgStatisticSystem()));
        services.Add(new ServiceDescriptor(typeof(INgItemSystem), new NgItemSystem()));
        services.Add(new ServiceDescriptor(typeof(INgExchangeSystem), new NgExchangeSystem()));
        services.Add(new ServiceDescriptor(typeof(INgShopSystem), new NgShopSystem()));
        services.Add(new ServiceDescriptor(typeof(INgDialogSystem), new NgDialogSystem()));
        services.Add(new ServiceDescriptor(typeof(INgSettingSystem), new NgSettingSystem()));
        services.Add(new ServiceDescriptor(typeof(INgBlindBoxSystem), new NgBlindBoxSystem()));
        services.Add(new ServiceDescriptor(typeof(INgQuestSystem), new NgQuestSystem()));
        services.Add(new ServiceDescriptor(typeof(INgCollectionSystem), new NgCollectionSystem()));
        services.Add(new ServiceDescriptor(typeof(INgAchievementSystem), new NgAchievementSystem()));
    }

    protected override void OnInit()
    {
        //Logo, Splash
    }
}
