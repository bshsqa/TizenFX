
using global::System;
using Tizen.NUI;
using Tizen.NUI.BaseComponents;
using NUnit.Framework;

using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Linq;
using System.CodeDom.Compiler;
using System.Linq.Expressions;

using System;
using System.Diagnostics;
using Tizen.NUI.Xaml;
using System.IO;


public enum LineType
{
    Paragraph,
    Heading,
    ListItem,
    Quote,
    CodeBlock,
    Table,
    Empty,
    ThematicBreak
}

public class MarkdownLine
{
    public LineType Type { get; set; } = LineType.Empty;

    public StringBuilder ContentBuffer { get; set; } = new StringBuilder();

    public StringBuilder TrailingBuffer { get; set; } = new StringBuilder();

    public int HeadingLevel { get; set; } = 0;

    public int IndentLevel { get; set; } = 0;

    public int blockIndent = 0;

    public string BlockInfoString;

    public StringBuilder Content { get; set; } = new StringBuilder();

    public int OrderedListIndex = -1; // < 0 : bullet, >= 0 : numbering list

    public override string ToString() => $"[{Type}] {(ContentBuffer.ToString() + TrailingBuffer.ToString()).TrimEnd()}, IndentLevel : {IndentLevel}" + (Type == LineType.Heading ? $", HeadingLevel : {HeadingLevel}" : "");
    public string TypeToString() => $"[{Type}]";
    public string InfoToString() => $"IndentLevel : {IndentLevel}" + (Type == LineType.Heading ? $", HeadingLevel : {HeadingLevel}" : "") + (OrderedListIndex > 0 ? $", OrderedListIndex : {OrderedListIndex}" : "") + ((BlockInfoString != null && BlockInfoString.Length > 0) ? $", BlockInfoString : {BlockInfoString}" : "");
    public string OriginalContentToString() => $"{(ContentBuffer.ToString() + TrailingBuffer.ToString()).TrimEnd()}";
    public string ContentToString() => $"{Content.ToString()}";
}

public class IndentEntry
{
    public int Indent { get; }
    public int ContentStart { get; }
    public int OrderedListIndex { get; set; }

    public IndentEntry(int indent, int contentStart, int orderedListIndex)
    {
        Indent = indent;
        ContentStart = contentStart;
        OrderedListIndex = orderedListIndex;
    }

    public override string ToString() => $"(Indent: {Indent}, ContentStart: {ContentStart})";
}

public class MarkdownStreamParser
{
    enum NewLineType
    {
        MergeToContent,

        MoveToNewLine,

        FinalizeAndReset,

        NotNewLine
    }

    enum CodeBlockType
    {
        Backtick,

        Space
    }

    private List<MarkdownLine> lines = new List<MarkdownLine>();
    private MarkdownLine activeLine = new MarkdownLine();
    private bool inTable = false;   // TODO
    private bool isLineNeverThematicBreak = false;
    private Stack<IndentEntry> indentStack = new Stack<IndentEntry>();
    private Stack<int> orderedListIndex;
    private bool requireLineFullUpdate = false;


    // For code block optimize
    private bool inCodeBlock = false;
    private bool requiredToCloseCodeBlock = false;
    private bool skipFirstNewLineInCodeblock = false;
    private CodeBlockType codeBlockType = CodeBlockType.Backtick;

    public MarkdownStreamParser()
    {
        lines.Add(activeLine);
        indentStack.Push(new IndentEntry(0, 0, -1));
    }

    private int CurrentIndentLevel => indentStack.Count - 1;

    double totaltime = 0.0;
    int tickCount = 0;

    public void InputChar(char c)
    {
        if (c == '\r')
        {
            return;
        }

        if (c == '\t')
        {
            AppendTabAsSpaces();
        }
        else
        {
            activeLine.TrailingBuffer.Append(c);
        }

        NewLineType newLineType = IsNewLineRequired();

        if (newLineType != NewLineType.NotNewLine)
        {
            requireLineFullUpdate = (requireLineFullUpdate || newLineType == NewLineType.MoveToNewLine || newLineType == NewLineType.FinalizeAndReset) ? true : false;
            FinalizeActiveLine(newLineType);
        }

        bool isLineBreakDetected = IsLineBreakDetected(activeLine.TrailingBuffer);
        if (isLineBreakDetected)
        {
            activeLine.ContentBuffer.Append(activeLine.TrailingBuffer.ToString());
            activeLine.TrailingBuffer.Clear();
        }

        UpdateActiveLine();
    }

    private void AppendTabAsSpaces()
    {
        string trailingBuffer = activeLine.TrailingBuffer.ToString();
        int trailingSpaces = 0;
        for (int i = trailingBuffer.Length - 1; i >= 0; --i)
        {
            if (trailingBuffer[i] != ' ')
            {
                break;
            }
            trailingSpaces++;
        }

        int spaceToAdd = 4 - (trailingSpaces % 4);
        if (spaceToAdd == 4)
        {
            spaceToAdd = 0;
        }

        activeLine.TrailingBuffer.Append(new string(' ', spaceToAdd));
    }

