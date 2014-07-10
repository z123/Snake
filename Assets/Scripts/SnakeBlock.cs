using UnityEngine;
using System.Collections;

public class SnakeBlock : MonoBehaviour {

	public float speed;
	public float blockMove;
	public Vector3 oldPosition;
	public bool moving;

	private GameObject parent;
	private Vector3 newPosition;
	private bool firstSnakeBlock;

	public void init(GameObject parent, bool firstSnakeBlock) {
		this.parent = parent;
		this.firstSnakeBlock = firstSnakeBlock;
	}

//	void Update() {
//		UpdatePosition();			
//	}

	public void UpdatePosition() {
		if (moving && transform.position == newPosition) {
			moving = false;
		} else if (!moving) {
			oldPosition = transform.position;
			if (!firstSnakeBlock) {
				newPosition = parent.GetComponent<SnakeBlock>().oldPosition;
			} else {
				newPosition = parent.GetComponent<Player>().oldPosition;
			}
			moving = true;
		}
		transform.position = Vector3.MoveTowards(transform.position, newPosition, Time.deltaTime * speed);
	}
}
