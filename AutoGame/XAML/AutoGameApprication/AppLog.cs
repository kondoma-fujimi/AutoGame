namespace AutoGameApprication
{
    internal static class AppLog
    {
        private static readonly object SyncRoot = new();
        private static readonly List<string> Entries = new();

        public static event EventHandler<string>? EntryAdded;

        public static IReadOnlyList<string> GetEntries()
        {
            lock (SyncRoot)
            {
                return Entries.ToArray();
            }
        }

        public static void Write(string message)
        {
            string entry = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff} {message}";
            lock (SyncRoot)
            {
                Entries.Add(entry);
            }

            EntryAdded?.Invoke(null, entry);
        }
    }
}
