﻿/*===============================
             hsp.cs
  Created by @kkrnt && @ygcuber
===============================*/

using System;
using System.Collections.Generic;
using System.Linq;

namespace hsp.cs
{
    class Analyzer
    {
        /// <summary>
        /// コード中の""で括られた文字列をエスケープ
        /// </summary>
        /// <param name="hspArrayString"></param>
        /// <returns></returns>
        public static string StringEscape(string hspArrayString)
        {
            var hspStringData = hspArrayString;
            while (true)
            {
                var preIndex = hspArrayString.IndexOf("\"", StringComparison.OrdinalIgnoreCase);
                if (preIndex == -1 || hspArrayString[preIndex - 1] == '\\') break;
                var x = hspArrayString.Substring(preIndex + 1);
                var postIndex = x.IndexOf("\"", StringComparison.OrdinalIgnoreCase);
                if (postIndex == -1 || hspArrayString[preIndex + postIndex] == '\\') break;
                var midString = hspArrayString.Substring(preIndex, postIndex + 2);
                Program.StringList.Add(midString);
                hspArrayString = hspArrayString.Replace(midString, "");
                hspStringData = hspStringData.Replace(midString, "＠＋＠" + (Program.StringList.Count - 1) + "＠ー＠");
            }
            return hspStringData;
        }

        /// <summary>
        /// エスケープした文字列を元に戻す
        /// </summary>
        /// <param name="hspArrayString"></param>
        /// <returns></returns>
        public static string StringUnEscape(string hspArrayString)
        {
            var hspStringData = hspArrayString;
            while (true)
            {
                var preStringIndex = hspArrayString.IndexOf("＠＋＠", StringComparison.OrdinalIgnoreCase);
                if (preStringIndex != -1)
                {
                    var postStringIndex = hspArrayString.IndexOf("＠ー＠", StringComparison.OrdinalIgnoreCase);
                    if (postStringIndex != -1)
                    {
                        var o = hspArrayString.Substring(preStringIndex, postStringIndex - preStringIndex + 3);
                        var index = int.Parse(o.Replace("＠＋＠", "").Replace("＠ー＠", ""));
                        hspArrayString = hspArrayString.Replace(o, Program.StringList[index]);
                        hspStringData = hspArrayString;
                    }
                }
                else
                {
                    break;
                }
            }
            return hspStringData;
        }

        public static string Preprocessor(string hspArrayString)
        {
            //要素単位で分解するために半角スペースでスプリット
            var sentence = hspArrayString.Replace("  ", " ").Split(' ').ToList();
            for (var i = 0; i < sentence.Count; i++)
            {
                //余計なものは省く
                sentence[i] = sentence[i].Trim();
                if (sentence[i] == null ||
                    sentence[i].Equals("\n") ||
                    sentence[i].Equals(""))
                    continue;
                if (Program.PreprocessorList.Contains(sentence[i]))
                {
                    switch (sentence[i])
                    {
                        case "#uselib":
                            HSP.Uselib(sentence, i);
                            break;
                    }
                }
            }
            //該当しなかったらスルー
            return string.Join(" ", sentence);
        }


        /// <summary>
        /// 関数呼び出し
        /// </summary>
        /// <param name="hspArrayString"></param>
        /// <returns></returns>
        public static string Function(string hspArrayString)
        {
            //要素単位で分解するために半角スペースでスプリット
            var sentence = hspArrayString.Replace("  ", " ").Split(' ').ToList();
            for (var j = 0; j < sentence.Count; j++)
            {
                //余計なものは省く
                //関数は必ず関数名の後に"("が来るはず
                sentence[j] = sentence[j].Trim();
                if (sentence[j] == null ||
                    sentence[j].Equals("\n") ||
                    sentence[j].Equals("") ||
                    !Program.FunctionList.Contains(sentence[j]) ||
                    sentence[j + 1][0] != '(')
                    continue;

                //初めに")"が来る行と, それまでに"("が幾つ出てくるか数える
                var bracketStartCount = 0;
                int k;
                for (k = j + 1; k < sentence.Count; k++)
                {
                    if (sentence[k].Equals("("))
                    {
                        bracketStartCount++;
                    }
                    if (sentence[k].Equals(")"))
                    {
                        break;
                    }
                }

                //"("の数だけ該当する")"をズラす
                for (var l = 0; l < bracketStartCount - 1; l++)
                {
                    var flag = false;
                    for (var m = k + 1; m < sentence.Count; m++)
                    {
                        if (sentence[m].Equals(")"))
                        {
                            k = m;
                            flag = true;
                            break;
                        }
                    }
                    if (!flag)
                    {
                        /*============================
                        //カッコの数がオカシイのでエラー
                        =============================*/
                        Console.WriteLine("Error");
                    }
                }

                //sentence[j]が関数名
                //sentence[k]が関数の")"
                //sentence[j + 1]～sentence[k]で"("～")"まで
                switch (sentence[j])
                {
                    case "int":
                        HSP.Int(sentence, j);
                        break;
                    case "double":
                        HSP.Double(sentence, j);
                        break;
                    case "str":
                        HSP.Str(sentence, j, k);
                        break;
                    case "abs":
                        HSP.Abs(sentence, j, k);
                        break;
                    case "absf":
                        HSP.Absf(sentence, j, k);
                        break;
                    case "sin":
                        HSP.Sin(sentence, j);
                        break;
                    case "cos":
                        HSP.Cos(sentence, j);
                        break;
                    case "tan":
                        HSP.Tan(sentence, j);
                        break;
                    case "atan":
                        HSP.Atan(sentence, j);
                        break;
                    case "deg2rad":
                        HSP.Deg2rad(sentence, j);
                        break;
                    case "rad2deg":
                        HSP.Rad2deg(sentence, j);
                        break;
                    case "expf":
                        HSP.Expf(sentence, j);
                        break;
                    case "logf":
                        HSP.Logf(sentence, j);
                        break;
                    case "powf":
                        HSP.Powf(sentence, j);
                        break;
                    case "sqrt":
                        HSP.Sqrt(sentence, j);
                        break;
                    case "instr":
                        HSP.Instr(sentence, j, k);
                        break;
                    case "strlen":
                        HSP.Strlen(sentence, j, k);
                        break;
                    case "strmid":
                        HSP.Strmid(sentence, j, k);
                        break;
                    case "strtrim":
                        HSP.Strtrim(sentence, j, k);
                        break;
                    case "limit":
                        HSP.Limit(sentence, j, k);
                        break;
                    case "limitf":
                        HSP.Limitf(sentence, j, k);
                        break;
                    case "length":
                        HSP.Length(sentence, j, k, 1);
                        break;
                    case "length2":
                        HSP.Length(sentence, j, k, 2);
                        break;
                    case "length3":
                        HSP.Length(sentence, j, k, 3);
                        break;
                    case "length4":
                        HSP.Length(sentence, j, k, 4);
                        break;
                    case "gettime":
                        HSP.Gettime(sentence, j);
                        break;
                    case "rnd":
                        HSP.Rnd(sentence, j, k);
                        break;
                }
            }
            //結果を反映
            return string.Join(" ", sentence);
        }

