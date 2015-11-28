﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WPFControls
{
    /// <summary>
    /// Interaction logic for ToolButton.xaml
    /// </summary>
    public partial class ToolButton : UserControl
    {
        public ToolButton(BitmapImage image)
        {
            InitializeComponent();

            Button.Content = new System.Windows.Controls.Image
            {
                Source = image,
                VerticalAlignment = VerticalAlignment.Center
            };
        }
    }
}