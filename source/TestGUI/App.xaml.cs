﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;
using TinyMVVM.Framework;

namespace TestGUI
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
          	ServiceLocator.SetLocator(new ServiceLocatorForClient());

            base.OnStartup(e);
            
        }
    }
}
