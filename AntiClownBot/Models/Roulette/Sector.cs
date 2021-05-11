using System;
using System.Collections.Generic;

namespace Roulette
{
    public enum Color
    {
        Zero,
        Black,
        Red,
    }
    
    public class Sector
    {
        public readonly int Number;
        public Color Color => colorByNum[Number];
        public bool IsEven => Number % 2 == 0;

        public Sector(int number)
        {
            if (number > 36)
                throw new ArgumentException("ты еблан в рулетке 37 ячеек");
            
            Number = number;
        }
        
        private static readonly Dictionary<int,Color> colorByNum = new()
        {
            {0, Color.Zero},
            {1, Color.Red},
            {2, Color.Black},
            {3, Color.Red},
            {4, Color.Black},
            {5, Color.Red},
            {6, Color.Black},
            {7, Color.Red},
            {8, Color.Black},
            {9, Color.Red},
            {10, Color.Black},
            {11, Color.Black},
            {12, Color.Red},
            {13, Color.Black},
            {14, Color.Red},
            {15, Color.Black},
            {16, Color.Red},
            {17, Color.Black},
            {18, Color.Red},
            {19, Color.Red},
            {20, Color.Black},
            {21, Color.Red},
            {22, Color.Black},
            {23, Color.Red},
            {24, Color.Black},
            {25, Color.Red},
            {26, Color.Black},
            {27, Color.Red},
            {28, Color.Black},
            {29, Color.Black},
            {30, Color.Red},
            {31, Color.Black},
            {32, Color.Red},
            {33, Color.Black},
            {34, Color.Red},
            {35, Color.Black},
            {36, Color.Red},
        };
    }
}