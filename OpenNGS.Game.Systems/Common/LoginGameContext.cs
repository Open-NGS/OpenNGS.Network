using Microsoft.Extensions.DependencyInjection;
using OpenNGS;
using OpenNGS.Systems;
using System;
using System.Collections.Generic;
using Systems;

public class LoginGameContext : GameContext
{
    //private IApplicationBuilder 
    public override void Configure(IApplicationBuilder app)
    {
    }

    public override void ConfigureServices(IServiceCollection services)
    {
        services.Add(new ServiceDescriptor(typeof(IExchangeSystem), new ExchangeSystem()));
    }

    protected override void OnInit()
    {
        //Logo, Splash
    }
}
