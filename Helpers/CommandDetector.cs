using System;

namespace ChatCommandInterceptor.Helpers;

public static class CommandDetector
{
    public static bool IsCommandMessage(ReadOnlySpan<char> message, string commandPrefix)
    {
        if (message.IsEmpty || message.IsWhiteSpace())
            return false;

        if (message.Length < commandPrefix.Length + 1)
            return false;

        var prefixSpan = commandPrefix.AsSpan();
        if (!message.StartsWith(prefixSpan, StringComparison.OrdinalIgnoreCase))
            return false;

        var afterPrefix = message[commandPrefix.Length..];
        return !afterPrefix.IsEmpty && !afterPrefix.IsWhiteSpace();
    }

    public static bool IsCommandMessage(string message, string commandPrefix)
    {
        return IsCommandMessage(message.AsSpan(), commandPrefix);
    }
}
