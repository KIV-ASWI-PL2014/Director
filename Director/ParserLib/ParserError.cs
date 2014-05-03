
namespace Director.ParserLib
{
    /// <summary>
    /// This class represents an error which may occur during Parser execution. All errors have to be traceable to the specific position (line and
    /// position variables) of a specific file (source).
    /// Other classes need to be prevented from changing of an error's inner data, so once the error have been created you can only get the data.
    /// </summary>
    public class ParserError
    {
        public int Line
        {
            get;
            private set;
        }

        public int Position
        {
            get;
            private set;
        }

        public string Source
        {
            get;
            private set;
        }

        public string Message
        {
            get;
            private set;
        }

        public ParserError(int line, int position, string message, string source)
        {
            Line = line;
            Position = position;
            Message = message;
            Source = source;
        }

        public int getLine()
        {
            return Line;
        }

        public int getPosition()
        {
            return Position;
        }

        public string getMessage()
        {
            return Message;
        }

        public string getSource()
        {
            return Source;
        }
    }
}