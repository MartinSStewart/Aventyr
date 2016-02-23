﻿using Game;
using OpenTK;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Editor
{
    [DataContract]
    public class EditorEntity : EditorObject
    {
        [DataMember]
        public Entity Entity { get; private set; }

        public EditorEntity(EditorScene editorScene, Entity entity)
            : base(editorScene)
        {
            Debug.Assert(entity.Scene == Scene.Scene);
            Entity = entity;
            /*Entity = new Entity(Scene.Scene);
            Entity.Name = "Entity";*/
        }

        public override IDeepClone ShallowClone()
        {
            EditorEntity clone = new EditorEntity(Scene, Entity);
            ShallowClone(clone);
            return clone;
        }

        protected void ShallowClone(EditorEntity destination)
        {
            base.ShallowClone(destination);
        }

        public override List<IDeepClone> GetCloneableRefs()
        {
            List<IDeepClone> list = base.GetCloneableRefs();
            list.Add(Entity);
            return list;
        }

        public override void UpdateRefs(IReadOnlyDictionary<IDeepClone, IDeepClone> cloneMap)
        {
            base.UpdateRefs(cloneMap);
            Entity = (Entity)cloneMap[Entity];
        }

        public override void Remove()
        {
            base.Remove();
            Entity.Remove();
        }

        public override void SetTransform(Transform2 transform)
        {
            base.SetTransform(transform);
            Entity.SetTransform(transform);
        }
    }
}
