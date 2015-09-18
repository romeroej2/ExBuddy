﻿namespace ExBuddy.Navigation
{
    using ExBuddy.Interfaces;

    public class FlightNavigationArgs : IFlightNavigationArgs
    {
        public FlightNavigationArgs()
        {
            this.Radius = 2.7f;
            this.InverseParabolicMagnitude = 6;
            this.Smoothing = 0.2f;
            this.LogWaypoints = true;
            this.ForcedAltitude = 8.0f;
        }

        public float Radius { get; set; }

        public int InverseParabolicMagnitude { get; set; }

        public float Smoothing { get; set; }

        public float ForcedAltitude { get; set; }

        public bool LogWaypoints { get; set; }

        public override string ToString()
        {
            return string.Concat(
                "R->",
                this.Radius,
                "IPM->",
                this.InverseParabolicMagnitude,
                "S->",
                this.Smoothing,
                "Alt->",
                this.ForcedAltitude);
        }
    }
}