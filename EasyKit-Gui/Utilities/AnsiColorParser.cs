using System.Text.RegularExpressions;

namespace EasyKit_Gui.Utilities;

public static class AnsiColorParser
{
    // Appends ANSI-colored text to a RichTextBox
    public static void AppendAnsiText(RichTextBox box, string text)
    {
        var colorMap = new Dictionary<int, Color>
        {
            [30] = Color.Black,
            [31] = Color.Red,
            [32] = Color.Green,
            [33] = Color.Yellow,
            [34] = Color.Blue,
            [35] = Color.Magenta,
            [36] = Color.Cyan,
            [37] = Color.White,
            [90] = Color.Gray,
            [0] = box.ForeColor // reset
        };

        Color currentColor = box.ForeColor;
        int lastIndex = 0;
        var regex = new Regex(@"\x1b\[([0-9;]+)m");
        var matches = regex.Matches(text);
        foreach (Match match in matches)
        {
            // Append text before the escape code
            if (match.Index > lastIndex)
            {
                box.SelectionStart = box.TextLength;
                box.SelectionLength = 0;
                box.SelectionColor = currentColor;
                box.AppendText(text.Substring(lastIndex, match.Index - lastIndex));
            }

            // Parse color code(s)
            var codes = match.Groups[1].Value.Split(';');
            foreach (var codeStr in codes)
                if (int.TryParse(codeStr, out int code))
                {
                    if (colorMap.ContainsKey(code))
                        currentColor = colorMap[code];
                    else if (code == 0)
                        currentColor = box.ForeColor;
                }

            lastIndex = match.Index + match.Length;
        }

        // Append remaining text
        if (lastIndex < text.Length)
        {
            box.SelectionStart = box.TextLength;
            box.SelectionLength = 0;
            box.SelectionColor = currentColor;
            box.AppendText(text.Substring(lastIndex));
        }

        box.SelectionColor = box.ForeColor;
    }
}