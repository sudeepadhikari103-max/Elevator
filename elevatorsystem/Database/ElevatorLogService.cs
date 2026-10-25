// Database Implementation for Elevator System

using System.Data;
using System.Data.SQLite;
using System.Diagnostics;
using System.Text;

namespace elevatorsystem.Database
{

    public sealed class ElevatorLogService
    {

        private readonly string _databasePath;

        private readonly object _dbLock = new();

        public ElevatorLogService()
        {
            string dataDirectory = Path.Combine(AppContext.BaseDirectory, "data");
            Directory.CreateDirectory(dataDirectory);

            _databasePath = Path.Combine(dataDirectory, "ElevatorLog.db");

            string legacyPath = Path.Combine(dataDirectory, "elevator_log.db");
            if (File.Exists(legacyPath) && !File.Exists(_databasePath))
            {
                try
                {
                    File.Copy(legacyPath, _databasePath, overwrite: false);
                }
                catch { }
            }

            InitializeDatabase();
        }

        private void InitializeDatabase()
        {
            lock (_dbLock)
            {
                try
                {
                    using var connection = OpenConnection();
                    MigrateSchema(connection);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"[ElevatorLogService] Database init error: {ex.Message}");
                    throw new InvalidOperationException(
                        "Unable to initialize the elevator SQLite database.", ex);
                }
            }
        }

