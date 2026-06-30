# Explorer Cleaner

一款 Windows 系统托盘工具，一键清理文件资源管理器缓存，解决"打开文件夹后资源管理器未响应"的问题。

A Windows system tray utility that cleans File Explorer caches with one click, fixing the "File Explorer not responding" issue.

## 截图 / Screenshots

> 待补充 / TODO

## 功能 / Features

- **6 项清理**：快速访问历史、缩略图缓存、图标缓存、资源管理器历史、DNS 缓存、临时文件
- **系统托盘**：开机自启，常驻右下角，不占任务栏
- **定时自动清理**：可配置 15 分钟 / 30 分钟 / 1 小时 / 2 小时间隔
- **按需选择**：可自由勾选要清理的项目
- **轻量**：单文件 185KB，内存占用 < 20MB
- **无需管理员权限**（部分系统临时文件可能跳过）

## 下载 / Download

从 [Releases](../../releases) 页面下载最新的 `ExplorerCleaner.exe`，双击运行即可。

## 使用说明 / Usage

- **右键托盘图标** → 立即清理 / 设置 / 退出
- **双击托盘图标** → 立即清理
- **设置窗口** → 勾选清理项，调整自动清理间隔，开关开机自启

## 清理项明细

| 清理项 | 说明 | 需要重启资源管理器 |
|--------|------|:---:|
| 快速访问历史 | 清除跳转列表、最近文件/文件夹记录 | 否 |
| 缩略图缓存 | 删除 `thumbcache_*.db` | 是 |
| 图标缓存 | 删除 `iconcache_*.db` | 是 |
| 资源管理器历史 | 清除地址栏历史和运行记录（注册表） | 否 |
| DNS 缓存 | `ipconfig /flushdns` | 否 |
| 临时文件 | 清理 `%Temp%` 和 `Windows\Temp` | 否 |

## 技术栈 / Tech Stack

- C# 12 / .NET 8
- Windows Forms
- System.Text.Json

## 本地构建 / Build

```bash
# 需要 .NET 8 SDK
git clone https://github.com/<your-username>/explorer-cleaner.git
cd explorer-cleaner/ExplorerCleaner
dotnet build -c Release
dotnet publish -c Release -o ../publish
```

## 项目结构 / Project Structure

```
ExplorerCleaner/
├── Program.cs                  # 入口
├── TrayApplication.cs          # 托盘管理
├── Models/
│   ├── CleanResult.cs          # 清理结果
│   └── Settings.cs             # 配置（JSON 读写）
├── Services/
│   ├── CleanerService.cs       # 清理调度引擎
│   ├── StartupManager.cs       # 开机自启
│   └── Cleaners/
│       ├── ICleaner.cs
│       ├── QuickAccessCleaner.cs
│       ├── ThumbnailCacheCleaner.cs
│       ├── IconCacheCleaner.cs
│       ├── ExplorerHistoryCleaner.cs
│       ├── DnsCacheCleaner.cs
│       └── TempFilesCleaner.cs
├── Forms/
│   ├── SettingsForm.cs         # 设置窗口
│   └── ProgressForm.cs         # 进度窗口
└── Resources/
    └── app.ico
```

## 许可 / License

MIT
