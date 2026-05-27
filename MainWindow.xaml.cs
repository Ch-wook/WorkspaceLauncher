using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using WorkspaceLauncher.Models;

namespace WorkspaceLauncher
{
    public partial class MainWindow : Window
    {
        private List<Workspace> _workspaces = new List<Workspace>();

        public MainWindow()
        {
            InitializeComponent();
            LoadData();
        }

        // ==================== 데이터 로드/저장 ====================

        private void LoadData()
        {
            _workspaces = DataManager.LoadWorkspaces();

            // 모든 프로그램 항목의 아이콘 로드
            foreach (var ws in _workspaces)
            {
                foreach (var item in ws.Items)
                {
                    item.LoadIcon();
                }
            }

            lstWorkspaces.ItemsSource = _workspaces;
        }

        private void SaveData()
        {
            DataManager.SaveWorkspaces(_workspaces);
        }

        private void RefreshAppItemsList()
        {
            if (lstWorkspaces.SelectedItem is Workspace selected)
            {
                lstAppItems.ItemsSource = null;
                lstAppItems.ItemsSource = selected.Items;
                UpdateStatus();
            }
        }

        private void UpdateStatus()
        {
            if (lstWorkspaces.SelectedItem is Workspace selected)
            {
                int total = selected.Items.Count;
                int programs = selected.Items.Count(i => i.ItemType == AppItemType.Program);
                int websites = selected.Items.Count(i => i.ItemType == AppItemType.Website);
                txtItemCount.Text = $"총 {total}개 항목 (프로그램: {programs}, 웹사이트: {websites})";
                txtLaunchButton.Text = total > 0 ? $"일괄 실행 ({total}개 항목)" : "일괄 실행";
            }
            else
            {
                txtItemCount.Text = "";
                txtLaunchButton.Text = "일괄 실행";
            }
        }

        // ==================== 커스텀 타이틀바 ====================

        private void btnMinimize_Click(object sender, RoutedEventArgs e)
            => WindowState = WindowState.Minimized;

        private void btnMaximize_Click(object sender, RoutedEventArgs e)
            => WindowState = WindowState == WindowState.Maximized
                ? WindowState.Normal : WindowState.Maximized;

        private void btnClose_Click(object sender, RoutedEventArgs e)
            => Close();

        // ==================== 작업 공간 관리 ====================

        private void lstWorkspaces_SelectionChanged(object sender,
            System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (lstWorkspaces.SelectedItem is Workspace selected)
            {
                txtSelectedWorkspace.Text = $"📂 {selected.Name}";
                lstAppItems.ItemsSource = selected.Items;
                UpdateStatus();
            }
            else
            {
                txtSelectedWorkspace.Text = "작업 공간을 선택하세요";
                lstAppItems.ItemsSource = null;
                UpdateStatus();
            }
        }

        /// <summary>
        /// 작업 공간 더블클릭 → 이름 변경
        /// </summary>
        private void lstWorkspaces_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (lstWorkspaces.SelectedItem is Workspace selected)
            {
                var inputWindow = new InputWindow("작업 공간 이름 변경:", selected.Name);
                inputWindow.Owner = this;
                if (inputWindow.ShowDialog() == true && !string.IsNullOrWhiteSpace(inputWindow.InputText))
                {
                    selected.Name = inputWindow.InputText;
                    lstWorkspaces.Items.Refresh();
                    txtSelectedWorkspace.Text = $"📂 {selected.Name}";
                    SaveData();
                    txtStatus.Text = $"✏️ '{selected.Name}'(으)로 이름이 변경되었습니다.";
                }
            }
        }

        private void btnAddWorkspace_Click(object sender, RoutedEventArgs e)
        {
            var inputWindow = new InputWindow("새 작업 공간의 이름을 입력하세요:", "새 작업 공간");
            inputWindow.Owner = this;
            if (inputWindow.ShowDialog() == true)
            {
                string newName = inputWindow.InputText;
                if (!string.IsNullOrWhiteSpace(newName))
                {
                    _workspaces.Add(new Workspace { Name = newName });
                    lstWorkspaces.Items.Refresh();
                    lstWorkspaces.SelectedIndex = _workspaces.Count - 1;
                    SaveData();
                    txtStatus.Text = $"✅ '{newName}' 작업 공간이 추가되었습니다.";
                }
            }
        }

