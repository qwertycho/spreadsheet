InputSubSystem inputSubSystem = InputSubSystem.GetInputSubSystem();
RenderSubsystem renderSubsystem = RenderSubsystem.GetInstance();
Spreadsheet spreadsheet = null;

for (int i = 0; i < args.Length; i++)
{
    switch (args[i])
    {
        case "-o":
            spreadsheet = Spreadsheet.Load(args[i + 1]);
            i++;
            break;
        case "-n":
            if(spreadsheet == null)
            {
                spreadsheet = new Spreadsheet(10, 10);
            }
            spreadsheet.SetName(args[i + 1]);
            i++;
            break;
        case "-s":
            spreadsheet = new Spreadsheet(int.Parse(args[i + 1]), int.Parse(args[i + 2]));
            i += 2;
            break;
    }
}

bool run = true;

//stop the application when the user presses ctl + q
inputSubSystem.Subscribe(new InputSubSystem.SubscriberData(ConsoleKey.Q, (x) => { run = false; }, true));
inputSubSystem.Start();

while (run)
{
    if (spreadsheet.HasChanges(true))
    {
        renderSubsystem.Render(spreadsheet);
    }
    Task.Delay(1).Wait();
}

Console.WriteLine("Save and Exit? (Y/N)");
var key = Console.ReadLine().First();
if (key == 'Y' || key == 'y')
{
    spreadsheet.GetExport().Save();
}
else
{
    Console.WriteLine("Exiting without saving");
}