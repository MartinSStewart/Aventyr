﻿using Game;
using Game.Portals;
using OpenTK;
using OpenTK.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using Game.Common;
using Game.Rendering;
using Game.Serialization;

namespace EditorLogic
{
    [DataContract]
    public class ControllerCamera : ICamera2, IPortalable, IShallowClone<ControllerCamera>, IStep, ISceneObject
    {
        public delegate void CameraObjectHandler(ControllerCamera camera);
        /// <summary>Event is fired if the camera Transform is modified by this controller.</summary>
        public event CameraObjectHandler CameraMoved;
        [DataMember]
        public bool IsPortalable { get; set; }
        [DataMember]
        public PortalPath Path { get; set; } = new PortalPath();
        [DataMember]
        Transform2 _worldTransformPrevious = new Transform2();
        public Transform2 WorldTransform
        {
            get { return _worldTransformPrevious?.ShallowClone(); }
            set { _worldTransformPrevious = value?.ShallowClone(); }
        }
        [DataMember]
        Transform2 _worldVelocityPrevious = Transform2.CreateVelocity();
        public Transform2 WorldVelocity
        {
            get { return _worldVelocityPrevious?.ShallowClone(); }
            set { _worldVelocityPrevious = value?.ShallowClone(); }
        }
        public IPortalCommon Parent => null;
        public List<IPortalCommon> Children => new List<IPortalCommon>();

        public ControllerEditor Controller { get; set; }

        [DataMember]
        public string Name { get; set; } = nameof(ControllerCamera);
        [DataMember]
        public float ZoomMin = 0.5f;
        [DataMember]
        public float ZoomMax = 1000f;
        [DataMember]
        public float KeyMoveSpeed = 0.013f;
        [DataMember]
        Queue<Vector2> _lazyPan = new Queue<Vector2>();
        [DataMember]
        public Transform2 Transform { get; set; } = new Transform2();
        /// <summary>
        /// This is not used for anything.
        /// </summary>
        public Transform2 Velocity { get { throw new NotImplementedException(); } set { throw new NotImplementedException(); } }
        const int QueueSize = 3;
        [DataMember]
        public IScene Scene { get; set; }
        [DataMember] float _zoomScrollFactor;
        [DataMember]
        public Action<EnterCallbackData, Transform2, Transform2> EnterPortal { get; set; }
        /// <summary>How much the camera zooms in/out with mouse scrolling. Value must be greater than 1.</summary>
        public float ZoomScrollFactor 
        { 
            get { return _zoomScrollFactor; }
            set 
            {
                Debug.Assert(value > 1);
                _zoomScrollFactor = value;
            }
        }
        [DataMember] float _zoomFactor;
        /// <summary>How much the camera zooms in/out with key input. Value must be greater than 1.</summary>
        public float ZoomFactor
        {
            get { return _zoomFactor; }
            set 
            {  
                Debug.Assert(value > 1);
                _zoomFactor = value;
            }
        }
        public float Aspect => (float)Controller.Window.CanvasSize.XRatio;
        public Vector2 ViewOffset => new Vector2();
        public double Fov => Math.PI / 4;
        public float ZNear => -1000f;
        public float ZFar => 1000f;
        public IVirtualWindow InputExt;

        public ControllerCamera(ControllerEditor controller, IVirtualWindow inputExt, IScene scene)
        {
            IsPortalable = true;
            Scene = scene;
            Controller = controller;
            Transform = Transform.SetSize(1);
            ZoomScrollFactor = 1.2f;
            ZoomFactor = 1.5f;
            InputExt = inputExt;
            for (int i = 0; i < QueueSize; i++)
            {
                _lazyPan.Enqueue(new Vector2());
            }
        }

        public void Remove()
        {
        }

        public Transform2 GetTransform() => Transform.ShallowClone();

        public void SetTransform(Transform2 transform) => Transform = transform.ShallowClone();

        public bool IsLocked() => Controller.ActiveTool.LockCamera();

