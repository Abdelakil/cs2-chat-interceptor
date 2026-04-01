# ChatCommandInterceptor Translations

This directory contains translation files for the ChatCommandInterceptor plugin.

## Supported Languages

- 🇺🇸 **English** (`en.json`) - Default language
- 🇪🇸 **Spanish** (`es.json`) - Español
- 🇫🇷 **French** (`fr.json`) - Français
- 🇩🇪 **German** (`de.json`) - Deutsch
- 🇷🇺 **Russian** (`ru.json`) - Русский
- 🇨🇳 **Chinese** (`zh.json`) - 中文

## Translation Keys

### `command_blocked`
Default message shown when a command-like message is blocked.

**English**: `[OSTORA] Command-like messages are not allowed in chat. Use console commands instead.`

### `command_blocked_with_command`
Message shown when a specific command is blocked. The `{0}` placeholder will be replaced with the actual command.

**English**: `[OSTORA] Command '{0}' is not allowed in chat. Use console commands instead.`

## Adding Custom Translations

### 1. Create a New Language File

Create a new JSON file with the language code (e.g., `ja.json` for Japanese):

```json
{
  "command_blocked": "[OSTORA] コマンドのようなメッセージはチャットで許可されていません。代わりにコンソールコマンドを使用してください。",
  "command_blocked_with_command": "[OSTORA] コマンド '{0}' はチャットで許可されていません。代わりにコンソールコマンドを使用してください。"
}
```

### 2. Update the Project

Add the new translation file to `ChatCommandInterceptor.csproj`:

```xml
<!-- Include translation files as embedded resources -->
<ItemGroup>
  <EmbeddedResource Include="translations\*.json" />
</ItemGroup>
```

### 3. Update TranslationManager

Add the new language file to the `LoadEmbeddedTranslations()` method in `TranslationManager.cs`:

```csharp
var translationFiles = new[] { 
    "en.json", "es.json", "fr.json", "de.json", "ru.json", "zh.json", "ja.json" 
};
```

### 4. Rebuild the Plugin

```bash
dotnet build --configuration Release
```

## Translation Guidelines

1. **Keep the plugin name**: Always include `[OSTORA]` in messages
2. **Maintain placeholders**: Keep `{0}` placeholders for command names
3. **Use appropriate tone**: Messages should be helpful and informative
4. **Test your translations**: Verify that placeholders work correctly

## Language Codes

Use standard ISO 639-1 language codes:

- `en` - English
- `es` - Spanish  
- `fr` - French
- `de` - German
- `ru` - Russian
- `zh` - Chinese
- `ja` - Japanese
- `pt` - Portuguese
- `it` - Italian
- `ko` - Korean
- `ar` - Arabic

## How Translations Work

1. **Embedded Resources**: Translation files are compiled into the plugin DLL
2. **Priority System**: 
   - First tries player's preferred language
   - Falls back to English
   - Finally falls back to the key itself
3. **SwiftlyS2 Integration**: Works alongside SwiftlyS2's translation system
4. **Hot Reload**: Translation changes require plugin recompilation

## Troubleshooting

### Translation Not Loading
- Check that the JSON file is valid
- Verify the file is included in the project file
- Check the build output for errors

### Missing Placeholders
- Ensure `{0}` placeholders are preserved in translations
- Test with actual commands to verify formatting

### Language Detection
- Currently defaults to English
- Player language detection can be enhanced in the future

## Contributing

To contribute new translations:

1. Fork the plugin
2. Add or update translation files
3. Test the translations
4. Submit a pull request

Thank you for helping make ChatCommandInterceptor multilingual! 🌍
