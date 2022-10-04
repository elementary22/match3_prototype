using System.Collections.Generic;

public class MatchData
{
    private List<MatchInfo> _data;
    public List<MatchInfo> data { get { return _data; } set { _data = value; } }

    public MatchData() {
        _data = new List<MatchInfo>();
    }
}
