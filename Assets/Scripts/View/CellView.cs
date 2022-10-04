using System;
using UnityEngine;
using UnityEngine.UI;

public class CellView : MonoBehaviour
{
    [SerializeField]
    private Image _icon;

    private int _x;
    private int _y;
    private FRUIT _fruit;

    public int x { get { return _x; } set { _x = value; } }
    public int y { get { return _y; } set { _y = value; } }
    public FRUIT fruit { get { return _fruit; } set { _fruit = value; } }

    private float _swipeAngle = 0;
    protected Vector2 mouseStartPosition;
    protected Vector2 mouseFinishPosition;
    protected bool selected;

    public Action<Vector2Int, Vector2Int> onSwap;

    public void SetSprite(Sprite sprite) {
        _icon.sprite = sprite;
    }

    protected void OnMouseDown() {
        mouseStartPosition = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        selected = true;
    }

    protected void OnMouseExit()
    {
        if (selected)
        {
            mouseFinishPosition = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
            selected = false;
            SwipeChecker checker = new SwipeChecker();
            onSwap?.Invoke(new Vector2Int(_x, _y), checker.GetDirection(CalculateAngle()));
        }
    }

    private float CalculateAngle()
    {
        _swipeAngle = Mathf.Atan2(mouseFinishPosition.y - mouseStartPosition.y, mouseFinishPosition.x - mouseStartPosition.x) * 180 / Mathf.PI;
        return _swipeAngle;
    }

    private void OnDestroy() {
        onSwap = null;
    }
}