        private static void MigrateSchema(SQLiteConnection connection)
        {
            bool tableExists = false;
            using (var chkCmd = connection.CreateCommand())
            {
                chkCmd.CommandText = "SELECT COUNT(*) FROM sqlite_master WHERE type='table' AND name='ElevatorLog';";
                tableExists = Convert.ToInt32(chkCmd.ExecuteScalar()) > 0;
            }

            if (!tableExists)
            {
                Exec(connection, @"
                    CREATE TABLE IF NOT EXISTS ElevatorLog (
                        Eid             INTEGER PRIMARY KEY AUTOINCREMENT,
                        LogDate         TEXT    NOT NULL DEFAULT '',
                        LogTime         TEXT    NOT NULL DEFAULT '',
                        ActionPerformed TEXT    NOT NULL DEFAULT '',
                        Timestamp       TEXT    NOT NULL DEFAULT '',
                        EventType       TEXT    NOT NULL DEFAULT 'SYSTEM',
                        Floor           INTEGER NOT NULL DEFAULT 1,
                        Status          TEXT    NOT NULL DEFAULT 'Info',
                        Details         TEXT    NOT NULL DEFAULT ''
                    );");
                return;
            }

            var existing = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            using (var cmd = connection.CreateCommand())
            {
                cmd.CommandText = "PRAGMA table_info(ElevatorLog);";
                using var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    existing.Add(reader.GetString(1));
                }
            }

            if (existing.Contains("Message"))
            {
                Exec(connection, @"
                    CREATE TABLE ElevatorLog_migrated (
                        Id        INTEGER PRIMARY KEY AUTOINCREMENT,
                        Timestamp TEXT    NOT NULL,
                        EventType TEXT    NOT NULL DEFAULT 'SYSTEM',
                        Floor     INTEGER NOT NULL DEFAULT 1,
                        Status    TEXT    NOT NULL DEFAULT 'Info',
                        Details   TEXT    NOT NULL DEFAULT ''
                    );");

                Exec(connection, @"
                    INSERT INTO ElevatorLog_migrated (Id, Timestamp, EventType, Floor, Status, Details)
                    SELECT
                        Id,
                        Timestamp,
                        COALESCE(EventType, 'SYSTEM'),
                        COALESCE(Floor, 1),
                        COALESCE(Status, 'Info'),
                        CASE
                            WHEN Details IS NOT NULL AND Details != '' THEN Details
                            ELSE COALESCE(Message, '')
                        END
                    FROM ElevatorLog;");

                Exec(connection, "DROP TABLE ElevatorLog;");
                Exec(connection, "ALTER TABLE ElevatorLog_migrated RENAME TO ElevatorLog;");
            }
            else
            {

                if (!existing.Contains("LogDate"))
                    Exec(connection, "ALTER TABLE ElevatorLog ADD COLUMN LogDate TEXT NOT NULL DEFAULT '';");

                if (!existing.Contains("LogTime"))
                    Exec(connection, "ALTER TABLE ElevatorLog ADD COLUMN LogTime TEXT NOT NULL DEFAULT '';");

                if (!existing.Contains("ActionPerformed"))
                    Exec(connection, "ALTER TABLE ElevatorLog ADD COLUMN ActionPerformed TEXT NOT NULL DEFAULT '';");

                if (!existing.Contains("EventType"))
                    Exec(connection, "ALTER TABLE ElevatorLog ADD COLUMN EventType TEXT NOT NULL DEFAULT 'SYSTEM';");

                if (!existing.Contains("Floor"))
                    Exec(connection, "ALTER TABLE ElevatorLog ADD COLUMN Floor INTEGER NOT NULL DEFAULT 1;");

                if (!existing.Contains("Details"))
                    Exec(connection, "ALTER TABLE ElevatorLog ADD COLUMN Details TEXT NOT NULL DEFAULT '';");
            }
        }

        public void LogEvent(string status, string message) =>
            LogEvent("INFO", 1, status, message);

        public void LogEvent(string eventType, int floor, string status, string details)
        {
            lock (_dbLock)
            {
                try
                {
                    DateTime now   = DateTime.Now;
                    string dateStr = now.ToString("yyyy-MM-dd");
                    string timeStr = now.ToString("HH:mm:ss");
                    string desc    = details ?? string.Empty;

                    using var connection = OpenConnection();
                    using var adapter    = new SQLiteDataAdapter("SELECT * FROM ElevatorLog WHERE 1=0", connection);
                    using var builder    = new SQLiteCommandBuilder(adapter);

                    adapter.InsertCommand = builder.GetInsertCommand();

                    var dt = new DataTable("ElevatorLog");
                    adapter.Fill(dt);

                    DataRow row = dt.NewRow();
                    if (dt.Columns.Contains("LogDate"))         row["LogDate"]         = dateStr;
                    if (dt.Columns.Contains("LogTime"))         row["LogTime"]         = timeStr;
                    if (dt.Columns.Contains("ActionPerformed")) row["ActionPerformed"] = desc;
                    if (dt.Columns.Contains("Timestamp"))       row["Timestamp"]       = $"{dateStr} {timeStr}";
                    if (dt.Columns.Contains("EventType"))       row["EventType"]       = (eventType ?? "SYSTEM").ToUpperInvariant();
                    if (dt.Columns.Contains("Floor"))           row["Floor"]           = floor;
                    if (dt.Columns.Contains("Status"))          row["Status"]          = status ?? "Info";
                    if (dt.Columns.Contains("Details"))         row["Details"]         = desc;
                    dt.Rows.Add(row);

                    adapter.Update(dt);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"[ElevatorLogService] Error logging event via SQLiteDataAdapter.Update: {ex.Message}");
                }
            }
        }

        public DataTable GetLogsDataTable(string? search = null, string? filterType = null)
        {
            lock (_dbLock)
            {
                var dt = new DataTable("ElevatorLogs");
                try
                {
                    using var connection = OpenConnection();
                    var sb = new StringBuilder(
                        "SELECT rowid AS Id, " +
                        "COALESCE(NULLIF(Timestamp, ''), LogDate || ' ' || LogTime) AS Timestamp, " +
                        "EventType, Floor, Status, " +
                        "COALESCE(NULLIF(Details, ''), ActionPerformed, '') AS Details " +
                        "FROM ElevatorLog WHERE 1=1");

                    if (!string.IsNullOrWhiteSpace(filterType) && filterType != "ALL")
                        sb.Append(" AND EventType = @filterType");

                    if (!string.IsNullOrWhiteSpace(search))
                        sb.Append(" AND (Details LIKE @search OR Status LIKE @search OR Timestamp LIKE @search)");

                    sb.Append(" ORDER BY Id DESC LIMIT 300;");

                    using var cmd = new SQLiteCommand(sb.ToString(), connection);

                    if (!string.IsNullOrWhiteSpace(filterType) && filterType != "ALL")
                        cmd.Parameters.AddWithValue("@filterType", filterType.ToUpperInvariant());

                    if (!string.IsNullOrWhiteSpace(search))
                        cmd.Parameters.AddWithValue("@search", $"%{search}%");

                    using var adapter = new SQLiteDataAdapter(cmd);
                    adapter.Fill(dt);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"[ElevatorLogService] Error fetching logs via SQLiteDataAdapter: {ex.Message}");
                }

                return dt;
            }
        }

