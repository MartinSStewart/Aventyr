﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Game;
using Game.Common;
using Game.Models;
using Game.Rendering;
using OpenTK;
using OpenTK.Graphics;

namespace Ui
{
    public class Button : IElement, IEnumerable<IElement>
    {
        public delegate void ClickHandler();
        public event ClickHandler OnClick;

        public Vector2 Size { get; set; }

        public Transform2 Transform { get; set; } = new Transform2();

        public ImmutableList<IElement> Children { get; set; } = new List<IElement>().ToImmutableList();

        public Button()
        {
        }

        public Button(out Button id, Transform2 transform, Vector2 size, Action onClick = null)
        {
            id = this;
            Transform = transform;
            Size = size;
            if (onClick != null)
            {
                OnClick += () => { onClick(); };
            }
        }

        public void Click()
        {
            OnClick?.Invoke();
        }

        public List<Model> GetModels()
        {
            return new[]
            {
                ModelFactory.CreatePlane(Size, Color4.Black),
            }.ToList();
        }

        public bool IsInside(Vector2 localPoint)
        {
            return MathEx.PointInRectangle(new Vector2(), Size, localPoint);
        }

        public IEnumerator<IElement> GetEnumerator() => Children.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        public void Add(IElement element)
        {
            Children = Children.Concat(new[] { element }).ToImmutableList();
        }
    }
}
