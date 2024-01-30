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
<<<<<<< HEAD
            App.SetAppBuilder(this);
=======

            foreach (var system in container) {

                var service = Services.GetService(system.ServiceType);
                if (service is ISystem)
                {
                    var sys = (ISystem)service;

                    Debug.Log($"SystemBuilder Init: {sys.GetSystemName()}");
                    sys.Init();
                }
                
                
            }
>>>>>>> 0818b958d2865e59204d29dea2775ff428dac879
        }

    }
}