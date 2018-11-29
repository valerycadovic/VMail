using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace EmailProtocols.Imap.Decoding
{
    public static class MessageDecoder
    {
        public static string DecodeQP(string text, string bodycharset)
        {
            var i = 0;
            var output = new List<byte>();
            while (i < text.Length)
            {
                try
                {
                    if (text[i] == '=' && text[i + 1] == '\r' && text[i + 2] == '\n')
                    {
                        i += 3;
                    }

                    else if (text[i] == '=')
                    {
                        string sHex = text;
                        sHex = sHex.Substring(i + 1, 2);
                        try
                        {
                            int hex = Convert.ToInt32(sHex, 16);
                            byte b = Convert.ToByte(hex);
                            output.Add(b);

                        }
                        catch (Exception e)
                        {

                        }
                        i += 3;
                    }
                    else
                    {
                        output.Add((byte)text[i]);
                        i++;
                    }
                }
                catch { }
            }


            if (String.IsNullOrEmpty(bodycharset))
                return Encoding.UTF8.GetString(output.ToArray());
            else
            {
                if (String.Compare(bodycharset, "ISO-2022-JP", true) == 0)
                    return Encoding.GetEncoding("Shift_JIS").GetString(output.ToArray());
                else
                    return Encoding.GetEncoding(bodycharset).GetString(output.ToArray());
            }
        }

        public static string DecodeEncodedLine(string text)
        {
            Regex regex = new Regex(@"\s*=\?(?<charset>.*?)\?(?<encoding>[qQbB])\?(?<value>.*?)\?=");
            string encoded = text;
            string decoded = string.Empty;

            while (encoded.Length > 0)
            {
                Match match = regex.Match(encoded);

                if (match.Success)
                {
                    decoded += encoded.Substring(0, match.Index);
                    string charset = match.Groups["charset"].Value;
                    string encoding = match.Groups["encoding"].Value.ToUpper();
                    string value = match.Groups["value"].Value;

                    if (encoding.Equals("B"))
                    {
                        var bytes = Convert.FromBase64String(value);
                        decoded += Encoding.GetEncoding(charset).GetString(bytes);
                    }
                    else if (encoding.Equals("Q"))
                    {
                        Regex reg = new Regex(@"(\=(?<byte>[0-9A-F][0-9A-F]))+", RegexOptions.IgnoreCase);
                        decoded += reg.Replace(value, new MatchEvaluator(m =>
                        {
                            byte[] bytes = m.Groups["byte"].Captures.Cast<Capture>().Select(c => (byte)Convert.ToInt32(c.Value, 16)).ToArray();
                            return Encoding.GetEncoding(charset).GetString(bytes);
                        })).Replace('_', ' ');
                    }
                    else
                    {
                        decoded += encoded;
                        break;
                    }

                    encoded = encoded.Substring(match.Index + match.Length);
                }
                else
                {
                    decoded += encoded;
                    break;
                }
            }
            return decoded;
        }

    }
}
