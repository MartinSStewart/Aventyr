﻿using Game;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using Game.Common;
using System.Collections.Immutable;

namespace TimeLoopInc
{
    public class Timeline<T> : ITimeline, IDeepClone<Timeline<T>> where T : IGridEntity, IDeepClone<IGridEntity>
    {
        public T this[int index] => (T)Path[index];

        public ImmutableList<IGridEntity> Path { get; private set; } = new List<IGridEntity>().ToImmutableList();

        public string Name => typeof(T).Name + " Timeline";

        public bool IsClosed { get; }

        public void Add(T entity)
        {
            Path = Path.Add(entity);    
        }

        public Timeline(bool isClosed = false)
        {
            IsClosed = IsClosed;
        }

        public Timeline(IList<T> path, bool isClosed = false)
            : this(isClosed)
        {
            Path = path.Cast<IGridEntity>().ToImmutableList();
        }

        public Timeline<T> DeepClone()
        {
            return new Timeline<T>
            {
                Path = Path.Select(item => item.DeepClone()).ToImmutableList()
            };
        }
    }

    public interface ITimeline
    {
        string Name { get; }
        ImmutableList<IGridEntity> Path { get; }
    }

    public static class ITimelineEx
    {
        public static int StartTime(this ITimeline timeline)
        {
            return timeline.Path.MinOrNull(item => item.StartTime) ?? 0;
        }

        public static int EndTime(this ITimeline timeline)
        {
            return timeline.StartTime() + 5;
        }
    }
}
