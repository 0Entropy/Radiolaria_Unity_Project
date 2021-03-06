﻿
namespace TriangleNet
{
    using UnityEngine;
    using System.Collections.Generic;
    using TriangleNet.Geometry;
    using System;

    public static class Extension
    {

        /// <summary>
        /// Add a polygon ring to the geometry.
        /// </summary>
        /// <param name="points">List of points which make up the polygon.</param>
        /// <param name="mark">Common boundary mark for all segments of the polygon.</param>
        public static void AddPoly(this InputGeometry geometry,
                IEnumerable<Point> points, int mark = 0)
        {
            // Save the current number of points.
            int N = geometry.Count;

            int m = 0;
            foreach (var pt in points)
            {
                geometry.AddPoint(pt.X, pt.Y, pt.Boundary, pt.Attributes);
                m++;
            }

            for (int i = 0; i < m; i++)
            {
                geometry.AddSegment(N + i, N + ((i + 1) % m), mark);
            }
        }

        /// <summary>
        /// Add a polygon ring to the geometry and make it a hole.
        /// </summary>
        /// <remarks>
        /// WARNING: This works for convex polygons, but not for non-convex regions in general.
        /// </remarks>
        /// <param name="points">List of points which make up the hole.</param>
        /// <param name="mark">Common boundary mark for all segments of the hole.</param>
        public static void AddPolyAsHole(this InputGeometry geometry,
                IEnumerable<Point> points, int mark = 0)
        {
            // Save the current number of points.
            int N = geometry.Count;

            // Hole coordinates
            double x = 0.0;
            double y = 0.0;

            int m = 0;
            foreach (var pt in points)
            {
                x += pt.X;
                y += pt.Y;

                geometry.AddPoint(pt.X, pt.Y, pt.Boundary, pt.Attributes);
                m++;
            }

            for (int i = 0; i < m; i++)
            {
                geometry.AddSegment(N + i, N + ((i + 1) % m), mark);
            }

            geometry.AddHole(x / m, y / m);
        }


    }
}