    private NewLineType IsNewLineRequired()
    {
        string trailingBuffer = activeLine.TrailingBuffer.ToString();
        string trimmedTrailingBuffer = trailingBuffer.Trim();
        string content = activeLine.ContentBuffer.ToString();

        if (trailingBuffer.Length == 0 || (!inCodeBlock && (trimmedTrailingBuffer.Length > 0 && trimmedTrailingBuffer[0] != '>') && trailingBuffer[trailingBuffer.Length - 1] != ' ' && trailingBuffer[trailingBuffer.Length - 1] != '\n'))
        {
            return NewLineType.NotNewLine;
        }

        // Handle Code Block
        // Code Block Start -> Copied new line. Do not merge trailingBuffer to ContentBuffer
        if (!inCodeBlock && (trailingBuffer.Length > 0 && trailingBuffer[trailingBuffer.Length - 1] == '\n') && IsCodeBlockMarker(trimmedTrailingBuffer))
        {
            if (string.IsNullOrWhiteSpace(content))
            {
                //Tizen.Log.Error("NUI", "Type1\n");
                return NewLineType.NotNewLine;
            }
                //Tizen.Log.Error("NUI", "Type2\n");
            return NewLineType.MoveToNewLine;
        }

        // Code Block Start or End-> Copied new line. Do not merge trailingBuffer to ContentBuffer
        if ((trailingBuffer.Length > 0 && trailingBuffer[trailingBuffer.Length - 1] == '\n') && IsCodeBlockMarker(trimmedTrailingBuffer))
        {
            if (string.IsNullOrWhiteSpace(content))
            {
                //Tizen.Log.Error("NUI", "Type3\n");
                return NewLineType.NotNewLine;
            }

            if (inCodeBlock)
            {
                requireLineFullUpdate = true;
                //Tizen.Log.Error("NUI", "Type4\n");
                return NewLineType.MergeToContent;
            }
            else
            {
                //Tizen.Log.Error("NUI", "Type5\n");
                return NewLineType.MoveToNewLine;
            }
        }

        if (inCodeBlock)
        {
            if (string.IsNullOrEmpty(trimmedTrailingBuffer))
            {
                //Tizen.Log.Error("NUI", "Type6\n");
                return NewLineType.NotNewLine;
            }

            int indent = 0;
            int contentStart = 0;
            ComputeIndentAndContentStart(trailingBuffer, out indent, out contentStart);

            var top = indentStack.Peek();
            int delta = indent - activeLine.blockIndent;
            if (delta < 0)
            {
                requiredToCloseCodeBlock = true;
                //Tizen.Log.Error("NUI", "Type7\n");
                return NewLineType.MoveToNewLine;
            }
                //Tizen.Log.Error("NUI", "Type8\n");
            return NewLineType.NotNewLine;
        }
        // Handle Code Block Finished

        // "\n   \n" -> Empry new line. Merge trailingBuffer to ContentBuffer
        if ((trailingBuffer.Length > 0 && trailingBuffer[trailingBuffer.Length - 1] == '\n') && string.IsNullOrWhiteSpace(trailingBuffer))
        {
                //Tizen.Log.Error("NUI", "Type9\n");
            return (string.IsNullOrEmpty(content)) ? NewLineType.MergeToContent : NewLineType.FinalizeAndReset;
        }

        // "  \n" -> Empry new line. Merge trailingBuffer to ContentBuffer
        if (trailingBuffer.Length >= 3 && trailingBuffer[trailingBuffer.Length - 1] == '\n' && trailingBuffer[trailingBuffer.Length - 2] == ' ' && trailingBuffer[trailingBuffer.Length - 3] == ' ')
        {
            if(activeLine.Type != LineType.Quote)
            {
                //Tizen.Log.Error("NUI", "Type10\n");
                return NewLineType.MergeToContent;
            }
        }

        // ThematicBreak -> Empry new line. Merge trailingBuffer to ContentBuffer
        if ((trailingBuffer.Length > 0 && trailingBuffer[trailingBuffer.Length - 1] == '\n') && IsThematicBreak(trimmedTrailingBuffer))
        {
                //Tizen.Log.Error("NUI", "Type11\n");
            return (string.IsNullOrEmpty(content)) ? NewLineType.MergeToContent : NewLineType.FinalizeAndReset;
        }

        // "\n" after heading -> Empry new line. Merge trailingBuffer to ContentBuffer
        if ((trailingBuffer.Length > 0 && trailingBuffer[trailingBuffer.Length - 1] == '\n') && activeLine.Type == LineType.Heading) //Regex.IsMatch(trimmedTrailingBuffer, @"^\s*#{1,6}\s"))
        {
                //Tizen.Log.Error("NUI", "Type12\n");
            return NewLineType.MergeToContent;
        }

        // List -> Copied new line. Do not merge trailingBuffer to ContentBuffer
        if (!string.IsNullOrWhiteSpace(content) && (trailingBuffer.Length > 0 && trailingBuffer[trailingBuffer.Length - 1] == ' ') && IsListItem(trailingBuffer))// Regex.IsMatch(trailingBuffer, @"^\s*([-*+]|\d+\.)\s$"))
        {
                //Tizen.Log.Error("NUI", "Type13\n");
            return NewLineType.MoveToNewLine;
        }

        // Heading -> Copied new line. Do not merge trailingBuffer to ContentBuffer
        int headingLevel = -1;
        if (!string.IsNullOrWhiteSpace(content) && (trailingBuffer.Length > 0 && trailingBuffer[trailingBuffer.Length - 1] == ' ') && IsHeadingWithLevel(trailingBuffer, out headingLevel))//&& Regex.IsMatch(trailingBuffer, @"^\s*#\s$")) <- TODO
        {
                //Tizen.Log.Error("NUI", "Type14\n");
            return NewLineType.MoveToNewLine;
        }

        // Quote -> Copied new line. Do not merge trailingBuffer to ContentBuffer
        if (!string.IsNullOrWhiteSpace(content) && (trimmedTrailingBuffer.Length > 0 && trimmedTrailingBuffer[0] == '>'))
        {
            if(activeLine.Type != LineType.Quote)
            {
                //Tizen.Log.Error("NUI", "Type15\n");
                // TODO '  \n' after Quote make multi line.
                return NewLineType.MoveToNewLine;
            }
        }

                //Tizen.Log.Error("NUI", "Type16\n");
        return NewLineType.NotNewLine;
    }

