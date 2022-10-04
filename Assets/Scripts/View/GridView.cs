using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GridView : MonoBehaviour
{
    [SerializeField]
    private Transform _backgroundContainer;
    [SerializeField]
    private Transform _cellContainer;
    private PrefabSettings _prefabSettings;
    private SpriteSettings _spriteSettings;
    private Cell[,] _grid;
    private CellView[,] _gridView;
    private int _width;
    private int _height;
    private int _moveCounter = 0;
    private int _destroyCounter = 0;
    private Vector2Int _cellPos;
    private Vector2Int _cellToSwapPos;
    public Action<Vector2Int, Vector2Int> onSwap;
    public Action<Vector2Int, Vector2Int> onSwapComplete;
    public Action onSwapBack;
    public Action onDestroyComplete;
    public Action onShiftComplete;
    public Action onRefillComplete;

    public void Init(PrefabSettings prefabSettings, SpriteSettings spriteSettings, Cell[,] grid)
    {
        _prefabSettings = prefabSettings;
        _spriteSettings = spriteSettings;
        _grid = grid;
        _width = grid.GetLength(0);
        _height = grid.GetLength(1);

    }

    public void GenerateGridView()
    {
        GenerateBackground();
        GenerateFruits();
    }

    private void GenerateBackground()
    {
        for (int x = 0; x < _width; x++)
        {
            for (int y = 0; y < _height; y++)
            {

                GameObject bg = Instantiate(_prefabSettings.GetBackgroundPrefab());
                bg.GetComponent<Image>().sprite = _spriteSettings.GetUIElementSprite(UIElement.BACKGROUND_ICE);
                bg.transform.SetParent(_backgroundContainer, false);
                bg.transform.localPosition = GetPosition(new Vector2Int(x, y));
            }
        }
    }

    private void GenerateFruits()
    {
        _gridView = new CellView[_width, _height];

        for (int x = 0; x < _width; x++)
        {
            for (int y = 0; y < _height; y++)
            {
                SetUpFruit(x, y);
            }
        }
    }

    private void SetUpFruit(int x, int y)
    {
        GameObject fruit = Instantiate(_prefabSettings.GetFruitPrefab());
        fruit.transform.SetParent(_cellContainer, false);
        fruit.transform.localPosition = GetPosition(new Vector2Int(x, y));

        CellView fruitView = fruit.GetComponent<CellView>();
        fruitView.x = x;
        fruitView.y = y;
        fruitView.fruit = _grid[x, y].fruit;
        fruitView.SetSprite(_spriteSettings.GetFruitSprite(fruitView.fruit));

        fruitView.onSwap += OnSwap;

        _gridView[x, y] = fruitView;
    }

    private void OnSwap(Vector2Int cell, Vector2Int direction)
    {
        onSwap?.Invoke(cell, direction);
    }

    public void Swap(Vector2Int presentPos, Vector2Int nextPos, bool swapBack = false)
    {
        _cellPos = presentPos;
        _cellToSwapPos = nextPos;

        CellView presentFruit = _gridView[presentPos.x, presentPos.y];
        CellView fruitToSwap = _gridView[nextPos.x, nextPos.y];

        Vector3 presentFruitPos = GetPosition(presentPos);
        Vector3 nextFruitPos = GetPosition(nextPos);

        CheckSwapStatus(presentFruit, fruitToSwap, presentFruitPos, nextFruitPos, swapBack);

        UpdateIndex(presentFruit, nextPos);
        UpdateIndex(fruitToSwap, presentPos);
    }

    private void CheckSwapStatus(CellView presentFruit, CellView fruitToSwap, Vector3 presentFruitPos, Vector3 nextFruitPos, bool status)
    {
        if (status)
        {
            Move(presentFruit, presentFruitPos, nextFruitPos);
            Move(fruitToSwap, nextFruitPos, presentFruitPos, OnSwapBack);
        }
        else
        {
            Move(presentFruit, presentFruitPos, nextFruitPos);
            Move(fruitToSwap, nextFruitPos, presentFruitPos, CompleteSwap);
        }
    }

    public void UpdateIndex(CellView fruit, Vector2Int index)
    {
        fruit.x = index.x;
        fruit.y = index.y;
        _gridView[fruit.x, fruit.y] = fruit;
    }

    private void CompleteSwap()
    {
        onSwapComplete?.Invoke(_cellPos, _cellToSwapPos);
    }

    private void OnSwapBack()
    {
        onSwapBack?.Invoke();
    }

    private Vector3 GetPosition(Vector2Int cell)
    {
        return new Vector3(cell.x * Config.offset, cell.y * Config.offset, 0);
    }

    private void Move(CellView cellToMove, Vector3 startPoint, Vector3 finishPoint, Action callback = null)
    {
        _moveCounter++;
        StartCoroutine(IMove(cellToMove, startPoint, finishPoint, callback));
    }

    private IEnumerator IMove(CellView cellToMove, Vector3 startPoint, Vector3 finishPoint, Action callback = null)
    {
        float step = 0;
        while (step < 1)
        {
            cellToMove.transform.localPosition = Vector3.Lerp(startPoint, finishPoint, step);
            step += Time.deltaTime / Config.moveTimer;
            yield return null;
        }
        cellToMove.transform.localPosition = finishPoint;
        _moveCounter--;
        if (_moveCounter == 0)
            callback?.Invoke();
    }

    public void DestroyMatches(MatchData matchData)
    {
        foreach (MatchInfo info in matchData.data)
        {
            foreach (Cell cell in info.matchList)
            {
                _destroyCounter++;
                CellView view = _gridView[cell.x, cell.y];
                StartCoroutine(IDestroyMatches(view));
            }
        }
    }

    private IEnumerator IDestroyMatches(CellView view)
    {

        List<Sprite> destroySprites = _spriteSettings.GetDestroySprites(view.fruit);
        foreach (Sprite sprite in destroySprites)
        {
            view.SetSprite(sprite);
            yield return new WaitForSeconds(0.06f);
        }
        Destroy(view.gameObject);
        _destroyCounter--;
        if (_destroyCounter == 0)
            onDestroyComplete?.Invoke();
    }

    public void ShiftCells(Dictionary<Vector2Int, Vector2Int> newPositions)
    {
        if (newPositions.Count == 0)
        {
            onShiftComplete?.Invoke();
            return;
        }

        foreach (var pos in newPositions)
        {
            CellView fruit = _gridView[pos.Key.x, pos.Key.y];
            Vector3 startPoint = GetPosition(new Vector2Int(pos.Key.x, pos.Key.y));
            Vector3 finishPoint = GetPosition(new Vector2Int(pos.Value.x, pos.Value.y));

            Move(fruit, startPoint, finishPoint, CompleteShift);
            UpdateIndex(fruit, new Vector2Int(pos.Value.x, pos.Value.y));
        }
    }

    private void CompleteShift()
    {
        onShiftComplete?.Invoke();
    }

    public void RefillCells(List<Cell> newCells)
    {
        foreach (Cell cell in newCells)
        {
            SetUpFruit(cell.x, cell.y);
        }

        onRefillComplete?.Invoke();
    }

    private void OnDestroy()
    {
        onSwap = null;
        onSwapComplete = null;
        onSwapBack = null;
        onDestroyComplete = null;
        onShiftComplete = null;
        onRefillComplete = null;
    }
}
