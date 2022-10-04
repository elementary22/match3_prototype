using UnityEngine;

public class SwipeChecker
{
    private Cell[,] _grid;
    private IDirectionCheck _directionChecker;

    public SwipeChecker(IDirectionCheck directionCheck = null, Cell[,] grid = null)
    {
        _directionChecker = directionCheck;
        _grid = grid;
    }

    public Vector2Int GetDirection(float angle)
    {
        if (angle > -45 && angle <= 45)
        {
            return new Vector2Int(1, 0); // Right Swipe 
        }
        else if (angle > 45 && angle <= 135)
        {
            return new Vector2Int(0, 1); // Up Swipe
        }
        else if (angle > 135 || angle <= -135)
        {
            return new Vector2Int(-1, 0); // Left Swipe
        }
        else if (angle >= -135 && angle < -45)
        {
            return new Vector2Int(0, -1); // Down Swipe
        }

        return Vector2Int.zero;
    }

    public bool CanSwap(Vector2Int currentIndex, Vector2Int direction)
    {
        return _directionChecker.DirectionAvailable(currentIndex + direction);
    }
}
