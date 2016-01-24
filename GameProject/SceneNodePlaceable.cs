﻿using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Game
{
    [DataContract]
    public class SceneNodePlaceable : SceneNode, ITransform2D
    {
        [DataMember]
        Transform2D _transform = new Transform2D();
        [DataMember]
        Transform2D _velocity = new Transform2D();
        /// <summary>
        /// Whether or not this entity will interact with portals when intersecting them
        /// </summary>
        [DataMember]
        public bool IsPortalable { get; set; }

        public SceneNodePlaceable(Scene scene)
            : base (scene)
        {
        }

        public override SceneNode Clone(Scene scene)
        {
            SceneNodePlaceable clone = new SceneNodePlaceable(scene);
            Clone(clone);
            return clone;
        }

        protected override void Clone(SceneNode destination)
        {
            base.Clone(destination);
            SceneNodePlaceable destinationCast = (SceneNodePlaceable)destination;
            destinationCast.SetTransform(GetTransform());
        }

        public virtual void SetTransform(Transform2D transform)
        {
            _transform = transform.Clone();
        }

        public virtual void SetPosition(Vector2 position)
        {
            Transform2D transform = GetTransform();
            transform.Position = position;
            SetTransform(transform);
        }

        public virtual void SetRotation(float rotation)
        {
            Transform2D transform = GetTransform();
            transform.Rotation = rotation;
            SetTransform(transform);
        }

        public virtual void SetScale(Vector2 scale)
        {
            Transform2D transform = GetTransform();
            transform.Scale = scale;
            SetTransform(transform);
        }

        public override Transform2D GetVelocity()
        {
            return _velocity.Clone();
        }

        public void SetVelocity(Transform2D transform)
        {
            _velocity = transform.Clone();
        }

        public override Transform2D GetTransform()
        {
            return _transform.Clone();
        }
    }
}
