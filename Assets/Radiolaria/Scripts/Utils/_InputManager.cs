using UnityEngine;
using System.Collections;
using System.Text;

public class _InputManager : _Singleton<_InputManager>
{
    public delegate void OnClickEvent(Vector3 position);
    public static event OnClickEvent OnClick;
    /// <summary>
    /// From Mouse Down to Mouse Up , Mouse is always over same object.
    /// </summary>
    /// <param name="obj"></param>
	public delegate void OnFocusEvent(GameObject obj, Vector2 pos);
	public static event OnFocusEvent OnFocus;

	public delegate void OnReleaseEvent();
	public static event OnReleaseEvent OnRelease;
	//"KeyCode.Home" 
	public delegate void OnExitEvent();
	public static event OnExitEvent OnExit;

	//"KeyCode.Escape" or Back Key
	public delegate void OnEscapeEvent();
	public static event OnEscapeEvent OnEscape;

	////鼠标左键/手势（单点）仅按下;
//	public delegate void OnFlashTapEvent(GameObject g);
//	public static event OnFlashTapEvent OnFlashTap;
//	//鼠标左键/手势（单点）点击;
//	public delegate bool OnShortTapEvent(GameObject g);
//	public event OnShortTapEvent OnShortTap;
//	//鼠标左键/手势（单点）長按;
//	public delegate bool OnLongTapEvent(GameObject g);
//	public event OnLongTapEvent OnLongTap;

	///鼠标左键/手势（按下无论是否有拖动对象）的移动，或（无论是否有拖动对象）的移动
	//开始；
	public delegate void OnMoveStartEvent(Vector2 pos);
	public static event OnMoveStartEvent OnMoveStart;
	//鼠标左键/手势（单点）拖拽;
	public delegate void OnMoveEvent(Vector2 pos);
	public static event OnMoveEvent OnMove;
	//鼠标左键/手势（单点）释放;
	public delegate void OnMoveEndEvent();
	public static event OnMoveEndEvent OnMoveEnd;

//	///鼠标左键/手势（按下无论是否有拖动对象）的移动，或（无论是否有拖动对象）的移动
//	//开始；
//	public delegate void OnMultiMoveStartEvent(Vector2 pos_0, Vector2 pos_1);
//	public static event OnMultiMoveStartEvent OnMultiMoveStart;
//	//鼠标左键/手势（单点）拖拽;
//	public delegate void OnMultiMoveEvent(float factor, float delta);
//	public static event OnMultiMoveEvent OnMultiMove;
//	//鼠标左键/手势（单点）释放;
//	public delegate void OnMultiMoveEndEvent();
//	public static event OnMultiMoveEndEvent OnMultiMoveEnd;

	public delegate void OnDragStartEvent(GameObject obj);
	public static event OnDragStartEvent OnDragStart;
//	//鼠标左键/手势（单点）拖拽;
	public delegate void OnDragEvent(GameObject obj, Vector2 position);
	public static event OnDragEvent OnDrag;
	//鼠标左键/手势（单点）释放;
	public delegate void OnDragEndEvent();
	public static event OnDragEndEvent OnDragEnd;
//	public delegate bool OnDropEvent(GameObject g);
//	public event OnDropEvent OnDrop;

//	鼠标左键+shift/手势（双点）开始;
//	public delegate void OnMultiStartEvent();
//	public static event OnMultiStartEvent OnMultiStart;

	//鼠标左键+shift/手势（双点）拖拽 （在此场景中设定为缩放）;
	public delegate void OnScaleEvent(float delta);
	public static event OnScaleEvent OnScale;

	//鼠标左键+shift/手势（双点）释放;
//	public delegate void OnMultiEndEvent();
//	public static event OnMultiEndEvent OnMultiEnd;
	
	public Camera RaycastCamera;

	public Vector3 mousePosition;
	public Vector3 clickPosition;
	public GameObject SelectedObject;// {private set; get;}

	//public Vector3 hitPoint{private set; get;}

	int selectable_layer_mask;//, ui_layer_mask;
	
	//private Rect checkAreaRect = new Rect();
	//for 240 dpi display, a touch almost is 3/8 inch, so it is about 90 Pixels dimension...
	//private const float checkAreaWidth = 90F;
	//private const float checkAreaHeight = 90F;
	//private const float tickTime = 0.01F;

	public bool isStationary = false;
	
	private bool isDragStart = false;

	private bool isMultiTouchStart = false;

//	private float startDistance;

	private Vector2 screenCenter;
//	StringBuilder textBuilder = new StringBuilder();

	/// <summary>
	/// Gets or sets the input_ y_ area_ factor.
	/// Action: the screen dimension is left bottom to right top!
	/// x : bottom value;
	/// y : upper value;
	/// </summary>
	/// <value>The input_ y_ area_ factor.</value>
	public Vector2 Input_Y_Area_Factor{set; get;}

