using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SwiftlyS2.Shared;
using SwiftlyS2.Shared.Plugins;
using ChatCommandInterceptor.Config;
using ChatCommandInterceptor.Translations;

namespace ChatCommandInterceptor;

[PluginMetadata(
    Id = "ChatCommandInterceptor",
    Name = "Chat Command Interceptor",
    Author = "SwiftlyS2",
    Version = "1.0.0",
    Description = "Intercepts chat messages that start with command prefixes and blocks them from execution with full configuration and translation support.",
    Website = "https://github.com/swiftly-solution/swiftlys2"
)]
public partial class ChatCommandInterceptor(ISwiftlyCore core) : BasePlugin(core)
{
    private TranslationManager? _translationManager;
}
