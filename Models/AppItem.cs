using System;
using System.Text.Json.Serialization;
using System.Windows.Media;

namespace WorkspaceLauncher.Models
{
    public enum AppItemType
    {
        Program,
        Website
    }

    public class AppItem
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Name { get; set; } = string.Empty;
        public string Path { get; set; } = string.Empty;
        public string Arguments { get; set; } = string.Empty;

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public AppItemType ItemType { get; set; } = AppItemType.Program;

        // 런타임 전용 속성 (JSON 직렬화에서 제외)
        [JsonIgnore]
        public ImageSource? IconSource { get; set; }

        [JsonIgnore]
        public string IconEmoji => ItemType == AppItemType.Website ? "🌐" : "🖥";

        [JsonIgnore]
        public string ItemTypeDisplay => ItemType == AppItemType.Program ? "프로그램" : "웹사이트";

        /// <summary>
        /// 파일 경로로부터 아이콘을 추출하여 IconSource에 캐싱합니다.
        /// </summary>
        public void LoadIcon()
        {
            if (ItemType == AppItemType.Program && !string.IsNullOrEmpty(Path))
            {
                try
                {
                    IconSource = IconHelper.GetIconFromPath(Path);
                }
                catch { }
            }
        }
    }
}
