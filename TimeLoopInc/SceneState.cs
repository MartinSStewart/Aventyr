﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeLoopInc
{
    public class SceneState// : IDeepClone<SceneState>
    {
        public Dictionary<IGridEntity, IGridEntityInstant> Entities { get; private set; } = new Dictionary<IGridEntity, IGridEntityInstant>();
        public int Time;
        public List<Player> Players = new List<Player>();
        public List<Block> Blocks = new List<Block>();
        public Player CurrentPlayer;
        public int StartTime => Blocks.OfType<IGridEntity>().Concat(Players).Min(item => item.StartTime);

        public void SetTimeToStart()
        {
            Time = StartTime;
            Entities.Clear();
        }

        //public SceneState DeepClone()
        //{
        //    var clone = (SceneState)MemberwiseClone();

        //    clone.Entities = new Dictionary<IGridEntity, IGridEntityInstant>();
        //    foreach (var entity in Entities.Keys)
        //    {
        //        clone.Entities.Add(entity.DeepClone(), Entities[entity].DeepClone());
        //    }
        //    return clone;
        //}
    }
}