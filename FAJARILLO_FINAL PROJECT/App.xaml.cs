using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using FAJARILLO_FINAL_PROJECT.CLASSES;

namespace FAJARILLO_FINAL_PROJECT
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            AppRepository.EnsureSeedData();
            base.OnStartup(e);
        }
    }
}
