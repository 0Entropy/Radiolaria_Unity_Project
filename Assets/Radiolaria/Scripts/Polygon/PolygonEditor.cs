using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PolygonEditor : MonoBehaviour {

	private Polygon _polygon;
	private List<Vector2> _vertices;

	public Polygon polygon {
		set {
			_polygon = value;
			_vertices = value.vertices;
		}
	}
	
	public _Vertex vertexPrefab;
	public _Block edgePrefab;

	public List<_Vertex> points = new List<_Vertex>();
	public List<_Block> blocks = new List<_Block>();
	
	//	public int _count = 0;
	
	//vertix and edge is in Polygon Layer...
	private const int SHAPE2D_LAYER = 8;
	private float fireRate = 0.5F;
	private float nextFire;
	private Vector3 oldInputPos;
	
	
//	private Transform SelectedChild = null;
	public Transform SelectedChild{ set; get; }
	
	#region Transfer,Insert and Remove _Point
	/// <summary>
	/// Directions the of 序列号index处NextPoint至此_Point的方向. 
	/// </summary>
	/// <returns>The 方向矢量 of.</returns>
	/// <param name="index">Index.</param>
	
	
	public int PreIndex(int index){
		
		return _polygon.PreIndex(index);
	}
	
	public int NextIndex(int index){
		
		return _polygon.NextIndex(index);
	}
	
	public _Vertex NextPoint(int index) {
		return points[NextIndex(index)];
	}
	
	public _Vertex PrePoint(int index) {
		return points[PreIndex(index)];
	}
	
	public _Vertex NextPoint(_Vertex point) {
		int index = points.IndexOf(point);
		return NextPoint (index);
	}
	
	public void TransferPoint (Transform point, Vector3 position) {

		point.position = position;
		Vector2 localPosition = point.localPosition;

		_Vertex _point = point.GetComponent<_Vertex>();
		
		int index = points.IndexOf(_point);
		_vertices[index] = localPosition;
		
		blocks[index].SetEndsPosition(_vertices[PreIndex(index)], localPosition);
		blocks[NextIndex(index)].SetEndsPosition(localPosition, _vertices[NextIndex(index)]);

	}
	
	public GameObject InsertPointAt(Transform edge, Vector3 position) {

		_Block originBlock = edge.GetComponent<_Block>();
		int index = blocks.IndexOf(originBlock);

		_Vertex point = vertexPrefab.Spawn();
		points.Insert(index, point);
		point.transform.parent = transform;
		point.transform.localRotation = Quaternion.identity;
		point.transform.position = position;//ition;

		Vector2 localPosition = point.transform.localPosition;

		originBlock.SetEndsPosition(localPosition, _vertices[index]);

		_Block block = edgePrefab.Spawn();
		blocks.Insert(index, block);
		block.transform.parent = transform;	
		block.SetEndsPosition(_vertices[PreIndex(index)], localPosition);

		
		_vertices.Insert(index, localPosition);

		return point.gameObject;

	}

	public GameObject InsertPointAt(Transform edge) {

		return InsertPointAt(edge, edge.position);
//		_Block originBlock = edge.GetComponent<_Block>();
//		int index = blocks.IndexOf(originBlock);
//		
//		_Point point = vertexPrefab.Spawn();
//		points.Insert(index, point);
//		point.transform.parent = transform;
//		point.transform.localRotation = Quaternion.identity;
//		point.transform.position = position;//ition;
//		
//		Vector2 localPosition = point.transform.localPosition;
//		
//		originBlock.SetEndsPosition(localPosition, vertices[index]);
//		
//		_Block block = edgePrefab.Spawn();
//		blocks.Insert(index, block);
//		block.transform.parent = transform;	
//		block.SetEndsPosition(vertices[PreIndex(index)], localPosition);
//		
//		
//		vertices.Insert(index, localPosition);
//		
//		return point.gameObject;
		
	}
	
	public void RemovePointAt(Transform point) {
					
		_Vertex originPoint = point.GetComponent<_Vertex>();
		int index = points.IndexOf(originPoint);
		_Block originBlock = blocks[index];
		
		blocks[NextIndex (index)].SetEndsPosition( _vertices[PreIndex (index)], _vertices[NextIndex (index)]);
		
		points.Remove (originPoint);
		blocks.Remove (originBlock);
		_vertices.RemoveAt (index);
		
		originBlock.Recycle();
		originPoint.Recycle();

		if(_vertices.Count < 1){
			RecyclePoints();
		}
	}

//	public void OnReverse(){
//		vertices.Reverse();
//		points.Reverse();
//		blocks.Reverse();
//		for(int i=0; i<vertices.Count; i++){
//			blocks[i].SetEndsPosition(vertices[PreIndex(i)], vertices[i]);
//			blocks[NextIndex(i)].SetEndsPosition(vertices[i], vertices[NextIndex(i)]);
//		}
//	}
	
	#endregion

	public void RecyclePoints(){
		foreach(_Vertex point in points){
			point.Recycle();
		}
		points.Clear();
		foreach(_Block block in blocks){
			block.Recycle();
		}
		blocks.Clear();
		SelectedChild = null;

		_vertices.Clear();
		_polygon = null;

		this.Recycle();
	}

	public void SpawnPoints(){

		for(int i=0; i<_vertices.Count; i++) {			
			SpawnPoint(i);
		}
	}
	
	private void SpawnPoint( int index ) {
		
		Vector2 position = _vertices[index];
		
//		GameObject pointObj = Instantiate( vertexPrefab) as GameObject;
//		pointObj.transform.parent = transform;
//		pointObj.transform.localRotation = Quaternion.identity;
//		pointObj.transform.localPosition = position;
//		
//		_Point point = pointObj.GetComponent<_Point>();
//		points.Add (point);

		_Vertex point = vertexPrefab.Spawn(transform.position, transform.rotation);
		points.Add(point);

		point.transform.parent = transform;
		point.transform.localRotation = Quaternion.identity;
		point.transform.localPosition = position;
		
//		GameObject blockObj = Instantiate( edgePrefab) as GameObject;
//		blockObj.transform.parent = transform;
//		
//		_Block block = blockObj.GetComponent<_Block>();
//		blocks.Add (block);
		_Block block = edgePrefab.Spawn(transform.position, transform.rotation);
		blocks.Add(block);

		block.transform.parent = transform;
		
		block.SetEndsPosition(_vertices[PreIndex(index)], position);
		
	}

	void Awak () {

	}

	// Use this for initialization
	void Start () {
		vertexPrefab.CreatePool();
		edgePrefab.CreatePool();

        Polygon poly = new Polygon();
        poly.SetVertices(2);

        polygon = poly;
        SpawnPoints();

        _InputManager.Instance.Init();
	}
	
	#region Edit

	
	// Update is called once per frame
	void Update () {

	}

	void OnEnable() {

		//_InputManager.OnFocus += OnLongTap;

        _InputManager.OnDrag += OnDrag;
        _InputManager.OnDragEnd += OnDrop;
	}
	
	void OnDisable() {

        //_InputManager.OnFocus -= OnLongTap;

        _InputManager.OnDrag -= OnDrag;
        _InputManager.OnDragEnd -= OnDrop;
	}

	void OnDrop()
	{ 
		
	}

//	public bool OnRelase(GameObject g){
//		if(g != null && g.transform.parent == transform && g.tag == "Vertex"){
//			RemovePointAt(g.transform);
//			return true;
//		}
//		return false;
//	}

	void OnLongTap(GameObject g){
		if(g != null && g.transform.parent == transform){
//			GameObject g = e.gameObject;
			//			Debug.LogError(":PolygonEditor.OnDrag ()");
			/*if(g.tag == "Vertex"){
				RemovePointAt(g.transform);//ScreenPointToLocal(position));
//				InputManager.Instance.FocusSelf();
				return;
			}*/
			if(g.tag == "Edge"){
				//			Debug.LogError(":PolygonEditor.OnDrag ()");
				InsertPointAt(g.transform);
				return;
			}
		}
	}

	void OnDrag(GameObject g, Vector2 pos){

        Debug.Log("Polygon Editor OnDrag.");
		if(g.transform.parent == transform){
//			GameObject g = e.gameObject;
//			Debug.LogError(":PolygonEditor.OnDrag ()");
			if(g.tag == "Vertex"){
				TransferPoint(g.transform, pos);//ScreenPointToLocal(position));
			}
//			if(g.tag == "Edge" && e.isTimeout && e.isStatic){
//				//			Debug.LogError(":PolygonEditor.OnDrag ()");
//				InputManager.Instance.focusedObject = InsertPointAt(g.transform, e.localPosition);
//				return  true;
//			}
		}
	}

	#endregion
}
