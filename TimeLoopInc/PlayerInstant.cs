﻿using Game.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeLoopInc
{
    public class PlayerInstant : IGridEntityInstant
    {
        public Transform2i Transform { get; set; }
        public Vector2i PreviousVelocity { get; set; }

        public PlayerInstant(Transform2i transform)
        {
            Transform = transform;
        }

        public IGridEntityInstant DeepClone() => (PlayerInstant)MemberwiseClone();
    }
}
