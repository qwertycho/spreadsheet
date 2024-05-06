using System.Text;

InputSubSystem inputSubSystem = InputSubSystem.GetInputSubSystem();

bool run = true;

inputSubSystem.Start();

Spreadsheet spreadsheet= new Spreadsheet();


string PadEnd(string s, int l)
{
    if(string.IsNullOrEmpty(s))
    {
        return new StringBuilder().Append(' ', l).ToString();
    }
    if(s.Length >= l)
    {
        return s.Substring(0, l);
    } else {
        return new StringBuilder(s).Append(' ', l - s.Length ).ToString();
    }
}

while(run)
{
    List<string> buffer = new List<string>();

    var size = spreadsheet.GetSize();
    CellData active = spreadsheet.GetActive();

    for(int y = 0; y < size.h; y++)
    {
        StringBuilder sb = new StringBuilder();

        var row = spreadsheet.GetRow(y);

        for(int x = 0; x < size.w; x++)
        {
            if(x == 0)
            {
                sb.Append('|');
            }

            int maxRowWidth = spreadsheet.GetMaxWidth(x);
            maxRowWidth = maxRowWidth >= 5? maxRowWidth : 5;
            
            CellData? cellData;
            if(row.TryGetValue(x, out cellData))
            {
                sb.Append(PadEnd(cellData.Data, maxRowWidth));
            } else 
            {
                sb.Append(PadEnd(" ", maxRowWidth));
            }
            sb.Append('|');
        }
        string rowString = sb.ToString();
        buffer.Add(rowString);
        buffer.Add(new StringBuilder().Append('-', rowString.Length).ToString());
    }

    buffer.Add($"{active.X}-{active.Y}");
    Console.Clear();

    for(int i = 0; i < buffer.Count; i++)
    {
        Console.WriteLine(buffer[i]);
    }
    buffer.Clear();

    Task.Delay(15).Wait();
}

public class CellData
{
    public int X { get; set; }
    public int Y { get; set; }
    public string Data = string.Empty;

    public CellData(int x, int y)
    {
        X = x;
        Y = y;
    }

    public void SetActive()
    {
        if(!string.IsNullOrEmpty(Data))
        {
            Data = $"> {Data}";
        } else {
            Data = "> ";
        }
    }

    public void SetInactive()
    {
        Data = Data.Substring(2);
    }

    public void DeleteLastData()
    {
        if(!string.IsNullOrEmpty(Data))
        {
            Data = Data.Remove(Data.Length-1);
        }
    }
}