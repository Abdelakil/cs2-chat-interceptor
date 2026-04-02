using Microsoft.Extensions.Logging;
using SwiftlyS2.Shared;
using SwiftlyS2.Shared.Players;
using ChatCommandInterceptor.Config;

namespace ChatCommandInterceptor.Translations;

/// <summary>
/// Translation Manager for ChatCommandInterceptor
/// Handles multi-language support with fallback system
/// </summary>
public class TranslationManager
{
    private readonly ISwiftlyCore _core;
    private readonly ILogger _logger;
    private ChatInterceptorConfig _config;

    public TranslationManager(ISwiftlyCore core, ILogger logger, ChatInterceptorConfig config)
    {
        _core = core;
        _logger = logger;
        _config = config;
        
        _logger.LogInformation("TranslationManager initialized using SwiftlyS2 built-in translation system");
    }

    /// <summary>
    /// Update configuration when config changes
    /// </summary>
    public void UpdateConfig(ChatInterceptorConfig config)
    {
        _config = config;
    }

    /// <summary>
    /// Gets a translated message for a player using SwiftlyS2's built-in translation system
    /// </summary>
    public string GetMessage(IPlayer? player, string key, params object[] args)
    {
        try
        {
            if (player == null || !player.IsValid)
            {
                // Fallback to the key itself if no player
                return GetFallbackMessage(key, args.Length > 0 ? args[0]?.ToString() ?? "" : "");
            }

            // Use SwiftlyS2's built-in translation system
            var localizer = _core.Translation.GetPlayerLocalizer(player);
            
            try
            {
                // Try to get the translated message
                var translation = args.Length > 0 ? localizer[key, args] : localizer[key];
                return translation;
            }
            catch (Exception)
            {
                // Translation not found, use fallback
                return GetFallbackMessage(key, args.Length > 0 ? args[0]?.ToString() ?? "" : "");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get translation for key: {Key}", key);
            return GetFallbackMessage(key, args.Length > 0 ? args[0]?.ToString() ?? "" : "");
        }
    }

    /// <summary>
    /// Gets the feedback message for a blocked command
    /// </summary>
    public string GetFeedbackMessage(IPlayer? player, string blockedCommand)
    {
        // Use custom message if configured (overrides translations)
        if (!string.IsNullOrEmpty(_config.Messages.CustomMessage))
        {
            return _config.Messages.CustomMessage;
        }

        // Use translated message with OSTORA prefix
        try
        {
            if (player == null || !player.IsValid)
            {
                return GetFallbackMessage(_config.Messages.TranslationKey, blockedCommand);
            }

            var localizer = _core.Translation.GetPlayerLocalizer(player);
            
            // Get prefix and message - always use command_blocked
            var prefix = localizer["chatinterceptor.general.prefix"];
            var message = localizer[_config.Messages.TranslationKey];
            
            return $"{prefix} {message}";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get translation, using fallback. TranslationKey: {Key}", 
                _config.Messages.TranslationKey);
            return GetFallbackMessage(_config.Messages.TranslationKey, blockedCommand);
        }
    }

    /// <summary>
    /// Fallback message system for when translations are not available
    /// </summary>
    private string GetFallbackMessage(string key, string blockedCommand)
    {
        var fallbacks = new Dictionary<string, string>
        {
            ["chatinterceptor.messages.command_blocked"] = "[red][OSTORA][red] [white]Access Denied! 🔒 Please use [yellow]!link [white]to connect your Discord and unlock this feature.",
            ["chatinterceptor.messages.command_blocked_with_command"] = "[red][OSTORA][red] [white] Command '{0}' is not allowed in chat."
        };

        if (fallbacks.TryGetValue(key, out var fallback))
        {
            return fallback;
        }

        return key; // Ultimate fallback
    }
}
