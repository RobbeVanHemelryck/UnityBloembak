using System;
using System.Collections.Generic;
using System.Linq;

namespace DefaultNamespace
{
    public class PlankLengthCalculator
    {
        private readonly double _plankWidth;
        private readonly double _minHeight;
        private readonly double _maxHeight;

        public PlankLengthCalculator(double plankWidth, double minHeight, double maxHeight)
        {
            _maxHeight = maxHeight;
            _minHeight = minHeight;
            _plankWidth = plankWidth;
        }
    
        public List<float> CalculateLengths(double diameter)
        {
            var omtrek = Math.PI * diameter;
            var planksAmount = Math.Round(omtrek / _plankWidth, 0);

            var planks = new List<decimal>();

            for (int i = 0; i < planksAmount; i++)
            {
                var degree = i * (360 / planksAmount) - 90;
                var rad = Math.PI / 180 * degree;
                var sin = (1 + Math.Sin(rad)) / 2;
                var adjustedSin = sin * (_maxHeight - _minHeight);
                var extraHeight = (decimal) (adjustedSin + _minHeight);
                var height = Math.Round(extraHeight, 1);
                planks.Add(height);
            }

            return planks.Select(x => (float)x).ToList();
        }
    }
}