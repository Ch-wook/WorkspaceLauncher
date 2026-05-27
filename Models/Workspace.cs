using System.Collections.Generic;

namespace WorkspaceLauncher.Models
{
    public class Workspace
    {
        public string Id { get; set; } = System.Guid.NewGuid().ToString();
        public string Name { get; set; } = string.Empty;
        public List<AppItem> Items { get; set; } = new List<AppItem>();
    }
}
