/////////////////////////////////////////////////
//
// ����� ��������� ���������� ��������� ������
//
// Copyright(c) 2016 UAShota
//
// Rev J  2020.05.15
//
/////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Text;

namespace Empire
{
    /// <summary>
    /// ������� ����� ������ � ����������� ��������� ������
    /// </summary>
    public class CommonCmd
    {
        /// <summary>
        /// ������ ��������� ������ ��������� � ���� ������ � ����� ���������� � ���� ����/��������
        /// </summary>
        private class CmdParser
        {
            #region ����������
            /// <summary>
            /// ����������� ����:��������
            /// </summary>
            private const char ccKeyValue = ':';

            /// <summary>
            /// ����������� ������
            /// </summary>
            private const char ccSpace = ' ';

            /// <summary>
            /// ����������� ������������� ������
            /// </summary>
            private const char ccQuote = '"';

            /// <summary>
            /// ������ ������ �����
            /// </summary>
            private const string csSeparators = "-/";

            /// <summary>
            /// ������� ������� � �����
            /// </summary>
            private int fPosition;

            /// <summary>
            /// ������� ������ �����
            /// </summary>
            private int fStart;

            /// <summary>
            /// ������� ���� �����
            /// </summary>
            private CmdParserItem fFlag = null;

            /// <summary>
            /// ������� ��������� �����
            /// </summary>
            private bool fFlagActive = false;

            /// <summary>
            /// ������� ��������� �����������
            /// </summary>
            private bool fSeparatorActive;

            /// <summary>
            /// ������� ���� ��� �������, ������ �������� ������ ����� �����������
            /// </summary>
            private bool fUnquotedPath;
            #endregion

            #region ��������� ������
            /// <summary>
            /// ������� ���������� ��������
            /// </summary>
            private bool EndOfText()
            {
                return fPosition >= CommandLine.Length;
            }

            /// <summary>
            /// ����������� ������� � ��������� �������
            /// </summary>
            private char Current()
            {
                return fPosition < CommandLine.Length ? CommandLine[fPosition] : (char)0;
            }

            /// <summary>
            /// ����������� ����������� ������� �� ������� �������
            /// </summary>
            private char Prev()
            {
                return fPosition > 0 ? CommandLine[fPosition - 1] : (char)0;
            }

            /// <summary>
            /// ����������� ������� � ��������� �������
            /// </summary>
            private char MoveAhead()
            {
                char tmpChar = Current();
                fPosition = Math.Min(fPosition + 1, CommandLine.Length);
                return tmpChar;
            }

            /// <summary>
            /// ����������� �������� �����
            /// </summary>
            /// <returns>������ �������� �����</returns>
            private string Copy()
            {
                return CommandLine.Substring(fStart, fPosition - fStart);
            }

            /// <summary>
            /// ������ �������� �����
            /// </summary>
            private void ParseParam()
            {
                // ������ ���� ������ ��� �����
                if (FileName == null)
                {
                    FileName = Copy();
                    if (FileName.StartsWith(ccQuote.ToString()))
                        FileName = FileName.Substring(1, FileName.Length - 2);
                    return;
                }
                // ���� ��� ����� ������������ ��������
                if (!fFlagActive)
                {
                    Flags.Add(new CmdParserItem() { Key = Copy() });
                    return;
                }
                // ���� ���� ����, �� ��������� ���� ��� ��� ��������
                if (fFlag != null)
                {
                    // ������� �������� �����
                    fFlag.Value = Copy();
                    Flags.Add(fFlag);
                    // ������������ � �������� ������ �����
                    fFlag = null;
                    fFlagActive = false;
                    fSeparatorActive = false;
                }
                else
                    fFlag = new CmdParserItem() { Key = Copy() };
            }

