using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class theStack : MonoBehaviour {
	public Text scoreText;
	public Color32[] gameColors = new Color32[4];
	public Material stackMat;
	public GameObject endPanel;

	private const float BOUNDS_SIZE = 3.5F;
	private const float STACK_MOCING_SPEED = 5.0F;
	private const float ERROR_MARGIN = 0.25F;
	private const float STACK_BOUNDS_GAIN = 0.25F;
	private const int   COMBO_STACK_GAIN = 2;

	private GameObject[] Stack;
	private Vector2 stackBounds = new Vector2 (BOUNDS_SIZE, BOUNDS_SIZE);

	private int scoreCount = 0;
	private int stackIndex;
	private int combo = 0;

	private float tileTransition = .0f;
	private float tileSpeed = 2.5f;
	private float secondaryPosition;

	private bool isMovingOnX = true;
	private bool gameOver = false;

	private Vector3 desirePosition;
	private Vector3 lastTilePosition;

	// Use this for initialization
	void Start () {
		Stack = new GameObject[transform.childCount];
		for (int i = 0; i < transform.childCount; i++) {
			Stack [i] = transform.GetChild (i).gameObject;
			ColorMesh (Stack [i].GetComponent<MeshFilter> ().mesh);
		}
	
		stackIndex = transform.childCount - 1;
	}


	private void CreateRubble(Vector3 pos,Vector3 scale){
		GameObject go = GameObject.CreatePrimitive (PrimitiveType.Cube);
		go.transform.localPosition = pos;
		go.transform.localScale = scale;
		go.AddComponent<Rigidbody> ();

		go.GetComponent<MeshRenderer> ().material = stackMat; 
		ColorMesh (go.GetComponent<MeshFilter> ().mesh);
	}

	
	// Update is called once per frame
    private void Update () {
		if (Input.GetMouseButtonDown (0)) {
			if (PlaceTile ()) {
				SpawnTile ();
				scoreCount++;
				scoreText.text = scoreCount.ToString();
			} else {
				EndGame ();
			}
		}
		MoveTile ();

		//???
		transform.position = Vector3.Lerp (transform.position, desirePosition, STACK_MOCING_SPEED);
	}

	private void MoveTile(){
		Transform t = Stack [stackIndex].transform;
		t.localScale = new Vector3 (stackBounds.x, 1, stackBounds.y);
		if (gameOver) {
			return;
		}

		tileTransition += Time.deltaTime * tileSpeed;
		//two way —— x or z moving
		if(isMovingOnX)
			Stack [stackIndex].transform.localPosition = new Vector3 (Mathf.Sin (tileTransition)* BOUNDS_SIZE, scoreCount, secondaryPosition);
		else
			Stack [stackIndex].transform.localPosition = new Vector3 (secondaryPosition, scoreCount, Mathf.Sin (tileTransition)* BOUNDS_SIZE);
	}

	private void SpawnTile(){
		Transform t = Stack [stackIndex].transform;
		//spawn tile on the last tile
		lastTilePosition = Stack [stackIndex].transform.localPosition;

		//index loop
		stackIndex--;
		if (stackIndex < 0) {
			stackIndex = transform.childCount - 1;
		}
		desirePosition = Vector3.down * scoreCount;
		//localposition is the position related to the parent
		//t.localPosition = new Vector3 (0, scoreCount, 0);
		t.localScale = new Vector3 (stackBounds.x, 1, stackBounds.y);

		ColorMesh (Stack [stackIndex].GetComponent<MeshFilter> ().mesh);
	  
	}

	private void ColorMesh(Mesh mesh){
		Vector3[] vertices = mesh.vertices;
		Color32[] colors = new Color32[vertices.Length];
		float f = Mathf.Sin (scoreCount * 0.25f);

		for (int i = 0; i < vertices.Length; i++) {
			colors[i] = Lerp4(gameColors[0],gameColors[1],gameColors[2],gameColors[3],f);
		}

		mesh.colors32 = colors;
	}

	private bool PlaceTile()
	{
		Transform t = Stack [stackIndex].transform;

		if (isMovingOnX) {
			float deltaX = lastTilePosition.x - t.position.x;
			if (Mathf.Abs (deltaX) > ERROR_MARGIN) {
				//cut the tile
				combo = 0;
				stackBounds.x -= Mathf.Abs (deltaX); 
				if (stackBounds.x <= 0)
					return false;
				
				float middle = lastTilePosition.x + t.localPosition.x / 2; 
				t.localScale = new Vector3 (stackBounds.x, 1, stackBounds.y);
				CreateRubble (
					new Vector3 ((t.position.x>0)
						? t.position.x + (t.localScale.x / 2)
						: t.position.x - (t.localScale.x/2)
						, t.position.y
						, t.position.z),
					new Vector3 (Mathf.Abs (deltaX), 1, t.localScale.z)
				);
				t.localPosition = new Vector3 (middle - (lastTilePosition.x / 2), scoreCount, lastTilePosition.z);

			} else {
				if (combo > COMBO_STACK_GAIN) {
					if (stackBounds.x > BOUNDS_SIZE)
						stackBounds.x = BOUNDS_SIZE;
					
					//stackBounds.x += STACK_BOUNDS_GAIN;
					float middle = lastTilePosition.x + t.localPosition.x / 2; 
					t.localScale = new Vector3 (stackBounds.x, 1, stackBounds.y);
					CreateRubble (
						new Vector3 ((t.position.x>0)
							? t.position.x + (t.localScale.x / 2)
							: t.position.x - (t.localScale.x/2)
							, t.position.y
							, t.position.z),
						new Vector3 (Mathf.Abs (deltaX), 1, t.localScale.z)
					);
					t.localPosition = new Vector3 (middle - (lastTilePosition.x / 2), scoreCount, lastTilePosition.z);

				}
				combo++;
				t.localPosition = new Vector3 (lastTilePosition.x, scoreCount, lastTilePosition.z);
			}
		}
		 else {
			float deltaZ = lastTilePosition.z - t.position.z;
			if (Mathf.Abs (deltaZ) > ERROR_MARGIN) {
				//cut the tile
				combo = 0;
				stackBounds.y -= Mathf.Abs (deltaZ);
				if (stackBounds.y <= 0) 
					return false;
				
				float middle = lastTilePosition.z + t.localPosition.z / 2; 
				t.localScale = new Vector3 (stackBounds.x, 1, stackBounds.y);
				CreateRubble (
					new Vector3 (
						t.position.x
						, t.position.y
						, (t.position.z>0)
						? t.position.z+ (t.localScale.z/ 2)
						: t.position.z - (t.localScale.z/2)),
					new Vector3 (t.localScale.x, 1, Mathf.Abs (deltaZ))
				);
				t.localPosition = new Vector3 (lastTilePosition.x, scoreCount,middle - (lastTilePosition.z / 2));

			}else {
				if (combo > COMBO_STACK_GAIN) {
					if (stackBounds.y > BOUNDS_SIZE)
						stackBounds.y = BOUNDS_SIZE;
					
					//stackBounds.y += STACK_BOUNDS_GAIN;
					float middle = lastTilePosition.z + t.localPosition.z / 2; 
					t.localScale = new Vector3 (stackBounds.x, 1, stackBounds.y);
					CreateRubble (
						new Vector3 (
							t.position.x
							, t.position.y
							, (t.position.z>0)
							? t.position.z+ (t.localScale.z/ 2)
							: t.position.z - (t.localScale.z/2)),
						new Vector3 (t.localScale.x, 1, Mathf.Abs (deltaZ))
					);
					t.localPosition = new Vector3 (lastTilePosition.x, scoreCount,middle - (lastTilePosition.z / 2));

				}
				combo++;
				t.localPosition = new Vector3 (lastTilePosition.x, scoreCount, lastTilePosition.z);
			}
		}

		Debug.Log (combo);

		secondaryPosition = (isMovingOnX)
			? t.localPosition.x
			: t.localPosition.z;
		
		isMovingOnX = !isMovingOnX;
		return true;
	}

    //渐变颜色
	private Color32 Lerp4(Color32 a, Color32 b, Color32 c,Color32 d,float t){
		if (t < 0.33f) {
			return Color.Lerp (a, b, t / 0.33f);
		} else if (t < 0.66f) {
			return Color.Lerp (b, c, (t - 0.33f) / 0.33f);
		} else {
			return Color.Lerp (c, d, (t - 0.66f) / 0.66f);
		}
	}

	private void EndGame(){

		if (PlayerPrefs.GetInt ("score") < scoreCount) {
			PlayerPrefs.SetInt ("score", scoreCount);
		
		}
		Debug.Log ("lose");
		gameOver = true;
		endPanel.SetActive(true);
		Stack [stackIndex].AddComponent<Rigidbody> ();
	}

	public void OnButtonClick(string sceneName){
		SceneManager.LoadScene (sceneName);
	}

	public void toMenu(){
		SceneManager.LoadScene ("Menu");
	}

	public void Retry(){
		SceneManager.LoadScene ("Stack");
	}
}