    private void FinalizeActiveLine(NewLineType newLineType)
    {
        if (newLineType == NewLineType.MergeToContent)
        {
            activeLine.ContentBuffer.Append(activeLine.TrailingBuffer.ToString());
            activeLine.TrailingBuffer.Clear();
        }

        string newLineString = activeLine.TrailingBuffer.ToString();
        activeLine.TrailingBuffer.Clear();

        UpdateActiveLine();

        StartNewLine(newLineString);

        if (newLineType == NewLineType.FinalizeAndReset)
        {
            activeLine.ContentBuffer.AppendLine(activeLine.TrailingBuffer.ToString());
            activeLine.TrailingBuffer.Clear();
            UpdateActiveLine();
            StartNewLine(activeLine.TrailingBuffer.ToString());
        }
    }

    private void StartNewLine(string newLineString)
    {
        bool isActiveLineEmpty = activeLine.Type == LineType.Empty;
        bool isPreviousActiveLineEmpty = ((lines.Count < 2) || (lines[lines.Count - 2].Type == LineType.Empty));

        if (isActiveLineEmpty && isPreviousActiveLineEmpty)
        {
            if (lines.Count >= 2)
            {
                lines[lines.Count - 2].ContentBuffer.Append(activeLine.ContentBuffer.ToString());
                activeLine.ContentBuffer.Clear();
            }
        }
        else
        {
            activeLine = new MarkdownLine();
            lines.Add(activeLine);
        }
        activeLine.TrailingBuffer.Append(newLineString);
        isLineNeverThematicBreak = false;
    }

    private void UpdateActiveLine()
    {
        LineType previousType = activeLine.Type;
        UpdateActiveLineType();

        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();
        UpdateActiveLineContent(previousType != activeLine.Type);

        stopwatch.Stop();
        double elapsedMilliseconds = stopwatch.ElapsedTicks / 10000.0;
        Tizen.Log.Error("NUI", $"Time : {elapsedMilliseconds}\n");
        if (elapsedMilliseconds < 30.0)
        {
            tickCount++;
            totaltime += elapsedMilliseconds;
            double aveTime = totaltime / tickCount;
            Tizen.Log.Error("NUI", $"AveTime : {aveTime}\n");
        }
        else
        {
            Tizen.Log.Error("NUI", $"OUTLIER\n");
        }

        if (elapsedMilliseconds > 10.0)
        {
            Tizen.Log.Error("NUI", $"current input : , content : {activeLine.ContentBuffer.ToString()}, trail : {activeLine.TrailingBuffer.ToString()}\n");
        }
    }

    private void UpdateActiveLineType()
    {
        string line = activeLine.ContentBuffer.ToString() + activeLine.TrailingBuffer.ToString();
        string trimmed = line.Trim();

        if (!inCodeBlock && string.IsNullOrWhiteSpace(line))
        {
            activeLine.Type = LineType.Empty;
            activeLine.IndentLevel = 0;
            return;
        }

        if (UpdateCodeBlock(line, trimmed))
        {
            return;
        }

        // ThematicBreak should be checked before ListItem
        // TODO How to move this after ListItem
        if (!isLineNeverThematicBreak && trimmed.Length > 0)
        {
            char lastChar = trimmed[trimmed.Length - 1];
            if (lastChar != '-' && lastChar != '_' && lastChar != '*')
            {
                isLineNeverThematicBreak = true;
            }
            else
            {
                if (trimmed.Length > 3 && IsThematicBreak(trimmed))
                {
                    activeLine.Type = LineType.ThematicBreak;
                    activeLine.IndentLevel = 0;
                    return;
                }
            }
        }

        // Heading
        int headingLevel = -1;
        if (activeLine.Type == LineType.Heading || (line.Length > 0 && line[line.Length - 1] == ' ' && IsHeadingWithLevel(line, out headingLevel))) //Regex.IsMatch(line, @"^\s*#{1,6}\s")))
        {
            if (activeLine.Type != LineType.Heading)
            {
                activeLine.Type = LineType.Heading;

                activeLine.IndentLevel = 0;

                activeLine.HeadingLevel = headingLevel;
            }
            return;
        }

        // ListItem
        int indent = 0;
        int contentStart = 0;
        if (activeLine.Type == LineType.ListItem || (line.Length > 0 && line[line.Length - 1] == ' ' && IsListItemWithIndentAndContentStart(line, out indent, out contentStart)))
        {
            if (activeLine.Type != LineType.ListItem)
            {
                activeLine.IndentLevel = ComputeIndentLevel(indent, contentStart, true);

                if (contentStart - indent > 2)
                {
                    if (indentStack.Peek().OrderedListIndex == -1)
                    {
                        string subString = line.Substring(indent, contentStart - indent - 2);
                        indentStack.Peek().OrderedListIndex = Convert.ToInt32(subString);
                    }
                    else
                    {
                        indentStack.Peek().OrderedListIndex++;
                    }
                }
                else
                {
                    indentStack.Peek().OrderedListIndex = -1;
                }
                activeLine.OrderedListIndex = indentStack.Peek().OrderedListIndex;
            }
            activeLine.Type = LineType.ListItem;
            return;
        }

        // Quote
        if (activeLine.Type == LineType.Quote || (trimmed.Length > 0 && trimmed[0] == '>'))
        {
            if (CurrentIndentLevel > 0 && activeLine.Type != LineType.Quote)
            {
                ComputeIndentAndContentStart(line, out indent, out contentStart);
                activeLine.IndentLevel = ComputeIndentLevel(indent, contentStart, false);
            }
            activeLine.Type = LineType.Quote;
            return;
        }

        activeLine.Type = LineType.Paragraph;
        if (CurrentIndentLevel > 0 && indentStack.Count > 1)
        {
            if (activeLine.IndentLevel == 0)
            {
                ComputeIndentAndContentStart(line, out indent, out contentStart);
                activeLine.IndentLevel = ComputeIndentLevel(indent, contentStart, false);
            }
        }
        else
        {
            activeLine.IndentLevel = 0;
        }
    }

