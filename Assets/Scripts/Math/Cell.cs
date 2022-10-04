public class Cell
{
    private int _x;
    private int _y;
    private FRUIT _fruit;

    public CELL_STATUS status = CELL_STATUS.STABLE;

    public int x { get { return _x; } set { _x = value; } }
    public int y { get { return _y; } set { _y = value; } }
    public FRUIT fruit { get { return _fruit; } set { _fruit = value; } }

    public Cell(int x, int y, FRUIT fruit) {
        _x = x; 
        _y = y;
        _fruit = fruit;
    }

    public void SetStatus(CELL_STATUS status) {
        this.status = status;
    }
}
