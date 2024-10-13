namespace DH.Database.MigrationUtility;

internal class ConsoleFileLogger : IDisposable
{
    readonly StreamWriter _logStream;
    bool disposedValue;
    public string FullFileName { get; private set; }

    public ConsoleFileLogger()
    {
        FullFileName = Path.Combine(AppContext.BaseDirectory, DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".log");
        _logStream = new StreamWriter(FullFileName);
    }

    public void WriteLine(string m)
    {
        _logStream.WriteLine(m);
        Console.WriteLine(m);
    }

    public void WriteLine()
    {
        _logStream.WriteLine();
        Console.WriteLine();
    }

    public void MentionHerself()
    {
        Console.WriteLine();
        Console.WriteLine("-------------------------------------------------------------");
        Console.WriteLine("You can find this log in " + FullFileName);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                _logStream.Dispose();
            }

            disposedValue = true;
        }
    }

    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