	bool IsOut{
		get{
			return Input.mousePosition.y > Screen.height * Input_Y_Area_Factor.y ||
				Input.mousePosition.y < 128;
		}
//		return Input.Get
	}

//	public bool IsTouchEnable{set; get;}

	public override void Init()
	{
        //		IsTouchEnable = true;
        Input_Y_Area_Factor =  new Vector2(0.0f, 1.0f);//Vector2.zero;//
        screenCenter = new Vector2(Screen.width * 0.5F, - Screen.height* 0.5F);
		selectable_layer_mask = (1 << LayerMask.NameToLayer ("Selectable"));// | (1 << LayerMask.NameToLayer ("Skeleton Layer"));
		//TODO
	}



	public override void Release()
	{
		//TODO
	}

//	void Start() {
//		screenCenter = new Vector2(Screen.width * 0.5F, - Screen.height* 0.5F);
//		selectable_layer_mask = (1 << LayerMask.NameToLayer ("Selectable Layer")) | (1 << LayerMask.NameToLayer ("Skeleton Layer"));
//	}

	void Update ()
	{

	#if(((UNITY_ANDROID||UNITY_IPHONE)&&!UNITY_EDITOR))
		UndateTouch();
	#else
		//UpdateMouse();
	#endif

		if (Input.GetKeyDown (KeyCode.Escape)) {
			if(OnEscape != null)
				OnEscape();
		}

		if (Input.GetKeyDown (KeyCode.Home)) {
			if(OnExit != null)
				OnExit();
		}
	}

	void UndateTouch()
	{
//		if(!IsTouchEnable)
//			return;
		int count = Input.touchCount;
		if ( count == 1 ) {
			singleTouch();
		}else if( count >= 2 ) {
			multiTouch();
		}else {
			if(OnRelease != null)
				OnRelease();
			SelectedObject = null;
		}
	}

	GameObject DownObj, UpObj;
	public void ClearFocusObjects(){
		DownObj = null;
		UpObj = null;
	}

	void singleTouch ()
	{
		if(IsOut)
			return;

		if (isMultiTouchStart) {
			isMultiTouchStart = false;
		}

        Vector2 worldPosition = RaycastCamera.ScreenToWorldPoint(Input.GetTouch(0).position);

        if (Input.GetTouch (0).phase == TouchPhase.Began){
            //			OnRaycast(Input.GetTouch(0).position);
            SelectedObject = DownObj = OnTouch(Input.GetTouch(0).position);
			UpObj = null;

            if (OnClick != null && DownObj == null && UpObj == null && SelectedObject == null)
            {

                OnClick(worldPosition);
                return;
            }
        }

		if (Input.GetTouch (0).phase == TouchPhase.Stationary){
			/*if(!isDragStart && SelectedObject && OnDragStart != null){
				isDragStart = true;
				OnDragStart(SelectedObject);
			}*/
		}
		else if (Input.GetTouch (0).phase == TouchPhase.Moved) {
			//Vector2 delta = Input.GetTouch(0).deltaPosition;
            //Vector2 position = RaycastCamera.ScreenToWorldPoint(Input.GetTouch(0).position);
			if(OnMove != null){
                OnMove(worldPosition);// delta);
			}
			if(!isDragStart && SelectedObject && OnDragStart != null){
				isDragStart = true;
				OnDragStart(SelectedObject);
			}
			else{
                if (OnDrag != null)
                    OnDrag(SelectedObject,  worldPosition);//delta);//
            }
		}
		else if(Input.GetTouch (0).phase == TouchPhase.Ended || Input.GetTouch (0).phase == TouchPhase.Canceled) {
			if(OnRelease != null)
				OnRelease();

			if(isDragStart && SelectedObject && OnDragEnd != null){
				OnDragEnd();
				isDragStart = false;
			}

			UpObj = OnTouch(Input.GetTouch(0).position);
            
			if(OnFocus != null && DownObj != null && UpObj != null && DownObj == UpObj){
                
                OnFocus(UpObj, worldPosition);
			}

			SelectedObject = null;
		}
	}


