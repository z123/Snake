using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {

	public float speed = 1.0f;
	public GameObject snakeBlock;
	public GameObject blueBall;
	public Direction direction;
	public Direction nextDirection;
	public Vector3 oldPosition;

	private SpriteRenderer spriteRenderer;
	private float blockMove;
	private bool moving;
	private Vector3 newPosition;
	private float leftBound;
	private float rightBound;
	private float topBound;
	private float bottomBound;
	private ArrayList snakeBlocks;
	private bool okToAddBlock;

	// Use this for initialization
	void Start () {
		spriteRenderer = GetComponent<SpriteRenderer>();
//		blockMove =  spriteRenderer.sprite.bounds.extents.x * 2;
		blockMove = 0.32f;
		leftBound = -15 * blockMove + blockMove / 2.0f;
		rightBound = leftBound * -1;
		topBound = 11 * blockMove - blockMove / 2.0f;
		bottomBound = -1 * topBound;
		snakeBlocks = new ArrayList();
		okToAddBlock = false;
		spawnBlueBall();
	}
	
	// Update is called once per frame
	void Update () {
		UpdateDirection();
		UpdatePosition();
		UpdateTailPosition();
		if (okToAddBlock) {
			addBlock();
		}
	}

	void UpdateDirection() {
		if ((Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow)) && direction != Direction.Down) {
			nextDirection = Direction.Up;
		} else if ((Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)) && direction != Direction.Right) {
			nextDirection = Direction.Left;
		} else if ((Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)) && direction != Direction.Up) {
			nextDirection = Direction.Down;
		} else if ((Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) && direction != Direction.Left) {
			nextDirection = Direction.Right;
		}

		if (!moving) {
			this.direction = nextDirection;
			switch (direction) {
				case Direction.Down:
				transform.eulerAngles = new Vector3(0,0,90);
					break;
				case Direction.Left:
					transform.eulerAngles = new Vector3(0,0,0);
					break;
				case Direction.Right:
					transform.eulerAngles = new Vector3(0,0,180);
					break;
				case Direction.Up:
					transform.eulerAngles = new Vector3(0,0,270);
					break;
			}
		}
	}

	void UpdatePosition() {
		if (moving && transform.position == newPosition) {
			moving = false;
		} else if (!moving) {
			newPosition = transform.position;
			oldPosition = transform.position;
			switch (direction) {
				case Direction.Down:
					newPosition.y -= blockMove;
					break;
				case Direction.Left:
					newPosition.x -= blockMove;
					break;
				case Direction.Right:
					newPosition.x += blockMove;
					break;
				case Direction.Up:
					newPosition.y += blockMove;
					break;
				default:
					return;
					break;
			}
			moving = true;
		}
		transform.position = Vector3.MoveTowards(transform.position, newPosition, Time.deltaTime * speed);
		CheckBounds();
	}

	void UpdateTailPosition() {
		foreach (GameObject block in snakeBlocks) {
			block.GetComponent<SnakeBlock>().UpdatePosition();
		}
	}

	void CheckBounds() {
		float x = transform.position.x;
		float y = transform.position.y;

		if (x <= leftBound || x >= rightBound || y >= topBound || y <= bottomBound) {
			Reset();
		}
	}

	void Reset() {
		transform.position = new Vector3(0.16f, 0.16f, transform.position.z);
		Application.LoadLevel(Application.loadedLevel);
	}

	void addBlock() {
		GameObject lastBlock;
		Vector3 pos;
		bool isLastBlockMoving = false;
		if (snakeBlocks.Count == 0) {
			lastBlock = transform.root.gameObject;
			isLastBlockMoving = moving;
 		} else {
			lastBlock = (GameObject)snakeBlocks[snakeBlocks.Count - 1];
			isLastBlockMoving = lastBlock.GetComponent<SnakeBlock>().moving;
		}

		if (isLastBlockMoving) {
			return;
		} else {
			okToAddBlock = false;
		}

		pos = lastBlock.transform.position;
		switch (direction) {
			case Direction.Down:
				pos.y += 0.32f;
				break;
			case Direction.Left:
				pos.x += 0.32f;
				break;
			case Direction.Right:
				pos.x -= 0.32f;
				break;
			case Direction.Up:
				pos.y -= 0.32f;
				break;
			default:
				break;
		}

		Quaternion spawnRotation = Quaternion.identity;
		GameObject block = (GameObject)Instantiate(snakeBlock, pos, spawnRotation);
		block.GetComponent<SnakeBlock>().init(lastBlock, snakeBlocks.Count == 0);
		snakeBlocks.Add(block);
	}

	void OnTriggerEnter2D(Collider2D other) {
		if (other.tag.Equals("snakeBlock")) {
			Reset();
		} else {
			Destroy(other.gameObject);
			spawnBlueBall();
			okToAddBlock = true;
		}
	}

	void spawnBlueBall() {
		float y = Random.Range(-10, 10) * 0.32f - 0.16f;
		float x = Random.Range(-14, 14) * 0.32f - 0.16f;
		Vector3 pos = new Vector3(x, y);
		Instantiate(blueBall, pos, Quaternion.identity);
	}
}
