namespace PeanutVision.Console;

/// <summary>
/// Interface for console menu commands.
/// </summary>
public interface ICommand
{
    /// <summary>
    /// Display name shown in the menu.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Short description shown in the menu.
    /// </summary>
    string Description { get; }

    /// <summary>
    /// Key to press to select this command (e.g., '1', '2').
    /// </summary>
    char Key { get; }

    /// <summary>
    /// Executes the command.
    /// </summary>
    void Execute(CommandContext context);
}

/// <summary>
/// Base class for commands with common helper methods.
/// </summary>
public abstract class CommandBase : ICommand
{
    public abstract string Name { get; }
    public abstract string Description { get; }
    public abstract char Key { get; }

    public abstract void Execute(CommandContext context);

    #region Console Helpers

    protected static void Print(string text = "") => System.Console.WriteLine(text);

    protected static void PrintInline(string text) => System.Console.Write(text);

    protected static void PrintHeader(string title)
    {
        Print();
        Print("═══════════════════════════════════════════════════════════════");
        Print($"                      {title,-30}");
        Print("═══════════════════════════════════════════════════════════════");
    }

    protected static void PrintSection(string title)
    {
        Print($"\n  ┌─────────────────────────────────────────────────────────┐");
        Print($"  │{title,55}│");
        Print($"  └─────────────────────────────────────────────────────────┘");
    }

    protected static void PrintFooter()
    {
        Print("\n═══════════════════════════════════════════════════════════════");
    }

    protected static void PrintError(string message)
    {
        Print($"\n  [ERROR] {message}");
    }

    protected static void PrintSuccess(string message)
    {
        Print($"     [OK] {message}");
    }

    protected static void PrintInfo(string message)
    {
        Print($"     [INFO] {message}");
    }

    protected static void PrintSkipped()
    {
        Print("     [SKIPPED]");
    }

    protected static void WaitForKey()
    {
        Print("\n  Press any key to continue...");
        System.Console.ReadKey(true);
    }

    protected static ConsoleKeyInfo ReadKey(bool intercept = false)
    {
        return System.Console.ReadKey(intercept);
    }

    protected static string? ReadLine()
    {
        return System.Console.ReadLine();
    }

    protected static bool RequireHardware(CommandContext context, string operation)
    {
        if (!context.IsHardwareAvailable)
        {
            Print($"\n[ERROR] No boards detected. Cannot {operation}.");
            WaitForKey();
            return false;
        }
        return true;
    }

    #endregion
}