        public static string Macro(string hspArrayString)
        {
            //要素単位で分解するために半角スペースでスプリット
            var sentence = hspArrayString.Replace("  ", " ").Split(' ').ToList();
            for (var i = 0; i < sentence.Count; i++)
            {
                //余計なものは省く
                sentence[i] = sentence[i].Trim();
                if (sentence[i] == null ||
                    sentence[i].Equals("\n") ||
                    sentence[i].Equals(""))
                    continue;
                if (Program.MacroList.Contains(sentence[i]))
                {
                    switch (sentence[i])
                    {
                        case "m_pi":
                            HSP.M_pi(sentence, i);
                            break;
                        case "and":
                        case "not":
                        case "or":
                        case "xor":
                            HSP.BitwiseOperation(sentence, i, sentence[i]);
                            break;
                        case "mousex":
                        case "mousey":
                            HSPGUI.Mouse(sentence, i, sentence[i].Substring(5));
                            break;
                        case "dir_cmdline":
                        case "dir_cur":
                        case "dir_desktop":
                        case "dir_exe":
                        case "dir_mydoc":
                        case "dir_sys":
                        case "dir_win":
                            HSP.Directory(sentence, i, sentence[i].Substring(4));
                            break;
                        case "ginfo_mx":
                        case "ginfo_my":
                        case "ginfo_sizex":
                        case "ginfo_sizey":
                        case "ginfo_r":
                        case "ginfo_g":
                        case "ginfo_b":
                        case "ginfo_cx":
                        case "ginfo_cy":
                        case "ginfo_dispx":
                        case "ginfo_dispy":
                        case "ginfo_wx1":
                        case "ginfo_wx2":
                        case "ginfo_wy1":
                        case "ginfo_wy2":
                        case "ginfo_sel":
                            HSPGUI.Ginfo(sentence, i, sentence[i].Substring(6));
                            break;
                        case "hwnd":
                            HSPGUI.Hwnd(sentence, i);
                            break;
                        case "__date__":
                            HSPGUI.__date__(sentence, i);
                            break;
                        case "__time__":
                            HSPGUI.__time__(sentence, i);
                            break;
                        case "msgothic":
                        case "msmincho":
                            HSPGUI.Ms(sentence, i, sentence[i].Substring(2));
                            break;
                        case "font_normal":
                        case "font_bold":
                        case "font_italic":
                        case "font_underline":
                        case "font_strikeout":
                            HSPGUI.Font(sentence, i, sentence[i].Substring(5));
                            break;
                        case "screen_normal":
                        case "screen_hide":
                        case "screen_fixedsize":
                        case "screen_tool":
                        case "screen_frame":
                            HSPGUI.Screen(sentence, i, sentence[i].Substring(7));
                            break;
                    }
                }
            }
            //結果を反映
            return string.Join(" ", sentence);
        }

        public static string ArrayVariable(string hspArrayString)
        {
            hspArrayString = hspArrayString.Replace("  ", " ");
            var sentence = hspArrayString.Split(' ').ToList();
            for (var j = 0; j < sentence.Count; j++)
            {
                sentence[j] = sentence[j].Trim();
                if (!Program.ArrayVariableList.Contains(sentence[j]) ||
                    sentence[j + 1][0] != '(')
                    continue;

                sentence[j + 1] = "[";
                var bracketStartCount = 1;
                for (var k = j + 2; k < sentence.Count; k++)
                {
                    if (sentence[k].Equals("("))
                    {
                        bracketStartCount++;
                    }
                    if (sentence[k].Equals(")"))
                    {
                        bracketStartCount--;
                    }
                    if (bracketStartCount == 0)
                    {
                        sentence[k] = "]";
                        break;
                    }
                }
            }
            return string.Join(" ", sentence);
        }
    }
}
