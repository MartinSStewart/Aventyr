﻿using System.Collections.Generic;
using Game.Portals;
using OpenTK;
using OpenTK.Graphics;
using Game.Common;
using Game.Models;

namespace Game.Rendering
{
    public interface IRenderLayer
    {
        List<IRenderable> Renderables { get; }
        List<IPortalRenderable> Portals { get; }
        ICamera2 Camera { get; }
        bool RenderPortalViews { get; }
        bool DepthTest { get; }
    }

    public static class IRenderLayerEx
    {
        public static IRenderable DrawText(Font font, Vector2 position, string text, Vector2 alignment = new Vector2(), int lineSpacing = 0)
        {
            return new TextEntity(font, position, text, alignment, lineSpacing);
        }

        public static void DrawRectangle(this IRenderLayer layer, Vector2 topLeft, Vector2 bottomRight)
        {
            DrawRectangle(layer, topLeft, bottomRight, Color4.Black);
        }

        public static void DrawRectangle(this IRenderLayer layer, Vector2 topLeft, Vector2 bottomRight, Color4 color)
        {
            var plane = new Model(
                ModelFactory.CreatePlaneMesh(
                    Vector2.ComponentMin(topLeft, bottomRight), 
                    Vector2.ComponentMax(topLeft, bottomRight), 
                    color));
            if (color.A < 1)
            {
                plane.IsTransparent = true;
            }
            var renderable = GetRenderable(plane);
            layer.Renderables.Add(renderable);
        }

        public static IRenderable DrawLine(LineF line)
        {
            return DrawLine(line, Color4.Black);
        }

        public static IRenderable DrawLine(LineF line, Color4 color)
        {
            var plane = ModelFactory.CreateLines(new[] { line });
            if (color.A < 1)
            {
                plane.IsTransparent = true;
            }
            return GetRenderable(plane);
        }

        public static IRenderable DrawTriangle(Vector2 v0, Vector2 v1, Vector2 v2)
        {
            return DrawTriangle(v0, v1, v2, Color4.Black);
        }

        public static IRenderable DrawTriangle(Vector2 v0, Vector2 v1, Vector2 v2, Color4 color)
        {
            var vArray = MathEx.SetWinding(new[] { v0, v1, v2 }, false);
            var triangle = ModelFactory.CreateTriangle(
                new Vector3(vArray[0]),  
                new Vector3(vArray[1]), 
                new Vector3(vArray[2]), 
                color);
			if (color.A < 1)
			{
				triangle.IsTransparent = true;
			}
            return GetRenderable(triangle);
        }

        static Renderable GetRenderable(Model model)
        {
			var renderable = new Renderable();
			renderable.IsPortalable = false;
			renderable.Models.Add(model);
            return renderable;
        }
    }
}