    private bool UpdateCodeBlock(string line, string trimmed)
    {
        if (requiredToCloseCodeBlock)
        {
            inCodeBlock = false;
            requiredToCloseCodeBlock = false;
            return true;
        }

        if (inCodeBlock && codeBlockType == CodeBlockType.Backtick && line.Length > 3 && line[line.Length - 1] == '\n')
        {
            bool isFinishMarker = true;
            if (trimmed[trimmed.Length - 1] != '`' || trimmed[trimmed.Length - 2] != '`' || trimmed[trimmed.Length - 3] != '`')
            {
                isFinishMarker = false;
            }

            for (int i = trimmed.Length - 4; i >= 0; --i)
            {
                if (trimmed[i] == '\n')
                {
                    break;
                }

                if (trimmed[i] != ' ')
                {
                    isFinishMarker = false;
                    break;
                }
            }
            if (isFinishMarker)
            {
                inCodeBlock = false;
                return true;
            }
        }

        // "```" should be checked before to check "    ";
        if (line[line.Length - 1] == '\n' && IsCodeBlockMarker(trimmed))
        {
            if (!inCodeBlock)
            {
                // TODO Need to control strings in code block
                activeLine.IndentLevel = 0;
                if (CurrentIndentLevel > 0 && activeLine.Type != LineType.CodeBlock)
                {
                    int indent = 0;
                    int contentStart = 0;
                    ComputeIndentAndContentStart(line, out indent, out contentStart);
                    int blockIndentLevel = ComputeIndentLevel(indent, contentStart, false);
                    if (blockIndentLevel == CurrentIndentLevel)
                    {
                        activeLine.blockIndent = indent;
                        activeLine.IndentLevel = blockIndentLevel;
                    }
                }
                activeLine.Type = LineType.CodeBlock;
                inCodeBlock = true;
                codeBlockType = CodeBlockType.Backtick;
            }
            return true;
        }

        if (line.Length > 3 && line[0] == ' ' && line[1] == ' ' && line[2] == ' ' && line[3] == ' ')
        {
            int indent = 0;
            int contentStart = 0;
            ComputeIndentAndContentStart(line, out indent, out contentStart);

            if (indent >= 4 + indentStack.Peek().ContentStart)
            {
                if (!inCodeBlock)
                {
                    inCodeBlock = true;
                    codeBlockType = CodeBlockType.Space;
                    activeLine.Type = LineType.CodeBlock;

                    activeLine.blockIndent = indent;
                    activeLine.IndentLevel = CurrentIndentLevel;
                }
                return true;
            }
        }

        // Do not change type during code block.
        if (inCodeBlock)
        {
            return true;
        }
        return false;
    }

    private bool IsCodeBlockMarker(string trimmedLine)
    {
        return (trimmedLine.Length > 2 && trimmedLine[0] == '`' && trimmedLine[1] == '`' && trimmedLine[2] == '`');
    }

