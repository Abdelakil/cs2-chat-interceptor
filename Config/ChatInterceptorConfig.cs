using System.Collections.Generic;

namespace ChatCommandInterceptor.Config;

public class ChatInterceptorConfig
{
    /// <summary>
    /// Whether the plugin is enabled
    /// </summary>
    public bool Enabled { get; set; } = true;

    /// <summary>
    /// Command prefixes to block (list of strings)
    /// </summary>
    public List<string> CommandPrefixes { get; set; } = new() { "!", "/" };

    /// <summary>
    /// Messages configuration
    /// </summary>
    public MessageConfig Messages { get; set; } = new();

    /// <summary>
    /// Permission configuration
    /// </summary>
    public PermissionConfig Permissions { get; set; } = new();

    /// <summary>
    /// Whitelist configuration
    /// </summary>
    public WhitelistConfig Whitelist { get; set; } = new();
}

public class MessageConfig
{
    /// <summary>
    /// Translation key for the feedback message (from resources/translations/)
    /// </summary>
    public string TranslationKey { get; set; } = "chatinterceptor.messages.command_blocked";

    /// <summary>
    /// Custom feedback message that overrides translations (set to null to use translations)
    /// </summary>
    public string? CustomMessage { get; set; } = null;

    /// <summary>
    /// Message type for feedback
    /// </summary>
    public string MessageType { get; set; } = "Chat";
}

public class PermissionConfig
{
    /// <summary>
    /// Permission required to bypass the interceptor
    /// </summary>
    public string BypassPermission { get; set; } = "ostora.discord.linked";

    /// <summary>
    /// Whether to enable permission bypass
    /// </summary>
    public bool EnableBypass { get; set; } = true;
}

public class WhitelistConfig
{
    /// <summary>
    /// Commands that are always allowed (never blocked)
    /// </summary>
    public List<string> Commands { get; set; } = new() { "!admin", "!link" };

    /// <summary>
    /// Players that are always allowed to use commands
    /// </summary>
    public List<string> Players { get; set; } = new();
}
