using System;
using Dalamud.Game.Command;
using Dalamud.IoC;
using Dalamud.Plugin;
using System.IO;
using Dalamud.Interface.Windowing;
using Dalamud.Plugin.Services;
using PoleRotation.Service;
using PoleRotation.Windows;

namespace PoleRotation;

public sealed class PoleRotation : IDalamudPlugin
{
    [PluginService] internal static IDalamudPluginInterface PluginInterface { get; private set; } = null!;
    [PluginService] internal static ITextureProvider TextureProvider { get; private set; } = null!;
    [PluginService] internal static ICommandManager CommandManager { get; private set; } = null!;
    [PluginService] internal static IClientState ClientState { get; private set; } = null!;
    [PluginService] internal static IDataManager DataManager { get; private set; } = null!;
    [PluginService] internal static IFramework Framework { get; set; } = null!;
    [PluginService] internal static IGameGui GameGui { get; set; } = null!;

    [PluginService] internal static IPluginLog Log { get; private set; } = null!;

    // private const string CreateCommandName = "/pr";
    private const string UiCommand = "/prw";

    public Configuration Configuration { get; init; }
    
    public SnappingService SnappingService { get; init; }
    
    public WorldOverlayService WorldOverlayService { get; init; }

    public readonly WindowSystem WindowSystem = new("PoleRotation");
    private ConfigWindow ConfigWindow { get; init; }
    private MainWindow MainWindow { get; init; }
    private CreateSnappingWindow CreateSnappingWindow { get; init; }

    public PoleRotation()
    {
        Configuration = PluginInterface.GetPluginConfig() as Configuration ?? new Configuration();

        SnappingService = new SnappingService(Configuration);
        WorldOverlayService = new WorldOverlayService(this, SnappingService);
        WorldOverlayService.Initialize();

        ConfigWindow = new ConfigWindow(this);
        MainWindow = new MainWindow(this);
        CreateSnappingWindow = new CreateSnappingWindow(this, SnappingService);

        WindowSystem.AddWindow(ConfigWindow);
        WindowSystem.AddWindow(MainWindow);
        WindowSystem.AddWindow(CreateSnappingWindow);

        // CommandManager.AddHandler(CreateCommandName, new CommandInfo(OnCreateCommand)
        // {
        //     HelpMessage = "A useful message to display in /xlhelp"
        // });
        CommandManager.AddHandler(UiCommand, new CommandInfo(OnUICommand)
        {
            HelpMessage = "A useful message to display in /xlhelp"
        });

        PluginInterface.UiBuilder.Draw += DrawUi;

        // This adds a button to the plugin installer entry of this plugin which allows
        // to toggle the display status of the configuration ui
        PluginInterface.UiBuilder.OpenConfigUi += ToggleConfigUi;

        // Adds another button that is doing the same but for the main ui of the plugin
        PluginInterface.UiBuilder.OpenMainUi += ToggleMainUi;

        // Add a simple message to the log with level set to information
        // Use /xllog to open the log window in-game
        Log.Information($"===Starting {PluginInterface.Manifest.Name}===");
    }

    public void Dispose()
    {
        WindowSystem.RemoveAllWindows();

        ConfigWindow.Dispose();
        MainWindow.Dispose();
        
        WorldOverlayService.Dispose();

        // CommandManager.RemoveHandler(CreateCommandName);
    }

    // private void OnCreateCommand(string command, string args)
    // {
    //     Log.Debug("Calling create command");
    //
    //     if (args == string.Empty)
    //     {
    //         Log.Warning("Must provide a name as first argument");
    //         return;
    //     }
    //     
    //     SnappingService.NewSnapping(args);
    // }

    private void OnUICommand(string command, string args)
    {
        Log.Debug("Calling UICommand");
        ToggleMainUi();
    }

    private void DrawUi() => WindowSystem.Draw();

    public void ToggleConfigUi() => ConfigWindow.Toggle();
    public void ToggleMainUi() => MainWindow.Toggle();
    public void ToggleCreateUi() => CreateSnappingWindow.Toggle();

    public bool IsMainWindowOpen() => MainWindow.IsOpen;
}