    private void UpdateActiveLineContent(bool typeChanged)
    {
        if (typeChanged || requireLineFullUpdate)
        {
            activeLine.Content.Clear();
            switch (activeLine.Type)
            {
                case LineType.Paragraph:
                    {
                        char prevChar = ' ';
                        for (int i = 0; i < 2; ++i)
                        {
                            StringBuilder builder = (i == 0) ? activeLine.ContentBuffer : activeLine.TrailingBuffer;

                            int bufferPoint = 0;
                            int bufferLength = builder.Length;


                            while (bufferPoint < bufferLength && char.IsWhiteSpace(builder[bufferPoint]))
                            {
                                bufferPoint++;
                            }

                            while (bufferPoint < bufferLength)
                            {
                                char currentChar = (builder[bufferPoint] == '\n') ? ' ' : builder[bufferPoint];
                                if (prevChar != ' ' || currentChar != ' ')
                                {
                                    activeLine.Content.Append(currentChar);
                                }
                                prevChar = currentChar;
                                bufferPoint++;
                            }
                        }
                        break;
                    }
                case LineType.Heading:
                    {
                        int trailingBufferStart = 0;
                        int trailingBufferEnd = activeLine.TrailingBuffer.Length - 1;
                        while (trailingBufferStart < trailingBufferEnd && char.IsWhiteSpace(activeLine.TrailingBuffer[trailingBufferStart]))
                        {
                            trailingBufferStart++;
                        }

                        while (trailingBufferStart < trailingBufferEnd && activeLine.TrailingBuffer[trailingBufferStart] == '#')
                        {
                            trailingBufferStart++;
                        }

                        while (trailingBufferStart < trailingBufferEnd && char.IsWhiteSpace(activeLine.TrailingBuffer[trailingBufferStart]))
                        {
                            trailingBufferStart++;
                        }

                        while (trailingBufferEnd >= trailingBufferStart && char.IsWhiteSpace(activeLine.TrailingBuffer[trailingBufferEnd]))
                        {
                            trailingBufferEnd--;
                        }

                        char prevChar = ' ';
                        for (int i = trailingBufferStart; i <= trailingBufferEnd; ++i)
                        {
                            char currentChar = (activeLine.TrailingBuffer[i] == '\n') ? ' ' : activeLine.TrailingBuffer[i];
                            if (prevChar != ' ' || currentChar != ' ')
                            {
                                activeLine.Content.Append(currentChar);
                            }
                            prevChar = currentChar;
                        }
                        break;
                    }
                case LineType.ListItem:
                    {
                        StringBuilder builder = (activeLine.ContentBuffer.Length == 0) ? activeLine.TrailingBuffer : activeLine.ContentBuffer;
                        int bufferStart = 0;
                        int bufferEnd = builder.Length - 1;
                        while (bufferStart < bufferEnd && char.IsWhiteSpace(builder[bufferStart]))
                        {
                            bufferStart++;
                        }

                        if (builder[bufferStart] == '-' || builder[bufferStart] == '+' || builder[bufferStart] == '*')
                        {
                            bufferStart++;
                        }
                        else
                        {
                            while (bufferStart < bufferEnd && char.IsDigit(builder[bufferStart]))
                            {
                                bufferStart++;
                            }
                            if (builder[bufferStart] == '.')
                            {
                                bufferStart++;
                            }
                        }

                        while (bufferStart < bufferEnd && char.IsWhiteSpace(builder[bufferStart]))
                        {
                            bufferStart++;
                        }

                        while (bufferEnd >= bufferStart && char.IsWhiteSpace(builder[bufferEnd]))
                        {
                            bufferEnd--;
                        }

                        char prevChar = ' ';
                        for (int i = bufferStart; i <= bufferEnd; ++i)
                        {
                            char currentChar = (builder[i] == '\n') ? ' ' : builder[i];
                            if (prevChar != ' ' || currentChar != ' ')
                            {
                                activeLine.Content.Append(currentChar);
                            }
                            prevChar = currentChar;
                        }
                        break;
                    }
                case LineType.Quote:
                    {
                        char prevChar = ' ';
                        for (int i = 0; i < 2; ++i)
                        {
                            StringBuilder builder = (i == 0) ? activeLine.ContentBuffer : activeLine.TrailingBuffer;

                            int bufferPoint = 0;
                            int bufferLength = builder.Length;

                            while (bufferPoint < builder.Length)
                            {
                                while (bufferPoint < bufferLength && char.IsWhiteSpace(builder[bufferPoint]))
                                {
                                    bufferPoint++;
                                }

                                if (bufferPoint < bufferLength && builder[bufferPoint] == '>')
                                {
                                    bufferPoint++;
                                }

                                while (bufferPoint < bufferLength && char.IsWhiteSpace(builder[bufferPoint]))
                                {
                                    bufferPoint++;
                                }

                                while (bufferPoint < bufferLength && builder[bufferPoint] != '\n')
                                {
                                    if (prevChar != ' ' || builder[bufferPoint] != ' ')
                                    {
                                        activeLine.Content.Append(builder[bufferPoint]);
                                    }
                                    prevChar = builder[bufferPoint];
                                    bufferPoint++;
                                }

                                if (bufferPoint < bufferLength && builder[bufferPoint] == '\n')
                                {
                                    if (bufferPoint > 2 && builder[bufferPoint - 1] == ' ' && builder[bufferPoint - 2] == ' ')
                                    {
                                        activeLine.Content.Append('\n');
                                    }
                                    else
                                    {
                                        if (prevChar != ' ')
                                        {
                                            activeLine.Content.Append(' ');
                                        }
                                    }
                                    prevChar = ' ';
                                    bufferPoint++;
                                }
                            }
                        }
                        break;
                    }
                case LineType.CodeBlock:
                    {
                        string content = "";
                        string contentLine = activeLine.ContentBuffer.ToString() + activeLine.TrailingBuffer.ToString();
                        var lines = contentLine.Split('\n').ToList();

                        string firstLine = lines[0];
                        Match startMatch = Regex.Match(firstLine, @"^(\s*)(`{3,})([^\r\n]*)?$");
                        if (startMatch.Success)
                        {
                            activeLine.BlockInfoString = startMatch.Groups[3].Value.Trim();
                        }

                        int codeStartLine = codeBlockType == CodeBlockType.Backtick ? 1 : 0;
                        for (int i = codeStartLine; i < lines.Count; ++i)
                        {
                            string line = lines[i];
                            if (Regex.IsMatch(line, @"^\s*`{3,}\s*$") || line.Length <= activeLine.blockIndent)
                            {
                                break;
                            }
                            content = content + (content.Length == 0 ? "" : "\n") + line.Substring(activeLine.blockIndent);
                        }
                        activeLine.Content.Clear();
                        activeLine.Content.Append(content);
                        break;
                    }
                case LineType.Table:
                    {
                        string contentLine = activeLine.ContentBuffer.ToString() + activeLine.TrailingBuffer.ToString();
                        activeLine.Content.Append(contentLine);
                        break;
                    }
                case LineType.ThematicBreak:
                    {
                        activeLine.Content.Clear();
                        activeLine.Content.Append("---");
                        break;
                    }
            }
            requireLineFullUpdate = false;
            return;
        }
        else
        {
            if (activeLine.Type == LineType.ThematicBreak)
            {
                return;
            }

            if (activeLine.Type == LineType.Empty)
            {
                activeLine.Content.Clear();
                return;
            }

            if (activeLine.ContentBuffer.Length == 0 && activeLine.TrailingBuffer.Length == 0)
            {
                return;
            }

            char lineLastChar = (activeLine.TrailingBuffer.Length > 0) ? activeLine.TrailingBuffer[activeLine.TrailingBuffer.Length - 1] : activeLine.ContentBuffer[activeLine.ContentBuffer.Length - 1];
            if (activeLine.Type == LineType.CodeBlock)
            {
                StringBuilder builder = (activeLine.TrailingBuffer.Length > 0) ? activeLine.TrailingBuffer : activeLine.ContentBuffer;

                bool needToSkip = false;
                if (lineLastChar == ' ')
                {
                    int whiteCount = 0;
                    int linePoint = builder.Length - 1;
                    while (linePoint >= 0 && builder[linePoint] != '\n')
                    {
                        if (builder[linePoint] == ' ')
                        {
                            whiteCount++;
                        }
                        else
                        {
                            whiteCount = activeLine.blockIndent + 1;
                            break;
                        }
                        linePoint--;
                    }

                    if (whiteCount <= activeLine.blockIndent)
                    {
                        needToSkip = true;
                    }
                }
                if (!needToSkip)
                {
                    activeLine.Content.Append(lineLastChar);
                }
                return;
            }

            if (lineLastChar == ' ' && (activeLine.Content.Length == 0 || activeLine.Content[activeLine.Content.Length - 1] == ' '))
            {
                return;
            }

            if(activeLine.Type == LineType.Quote && lineLastChar == '\n')
            {
                StringBuilder builder = (activeLine.TrailingBuffer.Length > 0) ? activeLine.TrailingBuffer : activeLine.ContentBuffer;
                if (builder.Length > 2 && builder[builder.Length - 1] == ' ' && builder[builder.Length - 2] == ' ')
                {
                    activeLine.Content.Append('\n');
                    return;
                }
            }

            if (lineLastChar == '\n')
            {
                lineLastChar = ' ';
            }
            activeLine.Content.Append(lineLastChar);
        }
    }

