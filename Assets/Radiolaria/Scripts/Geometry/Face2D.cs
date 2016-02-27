namespace Geometry
{

    using UnityEngine;
    using System.Collections.Generic;
    using System.Text;
    using System.Linq;

    public class Face2D
    {
        private readonly Lazy<List<Point2D>> _lazyBasicPoints;
        private readonly Lazy<List<Point2D>> _lazyActualPoints;

        public string Name { set; get; }

        public Shape2D Shape { set; get; }
        public List<Edge2D> BasicEdges { get; set; }

        //Basic points can be always called from a lazy method.
        public List<Point2D> BasicPoints { get { return _lazyBasicPoints.Value; } }//return GetBasiclPointsInWindingOrder(); } }//

        public List<Point2D> ActualPoints { get { return GetActualPointsInWindingOrder(); } }// _lazyActualPoints.Value; } }

        public Face2D(params Edge2D[] edges)
        {
            BasicEdges = new List<Edge2D>(edges);
            BasicEdges.ForEach(e => e.AddFaces(this));
            _lazyBasicPoints = new Lazy<List<Point2D>>(GetBasiclPointsInWindingOrder);
            _lazyActualPoints = new Lazy<List<Point2D>>(GetActualPointsInWindingOrder);
        }

        private List<Point2D> GetBasiclPointsInWindingOrder()
        {
            //return Edges.SelectMany(e => e.Points).Distinct().ToList();
            List<Point2D> points = new List<Point2D>();
            // The edges were added in order, but we don't know if the points within the edges are in order. 
            // Therefore we look at the previous edge to figoure out which point has already been visited

            Edge2D previous = BasicEdges.First();
            
            foreach (var current in BasicEdges.Skip(1).Take(BasicEdges.Count - 2))
            {
                if (points.Count == 0)
                {
                    Point2D firstPoint = previous.PointOnlyInThis(current);
                    points.Add(firstPoint);

                    Point2D secondPoint = previous.PointInBoth(current);
                    points.Add(secondPoint);
                }

                Point2D nextPoint = current.PointOnlyInThis(previous);
                points.Add(nextPoint);

                previous = current;
            }

            return points;
        }

        /*Point2D FirstPoint { get { return BasicEdges.First().PointInBoth(BasicEdges.Last()); } }*/

        public List<Point2D> GetActualPointsInWindingOrder()
        {
            List<Point2D> points = new List<Point2D>();

            Edge2D first = BasicEdges.First();
            Edge2D last = BasicEdges.Last();
            Point2D firstPoint = first.PointInBoth(last);
            points.Add(firstPoint);
            foreach (var currect in BasicEdges)//.Skip(1).Take(BasicEdges.Count - 2))
            {
                currect.AddAllMiddlePoints(points);

                Point2D nextPoint = currect.PointOnlyInThis(last);
                if (nextPoint == firstPoint) break;
                points.Add(nextPoint);

                last = currect;

            }

            return points;
        }
        
        private List<Point2D> DivideBasicEdges()
        {
            List<Point2D> points = new List<Point2D>();

            Edge2D first = BasicEdges.First();
            Edge2D last = BasicEdges.Last();
            Point2D firstPoint = first.PointInBoth(last);
            points.Add(firstPoint);
            foreach (var currect in BasicEdges)//.Skip(1).Take(BasicEdges.Count - 2))
            {
                //currect.OnDivide();
                points.Add(currect.OnDivide());

                Point2D nextPoint = currect.PointOnlyInThis(last);
                if (nextPoint == firstPoint) break;
                points.Add(nextPoint);

                last = currect;

            }

            return points;
        }

        public List<Face2D> AllParents
        {
            get
            {
                

                List<Face2D> parents = new List<Face2D>();
                Face2D parent = Parents;
                while(parent != null)
                {
                    parents.Add(parent);
                    parent = parent.Parents;
                }
                
                return parents;
            }
        }

        public bool IsMatchFor(Face2D face)
        {
            return face.BasicEdges.All(e => BasicEdges.Contains(e));
        }

        public bool IsOver(Vector3 pt)
        {

            return Utils.WN_PnPoly(
                new Vector2(pt.x, pt.y),
                ActualPoints.Select(p => new Vector2(p.Position.x, p.Position.y)).ToArray()
                ) != 0;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder(Name);
            if (!IsLowest)
            {
                foreach (var child in Childern)
                {
                    sb.Append(child.ToString());
                }

            }
            return string.Format("{{{0}}}", sb.ToString());
        }

        #region Subdivision /*----------------------------------------------------------------*/

        public float Scale { set; get; }

        public void AddRigidbodyAndSpring(float scale)
        {
            Scale = scale;
            foreach (var p in ActualPoints)
                p.AddRigidbody();
            foreach (var e in BasicEdges)
                e.AddSpringJoint(Scale);
        }
        
        /*private Point2D centerPoint;*/
        public Point2D CenterPoint { set; get; }
        public Vector3 Center
        {
            get
            {
                List<Point2D> points = ActualPoints;
                Vector3 result = points[0].Position;
                foreach (var pp in points.Skip(1))
                {
                    result += pp.Position;
                }
                return result / points.Count;
            }
        }

        public List<Face2D> Childern { set; get; }

        public Face2D Parents { set; get; }

        public Face2D Root
        {
            get
            {
                if (IsRoot)
                    return this;
                return Parents.Root;
            }
        }

        public bool IsRoot
        {
            get { return Parents == null; }
        }

        public bool IsLowest
        {
            get { return Childern == null || Childern.Count == 0; }
        }

        public bool IsSecondLowest
        {
            get
            {
                return
                    !IsLowest &&
                    Childern[0].IsLowest && Childern[1].IsLowest && Childern[2].IsLowest;
            }
        }
        
        protected Face2D FindLastSecondLowestChild()
        {
            if (!IsRoot) return Root.FindLastSecondLowestChild();

            if (IsLowest) return null;

            //List<Face2D> allSecondLowestChild = new List<Face2D>();

            Face2D result = this;
            float minScale = Scale;

            foreach(var child in Childern)
            {
                if (child.IsLowest) continue;
                if (child.IsSecondLowest && child.Scale < minScale) { result = child;  minScale = child.Scale; }
                Face2D face = child.FindSecondLowestChild();
                if (face != null && face.Scale < minScale) { result = face; minScale = face.Scale; }
            }

            return result;

        }

        protected Face2D FindSecondLowestChild()
        {

            if(IsLowest) return null;

            foreach (var child in Childern)
            {
                if (child.IsLowest) continue;
                if(child.IsSecondLowest) return child;
                Face2D face = child.FindSecondLowestChild();
                if (face != null) return face;
            }
            return null;
        }


        
        public void OnDivide()
        {

            if (Scale < 0.5f)
                return;

            if (!IsLowest)
                throw new System.Exception("This face has divided!");
            
            CenterPoint = new Point2D(Center, "Center");
            CenterPoint.AddRigidbody();

            List<Point2D> division = DivideBasicEdges();
            
            /*Face2D f0 = Shape.AddPoints(division[0], division[1], division[2], division[3], CenterPoint, division[11]);
            Face2D f1 = Shape.AddPoints(CenterPoint, division[3], division[4], division[5], division[6], division[7]);
            Face2D f2 = Shape.AddPoints(division[10], division[11], CenterPoint, division[7], division[8], division[9]);*/

            /*Face2D f0 = Shape.AddPoints(division[11], division[0], division[1], division[2], division[3], CenterPoint);
            Face2D f1 = Shape.AddPoints(division[3], division[4], division[5], division[6], division[7], CenterPoint);
            Face2D f2 = Shape.AddPoints( division[7], division[8], division[9], division[10], division[11], CenterPoint);*/

            Face2D f0 = Shape.AddPoints(division[1], division[2], division[3], division[4], division[5], CenterPoint);
            Face2D f1 = Shape.AddPoints( division[5], division[6], division[7], division[8], division[9], CenterPoint);
            Face2D f2 = Shape.AddPoints( division[9], division[10], division[11], division[0], division[1], CenterPoint);

            f0.Name = Name + "-0";
            f1.Name = Name + "-1";
            f2.Name = Name + "-2";

            Shape.RemoveFace(this);

            DivideTo(f0, f1, f2);
            

        }

        void DivideTo(params Face2D[] faces)
        {
            if (faces.Length != 3)
                throw new System.Exception("The Sub face array must has three childern!");

            if (Childern == null)
                Childern = new List<Face2D>(faces);
            else
                Childern.Clear();

            Childern.AddRange(faces);

            Childern.ForEach(c => { c.Parents = this; c.AddRigidbodyAndSpring(Scale * 0.5f); });
        }

                
        void HandleOnUnite()
        {
            //List<Point2D> allPoint = ActualPoints;

            Edge2D[] centerEdges = CenterPoint.Edges.ToArray();

            //Debug.Log("Center Edges Count : " + centerEdges.Length);

            for(int i = 0; i<centerEdges.Length; i++)
            {
                centerEdges[i].DestorySelf();
            }

            CenterPoint.DestroySelf();
            CenterPoint = null;

            foreach (var e in BasicEdges)
            {
                e.DivisionCount--;
                if(e.DivisionCount == 0)
                {
                    Edge2D[] middleEdges = e.MiddlePoint.Edges.ToArray();
                    for(int i =0; i< middleEdges.Length; i++)
                    {
                        middleEdges[i].DestorySelf();
                    }
                    e.OnUnite();
                }
            }
            
            foreach (var child in Childern)
            {
                Shape.RemoveFace(child);
            }

            Shape.AddFace(this);

            Childern.Clear();
        }

        public void OnUnite()
        {

            /*if (IsLowest)
            {
                if (IsRoot)
                    return;

                Parents.OnUnite();
            }
            else if (IsSecondLowest)
            {
                HandleOnUnite();
            }
            else
            {
                Face2D secondLowest = FindSecondLowestChild();

                if (secondLowest != null)
                {
                    secondLowest.OnUnite();
                }
            }*/

            Face2D lastChild = FindLastSecondLowestChild();
            if (lastChild != null)
                lastChild.HandleOnUnite();
        }
        #endregion  /*----------------------------------------------------------------*/

        #region UnitTest

        public Face2D(string name)
        {
            Name = name;
        }


        
        public static void UnitTest()
        {
            Face2D face_0 = new Face2D("0");

            Face2D face_0_0 = new Face2D("0_0");
            Face2D face_0_1 = new Face2D("0_1");
            Face2D face_0_2 = new Face2D("0_2");

            face_0.DivideTo(face_0_0, face_0_1, face_0_2);

            Face2D face_0_0_0 = new Face2D("0_0_0");
            Face2D face_0_0_1 = new Face2D("0_0_1");
            Face2D face_0_0_2 = new Face2D("0_0_2");

            face_0_0.DivideTo(face_0_0_0, face_0_0_1, face_0_0_2);

            Face2D face_0_2_0 = new Face2D("0_2_0");
            Face2D face_0_2_1 = new Face2D("0_2_1");
            Face2D face_0_2_2 = new Face2D("0_2_2");

            face_0_2.DivideTo(face_0_2_0, face_0_2_1, face_0_2_2);

            Face2D face_0_2_0_0 = new Face2D("0_2_0_0");
            Face2D face_0_2_0_1 = new Face2D("0_2_0_1");
            Face2D face_0_2_0_2 = new Face2D("0_2_0_2");

            face_0_2_0.DivideTo(face_0_2_0_0, face_0_2_0_1, face_0_2_0_2);



            Debug.Log(face_0.ToString());

            face_0_1.OnUnite();
        }



        public string LogBasicEdges()
        {
            StringBuilder sb = new StringBuilder(Name + " BasicEdges : \n");
            int ei = 0;
            foreach (var e in BasicEdges)
            {
                List<Point2D> points = e.GetActualPoints();
                sb.Append(ei + " : ");
                foreach (var p in points)
                {
                    sb.Append(p.Name + ", ");
                }
                sb.Remove(sb.Length - 2, 2);
                sb.Append("\n");
                ei++;
            }

            return sb.ToString();
        }


        #endregion
    }
}
