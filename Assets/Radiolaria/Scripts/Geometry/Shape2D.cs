namespace Geometry
{

    using UnityEngine;
    using System.Collections.Generic;
    //using System;
    using System.Linq;

    public class Shape2D
    {
        private readonly Lazy<List<Edge2D>> _lazyAllEdges;
        private readonly Lazy<List<Point2D>> _lazyAllPoints;
        private readonly Lazy<List<Edge2D>> _lazyExitedEdges;
        
        public Shape2D()
        {
            Faces = new List<Face2D>();

            _lazyAllEdges = new Lazy<List<Edge2D>>(GetAllEdges);
            _lazyAllPoints = new Lazy<List<Point2D>>(GetAllPoints);
            _lazyExitedEdges = new Lazy<List<Edge2D>>(GetAllExistedEdges);
        }

        public List<Face2D> Faces { get; set; }
        public List<Edge2D> AllEdges { get { return _lazyAllEdges.Value; } }
        public List<Point2D> AllPoints { get { return _lazyAllPoints.Value; } }
        public List<Edge2D> AllExistedEdges { get { return _lazyExitedEdges.Value; } }
        
        public Face2D AddFace(Face2D face)
        {
            if (Faces.Any(f => f.IsMatchFor(face)))
            {
                //throw new InvalidOperationException("There is already such a face in the shape!");
                Debug.Log("There is already such a face in the shape!");
                return face;
            }
            Faces.Add(face);
            face.Shape = this;
            _lazyExitedEdges.isValeChanged = true;
            _lazyAllEdges.isValeChanged = true;
            _lazyAllPoints.isValeChanged = true;
            return face;
        }
        
        public void RemoveFace(Face2D face)
        {
            Faces.Remove(face);

            _lazyExitedEdges.isValeChanged = true;
            _lazyAllEdges.isValeChanged = true;
            _lazyAllPoints.isValeChanged = true;
            //face.BasicEdges.ForEach(e => e.Faces.Remove(face));
            /*_lazyAllEdges.isValeChanged = true;
             _lazyAllPoints.isValeChanged = true;*/
        }

        public void Clear()
        {
            Faces.Clear();

            _lazyExitedEdges.isValeChanged = true;
            _lazyAllEdges.isValeChanged = true;
            _lazyAllPoints.isValeChanged = true;
        }

        private List<Edge2D> GetAllEdges()
        {
            return Faces.SelectMany(f => f.BasicEdges).Distinct().ToList();
        }

        private List<Point2D> GetAllPoints()
        {
            return Faces.SelectMany(f => f.ActualPoints).Distinct().ToList();
        }

        private List<Edge2D> GetAllExistedEdges()
        {
            List<Edge2D> result = Faces.SelectMany(f => f.AllParents).SelectMany(f => f.BasicEdges).Distinct().ToList();
            result.AddRange(AllEdges);
            return result;
        }

        public override string ToString()
        {
            return string.Format("Faces : {0}, Edges : {1}, Points : {2}", Faces.Count, GetAllEdges().Count, GetAllPoints().Count);
        }

        public Face2D AddPoints(params Point2D[] points)
        {
            //newEdges = new List<Edge2D>();
            int size = points.Length;
            if (size < 3)
                throw new System.Exception("The points count cannot less than 3.");
            

            List<Edge2D> edges = new List<Edge2D>();

            for (int i = 0, j = 1; i < size; i++, j = (i + 1) % size)
            {
                edges.Add(new Edge2D(points[i], points[j]));
            }

            bool[] isEdgeExits = new bool[size];

            List<Edge2D> allEdges = AllExistedEdges;

            foreach (var ss in allEdges)// allEdges)
            {
                for (int i = 0; i < size; i++)
                {
                    if (edges[i].IsMatchFor(ss))
                    {
                        Edge2D match = edges[i];
                        foreach(var p in match.Points)
                        {
                            p.Edges.Remove(match);
                        }

                        edges[i] = ss;
                        isEdgeExits[i] = true;

                    }
                }
            }

            for (int i = 0; i < size; i++)
            {
                if (!isEdgeExits[i])
                {
                    allEdges.Add(edges[i]);
                }
            }

            

            /*Face2D result = new Face2D(edges.ToArray());*/
            /*AddFace(new Face2D(edges.ToArray()));*/

            

            /*Debug.Log("Local variable allEdges count :" + allEdges.Count);
            Debug.Log("world variable GetAllEdges count :" + GetAllEdges().Count);*/

            return AddFace(new Face2D(edges.ToArray()));

        }

        public void OnDivide(Vector3 position)
        {
            foreach (var f in Faces)
            {
                if (f.IsOver(position))
                {
                    f.OnDivide();
                    //f.OnUnite();
                    break;
                }
            }
        }

        public void OnUnite(Vector3 position)
        {
            foreach (var f in Faces)
            {
                if (f.IsOver(position))
                {
                    f.OnUnite();
                    //f.OnUnite();
                    break;
                }
            }
        }
    }
}
