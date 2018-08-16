using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace octavia
{
	/// <summary>
	/// Handles error messages related to incorrect command-line arguments.
	/// </summary>
	class ArgumentError
	{
		/// <summary>
		/// The argument with the error.
		/// </summary>
		public string Argument { get; set; }

		/// <summary>
		/// A description of the error.
		/// </summary>
		public string Description { get; set; }

		/// <summary>
		/// Initializes a new ArgumentError object.
		/// </summary>
		/// <param name="argument">The argument with the error.</param>
		/// <param name="description">The error's description.</param>
		public ArgumentError(string argument, string description)
		{
			Argument = argument;
			Description = description;
		}

		/// <summary>
		/// Formats an output about the error.
		/// </summary>
		/// <returns>Returns the error in a formatted way.</returns>
		public override string ToString()
		{
			return $"ERROR {Argument}: {Description}";
		}
	}

	/// <summary>
	/// Holds information about the configuration file.
	/// </summary>
	class ConfigFile
	{
		/// <summary>
		/// If true, the FileLocation and Configuration properties are not null.
		/// </summary>
		public bool HasConfig { get; set; }

		/// <summary>
		/// The location of the configuration file.
		/// </summary>
		public string FileLocation { get; set; }
		
		/// <summary>
		/// The list and order of files to be included.
		/// </summary>
		public List<string> Configuration { get; set; }

		public ConfigFile(bool hasConfig, string fileLocation = null, List<string> configuration = null)
		{
			HasConfig = hasConfig;
			FileLocation = fileLocation;
			Configuration = configuration;
		}
	}

	class OctaviaOptions
    {
		private const string SourceArgument = "-src";
		private const string DestinationArgument = "-dest";
		private const string NoRegionsArgument = "-no-regions";
		private const string ExtensionArgument = "-ext";
		private const string ConfigArgument = "-conf";
		
		public string WatchFolder { get; set; }
		public string DestinationFile { get; set; }
		public bool NoRegions { get; set; }
		public List<ArgumentError> FatalError { get; set; }
		public string Extension { get; set; }
		public ConfigFile ConfigFile { get; set; }
		public List<string> Files { get; set; }

		public OctaviaOptions()
		{
			WatchFolder = null;
			DestinationFile = null;
			NoRegions = false;
			FatalError = new List<ArgumentError>();
			Extension = "css";
			ConfigFile = new ConfigFile(false);
			Files = new List<string>();
		}

		public static OctaviaOptions GetOctaviaOptions(string[] args)
		{
			OctaviaOptions o = new OctaviaOptions();

			#region Source directory checking (critical; contains, has value, directory exists).
			// Check if the arguments contain the source keyword.
			if (args.Contains(SourceArgument))
			{
				// Check if the arguments contain something after the source keyword (its value).
				if (Array.IndexOf(args, SourceArgument) + 1 < args.Length)
				{
					// Check if the given source directory exists.
					string sourceFolder = args[Array.IndexOf(args, SourceArgument) + 1];
					if (Directory.Exists(sourceFolder))
					{
						o.WatchFolder = sourceFolder;
					}
					else
					{
						o.FatalError.Add(new ArgumentError(SourceArgument, $"The source folder \"{sourceFolder}\" does not exist."));
					}
				}
				else
				{
					o.FatalError.Add(new ArgumentError(SourceArgument, $"Missing required value."));
				}
			}
			else
			{
				o.FatalError.Add(new ArgumentError(SourceArgument, $"Missing required parameter."));
			}
			#endregion

			#region Destination file checking (critical; contains, has value, file exists).
			// Check if the arguments contain the destination keyword.
			if (args.Contains(DestinationArgument))
			{
				// Check if the arguments contain something after the destination keyword (its value).
				if (Array.IndexOf(args, DestinationArgument) + 1 < args.Length)
				{
					// Check if the given destination file exists.
					string destinationFile = args[Array.IndexOf(args, DestinationArgument) + 1];
					if (File.Exists(destinationFile))
					{
						o.DestinationFile = destinationFile;
					}
					else
					{
						o.FatalError.Add(new ArgumentError(DestinationArgument, $"The destination file \"{destinationFile}\" does not exist."));
					}
					
				}
				else
				{
					o.FatalError.Add(new ArgumentError(DestinationArgument, $"Missing required value."));
				}
			}
			else
			{
				o.FatalError.Add(new ArgumentError(DestinationArgument, $"Missing required parameter."));
			}
			#endregion

			#region No regions checking (non-critical; contains).
			// Check if the arguments contain the no-regions keyword.
			if (args.Contains(NoRegionsArgument))
			{
				o.NoRegions = true;
			}
			#endregion

			#region File extension checking (non-critical; contains, has-value).
			// Check if the arguments contain the extension keyword.
			if (args.Contains(ExtensionArgument))
			{
				// Check if the argument contain something after the extension keyword (its value).
				if (Array.IndexOf(args, ExtensionArgument) + 1 < args.Length)
				{
					o.Extension = args[Array.IndexOf(args, ExtensionArgument) + 1];
				}
				else
				{
					o.FatalError.Add(new ArgumentError(ExtensionArgument, $"Argument exists but missing required value."));
				}
			}
			#endregion

			#region Configuration file checking (non-critical; contains, has value, file exists).
			// Check if the arguments contain the extension keyword.
			if (args.Contains(ConfigArgument))
			{
				// Check if the argument contain something after the extension keyword (its value).
				if (Array.IndexOf(args, ConfigArgument) + 1 < args.Length)
				{
					// Check if the config file actually exists.
					string configFile = args[Array.IndexOf(args, ConfigArgument) + 1];
					if (File.Exists(configFile))
					{
						// Format the watch files so it is like: "src/file.css" and not just "file".
						foreach (string configLine in File.ReadAllLines(configFile))
						{
							o.Files.Add($"{o.WatchFolder}/{configLine}.{o.Extension}");
						}
					}
					else
					{
						o.FatalError.Add(new ArgumentError(ConfigArgument, $"The configuration file \"{configFile}\" does not exist."));
					}
				}
				else
				{
					o.FatalError.Add(new ArgumentError(ConfigArgument, $"Argument exists but missing required value."));
				}
			}
			else
			{
				// Here we don't need to format the watch files as they are already in the format of "src/file.css".
				o.Files = Directory.GetFiles(o.WatchFolder, $"*.{o.Extension}").ToList();
			}
			#endregion

			return o;
		}

		public static string GetHelp()
		{
			StringBuilder help = new StringBuilder();
			help.Append("\noctavia\n\n");
			help.Append("usage:\n");
			help.Append($"    {SourceArgument}: required - specify a source folder to watch.\n");
			help.Append($"    {DestinationArgument}: required - speficy a destination file to compile into.\n");
			help.Append($"    {NoRegionsArgument}: optional - prevent #region and #endregion comments.\n");
			help.Append($"    {ExtensionArgument}: optional - specify an extension to watch. Default: \"css\"\n");
			help.Append($"    {ConfigArgument}: optional - specify a configuration file to use.\n\n");
			help.Append("example:\n");
			help.Append($"    dotnet octavia.dll {SourceArgument} \"src\" {DestinationArgument} \"dist/output.css\" {NoRegionsArgument} {ExtensionArgument} \"scss\"\n");

			return help.ToString();
		}
	}
}
