using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace OpenNGS.DI
{
    public class SystemBuilder : IApplicationBuilder
    {
        public IServiceCollection container = new ServiceCollection();
        
        public IServiceProvider Services {get;private set;}


        public void InitApplication(IApplicationContext context)
        {
            context.ConfigureServices(container);
            Services = container.BuildServiceProvider();
            ApplicationContext.Systems = Services;
            context.Configure(this);
            App.SetAppBuilder(this);
        }

    }
}