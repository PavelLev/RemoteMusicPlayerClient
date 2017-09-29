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
                if (absentSegmentToShrinkIndex == -1)
                {
                    return absentSegments;
                }

                var absentSegmentToShrink = absentSegments[absentSegmentToShrinkIndex];

                if (removedSegments.Contains(absentSegmentToShrink))
                {
                    absentSegments.RemoveAt(absentSegmentToShrinkIndex);
                }
                else if (absentSegmentToShrink.Begin == removedSegment.Begin)
                {
                    absentSegments[absentSegmentToShrinkIndex] = new Segment(removedSegment.End + 1, absentSegmentToShrink.End);
                }
                else if (absentSegmentToShrink.End == removedSegment.End)
                {
                    absentSegments[absentSegmentToShrinkIndex] = new Segment(absentSegmentToShrink.Begin, removedSegment.Begin - 1);
                }
                else
                {
                    var newAbsentSegment = new Segment(removedSegment.End + 1, absentSegmentToShrink.End);
                    absentSegments[absentSegmentToShrinkIndex] = new Segment(absentSegmentToShrink.Begin, removedSegment.Begin - 1);

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
                var result = x.Begin - y.Begin;

                if (result == 0)
                {
                    result = x.End - y.End;
                }

                return result;

            });

            for (var i = 0; i < _segments.Count - 1; i++)
            {
                while (_segments[i].End >= _segments[i + 1].Begin)
                {
                    removedSegments.Add(new Segment(_segments[i + 1].Begin, _segments[i].End));
                    _segments[i] = new Segment(_segments[i].Begin, _segments[i + 1].End);
                    _segments.RemoveAt(i + 1);

                    if (i == _segments.Count - 1)
                    {
                        break;
                    }
                }
            }

            return removedSegments;
        }
    }
}