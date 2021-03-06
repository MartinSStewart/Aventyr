﻿using EditorLogic;
using System;
using System.Collections.Generic;
using System.Linq;
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
using EditorLogic.Tools;

namespace EditorWindow
{
    /// <summary>
    /// Interaction logic for ToolButton.xaml
    /// </summary>
    public partial class ToolButton : UserControl
    {
        /// <summary>
        /// Default Tool instantiation. Typically a copy of ToolDefault will be used.  Modifying ToolDefault will apply changes to all future copies.
        /// </summary>
        Tool _tool;
        ControllerEditor _controller;
        public ToolButton(ControllerEditor controller, Tool tool, BitmapImage image)
        {
            InitializeComponent();
            _tool = tool;
            _controller = controller;
            if (image != null)
            {
                Button.Content = new System.Windows.Controls.Image
                {
                    Source = image,
                    VerticalAlignment = VerticalAlignment.Center
                };
            }
            Button.Click += Button_Click;
        }

        void Button_Click(object sender, RoutedEventArgs e)
        {
            _controller.AddAction(() => { _controller.SetTool(_tool); });
        }
    }
}
