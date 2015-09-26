﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using System.ComponentModel;
using FarseerPhysics;
using Xna = Microsoft.Xna.Framework;
using FarseerPhysics.Factories;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Collision.Shapes;
using System.Diagnostics;
//using FarseerPhysics.Common;


namespace Game
{
    public class Controller : GameWindow
    {
        public Controller()
            : base((int) 800, (int) 600, new GraphicsMode(32, 24, 8, 1), "Game", GameWindowFlags.FixedWindow)
        {
            ContextExists = true;
        }
        InputExt InputExt;
        Camera cam, hudCam;
        Vector2 lastMousePos = new Vector2();
        /// <summary>
        /// Intended to keep pointless messages from the Poly2Tri library out of the console window
        /// </summary>
        public static StreamWriter Log = new StreamWriter("Triangulating.txt");
        public static bool ContextExists = false;
        Model background;
        Font Default;
        Entity box2;
        Entity intersectDot;
        public static List<int> iboGarbage = new List<int>();

        public static Dictionary<string, int> textures = new Dictionary<string, int>();
        public static Dictionary<string, ShaderProgram> Shaders = new Dictionary<string, ShaderProgram>();

        public static String fontFolder = Path.Combine(new String[2] {
            "assets",
            "fonts"
        });
        public static String shaderFolder = Path.Combine(new String[2] {
            "assets",
            "shaders"
        });
        public static String textureFolder = Path.Combine(new String[2] {
            "assets",
            "textures"
        });
        Matrix4 viewMatrix;
        Scene scene, hud;
        FontRenderer FontRenderer;
        Entity tempLine;
        float Time = 0.0f;
        /// <summary>
        /// The difference in seconds between the last OnUpdateEvent and the current OnRenderEvent.
        /// </summary>
        float TimeRenderDelta = 0.0f;
        private Entity player;
        Portal portal1;
        Entity text, text2;
        void initProgram()
        {
            scene = new Scene(this);
            hud = new Scene(this);
            hudCam = Camera.CameraOrtho(new Vector3(Width/2, Height/2, 0), Height, Width / (float)Height);

            System.Drawing.Text.PrivateFontCollection privateFonts = new System.Drawing.Text.PrivateFontCollection();
            privateFonts.AddFontFile(Path.Combine(fontFolder, "times.ttf"));
            Default = new Font(privateFonts.Families[0], 14);
            FontRenderer = new FontRenderer(Default);

            InputExt = new InputExt(this);
            lastMousePos = new Vector2(Mouse.X, Mouse.Y);

            // Load shaders from file
            Shaders.Add("default", new ShaderProgram(Path.Combine(shaderFolder, "vs.glsl"), Path.Combine(shaderFolder, "fs.glsl"), true));
            Shaders.Add("textured", new ShaderProgram(Path.Combine(shaderFolder, "vs_tex.glsl"), Path.Combine(shaderFolder, "fs_tex.glsl"), true));
            Shaders.Add("text", new ShaderProgram(Path.Combine(shaderFolder, "vs_text.glsl"), Path.Combine(shaderFolder, "fs_text.glsl"), true));

            // Load textures from file
            textures.Add("default.png", loadImage(Path.Combine(textureFolder, "default.png")));
            textures.Add("grid.png", loadImage(Path.Combine(textureFolder, "grid.png")));
            // Create our objects
            

            background = Model.CreatePlane();
            background.TextureID = textures["grid.png"];
            background.Transform.Position = new Vector3(0, 0, -10f);
            float size = 100;
            background.Transform.Scale = new Vector3(size, size, size);
            background.TransformUV.Scale = new Vector2(size, size);
            Entity back = scene.CreateEntity(new Vector2(0f, 0f));
            back.Models.Add(background);

            Portal portal0 = scene.CreatePortal();
            portal0.Transform.Rotation = (float)Math.PI;
            portal0.Transform.Position = new Vector2(.1f, 0f);
            portal0.Transform.Scale = new Vector2(-1.5f, -1.5f);

            Entity portalEntity0 = scene.CreateEntity();
            portalEntity0.Transform = portal0.Transform;
            portalEntity0.Models.Add(Model.CreatePlane());
            portalEntity0.Models[0].Transform.Scale = new Vector3(0.1f, 0.05f, 1);
            portalEntity0.Models[0].Transform.Position = new Vector3(0.05f, 0.4f, 0.5f);
            portalEntity0.Models.Add(Model.CreatePlane());
            portalEntity0.Models[1].Transform.Scale = new Vector3(0.05f, 1, 0.5f);

            portal1 = scene.CreatePortal();
            //portal1.Transform.Rotation = 4.4f;
            portal1.Transform.Position = new Vector2(-3f, 0f);
            portal1.Transform.Scale = new Vector2(-1f, -1f);

            Portal.Link(portal0, portal1);
            //Portal.Link(portal1, portal1);
            Entity portalEntity1 = scene.CreateEntity();
            portalEntity1.Transform = portal1.Transform;
            portalEntity1.Models.Add(Model.CreatePlane());
            portalEntity1.Models[0].Transform.Scale = new Vector3(0.1f, 0.05f, 1);
            portalEntity1.Models[0].Transform.Position = new Vector3(0.05f, 0.4f, 0.5f);
            portalEntity1.Models.Add(Model.CreatePlane());
            portalEntity1.Models[1].Transform.Scale = new Vector3(0.05f, 1, 0.5f);


            Portal portal2 = scene.CreatePortal();
            portal2.Transform.Rotation = 0.1f;//(float)Math.PI/4f;
            portal2.Transform.Position = new Vector2(0.1f, 2f);
            portal2.Transform.Scale = new Vector2(1f, 1f);

            Entity portalEntity2 = scene.CreateEntity();
            portalEntity2.Transform = portal2.Transform;
            portalEntity2.Models.Add(Model.CreatePlane());
            portalEntity2.Models[0].Transform.Scale = new Vector3(0.1f, 0.05f, 1);
            portalEntity2.Models[0].Transform.Position = new Vector3(0.05f, 0.4f, 0.5f);
            portalEntity2.Models.Add(Model.CreatePlane());
            portalEntity2.Models[1].Transform.Scale = new Vector3(0.05f, 1, 0.5f);


            Portal portal3 = scene.CreatePortal();
            portal3.Transform.Rotation = 0.4f;
            portal3.Transform.Position = new Vector2(-1f, 2f);
            portal3.Transform.Scale = new Vector2(-1f, 1f);

            Portal.Link(portal2, portal3);
            Entity portalEntity3 = scene.CreateEntity();
            portalEntity3.Transform = portal3.Transform;
            portalEntity3.Models.Add(Model.CreatePlane());
            portalEntity3.Models[0].Transform.Scale = new Vector3(0.1f, 0.05f, 1);
            portalEntity3.Models[0].Transform.Position = new Vector3(0.05f, 0.4f, 0.5f);
            portalEntity3.Models.Add(Model.CreatePlane());
            portalEntity3.Models[1].Transform.Scale = new Vector3(0.05f, 1, 0.5f);

            #region cubes
            Model tc = Model.CreateCube();
            tc.Transform.Position = new Vector3(1f, 3f, 0);
            Entity box = scene.CreateEntity(new Vector2(0, 0));
            box.Models.Add(tc);

            Model tc2 = Model.CreateCube();
            tc2.Transform.Position = new Vector3(-1f, 3f, 0);
            tc2.Transform.Rotation = new Quaternion(1, 0, 0, 1);
            box2 = scene.CreateEntity(new Vector2(0, 0));
            box2.Models.Add(tc2);
            #endregion

            #region player
            player = scene.CreateEntity();
            Model playerModel = Model.CreatePolygon(new Vector2[] {
                new Vector2(0.5f, 0), 
                new Vector2(0.35f, 0.15f), 
                new Vector2(0.15f, 0.15f), 
                new Vector2(0.15f, 0.35f), 
                new Vector2(0, 0.5f), 
                new Vector2(-0.5f, 0), 
                new Vector2(0, -0.5f)
            });
            //playerModel.Transform.Scale = new Vector3(-15, .2f, 1);
            playerModel.SetTexture(Controller.textures["default.png"]);
            player.IsPortalable = true;
            //player.Transform.Scale = new Vector2(.5f, .5f);
            player.Transform.Position = new Vector2(0f, 0f);
            player.Models.Add(playerModel);
            playerModel.SetTexture(Controller.textures["default.png"]);
            #endregion

            intersectDot = scene.CreateEntity();
            //intersectDot.Models.Add(Model.CreateCube());
            intersectDot.Models = portalEntity0.Models;
            intersectDot.Transform.Scale = new Vector2(1f, 1f);

            Vector2[] v = new Vector2[5] {
                new Vector2(0.01f, 0) * 2,
                new Vector2(1f, 0.5f) * 2,
                new Vector2(1f, -1f) * 2,
                new Vector2(-1f, -1.1f) * 2,
                new Vector2(-0.5f, 0)
            };
            
            Entity ground = scene.CreateEntityPolygon(new Vector2(0, -2), new Vector2(0, 0), v);
            ground.Models.Add(Model.CreatePolygon(v));

            ground.Transform.Rotation = 0.5f;
            
            Entity origin = scene.CreateEntityBox(new Vector2(0.4f, 0f), new Vector2(1.5f, 1.5f));

            text = hud.CreateEntity();
            text.Transform.Position = new Vector2(0, ClientSize.Height);
            text2 = hud.CreateEntity();
            text2.Transform.Position = new Vector2(0, ClientSize.Height - 40);

            cam = Camera.CameraOrtho(new Vector3(player.Transform.Position.X, player.Transform.Position.Y, 10f), 10, Width / (float)Height);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            initProgram();

            GL.ClearColor(Color.HotPink);
            GL.ClearStencil(0);
            GL.PointSize(5f);
            GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);

