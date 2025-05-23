﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ush4.Infrastructure
{
    public interface IWindowService
    {
        void ShowWindow<T>(object dataContext) where T : Window, new();
    }

    public class WindowService : IWindowService
    {
        public void ShowWindow<T>(object dataContext) where T : Window, new()
        {
            var window = new T
            {
                DataContext = dataContext
            };

            window.Show();
        }
    }

}
