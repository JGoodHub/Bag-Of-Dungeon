using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using GoodHub.Core.Runtime.Utils;
using NaughtyAttributes;
using UnityEngine;
using Random = UnityEngine.Random;

public class DiceController : MonoBehaviour
{

    public class DiceResult
    {

        public int Sum;
        public List<int> SetOneValues = new List<int>();
        public List<int> SetTwoValues = new List<int>();

    }


    [SerializeField] private GameObject _dicePrefab;
    [SerializeField] private Transform _diceContainer;
    [Space]
    [SerializeField] private Vector3 _spawnArea;
    [SerializeField] private float _slideOffset;
    [SerializeField] private float _slideTime;
    [SerializeField] private float _pauseTime;
    [SerializeField] private float _bounceForce;
    [SerializeField] private float _rotationTorque;

    private bool _throwInProgress;

    private List<DieItem> _allDice = new List<DieItem>();
    private List<DieItem> _leftDice = new List<DieItem>();
    private List<DieItem> _rightDice = new List<DieItem>();

    public void ThrowDice(int diceCount, DiceColour diceColour, bool autoClear, Action<DiceResult> resultsCallback = null, Action endedCallback = null)
    {
        ThrowDice(diceCount, diceColour, 0, DiceColour.White, autoClear, resultsCallback, endedCallback);
    }

    public void ThrowDice(int diceCountOne, DiceColour diceColourOne, int diceCountTwo, DiceColour diceColourTwo,
        bool autoClear, Action<DiceResult> resultsCallback = null, Action endedCallback = null)
    {
        if (_throwInProgress)
            return;

        StartCoroutine(ThrowDiceRoutine(diceCountOne, diceColourOne, diceCountTwo, diceColourTwo,
            autoClear, resultsCallback, endedCallback));
    }

    private IEnumerator ThrowDiceRoutine(int diceCountOne, DiceColour diceColourOne, int diceCountTwo, DiceColour diceColourTwo,
        bool autoClear, Action<DiceResult> resultsCallback, Action endedCallback)
    {
        _throwInProgress = true;

        int diceTotal = diceCountOne + diceCountTwo;

        // Get the spawn positions from the dice

        List<Vector2> cells = DivideRectangle(VectorUtils.Vec3ToVec2_XZ(_spawnArea), diceTotal, out Vector2 cellSize);
        cells = cells.OrderBy(cell => cell.x).ToList();

        // Separate into left and right positions

        List<Vector2> leftCells = new List<Vector2>();
        List<Vector2> rightCells = new List<Vector2>();

        for (int i = 0; i < cells.Count; i++)
        {
            if (i < diceCountOne)
            {
                leftCells.Add(cells[i]);
            }
            else
            {
                rightCells.Add(cells[i]);
            }
        }

        // Shuffle the two cell sets so any gaps are random

        for (int i = 0; i < leftCells.Count; i++)
        {
            GameMaster.Singleton.Random.NextSwapIndices(leftCells.Count, out int indexA, out int indexB);
            (leftCells[indexA], leftCells[indexB]) = (leftCells[indexB], leftCells[indexA]);
        }

        for (int i = 0; i < rightCells.Count; i++)
        {
            GameMaster.Singleton.Random.NextSwapIndices(rightCells.Count, out int indexA, out int indexB);
            (rightCells[indexA], rightCells[indexB]) = (rightCells[indexB], rightCells[indexA]);
        }

        // Spawn the dice for the left and right sets with a random rotation and positions (within the cell)

        _allDice = new List<DieItem>();


        for (int i = 0; i < diceTotal; i++)
        {
            DieItem dieItem = Instantiate(_dicePrefab, _diceContainer).GetComponent<DieItem>();

            dieItem.transform.rotation = Quaternion.Euler(Random.Range(0, 4) * 90, Random.Range(0, 4) * 90, Random.Range(0, 4) * 90);
            dieItem.transform.Rotate(Vector3.up, Random.Range(0f, 360f), Space.World);

            Vector3 offset = VectorUtils.Random(cellSize.x / 4, 0f, cellSize.y / 4);
            offset.y = 0.51f;

            if (i < diceCountOne)
            {
                dieItem.transform.localPosition = VectorUtils.Vec2ToVec3_XZ(leftCells[i]) + offset;
                dieItem.SetColour(diceColourOne);
                _leftDice.Add(dieItem);
            }
            else
            {
                dieItem.transform.localPosition = VectorUtils.Vec2ToVec3_XZ(rightCells[i - leftCells.Count]) + offset;
                dieItem.SetColour(diceColourTwo);
                _rightDice.Add(dieItem);
            }

            dieItem.SetFrozen(true);

            _allDice.Add(dieItem);
        }

        // Slide them into frame from off screen

        _diceContainer.localPosition = new Vector3(-_slideOffset, 0, 0);

        _diceContainer.DOLocalMoveX(0, _slideTime);

        yield return new WaitForSeconds(_slideTime + _pauseTime);

        // Throw them into the air

        foreach (DieItem dieItem in _allDice)
        {
            dieItem.BounceDie(_bounceForce * Random.Range(0.85f, 1.1f), _rotationTorque, 0f);
        }

        // Wait for them to settle, re-roll any cocked or stacked dice if needed

        yield return new WaitForSeconds(_pauseTime);

        float preSettlingTime = Time.time;

        yield return new WaitUntil(() =>
        {
            bool allSettled = true;

            foreach (DieItem dieItem in _allDice)
            {
                // Still moving
                if (dieItem.Speed > 0.03f)
                {
                    allSettled = false;
                    continue;
                }

                // At the peak of it's arc (zero speed at peak)
                if (dieItem.transform.localPosition.y > 3f)
                {
                    allSettled = false;
                    continue;
                }

                // It's either cocked or landed on top of another die, bounce it again
                if (dieItem.IsSkewed() || dieItem.transform.localPosition.y > 1f)
                {
                    dieItem.BounceDie(_bounceForce * Random.Range(0.9f, 1.1f), _rotationTorque, _bounceForce * 0.25f);
                    allSettled = false;
                    continue;
                }
            }

            // Catch for bugged out rolls
            if (Time.time - preSettlingTime > 25f)
                allSettled = true;

            return allSettled;
        });

        foreach (DieItem dieItem in _allDice)
        {
            dieItem.SetFrozen(true);
        }

        // Read the results

        List<int> leftResults = new List<int>();
        List<int> rightResults = new List<int>();

        foreach (DieItem dieItem in _leftDice)
        {
            leftResults.Add(dieItem.GetResult());
        }

        foreach (DieItem dieItem in _rightDice)
        {
            rightResults.Add(dieItem.GetResult());
        }

        resultsCallback?.Invoke(new DiceResult
        {
            Sum = leftResults.Sum() + rightResults.Sum(),
            SetOneValues = leftResults,
            SetTwoValues = rightResults
        });

        // Clear away the dice

        if (autoClear)
        {
            yield return new WaitForSeconds(_pauseTime);

            ClearDice(endedCallback);
        }
    }