            OnUpdateFrame(new FrameEventArgs());
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);
            TimeRenderDelta += (float)e.Time;
            text.Models.Clear();
            text.Models.Add(FontRenderer.GetModel(((float)e.Time).ToString(), new Vector2(0f, 0f), 0));

            GL.Viewport(0, 0, Width, Height);
            GL.Clear(ClearBufferMask.DepthBufferBit | ClearBufferMask.StencilBufferBit);

            Shaders["textured"].EnableVertexAttribArrays();
            Shaders["default"].EnableVertexAttribArrays();

            // Update model view matrices
            viewMatrix = cam.GetViewMatrix();
            //GL.Disable(EnableCap.StencilTest);
            GL.Enable(EnableCap.DepthTest);
            
            scene.DrawScene(viewMatrix, (float)e.Time);
            
            Vector2 viewPos = new Vector2(player.Transform.Position.X, player.Transform.Position.Y);
            TextWriter console = Console.Out;
            Console.SetOut(Controller.Log);
            scene.DrawPortalAll(scene.Portals.ToArray(), viewMatrix, viewPos, 6, TimeRenderDelta);
            Console.SetOut(console);

            GL.Clear(ClearBufferMask.DepthBufferBit);
            GL.Enable(EnableCap.Blend);
            GL.Disable(EnableCap.DepthTest);
            hud.DrawScene(hudCam.GetViewMatrix(), (float)e.Time);
            GL.Enable(EnableCap.DepthTest);
            Shaders["textured"].DisableVertexAttribArrays();
            Shaders["default"].DisableVertexAttribArrays();

