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

        public PWTSeed(uint Seed, uint Count)
        {
            this.Seed = Seed;
            this.Count = Count;
        }

        public ulong PWTRNG()
        {
            uint value1 = 0x0;
            uint value2 = 0x0;
            uint value3 = 0x0;
            uint Const1 = 0x0;
            uint Const2 = 0x0;
            uint Const3 = 0x0;
            uint MT1, MT2, MT3, MT4;
            uint val1, val2, val3;
            uint temp1, temp2, temp3, temp4, temp5, temp6;
            ulong PWT, PWT1, PWT2, PWTtemp;
            uint temp = Seed;

            for (uint i = 1; i <= 624; i++) //個体値乱数の生成
            {
                temp = ((temp ^ (temp >> 30)) * 0x6C078965) + i;
                if (i == Count + 1) value1 = temp;
                if (i == Count + 2) value2 = temp;
                if (i == Count + 3) value3 = temp;
                if (i == Count + 398) Const1 = temp;
                if (i == Count + 399) Const2 = temp;

            }

            MT1 = (value1 & 0x80000000) | (value2 & 0x7FFFFFFF);    //テーブル更新
            MT2 = (MT1 >> 0x1) ^ Const1;
            if (MT1 % 2 == 1) MT2 = MT2 ^ 0x9908B0DF;
            val1 = MT2;

            MT3 = (value2 & 0x80000000) | (value3 & 0x7FFFFFFF);
            MT4 = (MT3 >> 0x1) ^ Const2;
            if (MT3 % 2 == 1) MT4 = MT4 ^ 0x9908B0DF;
            val2 = MT4;

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
