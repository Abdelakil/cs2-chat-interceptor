# ChatCommandInterceptor

A SwiftlyS2 plugin that intercepts and blocks command-like messages in chat with multi-language support and hot-reload configuration.

## Features

### 🚀 Core Functionality
- **Multi-Prefix Support**: Block commands starting with multiple prefixes (default: `!`, `/`)
- **Whitelist System**: Allow specific commands and players to bypass restrictions
- **Permission Bypass**: Configurable permission system for admin access

### 🌍 Internationalization
- **Multi-Language Support**: Built-in translations for English, Spanish, French, German, Russian
- **Player Language Detection**: Automatic language detection based on player settings
- **Custom Messages**: Override translations with custom feedback messages

### ⚙️ Configuration
- **Hot-Reload Support**: Configuration changes apply without server restart
- **OSTORA Branding**: Professional red prefix with white messages
- **Flexible Messages**: Choose between translations or custom messages
- **Smart Detection**: Automatic translation key vs direct message handling

## Installation

1. Build the plugin: `dotnet build --configuration Release`
2. Copy the `build/net10.0` contents to your `addons/swiftlys2/plugins/ChatCommandInterceptor/` directory
3. Create the config file at `config/chat-interceptor.jsonc`
4. Restart your server or reload the plugin

## Configuration

### Basic Setup

```jsonc
{
  "Enabled": true,
  "CommandPrefixes": ["!", "/"],
  "Messages": {
    "TranslationKey": "chatinterceptor.messages.command_blocked",
    "CustomMessage": null,
    "MessageType": "Chat",
    "ShowBlockedCommand": true
  },
  "Permissions": {
    "BypassPermission": "chatcommand.bypass",
    "EnableBypass": true
  },
  "Whitelist": {
    "Commands": ["help", "rules", "discord"],
    "Players": ["AdminPlayer", "76561198000000000"]
  }
}
```

### Configuration Options

#### Main Settings
- `Enabled`: Enable/disable the plugin
- `CommandPrefixes`: **List of command prefixes to block** (default: `["!", "/"]`)

#### Message Configuration
- `TranslationKey`: Translation key for feedback (default: `"chatinterceptor.messages.command_blocked"`)
- `CustomMessage`: Override translation with custom message (default: `null`)
- `MessageType`: `Chat`, `Center`, or `Hint` (default: `"Chat"`)
- `ShowBlockedCommand`: Include blocked command in feedback (default: `true`)

#### Permission Configuration
- `BypassPermission`: Permission to bypass restrictions (default: `"chatcommand.bypass"`)
- `EnableBypass`: Enable permission bypass system (default: `true`)

#### Whitelist Configuration
- `Commands`: Commands that are never blocked (default: `[]`)
- `Players`: Players (by name or SteamID) that can use any command (default: `[]`)

## Permissions

### Bypass Permission
- `chatcommand.bypass`: Standard bypass permission

### Permission Examples
```csharp
// Grant bypass permission to a player
Core.Permission.SetPlayerPermission(steamId, "chatcommand.bypass");

// Check if player can bypass
bool canBypass = Core.Permission.PlayerHasPermission(steamId, "chatcommand.bypass");
```

## 🌍 Internationalization

### Supported Languages
- 🇺🇸 English (en)
- 🇪🇸 Spanish (es)  
- 🇫🇷 French (fr)
- 🇩🇪 German (de)
- 🇷🇺 Russian (ru)

### Translation Files
Translation files are located in `resources/translations/` and automatically loaded by SwiftlyS2:

```
resources/translations/
├── en.jsonc    # English
├── es.jsonc    # Spanish
├── fr.jsonc    # French
├── de.jsonc    # German
└── ru.jsonc    # Russian
```

### Translation Keys
- `chatinterceptor.general.prefix`: Plugin prefix
- `chatinterceptor.messages.command_blocked`: General command blocked message
- `chatinterceptor.messages.command_blocked_with_command`: Message with command name

### Custom Messages vs Translations

**Use Custom Messages for:**
- Single-language servers
- Quick customization
- Specific branding

**Use Translations for:**
- Multi-language servers
- Professional experience
- Consistent localization

## Hot Reload

The plugin supports automatic configuration reloading. When you edit the `chat-interceptor.jsonc` file:

1. Save the file
2. Plugin automatically detects changes
3. Configuration reloads without server restart
4. New settings take effect immediately

You'll see this log message: `"ChatCommandInterceptor configuration reloaded"`

## Troubleshooting

### Common Issues

1. **Commands still executing**: Check if player has bypass permissions or is whitelisted
2. **Configuration not updating**: Ensure the config file is properly formatted and saved
3. **Translations not working**: Verify translation files exist in `resources/translations/`

### Debug Logging

Enable detailed logging by checking your SwiftlyS2 logs for:
- `"ChatCommandInterceptor loaded successfully"` messages
- `"ChatCommandInterceptor configuration reloaded"` messages
- `"Blocked command from player"` messages

## Building

### Prerequisites
- .NET 10.0 SDK
- SwiftlyS2 development environment

### Build Commands
```bash
# Build release version
dotnet build --configuration Release

# Output directory: build/net10.0/
```

### Build Output
```
build/net10.0/
├── ChatCommandInterceptor.dll
├── ChatCommandInterceptor.deps.json
├── ChatCommandInterceptor.pdb
├── SwiftlyS2.CS2.dll
└── resources/
    └── translations/
        ├── en.jsonc
        ├── es.jsonc
        ├── fr.jsonc
        ├── de.jsonc
        └── ru.jsonc
```

## API Reference

### Core Classes
- `ChatInterceptorConfig`: Main configuration model
- `TranslationManager`: Handles multi-language support
- `MessageHelper`: Message formatting and display

### Hooks Used
- `OnCommandExecuteHookEvent`: Engine command interception
- `HookClientCommand`: SwiftlyS2 command interception  
- `ClientChatHookHandler`: Chat message interception

## License

This plugin follows the same license as SwiftlyS2.

## Support

For issues and support:
- Check the troubleshooting section
- Verify your configuration format
- Check SwiftlyS2 logs for error messages

## Changelog

### v1.0.0
- Initial release
- Multi-prefix command interception
- Whitelist system for commands and players
- Multi-language support with OSTORA branding
- Hot-reload configuration
- Permission bypass system
- Professional SwiftlyS2 integration
