using System;
using System.Collections.Generic;
using System.Linq;

namespace RemoteMusicPlayerClient.Utility.Segments
{
    public class SegmentCollection
    {
        private readonly int _length;
        private readonly List<Segment> _segments;

        public SegmentCollection(int length)
        {
            _length = length;
            _segments = new List<Segment>();
        }

        public List<Segment> Add(Segment newSegment)
        {
            // assuming Begin <= End
            if (newSegment.End > _length)
            {
                throw new ArgumentException("The segment exceeds boundaries of buffer");
            }

            _segments.Add(newSegment);
            var removedSegments = Sort();

            return removedSegments.Aggregate(new List<Segment> {newSegment}, (absentSegments, removedSegment) =>
            {
                var absentSegmentToShrinkIndex =
                    absentSegments.FindIndex(absentSegment => absentSegment.Contains(removedSegment));

                var absentSegmentToShrink = absentSegments[absentSegmentToShrinkIndex];

                if (removedSegments.Contains(absentSegmentToShrink))
                {
                    absentSegments.RemoveAt(absentSegmentToShrinkIndex);
                }
                else if (absentSegmentToShrink.Begin == removedSegment.Begin)
                {
                    absentSegmentToShrink.Begin = removedSegment.End + 1;
                }
                else if (absentSegmentToShrink.End == removedSegment.End)
                {
                    absentSegmentToShrink.End = removedSegment.Begin - 1;
                }
                else
                {
                    var newAbsentSegment = new Segment(removedSegment.End + 1, absentSegmentToShrink.End);
                    absentSegmentToShrink.End = removedSegment.Begin - 1;

                    absentSegments.Insert(absentSegmentToShrinkIndex + 1, newAbsentSegment);
                }

                return absentSegments;
            });
        }

        private List<Segment> Sort()
        {
            var removedSegments = new List<Segment>();
            
            _segments.Sort((x, y) =>
            {
                var result = y.Begin - x.Begin;

                if (result == 0)
                {
                    result = y.End - x.End;
                }

                return result;

            });

            for (var i = 0; i < _segments.Count - 1; i++)
            {
                while (_segments[i].End >= _segments[i + 1].Begin)
                {
                    removedSegments.Add(new Segment(_segments[i + 1].Begin, _segments[i].End));
                    _segments[i].End = _segments[i + 1].End;
                    _segments.RemoveAt(i + 1);
                }
            }

            return removedSegments;
        }
    }
}