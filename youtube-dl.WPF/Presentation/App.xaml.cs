using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using youtube_dl.WPF.Presentation.Composition.Autofac;

namespace youtube_dl.WPF.Presentation
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private readonly AutofacBootstrapper _bootstrapper;

        public App()
        {
            this._bootstrapper = new AutofacBootstrapper();
            this._bootstrapper.Initialize();
        }
    }
}