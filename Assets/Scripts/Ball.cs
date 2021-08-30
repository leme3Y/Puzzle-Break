using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
    // cached reference
    GameSystem _gameSystem;
    Cursor _cursor;

    // state variables
    [SerializeField] private Color.Color _color;

    public Cursor Cursor { get => _cursor; set => _cursor = value; }
    public Color.Color Color { get => _color; set => _color = value; }


    // Start is called before the first frame update
    void Start()
    {
        _gameSystem = FindObjectOfType<GameSystem>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.Mouse0))
        {
            Vector2 mousePos = _gameSystem.GetMousePosition2D();
            if ((int) mousePos.x == (int) transform.position.x &&
                (int) mousePos.y == (int) transform.position.y)
            {
                Hold();
            }
        }

        //if (Input.GetKey(KeyCode.Mouse0))
        //{
        //    Vector2 mousePos = _gameSystem.GetMousePosition2D();
        //    if ((int)mousePos.x == (int)transform.position.x &&
        //        (int)mousePos.y == (int)transform.position.y)
        //    {
        //        Hold();
        //    }
        //}
    }

    private void Hold()
    {
        Cursor = FindObjectOfType<Cursor>();
        if (Cursor.BallHeld != null)
        {
            return;
        }
        Cursor.LastBallPos = transform.position;
        Cursor.BallHeld = gameObject;
        transform.SetParent(Cursor.transform);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject == Cursor.BallHeld)
        {
            ChangePos();
        }
    }

    private void ChangePos()
    {
        Vector2 temp = transform.position;
        transform.position = _cursor.LastBallPos;
        _cursor.LastBallPos = temp;
    }

    public Vector2 ReturnPos()
    {
        return transform.position;
    }
}
