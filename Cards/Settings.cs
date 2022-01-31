using System;
using System.IO;

class Settings
{
    public bool CanLog = false;
    public string LogPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
    public string SettingsPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\Settings.txt";

    public Settings()
    {
        Console.WriteLine(SettingsPath);
        if (File.Exists(SettingsPath))
        {
            string[] settings = File.ReadAllLines(SettingsPath);
        }
        else
        {
            FileStream f = new FileStream(SettingsPath, FileMode.Create);
            f.Write("");
        }

    }
}
