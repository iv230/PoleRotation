using System;
using Dalamud.Game.Command;
using Dalamud.Interface.Windowing;
using Dalamud.IoC;
using Dalamud.Plugin;
using Dalamud.Plugin.Services;
using PoleRotation.Service;
using PoleRotation.Windows;

namespace PoleRotation;

public sealed class PoleRotation : IDalamudPlugin
{
    [PluginService] internal static IDalamudPluginInterface PluginInterface { get; private set; } = null!;
    [PluginService] internal static ICommandManager CommandManager { get; private set; } = null!;
    [PluginService] internal static IDataManager DataManager { get; set; } = null!;
    [PluginService] internal static IClientState ClientState { get; private set; } = null!;
    [PluginService] internal static IGameGui GameGui { get; set; } = null!;
    [PluginService] internal static IPluginLog Log { get; private set; } = null!;

    private const string UiCommand = "/poleposition";
    private const string UiCommandShorten = "/pp";
    private const string ConfigCommand = "/ppc";

    public readonly WindowSystem WindowSystem = new("PoleRotation");

    public Configuration.Configuration Configuration { get; init; }
    public SnappingService SnappingService { get; init; }
    public WorldOverlayService WorldOverlayService { get; init; }
    public PenumbraService PenumbraService { get; init; }
    
    private ConfigWindow ConfigWindow { get; init; }
    private MainWindow MainWindow { get; init; }
    private CreateSnappingWindow CreateSnappingWindow { get; init; }

    public PoleRotation()
    {
        Log.Info($"===Starting {PluginInterface.Manifest.Name}===");
        foreach (var sheet in DataManager.Excel.SheetNames)
            if (sheet.Contains("housing", StringComparison.OrdinalIgnoreCase))
                Log.Information($"Sheet: {sheet}");

        Configuration = PluginInterface.GetPluginConfig() as Configuration.Configuration ?? new Configuration.Configuration();

        // Services
        SnappingService = new SnappingService(Configuration);
        WorldOverlayService = new WorldOverlayService(this, SnappingService);
        PenumbraService = new PenumbraService();
        WorldOverlayService.Initialize();

        // Windows
        ConfigWindow = new ConfigWindow(Configuration);
        MainWindow = new MainWindow(this);
        CreateSnappingWindow = new CreateSnappingWindow(this, SnappingService, PenumbraService);
        
        WindowSystem.AddWindow(ConfigWindow);
        WindowSystem.AddWindow(MainWindow);
        WindowSystem.AddWindow(CreateSnappingWindow);

        // Dalamud plugin interface buttons
        PluginInterface.UiBuilder.OpenConfigUi += ToggleConfigUi;
        PluginInterface.UiBuilder.OpenMainUi += ToggleMainUi;
        PluginInterface.UiBuilder.Draw += DrawUi;

        // Commands
        CommandManager.AddHandler(UiCommand, new CommandInfo(OnUICommand)
        {
            HelpMessage = "Main plugin command"
        });
        CommandManager.AddHandler(UiCommandShorten, new CommandInfo(OnUICommand)
        {
            HelpMessage = "Shorter command"
        });
        CommandManager.AddHandler(ConfigCommand, new CommandInfo(OnConfigCommand)
        {
            HelpMessage = "Shorter command"
        });
    }

    public void Dispose()
    {
        WindowSystem.RemoveAllWindows();

        ConfigWindow.Dispose();
        MainWindow.Dispose();
        CreateSnappingWindow.Dispose();
        
        WorldOverlayService.Dispose();

        CommandManager.RemoveHandler(UiCommand);
        CommandManager.RemoveHandler(UiCommandShorten);
        CommandManager.RemoveHandler(ConfigCommand);
    }

    private void OnUICommand(string command, string args)
    {
        ToggleMainUi();
    }
    
    private void OnConfigCommand(string command, string args)
    {
        ToggleConfigUi();
    }

    private void DrawUi() => WindowSystem.Draw();

    public void ToggleConfigUi() => ConfigWindow.Toggle();
    public void ToggleMainUi() => MainWindow.Toggle();
    public void ToggleCreateUi() => CreateSnappingWindow.Toggle();

    public bool IsMainWindowOpen() => MainWindow.IsOpen;
}
