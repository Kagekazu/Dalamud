﻿using System.Diagnostics;
using System.Numerics;

using Dalamud.Utility;

namespace Dalamud.Interface.Animation;

/// <summary>
/// Base class facilitating the implementation of easing functions.
/// </summary>
public abstract class Easing
{
    // TODO: Use game delta time here instead
    private readonly Stopwatch animationTimer = new();

    private double valueInternal;

    /// <summary>
    /// Initializes a new instance of the <see cref="Easing"/> class with the specified duration.
    /// </summary>
    /// <param name="duration">The animation duration.</param>
    protected Easing(TimeSpan duration)
    {
        this.Duration = duration;
    }

    /// <summary>
    /// Gets or sets the origin point of the animation.
    /// </summary>
    public Vector2? Point1 { get; set; }

    /// <summary>
    /// Gets or sets the destination point of the animation.
    /// </summary>
    public Vector2? Point2 { get; set; }

    /// <summary>
    /// Gets the resulting point at the current timestep.
    /// </summary>
    public Vector2 EasedPoint { get; private set; }

    /// <summary>
    /// Gets or sets a value indicating whether the result of the easing should be inversed.
    /// </summary>
    public bool IsInverse { get; set; }

    /// <summary>
    /// Gets the current value of the animation, following unclamped logic.
    /// </summary>
    [Obsolete($"This field has been deprecated. Use either {nameof(ValueClamped)} or {nameof(ValueUnclamped)} instead.", true)]
    [Api13ToDo("Map this field to ValueClamped, probably.")]
    public double Value => this.ValueUnclamped;

    /// <summary>
    /// Gets the current value of the animation, from 0 to 1.
    /// </summary>
    public double ValueClamped => Math.Clamp(this.ValueUnclamped, 0, 1);

    /// <summary>
    /// Gets or sets the current value of the animation, not limited to a range of 0 to 1.
    /// Will return numbers outside of this range if accessed beyond animation time.
    /// </summary>
    public double ValueUnclamped
    {
        get
        {
            if (this.IsInverse)
                return 1 - this.valueInternal;

            return this.valueInternal;
        }

        protected set
        {
            this.valueInternal = value;

            if (this.Point1.HasValue && this.Point2.HasValue)
                this.EasedPoint = AnimUtil.Lerp(this.Point1.Value, this.Point2.Value, (float)this.valueInternal);
        }
    }

    /// <summary>
    /// Gets or sets the duration of the animation.
    /// </summary>
    public TimeSpan Duration { get; set; }

    /// <summary>
    /// Gets a value indicating whether the animation is running.
    /// </summary>
    public bool IsRunning => this.animationTimer.IsRunning;

    /// <summary>
    /// Gets a value indicating whether the animation is done.
    /// </summary>
    public bool IsDone => this.animationTimer.ElapsedMilliseconds > this.Duration.TotalMilliseconds;

    /// <summary>
    /// Gets the progress of the animation, from 0 to 1.
    /// </summary>
    protected double Progress => this.animationTimer.ElapsedMilliseconds / this.Duration.TotalMilliseconds;

    /// <summary>
    /// Starts the animation from where it was last stopped, or from the start if it was never started before.
    /// </summary>
    public void Start()
    {
        this.animationTimer.Start();
    }

    /// <summary>
    /// Stops the animation at the current point.
    /// </summary>
    public void Stop()
    {
        this.animationTimer.Stop();
    }

    /// <summary>
    /// Restarts the animation.
    /// </summary>
    public void Restart()
    {
        this.animationTimer.Restart();
    }

    /// <summary>
    /// Resets the animation.
    /// </summary>
    public void Reset()
    {
        this.animationTimer.Reset();
    }

    /// <summary>
    /// Updates the animation.
    /// </summary>
    public abstract void Update();
}
