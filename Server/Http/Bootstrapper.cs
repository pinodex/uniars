using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nancy;
using Nancy.TinyIoc;
using Nancy.Bootstrapper;
using Nancy.Authentication.Basic;
using Uniars.Server.Http.Auth;

namespace Uniars.Server.Http
{
    class Bootstrapper : DefaultNancyBootstrapper
    {
        protected override void ApplicationStartup(TinyIoCContainer  container, IPipelines pipelines)
        {
            base.ApplicationStartup(container, pipelines);

            StaticConfiguration.Caching.EnableRuntimeViewDiscovery = true;
            StaticConfiguration.Caching.EnableRuntimeViewUpdates = true;

            pipelines.EnableBasicAuthentication(new BasicAuthenticationConfiguration(                   
                container.Resolve<UserValidator>(), "UNIARS Server", UserPromptBehaviour.Always)
            );
        }
    }
}