    private void ComputeIndentAndContentStart(string line, out int indent, out int contentStart)
    {
        indent = 0;
        contentStart = 0;
        while (indent < line.Length && line[indent] == ' ')
        {
            indent++;
        }
        contentStart = indent;

        if (line[contentStart] == '-' || line[contentStart] == '+' || line[contentStart] == '*')
        {
            contentStart++;
        }
        else
        {
            while (contentStart < line.Length - 1 && char.IsDigit(line[contentStart]))
            {
                contentStart++;
            }
            if (line[contentStart] == '.')
            {
                contentStart++;
            }
        }

        // space after marker.
        contentStart++;
    }

    private int ComputeIndentLevel(int indent, int contentStart, bool isList)
    {
        while (indentStack.Count > 1 && indent < indentStack.Peek().Indent)
        {
            indentStack.Pop();
        }
        var top = indentStack.Peek();

        if (isList)
        {
            int delta = indent - top.ContentStart;
            if (indentStack.Count == 1 || (delta >= 0 && delta <= 2))
            {
                indentStack.Push(new IndentEntry(indent, contentStart, -1));
            }
        }
        else
        {
            while (indentStack.Count > 1 && indentStack.Peek().ContentStart > contentStart)
            {
                indentStack.Pop();
            }
        }

        return CurrentIndentLevel;
    }

    private bool IsLineBreakDetected(StringBuilder line)
    {
        return (line.Length > 0 && line[line.Length - 1] == '\n');
    }

    private int CountLeadingSpaces(string line)
    {
        int count = 0;
        foreach (char c in line)
        {
            if (c == ' ') count++;
            else break;
        }
        return count;
    }

