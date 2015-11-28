﻿using System;
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

using System.Data;
using System.Drawing;
using System.Windows.Forms;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using System.Threading;
using Game;
using System.Diagnostics;
using OpenTK;
using OpenTK.Input;
using System.IO;
using System.Reflection;
using WPFControls;

namespace Editor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : System.Windows.Window
    {
        GLLoop _loop;
        ControllerEditor ControllerEditor;
        delegate void SetControllerCallback(Entity entity);
        string localDir = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

        public MainWindow()
        {
            InitializeComponent();

            var property = new Xceed.Wpf.Toolkit.PropertyGrid.PropertyGrid();
            gridSideColumn.Children.Add(property);
            Grid.SetRow(property, 1);
            
            for (int i = 0; i < 3; i++)
            {
                ToolButton button = new ToolButton(new BitmapImage(new Uri(localDir + @"\assets\icons\entityIcon.png")));
                ToolPanel.Children.Add(button);
                /*var button = new System.Windows.Controls.Button();
                button.Width = 80;
                button.Height = 80;
                button.Content = new System.Windows.Controls.Image
                {
                    Source = new BitmapImage(new Uri(localDir + @"\assets\icons\entityIcon.png")),
                    //Source = new BitmapImage(new Uri("C:\\Users\\Martin\\Documents\\Visual Studio 2013\\Projects\\Game2\\Editor\\assets\\entityIcon.png")),
                    VerticalAlignment = VerticalAlignment.Center
                };
                ToolPanel.Children.Add(button);*/
            }
        }

        public void GLControl_Load(object sender, EventArgs e)
        {
            ControllerEditor = new ControllerEditor(glControl.ClientSize, new InputExt(glControl));
            //ControllerEditor.EntityAdded += ControllerEditor_EntityAdded;
            ControllerEditor.ScenePlayed += ControllerEditor_ScenePlayed;
            ControllerEditor.ScenePaused += ControllerEditor_ScenePaused;
            ControllerEditor.SceneStopped += ControllerEditor_ScenePaused;
            _loop = new GLLoop(glControl, ControllerEditor);
            _loop.Run(60);
        }

        public void GLControl_Paint(object sender, PaintEventArgs e)
        {

        }

        public void GLControl_Resize(object sender, EventArgs e)
        {
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            base.OnClosing(e);
            _loop.Stop();
            lock (_loop)
            {
            }
        }

        private void Button_Close(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Button_Play(object sender, RoutedEventArgs e)
        {
            ControllerEditor.ScenePlay();
        }

        private void Button_Pause(object sender, RoutedEventArgs e)
        {
            ControllerEditor.ScenePause();
        }

        private void Button_Stop(object sender, RoutedEventArgs e)
        {
            ControllerEditor.SceneStop();
        }

        private void ControllerEditor_ScenePaused(ControllerEditor controller, Scene scene)
        {
            //propertyGrid.Enabled = true;
            toolStart.IsEnabled = true;
            toolPause.IsEnabled = false;
            toolStop.IsEnabled = false;
            menuRunStop.IsEnabled = false;
            menuRunStart.IsEnabled = true;
            menuRunPause.IsEnabled = false;
        }

        private void ControllerEditor_ScenePlayed(ControllerEditor controller, Scene scene)
        {
            //propertyGrid.Enabled = false;
            toolStart.IsEnabled = false;
            toolPause.IsEnabled = true;
            toolStop.IsEnabled = true;
            menuRunStop.IsEnabled = true;
            menuRunStart.IsEnabled = false;
            menuRunPause.IsEnabled = true;
        }

        /*private void ControllerEditor_EntityAdded(LevelEditor.ControllerEditor controller, Entity entity)
        {
            SetCurrentEntity(entity);
        }

        private void SetCurrentEntity(Entity entity)
        {
            // InvokeRequired required compares the thread ID of the
            // calling thread to the thread ID of the creating thread.
            // If these threads are different, it returns true.
            if (this.propertyGrid.InvokeRequired)
            {
                SetControllerCallback d = new SetControllerCallback(SetCurrentEntity);
                this.Invoke(d, new object[] { entity });
            }
            else
            {
                propertyGrid.SelectedObject = new EntityProperty(entity);
            }
        }*/
    }
}