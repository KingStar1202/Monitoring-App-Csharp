﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

using Telerik.Windows.Controls;

namespace Camera
{
    /// <summary>
    /// Interaction logic for test.xaml
    /// </summary>
    public partial class test : RadRibbonWindow
    {
        static test()
        {
            test.IsWindowsThemeEnabled = false;
        }

        public test()
        {
            InitializeComponent();
        }
    }
}
