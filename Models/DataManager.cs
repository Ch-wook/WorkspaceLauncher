using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace WorkspaceLauncher.Models
{
    public static class DataManager
    {
        private static readonly string DataFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "WorkspaceLauncher", "workspaces.json");

        public static List<Workspace> LoadWorkspaces()
        {
            if (!File.Exists(DataFilePath))
            {
                return new List<Workspace>();
            }

            try
            {
                string json = File.ReadAllText(DataFilePath);
                return JsonSerializer.Deserialize<List<Workspace>>(json) ?? new List<Workspace>();
            }
            catch
            {
                return new List<Workspace>();
            }
        }

        public static void SaveWorkspaces(List<Workspace> workspaces)
        {
            var directory = Path.GetDirectoryName(DataFilePath);
            if (directory != null && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            var options = new JsonSerializerOptions { WriteIndented = true };
            string json = JsonSerializer.Serialize(workspaces, options);
            File.WriteAllText(DataFilePath, json);
        }
    }
}
