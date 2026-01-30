using PeanutVision.Console.Commands;

namespace PeanutVision.Console;

/// <summary>
/// Handles command registration and menu execution.
/// </summary>
public sealed class CommandRunner
{
    private readonly List<ICommand> _commands = new();
    private readonly CommandContext _context;

    public CommandRunner(CommandContext context)
    {
        _context = context;
        RegisterDefaultCommands();
    }

    /// <summary>
    /// Registers the default set of commands.
    /// </summary>
    private void RegisterDefaultCommands()
    {
        Register(new SystemStatusCommand());
        Register(new ContinuousAcquisitionCommand());
        Register(new SoftwareTriggerCommand());
        Register(new CalibrationCommand());
        Register(new BenchmarkCommand());
        Register(new CamFileInfoCommand());
    }

    /// <summary>
    /// Registers a command with the runner.
    /// </summary>
    public void Register(ICommand command)
    {
        _commands.Add(command);
    }

    /// <summary>
    /// Runs the main menu loop.
    /// </summary>
    public void Run()
    {
        while (true)
        {
            PrintMenu();
            var key = System.Console.ReadKey();
            System.Console.WriteLine();

            if (key.KeyChar == '0' || key.Key == ConsoleKey.Q)
            {
                System.Console.WriteLine("\nGoodbye!");
                return;
            }

            var command = _commands.FirstOrDefault(c => c.Key == key.KeyChar);
            if (command != null)
            {
                ExecuteCommand(command);
            }
            else
            {
                System.Console.WriteLine("\nInvalid option. Please try again.");
            }
        }
    }

    private void PrintMenu()
    {
        System.Console.WriteLine();
        System.Console.WriteLine("┌─────────────────────────────────────────────────────────────┐");
        System.Console.WriteLine("│                       MAIN MENU                             │");
        System.Console.WriteLine("├─────────────────────────────────────────────────────────────┤");

        foreach (var cmd in _commands)
        {
            System.Console.WriteLine($"│  {cmd.Key}. {cmd.Name,-20} - {cmd.Description,-23}│");
        }

        System.Console.WriteLine("│  0. Exit                                                    │");
        System.Console.WriteLine("└─────────────────────────────────────────────────────────────┘");
        System.Console.Write("\nSelect option: ");
    }

    private void ExecuteCommand(ICommand command)
    {
        try
        {
            command.Execute(_context);
        }
        catch (Exception ex)
        {
            System.Console.WriteLine($"\n[ERROR] Command failed: {ex.Message}");
            System.Console.WriteLine("\nPress any key to continue...");
            System.Console.ReadKey(true);
        }
    }
}
