using Newtonsoft.Json;

namespace RemoteMusicPlayerClient.Utility.Segments
{
    [JsonObject(MemberSerialization.OptIn)]
    public struct Segment
    {
        [JsonProperty]
        public int Begin { get; }
        [JsonProperty]
        public int End { get; }
        public int Count { get; } 

        [JsonConstructor]
        public Segment(int begin, int end)
        {
            Begin = begin;
            End = end;
            Count = End - Begin + 1;
        }

        public bool Contains(Segment value)
        {
            return Begin <= value.Begin && End >= value.End;
        }
    }
}