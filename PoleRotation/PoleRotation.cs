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

    private const string CreateCommandName = "/pr";
    private const string ReadCommandName = "/prr";
    private const string UICommand = "/prw";

    public Configuration Configuration { get; init; }
    
    public SnappingService SnappingService { get; init; }
    
    public WorldOverlayService WorldOverlayService { get; init; }

    public readonly WindowSystem WindowSystem = new("PoleRotation");
    private ConfigWindow ConfigWindow { get; init; }
    private MainWindow MainWindow { get; init; }

    public PoleRotation()
    {
        Configuration = PluginInterface.GetPluginConfig() as Configuration ?? new Configuration();

        SnappingService = new SnappingService(Configuration);
        WorldOverlayService = new WorldOverlayService(SnappingService);
        WorldOverlayService.Initialize();

        ConfigWindow = new ConfigWindow(this);
        MainWindow = new MainWindow(this);

        WindowSystem.AddWindow(ConfigWindow);
        WindowSystem.AddWindow(MainWindow);

        CommandManager.AddHandler(CreateCommandName, new CommandInfo(OnCreateCommand)
        {
            HelpMessage = "A useful message to display in /xlhelp"
        });
        CommandManager.AddHandler(ReadCommandName, new CommandInfo(OnReadCommand)
        {
            HelpMessage = "A useful message to display in /xlhelp"
        });
        CommandManager.AddHandler(UICommand, new CommandInfo(OnUICommand)
        {
            HelpMessage = "A useful message to display in /xlhelp"
        });

        PluginInterface.UiBuilder.Draw += DrawUI;

        // This adds a button to the plugin installer entry of this plugin which allows
        // to toggle the display status of the configuration ui
        PluginInterface.UiBuilder.OpenConfigUi += ToggleConfigUI;

        // Adds another button that is doing the same but for the main ui of the plugin
        PluginInterface.UiBuilder.OpenMainUi += ToggleMainUI;

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

        CommandManager.RemoveHandler(CreateCommandName);
    }

    private void OnCreateCommand(string command, string args)
    {
        Log.Debug("Calling create command");

        if (args == string.Empty)
        {
            Log.Warning("Must provide a name as first argument");
            return;
        }
        
        SnappingService.NewSnapping(args);
    }
    
    private void OnReadCommand(string command, string args)
    {
        Log.Debug("Calling read command");
        Log.Info(Configuration.Snappings.Count.ToString() ?? "Pas de config lol");
    }
    
    private void OnUICommand(string command, string args)
    {
        Log.Debug("Calling UICommand");
        ToggleMainUI();
    }

    private void DrawUI() => WindowSystem.Draw();

    public void ToggleConfigUI() => ConfigWindow.Toggle();
    public void ToggleMainUI() => MainWindow.Toggle();
}
