using Autofac;
using Caliburn.Micro;
using youtube_dl.WPF.Presentation.ViewModels;
using System;
using System.Collections.Generic;
using System.Windows;
using youtube_dl.WPF.Presentation.Views;

namespace youtube_dl.WPF.Presentation.Composition
{
    public class CustomBootstrapperBase<TShellViewModel> : BootstrapperBase // https://github.com/Caliburn-Micro/Caliburn.Micro/blob/master/src/Caliburn.Micro.Platform/Bootstrapper.cs
        where TShellViewModel : IScreen
    {
        #region properties

        protected IContainer Container { get; private set; }

        protected IDictionary<string, object> RootViewDIsplaySettings { get; set; }

        #endregion

        #region methods

        protected virtual void RegisterComponents(ContainerBuilder builder) { }

        protected override void Configure()
        {
            this.ConfigureBootstrapper();

            var builder = new ContainerBuilder();

            this.RegisterComponents(builder);

            this.Container = builder.Build();
        }

        protected override object GetInstance(Type service, string key)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                if (this.Container.TryResolve(service, out object obj))
                    return obj;
            }
            else
            {
                if (this.Container.TryResolveNamed(key, service, out object obj))
                    return obj;
            }
            throw new Exception(string.Format("Could not locate any instances of contract {0}.", key ?? service.Name)); // TODO: localize
        }

        protected override IEnumerable<object> GetAllInstances(Type service)
        {
            return this.Container.Resolve(typeof(IEnumerable<>).MakeGenericType(new[] { service })) as IEnumerable<object>;
        }

        protected override void BuildUp(object instance)
        {
            this.Container.InjectProperties(instance);
        }

        protected virtual void ConfigureBootstrapper()
        {
            var config = new TypeMappingConfiguration
            {
                DefaultSubNamespaceForViews = typeof(ShellView).Namespace,
                DefaultSubNamespaceForViewModels = typeof(ShellViewModel).Namespace
            };
            ViewLocator.ConfigureTypeMappings(config);
            ViewModelLocator.ConfigureTypeMappings(config);
        }

        protected override void OnStartup(object sender, StartupEventArgs e)
        {
            base.OnStartup(sender, e);

            this.DisplayRootViewFor<TShellViewModel>(this.RootViewDIsplaySettings);
        }

        #endregion
    }
}