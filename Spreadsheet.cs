public class Spreadsheet
{
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

    public Spreadsheet()
    {
        Sheet = new CellData[10, 10];
        xLength = 10;
        yLength = 10;

        Sheet[ActiveX, ActiveY] = new CellData(0, 0);
        ActiveCell = Sheet[ActiveX, ActiveY];
        ActiveCell.SetActive();

        InputSubSystem isub = InputSubSystem.GetInputSubSystem();
        isub.Subscribe(new InputSubSystem.SubscriberData(ConsoleKey.None, GetAction(this), false));
        isub.Subscribe(new InputSubSystem.SubscriberData(ConsoleKey.None, GetInputAction(this), false));

        Changes = true;
    }

    public int GetMaxWidth(int x)
    {
        int maxWidth = 0;

        for (int i = 0; i < yLength; i++)
        {
            CellData tmp = Sheet[i, x];
            if (tmp != null)
            {
                if (tmp.Data.Length > maxWidth)
                {
                    maxWidth = tmp.Data.Length;
                }
            }
        }

        return maxWidth;
    }

    public Dictionary<int, CellData> GetRow(int y)
    {
        Dictionary<int, CellData> datas = new Dictionary<int, CellData>();

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

        if (newX > Sheet.GetLength(0) - 1)
        {
            newX = 0;
        }
        else if (newX < 0)
        {
            newX = Sheet.GetLength(0) - 1;
        }

        if (newY > Sheet.GetLength(1) - 1)
        {
            newY = 0;
        }
        else if (newY < 0)
        {
            newY = Sheet.GetLength(1) - 1;
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
        ActiveCell.Data += c;
    }

    public void RemoveData()
    {
        ActiveCell.DeleteLastData();
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
                    ActiveCell.Data += " ";
                    break;

                default:
                    if (((int)x.Key >= 65 && (int)x.Key <= 90) || ((int)x.Key >= 106 && (int)x.Key <= 111))
                    {
                        string ch = ((char)x.Key).ToString();

                        if (!x.IsUpper)
                        {
                            ActiveCell.Data += ch.ToLower().First();
                        }
                        else
                        {
                            ActiveCell.Data += ch.First();
                        }
                    } else if ((int)x.Key >= 47 && (int)x.Key <=  57)
                    {
                        int num = Math.Abs(48 - (int)x.Key);
                        ActiveCell.Data += num.ToString();
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