        private void btnRemoveWorkspace_Click(object sender, RoutedEventArgs e)
        {
            if (lstWorkspaces.SelectedItem is Workspace selected)
            {
                if (MessageBox.Show(
                    $"'{selected.Name}' 작업 공간을 삭제하시겠습니까?\n포함된 {selected.Items.Count}개 항목도 함께 삭제됩니다.",
                    "삭제 확인", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {
                    _workspaces.Remove(selected);
                    lstWorkspaces.Items.Refresh();
                    SaveData();
                    txtStatus.Text = "🗑️ 작업 공간이 삭제되었습니다.";
                }
            }
        }

        // ==================== 프로그램 추가 (파일 선택 다이얼로그) ====================

        private void btnAddProgram_Click(object sender, RoutedEventArgs e)
        {
            if (lstWorkspaces.SelectedItem is not Workspace selected)
            {
                MessageBox.Show("먼저 좌측에서 작업 공간을 선택하세요.",
                    "알림", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var dialog = new Microsoft.Win32.OpenFileDialog
            {
                Title = "실행할 프로그램 또는 바로가기를 선택하세요",
                Filter = "실행 파일 (*.exe)|*.exe|바로가기 (*.lnk)|*.lnk|배치 파일 (*.bat;*.cmd)|*.bat;*.cmd|모든 파일 (*.*)|*.*",
                Multiselect = true
            };

            if (dialog.ShowDialog() == true)
            {
                foreach (var filePath in dialog.FileNames)
                {
                    string name = System.IO.Path.GetFileNameWithoutExtension(filePath);
                    var newItem = new AppItem
                    {
                        Name = name,
                        Path = filePath,
                        ItemType = AppItemType.Program
                    };
                    newItem.LoadIcon();
                    selected.Items.Add(newItem);
                }
                RefreshAppItemsList();
                SaveData();
                txtStatus.Text = $"✅ {dialog.FileNames.Length}개 프로그램이 추가되었습니다.";
            }
        }

        // ==================== 웹사이트 추가 (URL 입력) ====================

        private void btnAddWebsite_Click(object sender, RoutedEventArgs e)
        {
            if (lstWorkspaces.SelectedItem is not Workspace selected)
            {
                MessageBox.Show("먼저 좌측에서 작업 공간을 선택하세요.",
                    "알림", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var inputName = new InputWindow("웹사이트 이름을 입력하세요:", "새 웹사이트");
            inputName.Owner = this;
            if (inputName.ShowDialog() == true && !string.IsNullOrWhiteSpace(inputName.InputText))
            {
                var inputUrl = new InputWindow("웹사이트 URL을 입력하세요:", "https://");
                inputUrl.Owner = this;
                if (inputUrl.ShowDialog() == true && !string.IsNullOrWhiteSpace(inputUrl.InputText))
                {
                    string url = inputUrl.InputText.Trim();
                    // http:// 또는 https://가 없으면 자동 추가
                    if (!url.StartsWith("http://") && !url.StartsWith("https://"))
                        url = "https://" + url;

                    selected.Items.Add(new AppItem
                    {
                        Name = inputName.InputText,
                        Path = url,
                        ItemType = AppItemType.Website
                    });
                    RefreshAppItemsList();
                    SaveData();
                    txtStatus.Text = $"🌐 '{inputName.InputText}' 웹사이트가 추가되었습니다.";
                }
            }
        }

        // ==================== 항목 삭제 ====================

        private void btnRemoveApp_Click(object sender, RoutedEventArgs e)
        {
            if (lstWorkspaces.SelectedItem is Workspace selected
                && lstAppItems.SelectedItem is AppItem selectedApp)
            {
                selected.Items.Remove(selectedApp);
                RefreshAppItemsList();
                SaveData();
                txtStatus.Text = $"🗑️ '{selectedApp.Name}' 항목이 삭제되었습니다.";
            }
        }

        // ==================== 드래그 앤 드롭 ====================

        private void DropZone_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effects = DragDropEffects.Copy;
                dropZone.BorderBrush = (SolidColorBrush)FindResource("AccentPrimary");
                dropZone.Background = new SolidColorBrush(Color.FromRgb(0x28, 0x29, 0x55));
            }
            else
            {
                e.Effects = DragDropEffects.None;
            }
            e.Handled = true;
        }

        private void DropZone_DragLeave(object sender, DragEventArgs e)
        {
            dropZone.BorderBrush = (SolidColorBrush)FindResource("DropZoneBorder");
            dropZone.Background = (SolidColorBrush)FindResource("DropZoneBg");
        }

        private void DropZone_Drop(object sender, DragEventArgs e)
        {
            // 드롭 영역 색상 복원
            dropZone.BorderBrush = (SolidColorBrush)FindResource("DropZoneBorder");
            dropZone.Background = (SolidColorBrush)FindResource("DropZoneBg");

            if (lstWorkspaces.SelectedItem is not Workspace selected)
            {
                MessageBox.Show("먼저 좌측에서 작업 공간을 선택하세요.",
                    "알림", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                int addedCount = 0;

                foreach (var filePath in files)
                {
                    string ext = System.IO.Path.GetExtension(filePath).ToLower();
                    // 실행 파일, 바로가기, 배치 파일, 관리 콘솔 등 지원
                    if (ext is ".exe" or ".lnk" or ".bat" or ".cmd" or ".msc" or ".appref-ms")
                    {
                        string name = System.IO.Path.GetFileNameWithoutExtension(filePath);
                        var newItem = new AppItem
                        {
                            Name = name,
                            Path = filePath,
                            ItemType = AppItemType.Program
                        };
                        newItem.LoadIcon();
                        selected.Items.Add(newItem);
                        addedCount++;
                    }
                }

                if (addedCount > 0)
                {
                    RefreshAppItemsList();
                    SaveData();
                    txtStatus.Text = $"📂 {addedCount}개 프로그램이 드래그 앤 드롭으로 추가되었습니다.";
                }
                else
                {
                    MessageBox.Show(
                        "지원되는 파일 형식이 아닙니다.\n(.exe, .lnk, .bat, .cmd 파일을 드래그하세요)",
                        "알림", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
        }

        // ==================== 항목 더블클릭 → 개별 실행 ====================

        private void lstAppItems_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (lstAppItems.SelectedItem is AppItem item)
            {
                LaunchSingleItem(item);
            }
        }

        private void LaunchSingleItem(AppItem item)
        {
            try
            {
                ProcessStartInfo psi = new ProcessStartInfo
                {
                    FileName = item.Path,
                    Arguments = item.Arguments,
                    UseShellExecute = true
                };
                Process.Start(psi);
                txtStatus.Text = $"▶️ '{item.Name}' 실행됨";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"'{item.Name}' 실행 실패:\n{ex.Message}",
                    "오류", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // ==================== 일괄 실행 ====================

        private void btnLaunch_Click(object sender, RoutedEventArgs e)
        {
            if (lstWorkspaces.SelectedItem is not Workspace selected)
            {
                MessageBox.Show("실행할 작업 공간을 선택하세요.",
                    "알림", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            if (selected.Items.Count == 0)
            {
                MessageBox.Show("실행할 항목이 없습니다.",
                    "알림", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            int successCount = 0;
            int failCount = 0;

            foreach (var item in selected.Items)
            {
                try
                {
                    ProcessStartInfo psi = new ProcessStartInfo
                    {
                        FileName = item.Path,
                        Arguments = item.Arguments,
                        UseShellExecute = true
                    };
                    Process.Start(psi);
                    successCount++;
                }
                catch
                {
                    failCount++;
                }
            }

            string result = $"✅ {successCount}개 항목 실행 완료";
            if (failCount > 0)
                result += $" | ❌ {failCount}개 실패";

            txtStatus.Text = result;
        }
    }
}