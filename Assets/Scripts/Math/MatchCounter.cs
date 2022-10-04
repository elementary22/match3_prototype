using System.Collections.Generic;
using UnityEngine;

public class MatchCounter
{
    private Cell[,] _grid;
    private List<Vector2Int> _directions;
    private IDirectionCheck _directionChecker;

    public MatchCounter(Cell[,] grid, IDirectionCheck directionChecker)
    {
        _grid = grid;
        _directions = new List<Vector2Int> { new Vector2Int(0, 1), new Vector2Int(0, -1), new Vector2Int(1, 0), new Vector2Int(-1, 0) };
        _directionChecker = directionChecker;
    }

    public MatchData GetMatches()
    {
        MatchData matchData = new MatchData();

        foreach (Cell cell in _grid)
        {
            List<Cell> matchList = new List<Cell>();
            matchList.Add(cell);

            foreach (Vector2Int direction in _directions)
            {
                List<Cell> tempList = new List<Cell>();
                tempList = FindMatches(cell, direction, _grid);
                if (tempList.Count >= Config.minComboCount - 1)
                    matchList.AddRange(tempList);
            }

            if (matchList.Count >= Config.minComboCount)
            {
                foreach (Cell matchCell in matchList)
                {
                    matchCell.SetStatus(CELL_STATUS.DELETE);
                }
                MatchInfo info = new MatchInfo(matchList);
                matchData.data.Add(info);
            }
        }

        return matchData;
    }

    private List<Cell> FindMatches(Cell cell, Vector2Int direction, Cell[,] grid)
    {
        List<Cell> matches = new List<Cell>();

        if (_directionChecker.DirectionAvailable(new Vector2Int(cell.x, cell.y) + direction))
        {
            Cell nextCell = grid[cell.x + direction.x, cell.y + direction.y];

            while (nextCell.fruit == cell.fruit)
            {
                matches.Add(nextCell);
                if (!_directionChecker.DirectionAvailable(new Vector2Int(nextCell.x, nextCell.y) + direction) || nextCell.status == CELL_STATUS.DELETE)
                    break;

                nextCell = grid[nextCell.x + direction.x, nextCell.y + direction.y];
            }
        }
        return matches;
    }
}
