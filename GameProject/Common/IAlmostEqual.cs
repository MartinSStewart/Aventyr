﻿namespace Game.Common
{
    public interface IAlmostEqual<in T> where T : IAlmostEqual<T>
    {
        /// <summary>
        /// Check if this instance is within a delta of a comparison instance.
        /// </summary>
        bool AlmostEqual(T comparison, double delta);
        /// <summary>
        /// Check if this instance is within a delta of a comparison instance 
        /// or if the ratio between comparison and this instance is less than a given percentage.
        /// </summary>
        bool AlmostEqual(T comparison, double delta, double ratioDelta);
    }
}
