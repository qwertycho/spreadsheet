using System.Text.RegularExpressions;
using SpreadSheet.Calculator;

public class CellData
{
    public bool Active { get; set; }
    public int X { get; set; }
    public int Y { get; set; }
    public string Data { get; set;}

    public CellData(int x, int y)
    {
        X = x;
        Y = y;
        Data = string.Empty;
    }
    public void AddData(char c)
    {
        Data += c;
    }

    public void RemoveData()
    {
        if (!string.IsNullOrEmpty(Data))
        {
            Data = Data.Remove(Data.Length - 1);
        }
    }

    private void Log(string t)
    {
        File.AppendAllLines("/home/qwertycho/Desktop/cell.log", new string[] {t});
    }

    private List<KeyValuePair<string, string>> GetRef(Spreadsheet sht)
    {
        string pattern = @"\(\d+,\s*\d+\)";
        Regex rgx = new Regex(pattern);
        MatchCollection matches = rgx.Matches(Data);

        CellData[] refs = new CellData[matches.Count];

        List<KeyValuePair<string, string>> data = new List<KeyValuePair<string, string>>();
        
        foreach (Match match in matches)
        {
            Log(match.Value.ToString() + " test");

            var cordS = match.Value.Replace("(", "").Replace(")", "").Split(',');
            
            int x = int.Parse(cordS[0]);
            int y = int.Parse(cordS[1]);

            string cordData = sht.GetData(x, y);
            data.Add(new KeyValuePair<string, string>(match.Value, cordData));
        }
        return data;
    }

    public string GetData(Spreadsheet sht)
    {
        if(Active)
        {
            return Data;
        }
        if(Data.StartsWith("=(") || Data.StartsWith(" =(")|| Data.StartsWith("> =("))
        {
            try{
            var refVals = GetRef(sht);
            if(refVals.Count == 0)
            {
                return Data;
            }

            string tmp = Data;

            foreach (var val in refVals)
            {
                tmp = tmp.Replace(val.Key, val.Value);
            }

            Calculator calculator = new Calculator();

            return calculator.Caluculate(tmp).ToString();
            } catch (Exception e)
            {
                Log(e.Message);
                return "ERROR";
            }
        }

        return Data;
    }

    public void SetActive()
    {
        Active = true;
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
        Active = false;
    }

    public void DeleteLastData()
    {
        if (!string.IsNullOrEmpty(Data))
        {
            Data = Data.Remove(Data.Length - 1);
        }
    }
}