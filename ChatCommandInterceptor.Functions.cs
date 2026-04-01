using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SwiftlyS2.Shared;
using SwiftlyS2.Shared.Commands;
using SwiftlyS2.Shared.Misc;
using SwiftlyS2.Shared.Players;
using SwiftlyS2.Shared.Events;
using SwiftlyS2.Shared.Natives;
using SwiftlyS2.Shared.Permissions;
using ChatCommandInterceptor.Config;
using ChatCommandInterceptor.Helpers;
using ChatCommandInterceptor.Translations;

namespace ChatCommandInterceptor;

public partial class ChatCommandInterceptor
{
    private Guid? _clientCommandHook;
    private IOptionsMonitor<ChatInterceptorConfig>? _configMonitor;
    private ChatInterceptorConfig? _config;

    public override void Load(bool hotReload)
    {
        // Initialize configuration file with SwiftlyS2's configuration system
        _configMonitor = BuildConfigService<ChatInterceptorConfig>("chat-interceptor.jsonc", "ChatCommandInterceptor");
        _config = _configMonitor.CurrentValue;
        
        // Subscribe to config changes for hot reload
        _configMonitor!.OnChange((newConfig, name) =>
        {
            _config = newConfig;
            _translationManager?.UpdateConfig(newConfig);
            Core.Logger.LogInformation("ChatCommandInterceptor configuration reloaded");
        });

        // Initialize translation manager
        _translationManager = new TranslationManager(Core, Core.Logger, _config!);
        
        // Subscribe to command execution events (engine commands)
        Core.Event.OnCommandExecuteHook += OnCommandExecute;
        
        // Hook client commands (SwiftlyS2 registered commands)
        _clientCommandHook = Core.Command.HookClientCommand(OnClientCommand);
        
        Core.Logger.LogInformation("ChatCommandInterceptor loaded successfully with command prefixes: {CommandPrefixes}", string.Join(", ", _config!.CommandPrefixes));
    }

    private IOptionsMonitor<T> BuildConfigService<T>(string fileName, string sectionName) where T : class, new()
    {
        Core.Configuration
            .InitializeJsonWithModel<T>(fileName, sectionName)
            .Configure(cfg => cfg.AddJsonFile(Core.Configuration.GetConfigPath(fileName), optional: false, reloadOnChange: true));

        ServiceCollection services = new();
        services.AddSwiftly(Core)
            .AddOptions<T>()
            .BindConfiguration(sectionName);

        var provider = services.BuildServiceProvider();
        return provider.GetRequiredService<IOptionsMonitor<T>>();
    }

    public override void Unload()
    {
        Core.Event.OnCommandExecuteHook -= OnCommandExecute;
        if (_clientCommandHook.HasValue)
        {
            Core.Command.UnhookClientCommand(_clientCommandHook.Value);
        }
        
        Core.Logger.LogInformation("ChatCommandInterceptor unloaded.");
    }

    private HookResult OnClientCommand(int playerId, string commandLine)
    {
        if (!_config.Enabled)
            return HookResult.Continue;
        
        var player = Core.PlayerManager.GetPlayer(playerId);
        
        // Check permission bypass
        if (_config.Permissions.EnableBypass && HasBypassPermission(player))
        {
            Core.Logger.LogDebug("Player {PlayerName} bypassed command interception due to permissions", player?.Name);
            return HookResult.Continue;
        }

        // Check if this is a say command with command-like content
        if (commandLine.StartsWith("say ") || commandLine.StartsWith("say_team "))
        {
            var spaceIndex = commandLine.IndexOf(' ');
            if (spaceIndex >= 0 && spaceIndex < commandLine.Length - 1)
            {
                var message = commandLine[(spaceIndex + 1)..].Trim('"');
                if (ShouldBlockMessage(message, player))
                {
                    Core.Logger.LogInformation("Blocked say command from player {PlayerId}: {Command}", playerId, commandLine);
                    
                    if (player != null && player.IsValid)
                    {
                        HandleBlockedCommand(player, message);
                    }
                    
                    return HookResult.Stop;
                }
            }
        }

        // Also block direct commands starting with our prefixes
        if (ShouldBlockMessage(commandLine, player))
        {
            Core.Logger.LogInformation("Blocked direct command from player {PlayerId}: {Command}", playerId, commandLine);
            
            if (player != null && player.IsValid)
            {
                HandleBlockedCommand(player, commandLine);
            }
            return HookResult.Stop;
        }

        return HookResult.Continue;
    }

