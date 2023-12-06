﻿using Kumi.Game.Charts.Objects.Windows;
using Kumi.Game.Gameplay.Algorithms;
using Kumi.Game.Gameplay.Drawables;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Primitives;
using osu.Framework.Layout;
using osuTK;

namespace Kumi.Game.Gameplay;

public partial class ScrollingNoteContainer : Container<DrawableNote>
{
    public readonly IBindable<IScrollAlgorithm> Algorithm = new Bindable<IScrollAlgorithm>();
    
    private readonly HashSet<DrawableNote> layoutComputed = new HashSet<DrawableNote>();
    private readonly LayoutValue layoutCache = new LayoutValue(Invalidation.RequiredParentSizeToFit | Invalidation.DrawInfo);
    
    public ScrollingNoteContainer()
    {
        AddLayout(layoutCache);
    }

    public double TimeRange = 5000;

    public override void Add(DrawableNote drawable)
    {
        if (IsLoaded)
            setLifetimeStart(drawable);

        base.Add(drawable);
    }

    public override bool Remove(DrawableNote drawable, bool disposeImmediately)
    {
        layoutComputed.Remove(drawable);
        return base.Remove(drawable, disposeImmediately);
    }

    protected override void Update()
    {
        base.Update();
        
        if (layoutCache.IsValid)
            return;
        
        layoutComputed.Clear();

        foreach (var child in Children)
            setLifetimeStart(child);
        
        layoutCache.Validate();
    }

    protected override void UpdateAfterChildrenLife()
    {
        base.UpdateAfterChildrenLife();

        foreach (var child in Children)
        {
            if (child == null)
                continue;
            
            updatePosition(child, Time.Current);
            
            if (layoutComputed.Contains(child))
                continue;

            updateLayout(child);

            layoutComputed.Add(child);  
        }
    }

    private void updateLayout(DrawableNote note)
    {
        updatePosition(note, note.Note.StartTime);
        setLifetimeStart(note);
    }

    public double TimeAtScreenSpacePosition(Vector2 screenSpacePosition)
    {
        var pos = ToLocalSpace(screenSpacePosition);
        return timeAtPosition(pos.X, Time.Current);
    }

    private RectangleF getBoundingBox() => new RectangleF().Inflate(100);

    public double ComputeLifetimeStart(DrawableNote note)
    {
        var boundingBox = getBoundingBox();
        var startOffset = -boundingBox.Left;

        var adjustedTime = Algorithm.Value.TimeAt(-startOffset, note.Note.StartTime, TimeRange, DrawWidth);
        return adjustedTime - TimeRange;
    }
    
    private void setLifetimeStart(DrawableNote note)
    {
        var computedStartTime = ComputeLifetimeStart(note);
        note.LifetimeStart = Math.Min(note.Note.StartTime - note.Note.Windows.WindowFor(NoteHitResult.Bad), computedStartTime);
    }

    private void updatePosition(DrawableNote note, double currentTime)
    {
        var position = positionAtTime(note.Note.StartTime, currentTime);
        note.X = position;
    }

    private float positionAtTime(double time, double currentTime)
        => Algorithm.Value.PositionAt(time, currentTime, TimeRange, DrawWidth);

    private double timeAtPosition(float localPosition, double currentTime)
        => Algorithm.Value.TimeAt(localPosition, currentTime, TimeRange, DrawWidth);
}
