using System;
using System.Linq;
using System.IO;
using System.Text;
using System.Diagnostics;

namespace octavia
{
	class Program
    {
        static void Main(string[] args)
		{
			OctaviaOptions octaviaOptions = OctaviaOptions.GetOctaviaOptions(args);

			if (args.Length == 0 || args.Contains("-help"))
			{
				Console.WriteLine(OctaviaOptions.GetHelp());
				return;
			}

			if (octaviaOptions.FatalError.Count() > 0)
			{
				foreach (ArgumentError error in octaviaOptions.FatalError)
				{
					Console.WriteLine(error);
				}
				Console.WriteLine(OctaviaOptions.GetHelp());
				return;
			}

			FileSystemWatcher watcher = new FileSystemWatcher
			{
				Path = octaviaOptions.WatchFolder,
				IncludeSubdirectories = true,
				NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.DirectoryName,
				Filter = $"*.{octaviaOptions.Extension}"
			};

			watcher.Created += (sender, e) => OnWatchChange(sender, e, octaviaOptions);
			watcher.Changed += (sender, e) => OnWatchChange(sender, e, octaviaOptions);
			watcher.Renamed += (sender, e) => OnWatchChange(sender, e, octaviaOptions);
			watcher.Deleted += (sender, e) => OnWatchChange(sender, e, octaviaOptions);
			watcher.EnableRaisingEvents = true;

			Compile(octaviaOptions);

			while (Console.ReadKey().Key != ConsoleKey.Q) ;
			Console.Clear();
		}

		private static void OnWatchChange(object sender, FileSystemEventArgs e, OctaviaOptions octaviaOptions)
		{
			Compile(octaviaOptions);
		}

		private static void Compile(OctaviaOptions o)
		{
			Stopwatch stopwatch = new Stopwatch();
			Console.Clear();

			Console.WriteLine($"Watching directory {o.WatchFolder}\nPress [q] to stop.\n");

			stopwatch.Restart();

			StringBuilder compiled = new StringBuilder();
			
			bool next;
			int fileOpenedCount;

			foreach (string file in o.Files)
			{
				Console.WriteLine($"[{DateTime.Now.ToString("HH:mm:ss")}] reading: {file}");
				fileOpenedCount = 0;
				next = false;

				do
				{
					try
					{
						using (StreamReader streamReader = new StreamReader(file))
						{
							if (!o.NoRegions)
							{
								compiled.Append($"/* #region {file} */\n\n");
							}

							compiled.Append(streamReader.ReadToEnd());

							if (!o.NoRegions)
							{
								compiled.Append($"\n\n/* #endregion */\n\n");
							}

							next = true;
						}
					}
					catch (IOException)
					{
						if (fileOpenedCount == 10)
						{
							Console.WriteLine($"    {file} is locked. It will be retried on the next compilation.");
							next = true;
						}
						else
						{
							fileOpenedCount++;
						}
					}
				} while (!next);
			}

			Console.WriteLine($"\n[{DateTime.Now.ToString("HH:mm:ss")}] writing {o.DestinationFile}");
			using (StreamWriter streamWriter = new StreamWriter(o.DestinationFile))
			{
				streamWriter.Write(compiled);
			}

			stopwatch.Stop();
			Console.WriteLine($"\n    Done in: {stopwatch.ElapsedMilliseconds}ms");
		}
	}
}
