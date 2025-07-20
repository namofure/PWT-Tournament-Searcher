using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PWT_RNG
{
    internal class PWTSeed
    {
        private uint Seed;
        private uint Count;
        uint[] stateVector = new uint[624];
        protected const int N = 624;
        protected const int M = 397;
        protected const uint MATRIX_A = 0x9908b0df;
        protected const uint UPPER_MASK = 0x80000000;
        protected const uint LOWER_MASK = 0x7fffffff;

        public PWTSeed(uint Seed, uint Count)
        {
            this.Seed = Seed;
            this.Count = Count;

            //-----------------------------------------------------------------------------------
            stateVector[0] = Seed;
            for (uint i = 1; i < stateVector.Length; i++)
            {
                stateVector[i] = 0x6C078965 * (stateVector[i - 1] ^ (stateVector[i - 1] >> 30)) + i;
            }

            for (var k = 0; k < N - M; k++)
            {
                var temp = (stateVector[k] & UPPER_MASK) | (stateVector[k + 1] & LOWER_MASK);
                stateVector[k] = stateVector[k + M] ^ (temp >> 1);
                if ((temp & 1) == 1) stateVector[k] ^= MATRIX_A;
            }
            for (var k = N - M; k < N - 1; k++)
            {
                var temp = (stateVector[k] & UPPER_MASK) | (stateVector[k + 1] & LOWER_MASK);
                stateVector[k] = stateVector[k + (M - N)] ^ (temp >> 1);
                if ((temp & 1) == 1) stateVector[k] ^= MATRIX_A;
            }
            {
                var temp = (stateVector[N - 1] & UPPER_MASK) | (stateVector[0] & LOWER_MASK);
                stateVector[N - 1] = stateVector[M - 1] ^ (temp >> 1);
                if ((temp & 1) == 1) stateVector[N - 1] ^= MATRIX_A;
            }
            //--------------------------------------------------------------------------------------
        }
        public ulong PWTRNG()
        {
            uint val1, val2, val3;
            uint temp1, temp2, temp3, temp4, temp5, temp6;
            ulong PWT, PWT1, PWT2, PWTtemp;

            val1 = stateVector[Count + 1];
            val2 = stateVector[Count + 2];

            temp1 = (val1 >> 0xB) ^ val1;
            temp2 = ((temp1 << 0x7) & 0x9D2C5680) ^ temp1;
            temp3 = ((temp2 << 0xF) & 0xEFC60000) ^ temp2;
            PWT1 = ((temp3 >> 0x12) ^ temp3);

            temp4 = (val2 >> 0xB) ^ val2;
            temp5 = ((temp4 << 0x7) & 0x9D2C5680) ^ temp4;
            temp6 = ((temp5 << 0xF) & 0xEFC60000) ^ temp5;
            PWT2 = ((temp6 >> 0x12) ^ temp6);

            PWT = (PWT1 << 0x20) + PWT2;    //PWT乱数(仮)
            return PWT;

        }
    } 
}
