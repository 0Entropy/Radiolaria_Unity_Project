namespace Geometry
{

    using UnityEngine;
    using System.Collections.Generic;
    using System.Linq;

    public class Point2D : System.IEquatable<Point2D>
    {
        public string Name { set; get; }
        
        public bool IsOnBoundary { set; get; }

        //public Rigidbody Rigidbody { set; get; }

        public GameObject UnityObject { set; get; }

        private Vector3 _position = Vector3.zero;
        public Vector3 Position { get { return UnityObject ? UnityObject.transform.position : _position; } }
        public Vector3 LocalPosition { get { return UnityObject ? UnityObject.transform.localPosition : _position; } }

        private Point2D() : this(Vector3.zero, "NoName") { }

        public Point2D(Vector3 position) : this(position, "NoName") { }

        public Point2D(Vector3 position, string name)
        {
            _position = position;
            Name = name;

            Edges = new List<Edge2D>();
            _lazyAllFaces = new Lazy<List<Face2D>>(GetAllFaces);
        }

        public void AddGameObject(GameObject obj, float Scale = 1.0f, bool IsRigidbody = true)
        {
            obj.name = Name;
            obj.transform.position = _position;
            obj.transform.localScale = Vector3.one * Scale;

            UnityObject = obj;

            if (IsRigidbody)
            {

                Rigidbody rigidbody = obj.GetComponent<Rigidbody>();
                if (!rigidbody)
                    rigidbody = obj.AddComponent<Rigidbody>();

                //Rigidbody = rigidbody;
                rigidbody.drag = 16.0f;
                rigidbody.angularDrag = 16.0f;
                if (rigidbody.useGravity) rigidbody.useGravity = false;
                if (IsOnBoundary) rigidbody.isKinematic = true;
            }

        }

        public void AddRigidbody()
        {
            if (UnityObject && UnityObject.GetComponent<Rigidbody>())
                return;

            AddGameObject(new GameObject(), 0.1f, true);//GameObject.CreatePrimitive(PrimitiveType.Sphere);//
            
        }
        
        public List<Edge2D> Edges { set; get; }

        private readonly Lazy<List<Face2D>> _lazyAllFaces;
        public List<Face2D> AllFaces { get { return _lazyAllFaces.Value; } }

        private List<Face2D> GetAllFaces()
        {
            return Edges.SelectMany(e => e.Faces).Distinct().ToList();
        }

        public void DestroySelf()
        {
            if ( Edges.Count > 0)
            {
                throw new System.Exception("There is some edge still needs this point.");
            }

            UnityEngine.Object.Destroy(UnityObject);
            //Edges.Clear();
        }

        public override string ToString()
        {
            return Name;
        }

        #region Equatable

        public static bool operator ==(Point2D a, Point2D b)
        {
            if (System.Object.ReferenceEquals(a, b))
            {
                return true;
            }

            if (((object)a == null) || ((object)b == null))
            {
                return false;
            }

            return a.Equals(b);
        }

        public static bool operator !=(Point2D a, Point2D b)
        {
            return !(a == b);
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            Point2D other = obj as Point2D;
            if (other == null) return false;
            else return Equals(other);

        }

        public bool Equals(Point2D other)
        {

            if (other == null || this == null)
                return false;

            bool isEquals = !((UnityObject) ^ (other.UnityObject));

            if (!isEquals)
                return false;
            
            if(UnityObject && other.UnityObject)
            {
                return UnityObject == other.UnityObject;
            }

            return Position == other.Position;
            
        }

        public override int GetHashCode()
        {
            if (UnityObject)
                return Position.GetHashCode() ^ UnityObject.GetHashCode();
            return Position.GetHashCode() ^ Name.GetHashCode();
        }
        #endregion

        public static void UnitTest()
        {
            Point2D p1 = new Point2D();
            Point2D p2 = new Point2D();

            Debug.Log("p1 == p2 : " + (p1 == p2));

            p1.AddRigidbody();
            p2.AddRigidbody();

            Debug.Log("p1 == p2 : " + (p1 == p2));
        }
    }
}
