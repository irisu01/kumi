﻿using Kumi.Game.IO.Formats;
using Kumi.Game.Utils;

namespace Kumi.Game.Charts.Events;

/// <summary>
/// An event that defines a break in the <see cref="IChart"/>.
/// </summary>
public class BreakTimeEvent : Event, IHasEndTime
{
    protected override EventType ExpectedType => EventType.Break;
    protected override int ExpectedLength => 1;
    public float StartTime { get; }
    public float EndTime { get; private set; }

    public BreakTimeEvent(float startTime, float endTime)
        : base(startTime)
    {
        StartTime = startTime;
        EndTime = endTime;
    }
    protected override void Parse(string[] input)
    {
        EndTime = StartTime + StringUtils.AssertAndFetch<float>(input[0]);
    }
}