    private bool IsThematicBreak(string trimmedLine)
    {
        char thematicBreakTypeChar = trimmedLine[0];
        if (thematicBreakTypeChar != '-' && thematicBreakTypeChar != '_' && thematicBreakTypeChar != '*')
        {
            return false;
        }

        int count = 0;
        foreach (char c in trimmedLine)
        {
            if (c != thematicBreakTypeChar && c != ' ')
            {
                return false;
            }

            if (c == thematicBreakTypeChar)
            {
                count++;
            }

            if (count >= 3)
            {
                return true;
            }
        }
        return false;
    }

    private bool IsHeadingWithLevel(string line, out int level)
    {
        level = 0;
        int contentStart = 0;
        while (contentStart < line.Length && line[contentStart] == ' ')
        {
            contentStart++;
        }

        if(contentStart >= line.Length || line[contentStart] != '#')
        {
            return false;
        }

        while (contentStart < line.Length && line[contentStart] != ' ')
        {
            if (line[contentStart] != '#')
            {
                return false;
            }
            level++;
            contentStart++;
        }
        return true;
    }

    private bool IsListItem(string line)
    {
        int contentStart = 0;
        while (contentStart < line.Length && line[contentStart] == ' ')
        {
            contentStart++;
        }

        if(contentStart >= line.Length)
        {
            return false;
        }

        if (line[contentStart] != '-' && line[contentStart] != '+' && line[contentStart] != '*' && !char.IsDigit(line[contentStart]))
        {
            return false;
        }

        bool isOrderedList = (char.IsDigit(line[contentStart])) ? true : false;
        if (!isOrderedList)
        {
            if (contentStart < line.Length && line[contentStart + 1] == ' ')
            {
                return true;
            }
            return false;
        }

        while (contentStart < line.Length && char.IsDigit(line[contentStart]))
        {
            contentStart++;
        }

        if (contentStart < line.Length && line[contentStart] == '.' && line[contentStart + 1] == ' ')
        {
            return true;
        }
        return false;
    }

    private bool IsListItemWithIndentAndContentStart(string line, out int indent, out int contentStart)
    {
        indent = 0;
        contentStart = 0;
        while (indent < line.Length && line[indent] == ' ')
        {
            indent++;
        }

        contentStart = indent;
        if (contentStart >= line.Length)
        {
            indent = 0;
            contentStart = 0;
            return false;
        }

        indent = contentStart;
        if (line[contentStart] != '-' && line[contentStart] != '+' && line[contentStart] != '*' && !char.IsDigit(line[contentStart]))
        {
            indent = 0;
            contentStart = 0;
            return false;
        }

        bool isOrderedList = (char.IsDigit(line[contentStart])) ? true : false;
        if (!isOrderedList)
        {
            if (contentStart < line.Length && line[contentStart + 1] == ' ')
            {
                contentStart = indent + 2;
                return true;
            }
            indent = 0;
            contentStart = 0;
            return false;
        }

        while (contentStart < line.Length && char.IsDigit(line[contentStart]))
        {
            contentStart++;
        }

        if (contentStart < line.Length && line[contentStart] == '.' && line[contentStart + 1] == ' ')
        {
            contentStart += 2;
            return true;
        }

        indent = 0;
        contentStart = 0;
        return false;
    }

    public void PrintLines()
    {
        foreach (var line in lines)
        {
            Tizen.Log.Error("NUI", $"{line}\n");
        }
    }

    private int showPositionY = 0;
    private View activeView;
    private int previousLineIdx = 0;

    public void ShowContent(View root)
    {
        List<Vector4> colors = new List<Vector4> { new Vector4(1.0f, 0.0f, 0.0f, 0.3f), new Vector4(0.0f, 1.0f, 0.0f, 0.3f), new Vector4(0.0f, 0.0f, 1.0f, 0.3f) };

        for (int i = previousLineIdx; i < lines.Count; ++i)
        {
            var line = lines[i];
            if (activeView == null || i != previousLineIdx)
            {
                int colorIdx = (i + 1) % colors.Count;
                activeView = new View()
                {
                    Size = new Size(2000, 70),
                    PositionY = showPositionY,
                    PositionUsesPivotPoint = true,
                    PivotPoint = PivotPoint.TopLeft,
                    ParentOrigin = ParentOrigin.TopLeft,
                    BackgroundColor = colors[colorIdx],
                };

                TextLabel type = new TextLabel(line.TypeToString())
                {
                    Name = "type",
                    Size = new Size(500, 50),
                    PositionUsesPivotPoint = true,
                    PivotPoint = PivotPoint.TopLeft,
                    ParentOrigin = ParentOrigin.TopLeft
                };
                activeView.Add(type);

                TextLabel info = new TextLabel(line.InfoToString())
                {
                    Name = "info",
                    PositionY = 30,
                    Size = new Size(500, 50),
                    PositionUsesPivotPoint = true,
                    PivotPoint = PivotPoint.TopLeft,
                    ParentOrigin = ParentOrigin.TopLeft
                };
                activeView.Add(info);

                TextLabel content = new TextLabel(line.ContentToString())
                {
                    Name = "content",
                    PositionX = 500,
                    SizeWidth = 800,
                    PositionUsesPivotPoint = true,
                    PivotPoint = PivotPoint.TopLeft,
                    ParentOrigin = ParentOrigin.TopLeft,
                    MultiLine = true,
                };
                activeView.Add(content);

                TextLabel originalContent = new TextLabel(line.OriginalContentToString())
                {
                    Name = "originalContent",
                    PositionX = 1300,
                    PositionUsesPivotPoint = true,
                    PivotPoint = PivotPoint.TopLeft,
                    ParentOrigin = ParentOrigin.TopLeft,
                    MultiLine = true,
                };
                activeView.Add(originalContent);
                root.Add(activeView);
                showPositionY += 70;
            }
            else
            {
                uint childCnt = activeView.ChildCount;
                for (uint cidx = 0; cidx < childCnt; ++cidx)
                {
                    TextLabel textLabel = activeView.GetChildAt(cidx) as TextLabel;
                    if (textLabel == null)
                    {
                        continue;
                    }

                    if (textLabel.Name == "type")
                    {
                        textLabel.Text = line.TypeToString();
                    }
                    else if (textLabel.Name == "info")
                    {
                        textLabel.Text = line.InfoToString();
                    }
                    else if (textLabel.Name == "content")
                    {
                        textLabel.Text = line.ContentToString();
                    }
                    else if (textLabel.Name == "originalContent")
                    {
                        textLabel.Text = line.OriginalContentToString();
                    }
                }
            }
        }
        previousLineIdx = lines.Count - 1;
    }

