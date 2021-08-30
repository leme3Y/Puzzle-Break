using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cursor : MonoBehaviour
{
    // config params
    [SerializeField] GameSystem _gameSystem;

    // cached references
    GameObject _ballHeld;
    [SerializeField] BallZone _ballZone;

    // state variables
    Vector2 _lastBallPos;

    public GameObject BallHeld { get => _ballHeld; set => _ballHeld = value; }
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
        _ballHeld.transform.position = _lastBallPos;
        _ballHeld.transform.SetParent(_ballZone.transform);
        _ballHeld = null;
        _lastBallPos = new Vector2();
    }

}
