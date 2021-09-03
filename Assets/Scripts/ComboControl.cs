using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ComboControl : MonoBehaviour
{
    private Text _text;
    private BallZone _ballZone;

    private void Awake()
    {
        _text = GetComponent<Text>();
        _ballZone = FindObjectOfType<BallZone>();
        _text.text = string.Format("{0} combo!!", ++_ballZone.Combos);
    }

    // Update is called once per frame
    void Update()
    {
        _text.color = new UnityEngine.Color(_text.color.r, _text.color.g, _text.color.b, _text.color.a - Time.deltaTime);
        if (_text.color.a <= 0)
        {
            Destroy(gameObject);
        }
    }
}
