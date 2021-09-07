using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BallZone : MonoBehaviour
{
#pragma warning disable IDE0044
    // config params
    [SerializeField] int _zoneHeight;
    [SerializeField] int _zoneWidth;
    [SerializeField] GameObject[] _prefabBalls;
    [SerializeField] Canvas _canvas;
    [SerializeField] GameObject _prefabComboText;
    // the interval time of combo in second
    [SerializeField] float _intervalTimeOfCombo;
#pragma warning restore IDE0044

    // state variables
    private int _combos;
    private SortedSet<int> _removeSetAll;

    public int ZoneHeight { get => _zoneHeight; set => _zoneHeight = value; }
    public int ZoneWidth { get => _zoneWidth; set => _zoneWidth = value; }
    public int Combos { get => _combos; set => _combos = value; }

    // Start is called before the first frame update
    void Start()
    {
        NewGame();
    }

    // create a random but the ball that not repetition more than 2
    // in a holizatal or vertical line
    private void NewGame()
    {
        int posX = _zoneWidth;
        int posY = _zoneHeight - 3;
        var ballPos = new int[posY, posX];
        for (int y = 0; y < posY; y++)
        {
            for (int x = 0; x < posX; x++)
            {
                int n = _prefabBalls.Length;
                int[] colorPickable = new int[n];
                for (int i = 0; i < n; i++)
                {
                    colorPickable[i] = i;
                }
                try
                {
                    if (ballPos[y, x - 1] == ballPos[y, x - 2])
                    {
                        for (int colorCode = 0; colorCode < n; colorCode++)
                        {
                            try
                            {
                                if (colorPickable[colorCode] == ballPos[y, x - 1])
                                {
                                    colorPickable[colorCode] = colorPickable[n - 1];
                                    n--;
                                    break;
                                }
                            }
#pragma warning disable CS0168, IDE0059
                            catch (System.IndexOutOfRangeException IOE)
                            {
                                // Noting
                            }
#pragma warning restore CS0168, IDE0059
                        }
                    }
                }
#pragma warning disable CS0168, IDE0059
                catch (System.IndexOutOfRangeException IOE)
                {
                    // Noting
                }
#pragma warning restore CS0168, IDE0059


                try
                {
                    if (ballPos[y - 1, x] == ballPos[y - 2, x])
                    {
                        for (int colorCode = 0; colorCode < n; colorCode++)
                        {
                            try
                            {
                                if (colorPickable[colorCode] == ballPos[y - 1, x])
                                {
                                    colorPickable[colorCode] = colorPickable[n - 1];
                                    n--;
                                    break;
                                }
                            }
#pragma warning disable CS0168, IDE0059
                            catch (System.IndexOutOfRangeException IOE)
                            { 
                                // Noting;
                            }
#pragma warning restore CS0168, IDE0059
                        }
                    }
                }
#pragma warning disable CS0168, IDE0059
                catch (System.IndexOutOfRangeException IOE)
                {
                    // Noting
                }
#pragma warning restore CS0168, IDE0059

                ballPos[y, x] = colorPickable[Random.Range(0, n)];
            }
        }

        // create whole ball zone
        for (int y = 0; y < _zoneHeight - 3; y++)
        {
            for (int x = 0; x < _zoneWidth; x++)
            {
                var ball = Instantiate(
                    _prefabBalls[ballPos[y, x]],
                    new Vector2(x + 0.5f, y + 0.5f),
                    new Quaternion(0, 0, 0, 0),
                    transform
                    );

                // set ball color code
                ball.GetComponentInChildren<Ball>().Color =
                    (Color.Color)System.Enum.ToObject(typeof(Color.Color), ballPos[y, x]);
            }
        }
    }

    public void CheckLines()
    {
        Ball[] balls = GetComponentsInChildren<Ball>();
        var sortBallsByPos = new Ball[_zoneWidth * (_zoneHeight - 3)];
        // sort balls's index from ball position
        for (int i = 0; i < balls.Length; i++)
        {
            balls[i].IsCheck = true;
            var position = balls[i].ReturnPos();
            int index = (int)position.x + ((int)position.y) * _zoneWidth;
            sortBallsByPos[index] = balls[i];
        }

        var destroyList = new List<SortedSet<int>>();
        GroupMethod(sortBallsByPos, destroyList);

        RemoveNotLine(sortBallsByPos, destroyList);

        var finalList = new List<SortedSet<int>>();
        for (int i = 0; i < destroyList.Count; i++)
        {
            if (destroyList[i].Count >= 3)
            {
                finalList.Add(destroyList[i]);
            }
        }

        // finalList.cout > 0 than destroy ball, if not reset combos to 0 and turn off balls.IsCheck
        if (finalList.Count > 0)
        {
            _removeSetAll = new SortedSet<int>();
            StartCoroutine(DestroyBalls2D(sortBallsByPos, finalList, _intervalTimeOfCombo));
        }
        else
        {
            _combos = 0;
            for (int i = 0; i < balls.Length; i++)
            {
                balls[i].IsCheck = false;
            }
        }
    }

    private void GroupMethod(Ball[] balls, List<SortedSet<int>> destroyList)
    {
        for (int i = 0; i < balls.Length; i++)
        {
            // repeat number check
            bool isNewNumber = true;
            for (int listIndex = 0; listIndex < destroyList.Count; listIndex++)
            {
                if (destroyList[listIndex].Contains(i))
                {
                    isNewNumber = false;
                }
            }
            if (isNewNumber)
            {
                // if new number than create a temp set that calls group to
                // store destroyable ball index and add this set into destroyList
                var group = new SortedSet<int> { i };
                destroyList.Add(group);
            }
            else
            {
                continue;
            }

            // do round check in all the same color ball 
            RoundCheck(balls, destroyList[destroyList.Count - 1], i);
        }
    }

    // add round balls when the color is same
    private void RoundCheck(Ball[] balls, SortedSet<int> group, int i)
    {
        var rounds = new int[] { 1, _zoneWidth, -1, -_zoneWidth };
        foreach (var round in rounds)
        {
            try
            {
                // when the ball color of round is same and not add this ball before
                if (!group.Contains(i + round) && balls[i].Color == balls[i + round].Color)
                {
                    if (i % _zoneWidth == 0 && round == -1)
                    {
                        continue;
                    }

                    if (i % _zoneWidth == _zoneWidth - 1 && round == 1)
                    {
                        continue;
                    }

                    group.Add(i + round);

                    // when the color is same do that a new same color ball round check first
                    RoundCheck(balls, group, i + round);
                }
            }
#pragma warning disable CS0168, IDE0059
            // catch exception when i that Index out of range and do nothing
            catch (System.IndexOutOfRangeException IOE) { continue; }
#pragma warning restore CS0168, IDE0059
        }
    }

    // check list and remove the ball that not more than 3 ball in one line
    private void RemoveNotLine(Ball[] balls, List<SortedSet<int>> destroyList)
    {
        foreach (var set in destroyList)
        {
            // create a temp set for remove another set if it necessary
            var removeSet = new HashSet<int>();

            // check list and remove the ball that not more than 3 ball in one line
            foreach (int groupIndex in set)
            {
                var roundH = new int[] { 1, 2, -1, -2 };
                var roundV = new int[] { _zoneWidth, _zoneWidth * 2, -_zoneWidth, -_zoneWidth * 2 };
                int ballsInline = 1;
                for (int hI = 0; hI < roundH.Length; hI += 2)
                {
                    if (groupIndex % _zoneWidth == 0 && roundH[hI] == -1)
                    {
                        break;
                    }
                    if (groupIndex % _zoneWidth == _zoneWidth - 1 && roundH[hI] == 1)
                    {
                        continue;
                    }

                    try
                    {
                        if (balls[groupIndex].Color == balls[groupIndex + roundH[hI]].Color)
                        {
                            ballsInline++;
                            if (!(roundH[hI] == 1 && groupIndex % _zoneWidth == _zoneWidth - 2) &&
                                balls[groupIndex].Color == balls[groupIndex + roundH[hI + 1]].Color)
                            {
                                ballsInline++;
                            }
                        }
                    }
#pragma warning disable CS0168
                    catch (System.IndexOutOfRangeException IOE) {; }
#pragma warning restore CS0168
                }
                if (ballsInline >= 3)
                {
                    continue;
                }

                ballsInline = 1;
                for (int vI = 0; vI < roundV.Length; vI += 2)
                {
                    try
                    {
                        if (balls[groupIndex].Color == balls[groupIndex + roundV[vI]].Color)
                        {
                            ballsInline++;
                            if (balls[groupIndex].Color == balls[groupIndex + roundV[vI + 1]].Color)
                            {
                                ballsInline++;
                            }
                        }
                    }
#pragma warning disable CS0168
                    catch (System.IndexOutOfRangeException IOE) {; }
#pragma warning restore CS0168
                }

                if (ballsInline < 3)
                {
                    removeSet.Add(groupIndex);
                }
            }

            if (removeSet.Count > 0)
            {
                foreach (var removeItem in removeSet)
                {
                    set.Remove(removeItem);
                }
            }
        }
    }

    // destroy balls if it can and put new ball after
    IEnumerator DestroyBalls2D(Ball[] balls, List<SortedSet<int>> finalList, float second)
    {
        foreach (var index in finalList[0])
        {
            _removeSetAll.Add(index);
            balls[index].transform.parent.gameObject.SetActive(false);
            Destroy(balls[index].transform.parent.gameObject);
        }
        CreateComboText();
        finalList.RemoveAt(0);

        yield return new WaitForSeconds(second);

        if (finalList.Count > 0)
        {
            StartCoroutine(DestroyBalls2D(balls, finalList, second));
        }
        else
        {
            NextBalls(balls);
        }
    }

    private void NextBalls(Ball[] ballsRemained)
    {
        var tops = new int[_zoneWidth];

        // move down the remain of balls
        for (int i = 0; i < ballsRemained.Length; i++)
        {
            if (ballsRemained[i] == null)
            {
                continue;
            }

            tops[i % _zoneWidth] += 1;
            foreach (var index in _removeSetAll)
            {
                if (index >= i)
                {
                    break;
                }
                if (index % _zoneWidth == i % _zoneWidth)
                {
                    ballsRemained[i].MoveDown();
                }
            }
        }

        // create new balls
        var putPos = new int[_zoneWidth];

        foreach (var index in _removeSetAll)
        {
            var colorCode = Random.Range(0, _prefabBalls.Length);
            var ball = Instantiate(
                    _prefabBalls[colorCode],
                    new Vector2(index % _zoneWidth + 0.5f, _zoneHeight - 3 + 0.5f + putPos[index % _zoneWidth]),
                    new Quaternion(0, 0, 0, 0),
                    transform
                    );
            
            // set ball color code and move ball down
            var ballTemp = ball.GetComponentInChildren<Ball>();
            ballTemp.Color =
                (Color.Color)System.Enum.ToObject(typeof(Color.Color), colorCode);
            ballTemp.MoveDown(_zoneHeight - 3 - tops[index % _zoneWidth] + putPos[index % _zoneWidth]);

            tops[index % _zoneWidth]++;
            putPos[index % _zoneWidth]++;
        }

        // check lines again
        // need some time to let ball down move
        StartCoroutine(WaitToCheckLines());
    }

    IEnumerator WaitToCheckLines()
    {
        yield return new WaitForSeconds(_intervalTimeOfCombo);
        CheckLines();
    }

    private void CreateComboText()
    {
        Instantiate(_prefabComboText, _canvas.transform).GetComponent<Text>();
    }
}