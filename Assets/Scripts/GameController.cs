using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    [SerializeField]
    private PrefabSettings _prefabSettings;
    [SerializeField]
    private SpriteSettings _spriteSettings;
    [SerializeField]
    private GridView _gridView;
    private GridMath _gridMath;
    private SwipeChecker _swipeChecker;
    private MatchCounter _matchCounter;
    private List<Vector2Int> _activeCells;

    private Cell[,] _grid;
    private bool _isBusy = false;

    private void Start()
    {
        _gridMath = new GridMath();
        _grid = _gridMath.GenerateGrid(6, 8);

        _gridView.Init(_prefabSettings, _spriteSettings, _grid);
        _gridView.GenerateGridView();

        _swipeChecker = new SwipeChecker(_gridMath, _grid);
        _matchCounter = new MatchCounter(_grid, _gridMath);

        _gridView.onSwap += SwapCheck;
        _gridView.onSwapComplete += MatchCheck;
        _gridView.onSwapBack += UnblockSwap;
        _gridView.onDestroyComplete += ShiftCells;
        _gridView.onShiftComplete += RefillCells;
        _gridView.onRefillComplete += AnalyzeGrid;

        AnalyzeGrid();
    }

    private void MatchCheck(Vector2Int cell, Vector2Int cellToSwap)
    {
        MatchData matchData = _matchCounter.GetMatches();

        if (!isMatch(matchData))
            SwapCells(cell, cellToSwap, true);
        
        DestroyMatches(matchData);       
    }

    private void AnalyzeGrid()
    {
        UnblockSwap();
        MatchData matchData = _matchCounter.GetMatches();
        if (isMatch(matchData))
            DestroyMatches(matchData);
    }

    private bool isMatch(MatchData matchData)
    {
        return matchData.data.Count > 0;
    }

    private void DestroyMatches(MatchData matchData)
    {
        BlockSwap();
        _gridView.DestroyMatches(matchData);
    }

    private void SwapCheck(Vector2Int cell, Vector2Int direction)
    {
        if (_isBusy)
            return;

        if (_swipeChecker.CanSwap(cell, direction))
        {
            Vector2Int cellToSwap = cell + direction;
            SwapCells(cell, cellToSwap, false);
        }
    }

    private void SwapCells(Vector2Int cell, Vector2Int cellToSwap, bool swapBack)
    {
        BlockSwap();
        _gridMath.Swap(cell, cellToSwap);
        _gridView.Swap(cell, cellToSwap, swapBack);
    }

    private void ShiftCells()
    {
        Dictionary<Vector2Int, Vector2Int> newPositions = _gridMath.ShiftCells();
        _gridView.ShiftCells(newPositions);
    }

    private void RefillCells()
    {
        List<Cell> newCells = _gridMath.RefillCells();
        _gridView.RefillCells(newCells);
    }

    private void BlockSwap()
    {
        _isBusy = true;
    }

    private void UnblockSwap()
    {
        _isBusy = false;
    }

    private void OnDestroy()
    {
        _gridView.onSwap -= SwapCheck;
        _gridView.onSwapComplete -= MatchCheck;
        _gridView.onSwapBack -= UnblockSwap;
        _gridView.onDestroyComplete -= ShiftCells;
        _gridView.onShiftComplete -= RefillCells;
        _gridView.onRefillComplete -= AnalyzeGrid;
    }
}
