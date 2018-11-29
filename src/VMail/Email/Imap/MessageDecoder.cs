using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Email.Imap
{
    public static class MessageDecoder
    {
        /// <summary>
        /// Decodes the body.
        /// </summary>
        /// <param name="body">The body.</param>
        /// <param name="encoding">The encoding.</param>
        /// <param name="charset">The charset.</param>
        /// <returns>decoded text</returns>
        public static string DecodeBody(string body, string encoding, string charset)
        {
            switch (encoding)
            {
                case "quoted-printable":
                    return DecodeQuotedPrintable(body, charset);
                case "base64":
                    return DecodeBase64(body, charset);
                default:
                    return body;
            }
        }

        /// <summary>
        /// Decodes the base64.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="bodycharset">The bodycharset.</param>
        /// <returns></returns>
        private static string DecodeBase64(string text, string bodycharset)
        {
            var bytes = Convert.FromBase64String(text);
            return Encoding.GetEncoding(bodycharset).GetString(bytes);
        }

        /// <summary>
        /// Decodes the quoted printable.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="bodycharset">The bodycharset.</param>
        /// <returns></returns>
        private static string DecodeQuotedPrintable(string text, string bodycharset)
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
                        catch
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
            
            if (string.IsNullOrEmpty(bodycharset))
            {
                return Encoding.UTF8.GetString(output.ToArray());
            }

            if (string.Compare(bodycharset, "ISO-2022-JP", true) == 0)
            {
                return Encoding.GetEncoding("Shift_JIS").GetString(output.ToArray());
            }

            return Encoding.GetEncoding(bodycharset).GetString(output.ToArray());
        }

        /// <summary>
        /// Decodes one encoded line.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns></returns>
        public static string DecodeEncodedLine(string text)
        {
            var regex = new Regex(@"\s*=\?(?<charset>.*?)\?(?<encoding>[qQbB])\?(?<value>.*?)\?=");
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
                        decoded += reg.Replace(value, m =>
                        {
                            byte[] bytes = m.Groups["byte"]
                                .Captures
                                .Cast<Capture>()
                                .Select(c => (byte)Convert.ToInt32(c.Value, 16)).ToArray();

                            return Encoding.GetEncoding(charset).GetString(bytes);
                        }).Replace('_', ' ');
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
