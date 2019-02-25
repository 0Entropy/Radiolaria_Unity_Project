namespace Geometry
{

    using UnityEngine;
    using System;
    using System.Collections.Generic;

    public class Edge2D
    {

        public List<Point2D> Points { get; set; }
        public List<Face2D> Faces { get; set; }
        public Vector3 Middle { get { return (Points[0].Position + Points[1].Position) * 0.5f; } }

        public Edge2D(Point2D p1, Point2D p2)
        {
            if (p1 == p2)
            {
                throw new InvalidOperationException("Edge must be between two different points!");
            }
            //ConnectJoint(p1, p2);
            Points = new List<Point2D> { p1, p2 };

            Faces = new List<Face2D>();
            p1.Edges.Add(this);
            p2.Edges.Add(this);
        }

        public void AddFaces(Face2D face)
        {
            if (Faces.Contains(face))
            {
                throw new InvalidOperationException("Edge already contains face!");
            }
            Faces.Add(face);
        }

        public bool IsMatchFor(Point2D p1, Point2D p2)
        {
            Point2D j1 = Points[0];
            Point2D j2 = Points[1];
            return
                (j1 == p1 && j2 == p2) || (j1 == p2 && j2 == p1);
        }

        public bool IsMatchFor(Edge2D edge)
        {
            return IsMatchFor(edge.Points[0], edge.Points[1]);
        }

        public Point2D PointInBoth(Edge2D other)
        {
            Point2D p1 = Points[0];
            if (other.Points.Contains(p1))
            {
                return p1;
            }
            else
            {
                return Points[1];
            }
        }

        public Point2D PointOnlyInThis(Edge2D other)
        {
            Point2D p1 = Points[0];
            if (!other.Points.Contains(p1))
            {
                return p1;
            }
            else
            {
                return Points[1];
            }
        }

        public void DestorySelf()
        {
            foreach (var p in Points)
            {
                p.Edges.Remove(this);
            }
            //UnityEngine.Object.Destroy(Spring);
        }

        public string Name { get { return string.Format("[{0},{1}]", Points[0].Name, Points[1].Name); } }

        public override string ToString()
        {
            return string.Format("[{0},{1}]", Points[0].Name, Points[1].Name);
        }

        #region Subdivision

        public SpringJoint Spring { set; get; }

        public float SpringLenght { set; get; }

        /*private Point2D middlePoint;*/
        public Point2D MiddlePoint
        {
            set; get;
        }
        
        public int DivisionCount { set; get; }

        public Point2D OnDivide()
        {
            
            if (MiddlePoint == null)
            {
                MiddlePoint = new Point2D(Middle, string.Format("[{0}+{1}]", Points[0].Name, Points[1].Name));
                MiddlePoint.AddRigidbody();
                
            }
            DivisionCount++;
            return MiddlePoint;
            
        }

        public void OnUnite()
        {
            if (MiddlePoint == null)
            {
                return;
            }

            MiddlePoint.DestroySelf();
            MiddlePoint = null;
            DivisionCount = 0;

        }

        public void AddSpringJoint(float lenght)
        {
            AddSpringJoint(Points[0], Points[1], lenght);
        }

        void AddSpringJoint(Point2D p1, Point2D p2, float lenght)
        {
            if (Spring)
                return;

            if (!p1.UnityObject || !p1.UnityObject.GetComponent<Rigidbody>() || !p2.UnityObject || !p2.UnityObject.GetComponent<Rigidbody>())
                return;

            SpringLenght = lenght;
            Spring = p1.UnityObject.AddComponent<SpringJoint>();
            Spring.connectedBody = p2.UnityObject.GetComponent<Rigidbody>();
            Spring.minDistance = lenght * .05f;
            Spring.maxDistance = lenght * .5f;
            Spring.spring = 32.0f;
            Spring.damper = 32.0f;
            Spring.autoConfigureConnectedAnchor = false;
            Spring.enableCollision = true;
            Spring.connectedAnchor = Spring.anchor = Vector3.zero;
            Spring.axis = Vector3.back;
            /*sJoint.
            p1.JointTransform.GetComponent<SpringJoint>().connectedBody = p2.JointTransform.rigidbody;*/
        }

        public void AddAllMiddlePoints(List<Point2D> allPoints)
        {
            if (MiddlePoint == null)
            {
                //Debug.Log(string.Format("Edge {{{0}, {1}}} Do not have a middle point", Points[0].Name, Points[1].Name));
                return;
            }

            if (allPoints.Count < 1)
                throw new System.Exception("The Middle Points List must has a point!");

            Point2D firstPoint = allPoints[allPoints.Count - 1];
            if (!Points.Contains(firstPoint))
            {
                //Debug.Log(string.Format("Edge {{{0}, {1}}} Do not contains allPoints 's last point", Points[0].Name, Points[1].Name));
                return;
            }
            Point2D lastPoint = Points[0] == firstPoint ? Points[1] : Points[0];
            Edge2D firstEdge = MiddlePoint.Edges.Find(e => e.Points.Contains(firstPoint));
            if (firstEdge != null)
                firstEdge.AddAllMiddlePoints(allPoints);
            allPoints.Add(MiddlePoint);
            Edge2D lastEdge = MiddlePoint.Edges.Find(e => e.Points.Contains(lastPoint));
            if (lastEdge != null)
                lastEdge.AddAllMiddlePoints(allPoints);


        }

        public List<Point2D> GetActualPoints()
        {
            List<Point2D> result = new List<Point2D>();

            result.Add(Points[0]);

            AddAllMiddlePoints(result);

            result.Add(Points[1]);

            return result;
        }
        
        public List<Point2D> AllPoints
        {
            get
            {
                List<Point2D> result = new List<Point2D>();
                result.Add(Points[0]);
                AddAllMiddlePoints(result);
                result.Add(Points[1]);
                return result;
            }
        }

        #endregion

    }
}