    public void ClearDice(Action endedCallback = null)
    {
        if (_throwInProgress == false)
            return;

        StartCoroutine(ClearDiceRoutine(endedCallback));
    }

    private IEnumerator ClearDiceRoutine(Action endedCallback)
    {
        _diceContainer.DOLocalMoveX(_slideOffset, _slideTime);

        yield return new WaitForSeconds(_slideTime);

        foreach (DieItem dieItem in _allDice)
        {
            Destroy(dieItem.gameObject);
        }

        _allDice.Clear();
        _leftDice.Clear();
        _rightDice.Clear();

        _diceContainer.transform.localPosition = Vector3.zero;

        _throwInProgress = false;

        endedCallback?.Invoke();
    }

    public List<Vector2> DivideRectangle(Vector2 rectangle, int numCells, out Vector2 cellSize)
    {
        // Replace this hack with a nicer algorithm please

        Vector2Int cellCount = numCells switch
        {
            1 => new Vector2Int(1, 1), // 1
            2 => new Vector2Int(2, 1), // 2
            3 => new Vector2Int(3, 1), // 3
            4 => new Vector2Int(2, 2), // 4
            5 => new Vector2Int(3, 2), // 6
            6 => new Vector2Int(3, 2), // 6
            7 => new Vector2Int(3, 3), // 9
            8 => new Vector2Int(3, 3), // 9
            9 => new Vector2Int(3, 3), // 9
            10 => new Vector2Int(4, 3), // 12
            11 => new Vector2Int(4, 3), // 12
            12 => new Vector2Int(4, 3), // 12
            13 => new Vector2Int(4, 4), // 16
            14 => new Vector2Int(4, 4), // 16
            15 => new Vector2Int(4, 4), // 16
            16 => new Vector2Int(4, 4), // 16
            _ => new Vector2Int(5, 5)
        };

        float width = rectangle.x;
        float height = rectangle.y;

        float cellWidth = width / cellCount.x;
        float cellHeight = height / cellCount.y;

        cellSize = new Vector2(cellWidth, cellHeight);

        List<Vector2> cells = new List<Vector2>();

        for (int y = 0; y < cellCount.y; y++)
        {
            for (int x = 0; x < cellCount.x; x++)
            {
                float cellCenterX = (x * cellWidth) + (cellWidth * 0.5f) - (width * 0.5f);
                float cellCenterY = (y * cellHeight) + (cellHeight * 0.5f) - (height * 0.5f);

                Vector2 cellCenter = new Vector2(cellCenterX, cellCenterY);

                cells.Add(cellCenter);
            }
        }

        return cells;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireCube(_diceContainer.position, _spawnArea);
    }


    [Button]
    public void TestThrowFour()
    {
        ThrowDice(4, DiceColour.White, 0, DiceColour.White, true);
    }

    [Button]
    public void TestThrowSix()
    {
        ThrowDice(3, DiceColour.White, 3, DiceColour.Black, true);
    }

    [Button]
    public void TestThrowEleven()
    {
        ThrowDice(6, DiceColour.Grey, 5, DiceColour.Red, true);
    }

    [Button]
    public void TestThrowProbabilities()
    {
        StartCoroutine(TestThrowRoutine());
    }

    private IEnumerator TestThrowRoutine()
    {
        Dictionary<int, int> sideCounts = new Dictionary<int, int>
        {
            { 1, 0 },
            { 2, 0 },
            { 3, 0 },
            { 4, 0 },
            { 5, 0 },
            { 6, 0 }
        };

        bool waiting = false;

        while (true)
        {
            waiting = true;

            ThrowDice(16, DiceColour.White, true, result =>
            {
                foreach (int side in result.SetOneValues)
                {
                    sideCounts[side]++;
                }

                string output = string.Empty;

                foreach ((int side, int total) in sideCounts)
                {
                    output += $"| {side} : {total} | ";
                }

                Debug.Log(output);
            }, () =>
            {
                waiting = false;
            });

            yield return new WaitWhile(() => waiting);
        }
    }

}