namespace torrent.Script
{
    public interface InputReader
    {
        int Line { get; }
        int Row { get; }
        bool IsEnd();
        int Read();
        int Peek();
    }
}