using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cursor : MonoBehaviour
{
#pragma warning disable IDE0044
    // config params
    [SerializeField] GameSystem _gameSystem;

    // cached references
    Ball _ballHeld;
    [SerializeField] BallZone _ballZone;
#pragma warning restore IDE0044

    // state variables
    Vector2 _lastBallPos;

    public Ball BallHeld { get => _ballHeld; set => _ballHeld = value; }
    public Vector2 LastBallPos { get => _lastBallPos; set => _lastBallPos = value; }


    // Update is called once per frame
    void Update()
    {
        transform.position = _gameSystem.GetMousePosition2D();

        if (_ballHeld != null && Input.GetKeyUp(KeyCode.Mouse0))
        {
            ReleaseBall();
            _ballZone.CheckLines();
            
        }
    }

    private void ReleaseBall()
    {
        _ballHeld.gameObject.tag = "Ball";
        _ballHeld.transform.parent.position = _lastBallPos;
        _ballHeld.TargetPos = _lastBallPos;
        _ballHeld.transform.parent.SetParent(_ballZone.transform);
        _ballHeld = null;
        _lastBallPos = new Vector2();
    }

}