    private void OnCommandExecute(IOnCommandExecuteHookEvent @event)
    {
        if (!_config.Enabled || @event.HookMode != HookMode.Pre)
            return;

        var command = @event.Command.GetCommandString;
        if (string.IsNullOrEmpty(command))
            return;

        // Check if this is a chat command that contains our prefixes
        if (command.StartsWith("say ") || command.StartsWith("say_team "))
        {
            var spaceIndex = command.IndexOf(' ');
            if (spaceIndex >= 0 && spaceIndex < command.Length - 1)
            {
                var message = command[(spaceIndex + 1)..].Trim('"');
                if (ShouldBlockMessage(message, null))
                {
                    Core.Logger.LogInformation("Blocked chat command execution: {Command}", command);
                    
                    // Block the command execution
                    @event.Result = HookResult.Stop;
                    return;
                }
            }
        }
    }

    [ClientChatHookHandler]
    public HookResult OnClientChat(int playerId, string text, bool teamOnly)
    {
        if (!_config.Enabled)
            return HookResult.Continue;

        var player = Core.PlayerManager.GetPlayer(playerId);
        if (player == null || !player.IsValid)
            return HookResult.Continue;

        // Check permission bypass
        if (_config.Permissions.EnableBypass && HasBypassPermission(player))
        {
            Core.Logger.LogDebug("Player {PlayerName} bypassed chat interception due to permissions", player.Name);
            return HookResult.Continue;
        }

        if (string.IsNullOrWhiteSpace(text))
            return HookResult.Continue;

        if (ShouldBlockMessage(text, player))
        {
            Core.Logger.LogInformation("Blocked chat message from player {PlayerName}: {Text}", player.Name, text);
            
            HandleBlockedCommand(player, text);
            return HookResult.Stop;
        }

        return HookResult.Continue;
    }

    /// <summary>
    /// Checks if a message should be blocked based on configuration
    /// </summary>
    private bool ShouldBlockMessage(string message, IPlayer? player)
    {
        if (string.IsNullOrWhiteSpace(message))
            return false;

        // Check whitelist commands first
        foreach (var whitelisted in _config.Whitelist.Commands)
        {
            if (message.Equals(whitelisted, StringComparison.OrdinalIgnoreCase))
                return false;
        }

        // Check whitelist players
        if (player != null && player.IsValid)
        {
            foreach (var whitelistedPlayer in _config.Whitelist.Players)
            {
                if (player.Name.Equals(whitelistedPlayer, StringComparison.OrdinalIgnoreCase) ||
                    player.SteamID.ToString().Equals(whitelistedPlayer, StringComparison.OrdinalIgnoreCase))
                    return false;
            }
        }

        // Check command prefixes (case-insensitive)
        foreach (var prefix in _config.CommandPrefixes)
        {
            if (message.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
                return true;
        }

        return false;
    }

    /// <summary>
    /// Checks if a player has bypass permission
    /// </summary>
    private bool HasBypassPermission(IPlayer? player)
    {
        if (player == null || !player.IsValid)
            return false;

        try
        {
            // Check specific bypass permission
            if (!string.IsNullOrEmpty(_config.Permissions.BypassPermission) &&
                Core.Permission.PlayerHasPermission(player.SteamID, _config.Permissions.BypassPermission))
                return true;

            return false;
        }
        catch (Exception ex)
        {
            Core.Logger.LogError(ex, "Error checking bypass permission for player {PlayerId}", player.SteamID);
            return false;
        }
    }

    private void HandleBlockedCommand(IPlayer player, string originalMessage)
    {
        if (_translationManager == null)
            return;

        try
        {
            var feedbackMessage = _translationManager.GetFeedbackMessage(player, originalMessage);
            
            // Parse message type
            if (Enum.TryParse<MessageType>(_config.Messages.MessageType, out var messageType))
            {
                player.SendMessage(messageType, feedbackMessage);
            }
            else
            {
                player.SendMessage(MessageType.Chat, feedbackMessage);
            }
        }
        catch (Exception ex)
        {
            Core.Logger.LogError(ex, "Error sending feedback message to player {PlayerId}", player.SteamID);
        }
    }
}
