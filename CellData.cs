public class CellData
{
    public int X { get; set; }
    public int Y { get; set; }
    public string Data { get; set;}

    public CellData(int x, int y)
    {
        X = x;
        Y = y;
        Data = string.Empty;
    }

    public void SetActive()
    {
        if (!string.IsNullOrEmpty(Data))
        {
            Data = $"> {Data}";
        }
        else
        {
            Data = "> ";
        }
    }

    public void SetInactive()
    {
        Data = Data.Substring(2);
    }

    public void DeleteLastData()
    {
        if (!string.IsNullOrEmpty(Data))
        {
            Data = Data.Remove(Data.Length - 1);
        }
    }
}