        public (int totalRecords, int totalTrips) GetStatistics()
        {
            lock (_dbLock)
            {
                try
                {
                    using var connection = OpenConnection();

                    using var cmd1 = connection.CreateCommand();
                    cmd1.CommandText = "SELECT COUNT(*) FROM ElevatorLog;";
                    int total = Convert.ToInt32(cmd1.ExecuteScalar());

                    using var cmd2 = connection.CreateCommand();
                    cmd2.CommandText = "SELECT COUNT(*) FROM ElevatorLog WHERE EventType = 'ARRIVAL';";
                    int trips = Convert.ToInt32(cmd2.ExecuteScalar());

                    return (total, trips);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"[ElevatorLogService] Error fetching stats: {ex.Message}");
                    return (0, 0);
                }
            }
        }

        public List<string> GetLogEntries()
        {
            lock (_dbLock)
            {
                var entries = new List<string>();
                try
                {
                    using var connection = OpenConnection();
                    using var adapter    = new SQLiteDataAdapter("SELECT Timestamp, EventType, Floor, Status, Details FROM ElevatorLog ORDER BY Id DESC LIMIT 100;", connection);
                    var dt = new DataTable();
                    adapter.Fill(dt);

                    foreach (DataRow row in dt.Rows)
                    {
                        string ts      = row["Timestamp"]?.ToString() ?? "";
                        string type    = row["EventType"]?.ToString() ?? "INFO";
                        int    floor   = Convert.ToInt32(row["Floor"] ?? 1);
                        string status  = row["Status"]?.ToString()    ?? "";
                        string details = row["Details"]?.ToString()   ?? "";

                        entries.Add($"{ts} - [{type}|F{floor}] {status}: {details}");
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"[ElevatorLogService] Error fetching log entries: {ex.Message}");
                }

                return entries;
            }
        }

        public void ClearLogs()
        {
            lock (_dbLock)
            {
                try
                {
                    using var connection = OpenConnection();
                    Exec(connection, "DELETE FROM ElevatorLog; VACUUM;");
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"[ElevatorLogService] Error clearing logs: {ex.Message}");
                }
            }
        }

        public bool ExportToCsv(string filePath)
        {
            lock (_dbLock)
            {
                try
                {
                    DataTable dt = GetLogsDataTable();
                    var sb = new StringBuilder();
                    sb.AppendLine("Id,Timestamp,EventType,Floor,Status,Details");

                    foreach (DataRow row in dt.Rows)
                    {
                        string id      = row["Id"]?.ToString()        ?? "";
                        string time    = row["Timestamp"]?.ToString() ?? "";
                        string type    = row["EventType"]?.ToString() ?? "";
                        string floor   = row["Floor"]?.ToString()     ?? "";

                        string status  = $"\"{row["Status"]?.ToString()?.Replace("\"", "\"\"")}\"";
                        string details = $"\"{row["Details"]?.ToString()?.Replace("\"", "\"\"")}\"";

                        sb.AppendLine($"{id},{time},{type},{floor},{status},{details}");
                    }

                    File.WriteAllText(filePath, sb.ToString(), Encoding.UTF8);
                    return true;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"[ElevatorLogService] Error exporting CSV: {ex.Message}");
                    return false;
                }
            }
        }

        public bool BackupDatabase(string? destinationPath = null)
        {
            lock (_dbLock)
            {
                try
                {
                    string backupDir = Path.Combine(AppContext.BaseDirectory, "backups");
                    Directory.CreateDirectory(backupDir);

                    string targetFile = destinationPath ?? Path.Combine(
                        backupDir,
                        $"ElevatorLog_Backup_{DateTime.Now:yyyyMMdd_HHmmss}.db");

                    using var sourceConn = OpenConnection();
                    using var destConn   = new SQLiteConnection($"Data Source={targetFile};Version=3;");
                    destConn.Open();

                    sourceConn.BackupDatabase(destConn, "main", "main", -1, null, 0);
                    return true;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"[ElevatorLogService] Backup error: {ex.Message}");
                    return false;
                }
            }
        }

        public bool RestoreDatabase(string backupFilePath)
        {
            lock (_dbLock)
            {
                try
                {
                    if (!File.Exists(backupFilePath)) return false;

                    using var sourceConn = new SQLiteConnection($"Data Source={backupFilePath};Version=3;");
                    sourceConn.Open();

                    using var destConn = OpenConnection();
                    sourceConn.BackupDatabase(destConn, "main", "main", -1, null, 0);
                    return true;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"[ElevatorLogService] Restore error: {ex.Message}");
                    return false;
                }
            }
        }

        private SQLiteConnection OpenConnection()
        {
            var conn = new SQLiteConnection($"Data Source={_databasePath};Version=3;");
            conn.Open();
            return conn;
        }

        private static void Exec(SQLiteConnection connection, string sql)
        {
            using var cmd = connection.CreateCommand();
            cmd.CommandText = sql;
            cmd.ExecuteNonQuery();
        }
    }
}
