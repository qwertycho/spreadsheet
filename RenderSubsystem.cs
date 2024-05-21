using System.Text;

public class RenderSubsystem
{
    private List<string> Buffer = new List<string>();
    private static RenderSubsystem _instance = null;

    public static RenderSubsystem GetInstance()
    {
        {
            if (_instance == null)
            {
                _instance = new RenderSubsystem();
            }
            return _instance;
        }
    }

    public void Render(Spreadsheet spreadsheet)
    {
        var size = spreadsheet.GetSize();
        CellData active = spreadsheet.GetActive();

        for (int y = 0; y < size.h; y++)
        {
            StringBuilder sb = new StringBuilder();

            var row = spreadsheet.GetRow(y);

            for (int x = 0; x < size.w; x++)
            {
                if (x == 0)
                {
                    sb.Append('|');
                }

                int maxRowWidth = spreadsheet.GetMaxWidth(x);
                maxRowWidth = maxRowWidth >= 5 ? maxRowWidth : 5;

                CellData? cellData;
                if (row.TryGetValue(x, out cellData))
                {
                    sb.Append(Helpers.PadEnd(cellData.GetData(spreadsheet), maxRowWidth));
                }
                else
                {
                    sb.Append(Helpers.PadEnd(" ", maxRowWidth));
                }
                sb.Append('|');
            }
            string rowString = sb.ToString();
            Buffer.Add(rowString);
            Buffer.Add(new StringBuilder().Append('-', rowString.Length).ToString());
        }

        Buffer.Add($"{active.X}-{active.Y} | {spreadsheet.GetName()}");
        Console.Clear();

        for (int i = 0; i < Buffer.Count; i++)
        {
            Console.WriteLine(Buffer[i]);
        }
        Buffer.Clear();
    }
}