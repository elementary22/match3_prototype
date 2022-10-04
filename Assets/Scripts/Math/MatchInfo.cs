using System.Collections.Generic;

public class MatchInfo
{
    private List<Cell> _matchList;
    public List<Cell> matchList { get { return _matchList; } set { matchList = value; } }

    public MatchInfo(List<Cell> list) {
        _matchList = new List<Cell>();
        _matchList = list;
    }
}
