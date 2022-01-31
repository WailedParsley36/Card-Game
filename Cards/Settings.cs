using System;
using System.IO;
using System.Collections.Generic;

static class Settings
{
    public static bool CanLog = true;
    public static string LogPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + "\\StohanzlPatrik\\Card-Game\\Replays\\";
    public static string SettingsPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + "\\StohanzlPatrik\\Card-Game\\Settings.txt";

    //TODO: Save game in progress
    public static void Init()
    {
        if (!Directory.Exists(SettingsPath.TrimEnd("Settings.txt".ToCharArray())))
            Directory.CreateDirectory(SettingsPath.TrimEnd("Settings.txt".ToCharArray()));
        if (File.Exists(SettingsPath))
        {
            string[] settings = File.ReadAllLines(SettingsPath);
            foreach(string str in settings)
            {
                if (str.Contains("LogPath"))
                {
                    LogPath = str.Split('$')[1];
                }
                else if (str.Contains("Log"))
                {
                    bool success = bool.TryParse(str.Split(":")[1], out CanLog);
                    Console.WriteLine("Failed to load CanLog property!");
                }
            }
        }
        else
        {
            File.WriteAllText(SettingsPath, $"Log:{CanLog}\nLogPath${LogPath}");
        }
    }

    public static void Reset()
    {
        CanLog = true;
        LogPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + "\\StohanzlPatrik\\Card-Game\\Replays\\";
        SettingsPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + "\\StohanzlPatrik\\Card-Game\\Settings.txt";
        Console.WriteLine("Úspešně resetováno!");
        SaveSettings();
    }

    public static void SaveSettings()
    {
        File.WriteAllText(SettingsPath, $"Log:{CanLog}\nLogPath${LogPath}");
    }
    public static void LoadSettings()
    {
        if (!Directory.Exists(SettingsPath.TrimEnd("Settings.txt".ToCharArray())))
            Directory.CreateDirectory(SettingsPath.TrimEnd("Settings.txt".ToCharArray()));
        if (File.Exists(SettingsPath))
        {
            string[] settings = File.ReadAllLines(SettingsPath);
            foreach (string str in settings)
            {
                if (str.Contains("LogPath"))
                {
                    LogPath = str.Split('$')[1];
                }
                else if (str.Contains("Log"))
                {
                    bool success = bool.TryParse(str.Split(":")[1], out CanLog);
                    Console.WriteLine("Failed to load CanLog property!");
                }
            }
        }
    }

    public static void SaveLog(Player[] playerr)
    {
        if (!Directory.Exists(LogPath))
            Directory.CreateDirectory(LogPath);
        string message = "";
        string path = LogPath;
        foreach(string str in Table.messages)
        {
            message += str + "\n";
        }
        foreach (Player p in playerr)
        {
            path += p.Username + "vs";
        }
        path = path.Remove(path.Length-2);
        path += ".cGame";
        File.WriteAllText(path, message);
    }

    public static LoadedLog[] LoadLogs()
    {
        List<LoadedLog> logs = new List<LoadedLog>();
        foreach(string filePath in Directory.GetFiles(LogPath))
        {
            if (filePath.EndsWith(".cGame"))
            {
                string[] content = File.ReadAllLines(filePath);
                string[] splitted = filePath.TrimEnd(".cGame".ToCharArray()).Split('\\');
                logs.Add(new LoadedLog(splitted[splitted.Length-1], content, filePath));
            }
        }
        return logs.ToArray();
    }
}

class LoadedLog
{
    public string Name { get; private set; }
    public string[] Content { get; private set; }
    public string Path { get; private set; }

    public LoadedLog(string name, string[] content, string path)
    {
        Content = content;
        Name = name;
        Path = path;
    }

    public void OpenFolder()
    {
        System.Diagnostics.Process.Start("explorer.exe", @$"{Path}");
    }
}
