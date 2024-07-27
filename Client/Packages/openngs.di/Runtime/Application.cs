using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace OpenNGS
{
    public interface IApplicationContext
    {
        //public abstract void Startup(IConfiguration configuration);
        
        public void ConfigureServices(IServiceCollection services);


        public void Configure(IApplicationBuilder app);

    }

    public interface IApplicationBuilder
    {
        public IServiceProvider Services { get; }
    }

    public static class App
    {
        private static IApplicationBuilder m_builder;
        public static T GetService<T>()
        {
            return (T)m_builder.Services.GetService(typeof(T));
        }
        public static void SetAppBuilder(IApplicationBuilder _appBuilder)
        {
            m_builder = _appBuilder;
        }
    }
}