            GL.Flush();
            SwapBuffers();
        }

        private void ToggleFullScreen()
        {
            if (WindowState == OpenTK.WindowState.Normal)
            {
                WindowState = OpenTK.WindowState.Fullscreen;
                cam.Aspect = Width / (float)Height;
                hudCam.Aspect = cam.Aspect;
                hudCam.Scale = Height;
            }
            else if (WindowState == OpenTK.WindowState.Fullscreen)
            {
                WindowState = OpenTK.WindowState.Normal;
                ClientSize = new Size(800, 600);
                cam.Aspect = Width / (float)Height;
                hudCam.Aspect = cam.Aspect;
                hudCam.Scale = Height;
            }
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            base.OnUpdateFrame(e);
            Time += (float)e.Time;
            TimeRenderDelta = 0;
            
            InputExt.Update();
            if (InputExt.KeyPress(Key.F4))
            {
                ToggleFullScreen();   
            }

            

            scene.RemoveEntity(tempLine);
            int lineIndex = -1;
            tempLine = scene.CreateEntity();
            tempLine.Transform.Position = player.Transform.Position;

            Vector2 rayBegin = player.Transform.Position;
            Vector2 rayEnd = VectorExt2.Transform(new Vector2(Mouse.X / (float)(ClientSize.Width / 2) - 1f, -(Mouse.Y / (float)(ClientSize.Height / 2) - 1f)), viewMatrix.Inverted());
            tempLine.IsPortalable = true;
            tempLine.Models.Add(Model.CreateLine(new Vector2[2] {
                rayBegin - player.Transform.Position, 
                rayEnd - player.Transform.Position
                }));

            PortalPlacer.PortalPlace(portal1, new Line(rayBegin, rayEnd));

            text2.Models.Clear();
            text2.Models.Add(FontRenderer.GetModel(lineIndex.ToString()));

            #region camera movement
            if (Focused)
            {
                if (InputExt.KeyPress(Key.Escape))
                {
                    Exit();
                }
                Vector3 v = new Vector3();
                float camSpeed = .05f;
                if (InputExt.KeyDown(Key.ShiftLeft))
                {
                    camSpeed = .005f;
                }
                if (InputExt.KeyDown(Key.R))
                {
                    Quaternion rot = cam.Transform.Rotation;
                    rot.W += .01f;
                    cam.Transform.Rotation = rot;
                    player.Transform.Rotation += .01f;
                }
                if (InputExt.KeyDown(Key.W))
                {
                    v += cam.GetUp() * camSpeed * cam.Transform.Scale.Y;
                }
                else if (InputExt.KeyDown(Key.S))
                {
                    v -= cam.GetUp() * camSpeed * cam.Transform.Scale.Y;
                }
                if (InputExt.KeyDown(Key.A))
                {
                    v -= cam.GetRight() * camSpeed * cam.Transform.Scale.X;
                }
                else if (InputExt.KeyDown(Key.D))
                {
                    v += cam.GetRight() * camSpeed * cam.Transform.Scale.X;
                }
                if (InputExt.MouseWheelDelta() != 0)
                {
                    cam.Scale /= (float)Math.Pow(1.2, InputExt.MouseWheelDelta());
                }
                else if (InputExt.KeyDown(Key.Q))
                {
                    cam.Scale /= (float)Math.Pow(1.04, 1);
                }
                else if (InputExt.KeyDown(Key.E))
                {
                    cam.Scale /= (float)Math.Pow(1.04, -1);
                }
                Vector2[] vArray = new Vector2[2];
                IntersectPoint i = new IntersectPoint();
                Portal portalEnter = null;

                Vector2 posPrev = player.Transform.Position;
                player.Transform.Position += new Vector2(v.X, v.Y);
                player.PositionUpdate();
                foreach (Portal p in scene.Portals)
                {
                    vArray = p.GetWorldVerts();
                    portalEnter = p;
                    Vector2 v1 = new Vector2(player.Transform.Position.X, player.Transform.Position.Y);
                    i = MathExt.LineIntersection(vArray[0], vArray[1], posPrev, player.Transform.Position, true);
                    if (i.Exists)
                    {
                        break;
                    }
                }
                
                if (i.Exists)
                {
                    portalEnter.Enter(player.Transform);
                }

                cam.Transform = player.Transform.Get3D();
            }
            #endregion
            
            /*Console.Write(box2.Models[0].Transform.Rotation.W);
            Console.WriteLine();*/
            //portal1.Transform.Rotation += .001f;
            
            box2.Models[0].Transform.Rotation += new Quaternion(0, 0, 0, .01f);

            scene.Step();

            //get rid of all ibo elements no longer used
            lock ("delete")
            {
                foreach (int iboElement in iboGarbage.ToArray())
                {
                    int a = iboElement;
                    GL.DeleteBuffers(1, ref a);
                }
                iboGarbage.Clear();
            }
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);
            Log.Close();
            
            File.Delete("Triangulating.txt");
        }

        int loadImage(Bitmap image)
        {
            int texID = GL.GenTexture();

            GL.BindTexture(TextureTarget.Texture2D, texID);
            BitmapData data = image.LockBits(new System.Drawing.Rectangle(0, 0, image.Width, image.Height),
                ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, data.Width, data.Height, 0,
                OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, data.Scan0);

            image.UnlockBits(data);

            GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);

            return texID;
        }

        int loadImage(string filename)
        {
            try
            {
                Bitmap file = new Bitmap(filename);
                return loadImage(file);
            }
            catch (FileNotFoundException e)
            {
                return -1;
            }
        }
    }
}