            /// <summary>
            /// ������ �������� �����
            /// </summary>
            /// <param name="aEndChar">��������� ������ ���������� �����</param>
            private void ParseBlock(char aEndChar)
            {
                while (!EndOfText())
                {
                    // ������ ��������� ����� ��� ��� �������� ����� ����������� ����������
                    if (Current() == aEndChar || fFlagActive && Current() == ccKeyValue)
                    {
                        // ������� ������� ��� ������� ������ ����� ���� �� ������ �������� � ������������
                        if (fUnquotedPath && Current() == ccSpace)
                        {
                            MoveAhead();
                            continue;
                        }
                        // ��������� ����
                        if (Current() == aEndChar)
                            fSeparatorActive = false;
                        // ���� ��� ��������� ����������, ��������� ����:��������
                        if (!fSeparatorActive && Current() == ccKeyValue)
                        {
                            MoveAhead();
                            continue;
                        }
                        // ���� ���� �������� ����������� � ���� ����:��������
                        if (fSeparatorActive && Current() == ccKeyValue)
                        {
                            // �������� ���� � �������� ������
                            ParseParam();
                            MoveAhead();
                            fSeparatorActive = false;
                            fStart = fPosition;
                            // ���� ��������� ������ �������, �� ���� �����������
                            if (Current() == ccQuote)
                            {
                                aEndChar = ccQuote;
                                MoveAhead();
                            }
                            continue;
                        }
                        // ���� ������� ���� ������������� �� ������� - ���������� ������
                        if (Current() == ccQuote)
                            MoveAhead();
                        // �������� �������� ����
                        ParseParam();
                        break;
                    }
                    // ���� ���� ����������� ������ � ����� ��� ������ - ������ ����
                    else if (csSeparators.Contains(Current().ToString()) && Prev() == ccSpace)
                    {
                        // ���� ���� ������� ����� ������ ��� ������� - �������� ��� �����
                        if (fUnquotedPath)
                        {
                            ParseParam();
                            fUnquotedPath = false;
                        }
                        // ���� ������� ��� ���� - �������� � ������ �����
                        if (fFlagActive)
                        {
                            Flags.Add(fFlag);
                            fFlag = null;
                        }
                        else
                            fFlagActive = true;
                        fSeparatorActive = true;
                        break;
                    }
                    MoveAhead();
                }
                // ���� ��� ��������� ��������� - ������� ����������
                if (EndOfText())
                {
                    if (Prev() != ccQuote)
                        ParseParam();
                    if (fFlag != null)
                        Flags.Add(fFlag);
                }
                else
                    MoveAhead();
            }

            /// <summary>
            /// ������ ������ ����������, ���� �� ���� �� ����� ������
            /// </summary>
            /// <param name="aStr">
            /// ������
            /// </param>
            /// <param name="aQuote">
            /// ������ ����������
            /// </param>
            /// <param name="aOnlyDoubleQuote">
            /// ���� true - ��������� ������ ��� ������� ������� ���������� � ���� ������, 
            /// ����� - ��� ������� �� ����� �������
            /// </param>
            /// <returns>
            /// �������� ��� ������������ ������
            /// </returns>
            private void DequotedStr(StringBuilder aStr, char aQuote = '"', bool aOnlyDoubleQuote = true)
            {
                // �������� ������������� ��������� ������
                if (aStr.Length == 0)
                    return;
                // �������� ������ �������� ����������. � ����, ���� � ����� �� ������.
                if ((!aOnlyDoubleQuote && aStr[0] != aQuote && (aStr[aStr.Length - 1] != aQuote))
                    || (aOnlyDoubleQuote && (aStr.Length < 2 || aStr[0] != aQuote || aStr[aStr.Length - 1] != aQuote)))
                    return;
                // � ������ ������
                if (aStr[0] == aQuote)
                    aStr.Remove(0, 1);
                // � ����� 
                if (aStr.Length > 0 && aStr[aStr.Length - 1] == aQuote)
                    aStr.Remove(aStr.Length - 1, 1);
            }

