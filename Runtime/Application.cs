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
}