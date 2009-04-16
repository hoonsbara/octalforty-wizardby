using System.IO;

namespace octalforty.Wizardby.Console
{
    public class BufferedStreamReader : StreamReader
    {
        public BufferedStreamReader(string path, bool detectEncodingFromByteOrderMarks) :
            base(new BufferedStream(new FileStream(path, FileMode.Open, FileAccess.Read), 0x3FFFF), detectEncodingFromByteOrderMarks)
        {
        }
    }
}
