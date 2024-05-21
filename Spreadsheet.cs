using System.Reflection.Metadata;
using System.Runtime.CompilerServices;
using System.Text.Json;

public class Spreadsheet
{
    private string Name;
    private int xLength;
    private int yLength;
    /// <summary>
    /// y - x based
    /// </summary>
    private CellData[,] Sheet;

    private CellData ActiveCell;
    private int ActiveX = 0;
    private int ActiveY = 0;

    private bool Changes = false;

    public Spreadsheet(int w, int h)
    {
        Name = "Sheet1";

        Sheet = new CellData[h, w];
        xLength = w;
        yLength = h;

        Sheet[ActiveX, ActiveY] = new CellData(0, 0);
        ActiveCell = Sheet[ActiveX, ActiveY];
        ActiveCell.SetActive();

        InputSubSystem isub = InputSubSystem.GetInputSubSystem();
        isub.Subscribe(new InputSubSystem.SubscriberData(ConsoleKey.None, GetAction(this), false));
        isub.Subscribe(new InputSubSystem.SubscriberData(ConsoleKey.None, GetInputAction(this), false));

        Changes = true;
    }

    public bool HasChanges(bool reset)
    {
        bool tmp = Changes;
        if (reset)
        {
            Changes = false;
        }
        return tmp;
    }

    public void SetName(string name)
    {
        Name = name;
        Changes = true;
    }

    public string GetName()
    {
        return Name;
    }

    public string GetData(int x, int y)
    {
        CellData cData = Sheet[y, x];
        if(cData == null)
        {
            return "";
        } else {
            return cData.GetData(this);
        }
    }

    public static Spreadsheet Load(string path)
    {
        if (!File.Exists(path))
        {
            throw new FileNotFoundException(path);
        }

        string json = File.ReadAllText(path);
        ExportSpreadsheet export = JsonSerializer.Deserialize<ExportSpreadsheet>(json);

        Spreadsheet sht = new Spreadsheet(export.XLength, export.YLength);
        sht.Load(export);

        return sht;
    }
    public void Load(ExportSpreadsheet export)
    {
        Name = export.Name;

        xLength = export.XLength;
        yLength = export.YLength;

        Sheet = new CellData[yLength, xLength];

        for (int y = 0; y < yLength; y++)
        {
            foreach (var x in export.Collumns[y])
            {
                Sheet[y, x.Key] = x.Value;
                if (x.Value.Data.StartsWith("> "))
                {
                    ActiveX = x.Key;
                    ActiveY = y;
                    ActiveCell = Sheet[y, x.Key];
                    ActiveCell.Data = ActiveCell.Data.Substring(2);
                }
            }
        }

        ActiveCell.SetActive();
        Changes = true;
    }

    public ExportSpreadsheet GetExport()
    {
        ExportSpreadsheet export = new ExportSpreadsheet();
        export.Name = Name;
        export.XLength = xLength;
        export.YLength = yLength;
        export.Collumns = new List<Dictionary<int, CellData>>();

        for (int y = 0; y < yLength; y++)
        {
            export.Collumns.Add(GetRow(y));
        }

        return export;
    }

    public int GetMaxWidth(int x)
    {
        int maxWidth = 0;

        if (x >= xLength)
        {
            throw new IndexOutOfRangeException(x.ToString());
        }

        for (int i = 0; i < yLength; i++)
        {
            CellData tmp = Sheet[i, x];
            if (tmp != null)
            {
                string dt = tmp.GetData(this);
                if (dt.Length > maxWidth)
                {
                    maxWidth = dt.Length;
                }
            }
        }

        return maxWidth;
    }

    public Dictionary<int, CellData> GetRow(int y)
    {
        Dictionary<int, CellData> datas = new Dictionary<int, CellData>();

        if (y >= yLength)
        {
            throw new IndexOutOfRangeException(y.ToString());
        }

        for (int x = 0; x < xLength; x++)
        {
            CellData data = Sheet[y, x];
            if (data != null)
            {
                datas[x] = data;
            }
        }
        return datas;
    }

    public CellData MoveCursor(int xOffset, int yOffset)
    {
        int newX = ActiveX + xOffset;
        int newY = ActiveY + yOffset;

        if (newX > xLength - 1)
        {
            newX = 0;
        }
        else if (newX < 0)
        {
            newX = xLength - 1;
        }

        if (newY > yLength - 1)
        {
            newY = 0;
        }
        else if (newY < 0)
        {
            newY = yLength - 1;
        }

        //TODO: delete old active if it holds no data

        ActiveX = newX;
        ActiveY = newY;

        ActiveCell.SetInactive();

        ActiveCell = Sheet[ActiveY, ActiveX];
        if (ActiveCell == null)
        {
            ActiveCell = new CellData(ActiveX, ActiveY);
            Sheet[ActiveY, ActiveX] = ActiveCell;
        }
        ActiveCell.SetActive();

        Changes = true;

        return ActiveCell;
    }

    public CellData GetActive()
    {
        return ActiveCell;
    }

    public (int w, int h) GetSize()
    {
        return (xLength, yLength);
    }

    public void AddData(char c)
    {
        ActiveCell.AddData(c);
        Changes = true;
    }

    public void RemoveData()
    {
        ActiveCell.DeleteLastData();
        Changes = true;
    }

    private Action<InputSubSystem.EventData> GetInputAction(Spreadsheet sht)
    {
        return new Action<InputSubSystem.EventData>(x =>
        {
            switch (x.Key)
            {
                case ConsoleKey.Backspace:
                    sht.RemoveData();
                    break;

                case ConsoleKey.Delete:
                    sht.RemoveData();
                    break;

                case ConsoleKey.Enter:
                    sht.MoveCursor(0, 1);
                    break;

                case ConsoleKey.Spacebar:
                    sht.AddData(' ');
                    break;

                default:


                 if ((int)x.Info.KeyChar >= 33 && (int)x.Info.KeyChar <= 122)
                    {
                        string ch = ((char)x.Info.KeyChar).ToString();

                       sht.AddData(ch.First());
                    }
                    else if ((int)x.Key >= 47 && (int)x.Key <= 57)
                    {
                        int num = Math.Abs(48 - (int)x.Key);
                        sht.AddData(num.ToString().First());
                    }
                    break;
            }
        });
    }

    private Action<InputSubSystem.EventData> GetAction(Spreadsheet self)
    {
        return new Action<InputSubSystem.EventData>(x =>
        {
            switch (x.Key)
            {
                case ConsoleKey.UpArrow:
                    self.MoveCursor(0, -1);
                    break;
                case ConsoleKey.DownArrow:
                    self.MoveCursor(0, 1);
                    break;
                case ConsoleKey.LeftArrow:
                    self.MoveCursor(-1, 0);
                    break;
                case ConsoleKey.RightArrow:
                    self.MoveCursor(1, 0);
                    break;
            }
        });
    }
}

public class ExportSpreadsheet
{
    public string Name { get; set; }
    public int XLength { get; set; }
    public int YLength { get; set; }
    public List<Dictionary<int, CellData>> Collumns { get; set; }

    public void Save()
    {
        string path = $"{Name}.sheet";

        string json = JsonSerializer.Serialize(this);
        File.WriteAllText(path, json);
    }
}