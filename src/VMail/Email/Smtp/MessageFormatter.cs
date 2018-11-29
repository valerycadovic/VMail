using System;
using System.Collections.Generic;
using System.Text;

namespace Email.Smtp
{
    public static class MessageFormatter
    {
        public static string GetText(string from, string subject, string body, params string[] to)
        {
            return
                $"From: {from}{Environment.NewLine}" +
                $"Subject: =?UTF-8?B?{Convert.ToBase64String(Encoding.UTF8.GetBytes(subject))}?={Environment.NewLine}" +
                $"To: {ParseCC(to)}{Environment.NewLine}" +
                $"Mime-Version: 1.0;{Environment.NewLine}" +
                $"Content-Type: text/plain; charset=\"utf-8\";{Environment.NewLine}" +
                $"Content-Transfer-Encoding: base64" +
                Environment.NewLine +
                Convert.ToBase64String(Encoding.UTF8.GetBytes(body));
        }

        public static string GetHtml(string from, string subject, string body, params string[] to)
        {
            return
                $"From: {from}{Environment.NewLine}" +
                $"Subject: =?UTF-8?B?{Convert.ToBase64String(Encoding.UTF8.GetBytes(subject))}?={Environment.NewLine}" +
                $"To: {ParseCC(to)}{Environment.NewLine}" +
                $"Mime-Version: 1.0;{Environment.NewLine}" +
                $"Content-Type: text/html; charset=\"utf-8\";{Environment.NewLine}" +
                $"Content-Transfer-Encoding: base64" +
                Environment.NewLine +
                Environment.NewLine +
                Convert.ToBase64String(Encoding.UTF8.GetBytes(body));
        }

        private static string ParseCC(IEnumerable<string> cc)
        {
            var sb = new StringBuilder();
            foreach (var item in cc)
            {
                sb.Append($"<{item}>, ");
            }
            return sb.ToString().Remove(sb.ToString().Length - 2, 2);
        }
    }
}
