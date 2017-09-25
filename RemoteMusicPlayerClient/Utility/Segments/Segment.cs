namespace RemoteMusicPlayerClient.Utility.Segments
{
    public class Segment
    {
        public int Begin { get; set; }
        public int End { get; set; }
        public int Count => End - Begin + 1;

        public Segment(int begin, int end)
        {
            Begin = begin;
            End = end;
        }

        public bool Contains(Segment value)
        {
            return Begin <= value.Begin && End >= value.End;
        }
    }
}