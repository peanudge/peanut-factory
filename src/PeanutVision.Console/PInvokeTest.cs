using System.Runtime.InteropServices;

namespace PeanutVision.Console;

public static partial class PInvokeTest
{
	[LibraryImport("libc", EntryPoint = "getpid")]
	public static partial int GetProcessId();
}
