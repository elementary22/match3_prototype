using UnityEngine;
using System;
using System.Collections.Generic;

public class GridMath : IDirectionCheck
{
    private Cell[,] _grid;
    private int _width;
    private int _height;


    public Cell[,] GenerateGrid(int width, int height)
    {
        _width = width;
        _height = height;

        _grid = new Cell[width, height];

        for (int x = 0; x < width; x++)
        {

            for (int y = 0; y < height; y++)
            {

                _grid[x, y] = new Cell(x, y, GetRandomFruit());
            }
        }

        return _grid;
    }

    private FRUIT GetRandomFruit()
    {
        Array values = Enum.GetValues(typeof(FRUIT));
        FRUIT randomFruit = (FRUIT)values.GetValue(UnityEngine.Random.Range(0, values.Length));
        return randomFruit;
    }

    public void Swap(Vector2Int v1, Vector2Int v2)
    {
        Cell presentCell = _grid[v1.x, v1.y];
        Cell cellToSwap = _grid[v2.x, v2.y];

        UpdateIndex(presentCell, v2);
        UpdateIndex(cellToSwap, v1);
    }

    public void UpdateIndex(Cell cell, Vector2Int index)
    {

        cell.x = index.x;
        cell.y = index.y;
        _grid[cell.x, cell.y] = cell;
    }

    public bool DirectionAvailable(Vector2Int vector)
    {
        if (vector.x >= 0 && vector.y >= 0 &&
           vector.x < _grid.GetLength(0) && vector.y < _grid.GetLength(1))
        {
            return true;
        }

        return false;
    }

    public Dictionary<Vector2Int, Vector2Int> ShiftCells()
    {
        Vector2Int direction = new Vector2Int(0, 1);
        Dictionary<Vector2Int, Vector2Int> newPositions = new Dictionary<Vector2Int, Vector2Int>();

        foreach (Cell cell in _grid)
        {
            if (cell.status == CELL_STATUS.DELETE)
            {
                if (DirectionAvailable(new Vector2Int(cell.x, cell.y) + direction))
                {
                    Cell nextCell = FindFirstFruit(cell, direction);

                    if (nextCell.status != CELL_STATUS.DELETE)
                    {
                        newPositions.Add(new Vector2Int(nextCell.x, nextCell.y), new Vector2Int(cell.x, cell.y));
                        Swap(new Vector2Int(nextCell.x, nextCell.y), new Vector2Int(cell.x, cell.y));
                    }
                }
            }
        }
        return newPositions;
    }

    private Cell FindFirstFruit(Cell cell, Vector2Int direction)
    {
        Cell nextCell = _grid[cell.x + direction.x, cell.y + direction.y];
        while (nextCell.status == CELL_STATUS.DELETE)
        {
            if (!DirectionAvailable(new Vector2Int(nextCell.x, nextCell.y) + direction))
            {
                break;
            }
            nextCell = _grid[nextCell.x + direction.x, nextCell.y + direction.y];
        }
        return nextCell;
    }

    public List<Cell> RefillCells()
    {
        List<Cell> newCells = new List<Cell>();
        foreach (Cell cell in _grid)
        {
            if (cell.status == CELL_STATUS.DELETE)
            {
                Cell newCell = new Cell(cell.x, cell.y, GetRandomFruit());
                _grid[cell.x, cell.y] = newCell;
                newCells.Add(newCell);
            }
            cell.SetStatus(CELL_STATUS.STABLE);
        }
        
        return newCells;
    }
}