	private void multiTouch() {
		if (isDragStart) {
			isDragStart = false;
		}
		if ((Input.GetTouch (0).phase == TouchPhase.Began || Input.GetTouch (0).phase == TouchPhase.Stationary) 
		    && (Input.GetTouch (1).phase == TouchPhase.Began || Input.GetTouch (1).phase == TouchPhase.Stationary)) {
			if (!isMultiTouchStart){
			}
		} else if (Input.GetTouch (0).phase == TouchPhase.Moved || Input.GetTouch (1).phase == TouchPhase.Moved) {
			if (!isMultiTouchStart) {
				isMultiTouchStart = true;
//				startDistance = Vector2.Distance (Input.GetTouch (0).position, Input.GetTouch (1).position);
//				if(OnMultiMoveStart != null)
//					OnMultiMoveStart(Input.GetTouch(0).position, Input.GetTouch(1).position);
			} else {
//				float precent = startDistance / Vector2.Distance (Input.GetTouch (0).position, Input.GetTouch (1).position);
				Vector2 delta = Input.GetTouch (1).deltaPosition - Input.GetTouch (0).deltaPosition;
				if(delta.sqrMagnitude > 0.01F)//
				{
					Vector2 originDir = Input.GetTouch (1).position - Input.GetTouch (0).position;

					float dir = Vector2.Dot (originDir, delta);
					if(OnScale != null)
						OnScale (dir);
				}
//				if(OnMultiMove != null)
//					OnMultiMove(precent, (Input.GetTouch(0).position - Input.GetTouch(1).position).magnitude);

			}
		} else if (Input.GetTouch (0).phase == TouchPhase.Ended || Input.GetTouch (0).phase == TouchPhase.Canceled 
		           || Input.GetTouch (1).phase == TouchPhase.Ended || Input.GetTouch (1).phase == TouchPhase.Canceled) {
				//
			if(isMultiTouchStart){
//				if(OnMultiMoveEnd != null)
//					OnMultiMoveEnd();
				isMultiTouchStart = false;
			}
		}

	}

	void UpdateMouse(){
        
		if(IsOut)
			return;

        Vector3 worldPosition = RaycastCamera.ScreenToWorldPoint(Input.mousePosition);

        if (Input.GetMouseButtonDown(0))
		{

            SelectedObject = DownObj = OnTouch(Input.mousePosition);
			UpObj = null;

            if (OnClick != null && DownObj == null && UpObj == null && SelectedObject == null)
            {

                OnClick(worldPosition);
                return;
            }


            if (!isStationary)
			{
				isStationary = true;
			}
			
			clickPosition = Input.mousePosition;
			mousePosition = Input.mousePosition;
            
			if(Input.GetKey(KeyCode.LeftAlt)){
				isMultiTouchStart = true;
			}
			
		}//End of --- if (Input.GetMouseButtonDown(0))
		
		if (Input.GetMouseButton(0))// && currectObject != null)
		{
			mousePosition = Input.mousePosition;			
			if(isStationary)
			{
				if(mousePosition != clickPosition)//!checkAreaRect.Contains(mousePosition))
				{
					isStationary = false;
					if(SelectedObject != null && OnDragStart != null){
						isDragStart = true;
						OnDragStart(SelectedObject);
					}
				}
			}else{
				if(Input.GetKey(KeyCode.LeftAlt) && isMultiTouchStart){
					float x = Input.mousePosition.x;
					float y = Input.mousePosition.y;
//					float currectDistance = Vector2.Distance(new Vector2(x,y), new Vector2(Screen.width - x, Screen.height - y));
					Vector2 originDir = (Vector2)mousePosition - screenCenter;
					Vector2 delta = mousePosition - clickPosition;
					float dir = Vector2.Dot (originDir, delta);
					if(OnScale != null)
						OnScale(dir);
//					if(OnMultiMove != null){
//						OnMultiMove(currectDistance / startDistance,(mousePosition - clickPosition).magnitude);
//					}
				}else{
					if(OnMove != null){
						OnMove(mousePosition - clickPosition);
					}
					if(OnDrag != null && SelectedObject){
                        OnDrag(SelectedObject, worldPosition);// - clickPosition);
					}
				}
				clickPosition = mousePosition;
			}	
		}

		if (Input.GetMouseButtonUp (0)) {
			if(OnRelease != null)
				OnRelease();

			if(isDragStart && SelectedObject && OnDragEnd != null){
				OnDragEnd();
				isDragStart = false;
			}
			SelectedObject = null;

			UpObj = OnTouch(Input.mousePosition);

            
            if (OnFocus != null && UpObj != null && DownObj != null && DownObj == UpObj){
				OnFocus(UpObj, worldPosition);
			}

		}

		if (Input.GetKeyUp (KeyCode.LeftAlt)) {
			if(isMultiTouchStart){
				//
				isMultiTouchStart = false;
			}
		}

	}

	GameObject OnTouch(Vector3 pos){
		Ray ray = RaycastCamera.ScreenPointToRay(pos);
		RaycastHit hit;
		if (Physics.Raycast(ray, out hit, 20.0f, selectable_layer_mask))
        {
            return hit.transform.gameObject;
//			if(OnFocus != null && FocusedObject)
//				OnFocus(FocusedObject);
		}
		return null;
	} 

	/*void OnRaycast(Vector3 pos){

		Ray ray = RaycastCamera.ScreenPointToRay(pos);
		RaycastHit hit;
		if (Physics.Raycast(ray, out hit, Mathf.Infinity, selectable_layer_mask)){
			FocusedObject = hit.transform.gameObject;
			hitPoint = hit.point;
//			if(OnFocus != null && FocusedObject)
//				OnFocus(FocusedObject);
		}

	}*/

}