    public List<MarkdownLine> GetLines() => lines;
}


namespace Tizen.NUI.Samples
{
    public class CaptureTest : IExample
    {
        private Window window;
        private Animation animation;
        Timer timer = new Timer(1);
        int inputIdx = 0;
        bool played = false;

        TextLabel timeLabel;


        public void Activate()
        {
            window = NUIApplication.GetDefaultWindow();
            window.KeyEvent += WindowKeyEvent;

            var parser = new MarkdownStreamParser();
            string input = "### Heading\npara  \nnew para\n  - List item lv1\n   - List item lv1\n    - List item lv2\n     - List item lv2\n- List item lv1\n   - List item lv2\n     - List item lv3\n\n       >Continuation Quote lv3\n\n  Continuation Block lv1\n    - List item lv2\n\nParagraph\n     # Heading in code block\nParagraph\n# Heading\nParagraph text\n\nParagraph text2\n- List item lv1\n  - List item lv2\n    ```code block lv2\n    more code lv2\n  ```\n  more code lv1\n```  \nmore code lv0\n  ```\n  Paragraph text3\n    Paragraph text 4\n   ** *   *\n  _ _ __ \n __*___ \n - -- - \n      \n132. number list\n     13. number list\n     16. number list\n132. number list\n153. number list\n+ unordered list\n5. number list\n100. number list\n         space codeblock indented\n\n         space codeblock\n         more code block\n   paragraph code block failed\n\n- List lv1\n  - List lv2\n        Paragraph test space code block but failed.\n\n        space code block lv2.\n      Paragraph code block broken.\n    Paragraph continue.\n\n    Paragraph continue.\n\n  Paragraph lv1.\n\n>Quote lv1\n>Quote lv1 (inline)  \n>Quote lv1 (TODO continue)\n\nQuote lv1\n\n>Quote lv1 (Not continue)";
            //string input = "firstline\n- list\n  ```str\n  asdf\n ```\nnext code";
            //string input = "- List item lv1\n  - List item lv2\n    ```code block lv2\n    more code lv2\n  ```\n  more code lv1\n```  \nmore code lv0\n  ```\n  Paragraph text3\n    Paragraph text 4";

            View root = new View()
            {
                Size = new Size(window.Size),
                PositionUsesPivotPoint = true,
                PivotPoint = PivotPoint.BottomLeft,
                ParentOrigin = ParentOrigin.BottomLeft,
                WidthResizePolicy = ResizePolicyType.FitToChildren,
                HeightResizePolicy = ResizePolicyType.FitToChildren,
            };
            window.Add(root);

            timeLabel = new TextLabel()
            {
                Name = "timeLabel",
                PositionUsesPivotPoint = true,
                PivotPoint = PivotPoint.Center,
                ParentOrigin = ParentOrigin.Center,
                MultiLine = true,
                SizeWidth = 200,
            };

            //            window.Add(timeLabel);

            timer.Tick += (e, s) =>
            {
                parser.InputChar(input[inputIdx]);
                parser.ShowContent(root);

                inputIdx++;

                if (inputIdx < input.Length)
                {
                    return true;
                }
                else
                {
                    Tizen.Log.Error("NUI", $"Input : \n{input}");
                    parser.PrintLines();
                    return false;
                }
            };


            window.TouchEvent += (s, e) =>
            {
                if (!played)
                {
                    timer.Start();
                    played = true;
                }
            };

            window.KeyEvent += (s, e) =>
            {
                if (e.Key.State == Key.StateType.Down)
                {
                    if (e.Key.KeyPressedName == "Up")
                    {
                        root.PositionY -= 10;
                    }
                    if (e.Key.KeyPressedName == "Down")
                    {
                        root.PositionY += 10;
                    }
                }
            };

        }

        private void WindowKeyEvent(object sender, Window.KeyEventArgs e)
        {
            if (e.Key.State == Key.StateType.Up)
            {
            }
        }

        public void Deactivate()
        {
        }
    }
}