            /// <summary>
            /// ������ ������ ����������, ���� �� ���� �� ����� ������
            /// </summary>
            /// <param name="aStr">
            /// ������
            /// </param>
            /// <param name="aQuote">
            /// ������ ����������
            /// </param>
            /// <param name="aOnlyDoubleQuote">
            /// ���� true - ��������� ������ ��� ������� ������� ���������� � ���� ������, 
            /// ����� - ��� ������� �� ����� �������
            /// </param>
            /// <returns>
            /// �������� ��� ������������ ������
            /// </returns>
            private string DequotedStr(string aStr, char aQuote = '"', bool aOnlyDoubleQuote = true)
            {
                // �������� ������������� ��������� ������
                if (string.IsNullOrEmpty(aStr))
                    return aStr;
                // ��������� ������
                StringBuilder tmpStr = new StringBuilder(aStr);
                DequotedStr(tmpStr, aQuote, aOnlyDoubleQuote);
                return tmpStr.ToString();
            }
            #endregion

            /// <summary>
            /// �����������
            /// </summary>
            /// <param name="aCommandLine">��������� ������</param>
            internal CmdParser(string aCommandLine)
            {
                CommandLine = aCommandLine;
                fUnquotedPath = !CommandLine.StartsWith(ccQuote.ToString());

                // ����� ���� �������� ������
                while (!EndOfText())
                {
                    // ������� ������� ������
                    fStart = fPosition;
                    // ���������� ��������� � �������� ���� ����������� ���������
                    if (Current() == ccQuote)
                        ParseBlock(MoveAhead());
                    else
                        ParseBlock(ccSpace);
                }
            }

            /// <summary>
            /// ����� ���������� ����� � ����������
            /// </summary>
            /// <param name="aName">��� �����</param>
            /// <returns>������� ����� � ����������</returns>
            public bool Exists(string aName)
            {
                foreach (CmdParserItem tmpFlag in Flags)
                    if (tmpFlag.Key.Equals(aName, StringComparison.CurrentCultureIgnoreCase))
                        return true;
                return false;
            }

            /// <summary>
            /// �������� �������� ���������
            /// </summary>
            public string ParamValue(string aName)
            {
                foreach (CmdParserItem tmpFlag in Flags)
                    if (tmpFlag.Key.Equals(aName, StringComparison.CurrentCultureIgnoreCase))
                        return DequotedStr(tmpFlag.Value);
                return string.Empty;
            }

            /// <summary>
            /// ������ �������� ��������� ������
            /// </summary>
            public List<CmdParserItem> Flags { get; } = new List<CmdParserItem>();

            /// <summary>
            /// ������� ��������� ������ �������
            /// </summary>
            public string CommandLine { get; }

            /// <summary>
            /// ��� ������������ �����
            /// </summary>
            public string FileName { get; private set; }
        }

        /// <summary>
        /// ����� ����������� ���� ����-��������
        /// </summary>
        private class CmdParserItem
        {
            /// <summary>
            /// ����
            /// </summary>
            public string Key { get; set; }
            /// <summary>
            /// ��������
            /// </summary>
            public string Value { get; set; }
        }

        /// <summary>
        /// ������ ��� ��������� ������ ����������
        /// </summary>
        private static CmdParser Parser { get; }

        /// <summary>
        /// ����������� ����������� ��� ������������� ����� ������
        /// </summary>
        static CommonCmd()
        {
            Parser = new CmdParser(Environment.CommandLine);
        }

        /// <summary>
        /// ����� ���������� ����� � ����������
        /// </summary>
        /// <param name="aKey">��� �����</param>
        /// <returns>������� ������� �����</returns>
        public static bool Exists(object aKey)
        {
            return Parser.Exists(aKey.ToString());
        }

        /// <summary>
        /// ����� ���������� ����� � ����������
        /// </summary>
        /// <param name="aKey">��� �����</param>
        /// <returns>�������� �����</returns>
        public static string AsString(object aKey)
        {
            return Parser.ParamValue(aKey.ToString());
        }

        /// <summary>
        /// ����� ���������� ����� � ����������
        /// </summary>
        /// <param name="aKey">��� �����</param>
        /// <returns>�������� �����</returns>
        public static int AsInt(object aKey)
        {
            return int.Parse(Parser.ParamValue(aKey.ToString()));
        }
    }
}