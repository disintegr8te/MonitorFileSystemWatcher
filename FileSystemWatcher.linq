//Path to the Logfile
private string logFileName = @"C:\temp\FileSystemWatcher.csv";
private static string DateFormat = "yyyy-MM-dd HH:mm:ss";
private CsvWriter csv;

public void Main()
{
//Folder/Share to Monitor
    Watch(@"C:\TestFolder");
}

[PermissionSet(SecurityAction.Demand, Name="FullTrust")]
public void Watch(string path)
{

   // Create a new FileSystemWatcher and set its properties.
   FileSystemWatcher watcher = new FileSystemWatcher();
   watcher.Path = path;
   /* Watch for changes in LastAccess and LastWrite times, and
      the renaming of files or directories. */
   watcher.NotifyFilter = /*NotifyFilters.LastAccess |*/ NotifyFilters.LastWrite
      | NotifyFilters.FileName | NotifyFilters.DirectoryName;
   watcher.InternalBufferSize = 65536;
   watcher.IncludeSubdirectories = true;
   
   // Only watch text files.
   watcher.Filter = "*.*";

   // Add event handlers.
   watcher.Changed += new FileSystemEventHandler(OnChanged);
   watcher.Created += new FileSystemEventHandler(OnCreated);
   watcher.Deleted += new FileSystemEventHandler(OnDeleted);
   watcher.Renamed += new RenamedEventHandler(OnRenamed);
   
   bool fileExists = File.Exists(logFileName);
   
   using (FileStream fs = new FileStream(logFileName, FileMode.Append, FileAccess.Write))
   using (TextWriter writer = new StreamWriter(fs))
   using (csv = new CsvWriter(writer))
   {
   		if (!fileExists)
		{
			csv.WriteHeader<LogData>();
			csv.NextRecord();
		}
   
		// Begin watching.
		watcher.EnableRaisingEvents = true;
		
		// Wait for the user to quit the program.
		Console.WriteLine(@"Press 'q' to stop.");
		while(Console.Read()!='q');
   }
}

// Define the event handlers.
private void OnChanged(object source, FileSystemEventArgs e)
{
	Console.WriteLine("{0}\tchanged\t{1}", DateTime.Now.ToString(DateFormat), e.FullPath);
	csv.WriteRecord<LogData>(new LogData {
		DateTime = DateTime.Now,
		Type = e.ChangeType.ToString(),
		FullName = e.FullPath,
		UserName = LogData.GetUserName(e.FullPath)
	});
	csv.NextRecord();
}

private void OnCreated(object source, FileSystemEventArgs e)
{
	Console.WriteLine("{0}\tcreated\t{1}", DateTime.Now.ToString(DateFormat), e.FullPath);
	csv.WriteRecord<LogData>(new LogData {
		DateTime = DateTime.Now,
		Type = e.ChangeType.ToString(),
		FullName = e.FullPath,
		UserName = LogData.GetUserName(e.FullPath)
	});
	csv.NextRecord();
}

private void OnDeleted(object source, FileSystemEventArgs e)
{
	Console.WriteLine("{0}\tdeleted\t{1}", DateTime.Now.ToString(DateFormat), e.FullPath);
	csv.WriteRecord<LogData>(new LogData {
		DateTime = DateTime.Now,
		Type = e.ChangeType.ToString(),
		FullName = e.FullPath
	});
	csv.NextRecord();
}

private void OnRenamed(object source, RenamedEventArgs e)
{
   Console.WriteLine("{0}\trenamed\t{1}\t{2}", DateTime.Now.ToString(DateFormat), e.OldFullPath, e.FullPath);
   csv.WriteRecord<LogData>(new LogData {
		DateTime = DateTime.Now,
		Type = e.ChangeType.ToString(),
		FullName = e.FullPath,
		OldFullName = e.OldFullPath,
		UserName = LogData.GetUserName(e.FullPath)
	});
	csv.NextRecord();
}

public class LogData
{
	public DateTime DateTime {get; set;}
	public string Type {get; set;}
	public string FullName {get; set;}
	public string OldFullName {get; set;}
	public string UserName {get; set;}
	
	public static string GetUserName(string fileName)
	{
        string userName = null;

        try
        {
            userName = new FileInfo(fileName)
            .GetAccessControl()
            .GetOwner(typeof(System.Security.Principal.NTAccount))
            .ToString();
        }
		catch (UnauthorizedAccessException ex)
        {
			userName = "Unauthorized";
        }
		catch (FileNotFoundException ex)
        {
            userName = "FileNotFound";
        }

        return userName;
	}
}
