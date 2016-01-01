﻿using Game;
using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Editor
{
    public class EditorEntity : EditorObject
    {
        public Entity Entity { get; private set; }
        public Entity Marker { get; private set; }

        public EditorEntity(ControllerEditor controller, Scene scene, Scene overlay)
            : base(controller)
        {
            Entity = new Entity(scene);
            Marker = new Entity(overlay);
            Marker.SetParent(Entity);
            Model circle = ModelFactory.CreateCircle(new Vector3(0, 0, 10), 0.05f, 10);
            circle.SetColor(new Vector3(1f, 0.5f, 0f));
            Marker.Models.Add(circle);
        }

        public EditorEntity(EditorEntity editorEntity)
            : base(editorEntity)
        {
            //Entity = new Entity(editorEntity);
        }

        public void Remove()
        {
            Entity.Scene.RemoveEntity(Entity);
            Marker.Scene.RemoveEntity(Marker);
        }

        public override void SetTransform(Transform2D transform)
        {
            base.SetTransform(transform);
            Entity.SetTransform(transform);
        }

        public override void SetPosition(Vector2 position)
        {
            base.SetPosition(position);
            Entity.SetPosition(position);
        }

        public override Transform2D GetTransform()
        {
            return Entity.GetTransform();
        }

        public override Transform2D GetWorldTransform()
        {
            return Entity.GetWorldTransform();
        }
    }
}
