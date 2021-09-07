using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
    // cached reference
    GameSystem _gameSystem;
    Cursor _cursor;
    Rigidbody2D _rigidbody2D;

    // state variables
    private Color.Color _color;
    Vector2 _targetPos;
    bool _isCheck;
    bool _isMoveDown;

    public Cursor Cursor { get => _cursor; set => _cursor = value; }
    public Color.Color Color { get => _color; set => _color = value; }
    public Rigidbody2D Rigidbody2D { get => _rigidbody2D; set => _rigidbody2D = value; }
    public Vector2 TargetPos { get => _targetPos; set => _targetPos = value; }
    public bool IsCheck { get => _isCheck; set => _isCheck = value; }

    private void Awake()
    {
        _isMoveDown = false;
        _targetPos = transform.parent.position;
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _gameSystem = FindObjectOfType<GameSystem>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!_isCheck && Input.GetKey(KeyCode.Mouse0))
        {
            Vector2 mousePos = _gameSystem.GetMousePosition2D();
            if ((int) mousePos.x == (int) transform.parent.position.x &&
                (int) mousePos.y == (int) transform.parent.position.y)
            {
                Hold();
            }
        }
    }

    private void Hold()
    {
        Cursor = FindObjectOfType<Cursor>();
        if (Cursor.BallHeld != null)
        {
            return;
        }
        Cursor.LastBallPos = transform.parent.position;
        Cursor.BallHeld = this;
        gameObject.tag = "BallHeld";
        transform.parent.SetParent(Cursor.transform);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (Cursor != null && collision.gameObject.CompareTag("Ball"))
        {
            ChangePos(collision);
        }
    }

    private void ChangePos(Collision2D collision)
    {
        var collisionBall = collision.gameObject.GetComponent<Ball>();
        Vector2 temp = collision.transform.parent.position;
        collision.transform.parent.position = _cursor.LastBallPos;
        collisionBall.TargetPos = _cursor.LastBallPos;
        _cursor.LastBallPos = temp;
    }



    public Vector2 ReturnPos()
    {
        return _rigidbody2D.position;
    }

    public void MoveDown()
    {
        _rigidbody2D.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation;
        _rigidbody2D.velocity = new Vector2(0f, -9.81f);
        _targetPos.y -= 1;
        _isMoveDown = true;
    }

    public void MoveDown(float distance)
    {
        _rigidbody2D.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation;
        _rigidbody2D.velocity = new Vector2(0f, -9.81f);
        _targetPos.y -= distance;
        _isMoveDown = true;
    }

    private void FixedUpdate()
    {
        if (_isMoveDown && transform.position.y <= _targetPos.y)
        {
            _isMoveDown = false;
            _rigidbody2D.constraints = RigidbodyConstraints2D.FreezeAll;
            transform.parent.position = _targetPos;
            transform.localPosition = new Vector2();
            _rigidbody2D.velocity = new Vector2(0, 0);
        }
        else if (_isMoveDown)
        {
            _rigidbody2D.MovePosition(_rigidbody2D.position + _rigidbody2D.velocity * Time.fixedDeltaTime);
        }
    }
}
