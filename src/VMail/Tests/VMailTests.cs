using Email.Imap;
using NUnit.Framework;

namespace Tests
{
    [TestFixture]
    public class VMailTests
    {
        [TestCase("=?UTF-8?Q?=D0=92=D0=B0=D0=BB=D0=B5=D1=80=D0=B0?=", ExpectedResult = "Валера")]
        [TestCase("=?UTF-8?B?0JLQsNC70LXRgNCw?=", ExpectedResult = "Валера")]
        [TestCase("Valera=?UTF-8?B?0JLQsNC70LXRgNCw?==?UTF-8?Q?=D0=92=D0=B0=D0=BB=D0=B5=D1=80=D0=B0?=Valera",
            ExpectedResult = "ValeraВалераВалераValera")]
        [TestCase("Валера", ExpectedResult = "Валера")]
        public string Can_DecodeEncodedString(string text)
            => MessageDecoder.DecodeEncodedLine(text);
    }
}
