using System.Collections.Generic;

public static class Debugger
{
    public static string WholeTextInfo => GetWholeTextInfo();

    private static Dictionary<string, object> _textInfo = new Dictionary<string, object>();

    public static void Log(string rawText, object value)
    {
        if (_textInfo.ContainsKey(rawText) == false)
            _textInfo.Add(rawText, value);
        else
            _textInfo[rawText] = value;
    }

    private static string GetWholeTextInfo()
    {
        string text = "";

        foreach (var key in _textInfo.Keys)
            text += key + ":\n" + _textInfo[key] + "\n";

        return text;
    }
}