        public void StepBegin(IScene scene, float stepSize)
        {
            if (IsLocked())
            {
                return;
            }
            bool isMoved = false;

            //Handle user input for zooming the camera.
            {
                float scale = GetTransform().Size;
                if (InputExt.MouseInside())
                {
                    if (InputExt.MouseWheelDelta != 0)
                    {
                        scale /= (float)Math.Pow(ZoomScrollFactor, InputExt.MouseWheelDelta);
                        isMoved = true;
                    }
                }
                if (InputExt.ButtonPress(Key.KeypadPlus) || InputExt.ButtonPress(Key.Plus))
                {
                    scale /= ZoomFactor;
                    isMoved = true;
                }
                if (InputExt.ButtonPress(Key.KeypadMinus) || InputExt.ButtonPress(Key.Minus))
                {
                    scale *= ZoomFactor;
                    isMoved = true;
                }
                scale = MathHelper.Clamp(Math.Abs(scale), ZoomMin, ZoomMax) * Math.Sign(GetTransform().Size);
                this.SetSize(scale);
            }

            //Handle user input to reset the camera's orientation and center it on the current selected object if it exists.
            if (InputExt.ButtonPress(Key.Space))
            {
                Transform2 transform = GetTransform().SetMirrorX(false).SetRotation(0);
                transform = transform.SetSize(Math.Abs(transform.Size));
                EditorObject selected = Controller.Selection.First;
                if (selected != null)
                {
                    transform.Position = selected.GetTransform().Position;
                    if (selected is EditorPortal)
                    {
                        transform.Position += selected.GetWorldTransform().GetRight() * Portal.EnterMinDistance;
                    }
                }
                SetTransform(transform);
                isMoved = true;
            }

            //Handle user input to pan the camera.
            {
                Vector2 v = new Vector2();
                if (!InputExt.ButtonDown(KeyBoth.Control))
                {
                    if (InputExt.ButtonDown(Key.Left))
                    {
                        v += GetTransform().GetRight() * -KeyMoveSpeed * Math.Abs(GetTransform().Size);
                    }
                    if (InputExt.ButtonDown(Key.Right))
                    {
                        v += GetTransform().GetRight() * KeyMoveSpeed * Math.Abs(GetTransform().Size);
                    }
                    if (InputExt.ButtonDown(Key.Up))
                    {
                        v += GetTransform().GetUp() * KeyMoveSpeed * Math.Abs(GetTransform().Size);
                    }
                    if (InputExt.ButtonDown(Key.Down))
                    {
                        v += GetTransform().GetUp() * -KeyMoveSpeed * Math.Abs(GetTransform().Size);
                    }
                }
                if (InputExt.MouseInside() && InputExt.ButtonDown(MouseButton.Middle))
                {
                    var mouseVelocity = this.ScreenToWorld(InputExt.MousePositionPrevious - InputExt.MousePosition, Controller.Window.CanvasSize);
                    _lazyPan.Enqueue(mouseVelocity - this.ScreenToWorld(new Vector2(), Controller.Window.CanvasSize));
                }
                else
                {
                    _lazyPan.Enqueue(v);
                }
                _lazyPan.Dequeue();
            }

            //If the camera has been moved then call events.
            if ((isMoved || WorldVelocity.Position != new Vector2()))
            {
                CameraMoved?.Invoke(this);
            }
        }

        public void StepEnd(IScene scene, float stepSize)
        {
            var settings = new Ray.Settings();
            //settings.IgnorePortalVelocity = true;
            var result = Ray.RayCast(Transform, GetVelocity(), Scene.GetPortalList(), settings);
            Transform = result.WorldTransform;
            //SetVelocity(result.GetVelocity());
            WorldTransform = GetTransform();

            var velocity = GetVelocity();
            foreach (var portalEnter in result.PortalsEntered)
            {
                velocity = Portal.EnterVelocity(portalEnter.EnterData.EntrancePortal, (float)portalEnter.EnterData.PortalT, velocity);
            }
            SetVelocity(velocity);
        }

        public Transform2 GetVelocity()
        {
            Vector2 velocity = _lazyPan.Aggregate((item, acc) => item + acc) / _lazyPan.Count;
            return Transform2.CreateVelocity(velocity);
        }

        public void SetVelocity(Transform2 velocity)
        {
            _lazyPan = new Queue<Vector2>();
            for (int i = 0; i < QueueSize; i++)
            {
                _lazyPan.Enqueue(velocity.Position);
            }
        }

        public ControllerCamera ShallowClone()
        {
            ControllerCamera clone = new ControllerCamera(Controller, InputExt, Scene)
            {
                _lazyPan = new Queue<Vector2>(_lazyPan),
                _zoomFactor = _zoomFactor,
                ZoomMin = ZoomMin,
                ZoomMax = ZoomMax,
                KeyMoveSpeed = KeyMoveSpeed,
                Transform = Transform
            };
            return clone;
        }

        public List<IPortal> GetPortalChildren() => new List<IPortal>();
    }
}
