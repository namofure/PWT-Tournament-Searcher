using static PWT_RNG.PID;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime;
using System.Text;
using System.Threading.Tasks;
using System.Reflection.Metadata.Ecma335;

namespace PWT_RNG
{
    internal class TournamentSearcher
    {
        static void Main(string[] args)
        {
            do
            {
                Console.WriteLine("\n======================================");
                Console.Write("PWT Tournament Searcher\n");

                    string outputPath = $"TournamentList.txt";
                    string excludePath = "出禁リスト.txt";

                if (!File.Exists("Config.txt"))
                {
                    Console.WriteLine("Config.txtが見つかりませんでした");
                    Console.ReadKey();
                    return;
                }

                List<string> ExcludeList = new();

                if (File.Exists(excludePath))
                {
                    ExcludeList = File.ReadAllLines(excludePath)
                        .Select(line => line.Trim())
                        .Where(line => !string.IsNullOrWhiteSpace(line))
                        .ToList();
                }
                else
                {
                    Console.WriteLine("出禁リスト.txtが見つかりませんでした");
                    Console.ReadKey();
                    return;
                }

                if(Offset > 622) Offset = 622;
                int Flag = 0;

                using (StreamWriter writer = new(outputPath))
                {
                    writer.WriteLine("【World Leaders Tournament】");
                    Console.WriteLine("[年, 月, 秒, 時, 分, 秒, VCount, Timer0, 初期SEED]\n");
                    writer.WriteLine("[年, 月, 秒, 時, 分, 秒, VCount, Timer0, 初期SEED]\n");

                    for (int n = 0; n < PID.Count; n++)     //初期SEED
                    {
                        InSeedData SeedData = PID.GenSeed();
                        ulong Seed = SeedData.Seed;
                        ulong Temp = Seed;

                        for (ulong count = 1; count < PID.Offset; count++)      //MT
                        {
                            if (CountFlag == 100)
                            {
                                Console.ReadKey();
                                return;
                            }

                            var PWT = new PWTSeed(Seed, count);
                            ulong PWTSeed = PWT.PWTRNG();

                            List<ulong> TrainerIDs = [8, 14, 15, 16, 17, 18, 19, 53];
                            List<ulong> JoinIDs = new List<ulong>();

                            ulong JoinIndex = 0x2E;

                            ulong temp = NextSeed(PWTSeed);

                            for (int i = 0; i < 7; i++, temp = NextSeed(temp))
                            {
                                ulong TrainerIndex = 0;
                                ulong RawTrainerIndex = ((temp >> 32) * JoinIndex) >> 32;

                                while (RawTrainerIndex > 0)
                                {
                                    TrainerIndex++;

                                    if (TrainerIDs.Contains(TrainerIndex))
                                    {
                                        continue;
                                    }

                                    RawTrainerIndex--;
                                }
                                if (TrainerIDs.Contains(TrainerIndex))
                                {
                                    TrainerIndex++;   //例外処理？
                                }

                                TrainerIDs.Add(TrainerIndex);
                                JoinIDs.Add(TrainerIndex);
                                JoinIndex--;
                            }

                            JoinIDs.Sort();

                            string[] TrainerName = {"チェレン","ホミカ","アーティ","カミツレ","ヤーコン","フウロ","ジャガ","シズイ","ベル","ポッド",
                            "コーン","デント","アロエ","ハチク","グリーン","ワタル","ダイゴ","ミクリ","シロナ","アデク",
                            "タケシ","カスミ","マチス","エリカ","ナツメ","カツラ","サカキ","ハヤト","ツクシ","アカネ",
                            "マツバ","シジマ","ミカン","ヤナギ","イブキ","アンズ","ツツジ","トウキ","テッセン","アスナ",
                            "センリ","ナギ","フウ","ラン","アダン","ヒョウタ","ナタネ","メリッサ","スモモ","マキシ",
                            "トウガン","スズナ","デンジ","レッド","YOU"};

                            List<string> JoinedTrainer = JoinIDs.ConvertAll(id => TrainerName[(ulong)id]);
                            JoinedTrainer.Insert(0, "YOU");
                            JoinIDs.Insert(0, 0xFF);

                            temp = NextSeed(PWTSeed);

                            for (ulong i = 0; i < 8; i++, temp = NextSeed(temp))
                            {
                                ulong SwapIndex = ((temp >> 32) * (8 - i)) >> 32;

                                int Swap = (int)SwapIndex;
                                int I = (int)(7 - i);

                                ulong tempID = JoinIDs[Swap];
                                JoinIDs[Swap] = JoinIDs[I];
                                JoinIDs[I] = tempID;

                                string tempName = JoinedTrainer[Swap];
                                JoinedTrainer[Swap] = JoinedTrainer[I];
                                JoinedTrainer[I] = tempName;

                            }

                            byte[] A = { 05, 05, 05, 05, 01, 01, 01, 01 };
                            byte[] B = { 03, 03, 01, 01, 07, 07, 05, 05 };
                            byte[] C = { 01, 00, 03, 02, 05, 04, 07, 06 };

                            int YourIndex = JoinedTrainer.IndexOf("YOU");
                            int AIndex = A[YourIndex];
                            int BIndex = B[YourIndex];
                            int CIndex = C[YourIndex];

                            var WithoutYou = JoinedTrainer
                                .Where(name => name != "YOU")
                                .ToList();

                            temp = NextSeed(temp);
                            temp = NextSeed(temp);

                            for (ulong i = 0; i < 4; i++, temp = NextSeed(temp))
                            {
                                ulong SwapIndex = ((temp >> 32) * (4 - i)) >> 32;

                                int Swap = (int)(SwapIndex + 3);
                                int I = (int)(6 - i);
                                string tempName = WithoutYou[Swap];
                                WithoutYou[Swap] = WithoutYou[I];
                                WithoutYou[I] = tempName;

                            }

                            string[] WorldLeaders = new string[8];

                            WorldLeaders[YourIndex] = JoinedTrainer[YourIndex];
                            WorldLeaders[AIndex] = WithoutYou[0];
                            WorldLeaders[BIndex] = WithoutYou[1];
                            WorldLeaders[CIndex] = WithoutYou[2];

                            int Count = 3;
                            for (int i = 0; i < 8; i++)
                            {
                                if (WorldLeaders[i] == null)
                                {
                                    WorldLeaders[i] = WithoutYou[Count];
                                    Count++;
                                }
                            }

                            //-----------------------------------------
                            ulong maxY = (count - 1) / 7;
                            ulong x = 0;
                            ulong y = 0;
                            bool found = false;

                            for (ulong yTry = maxY; yTry <= maxY; yTry--)
                            {
                                ulong remaining = (count - 1) - 7 * yTry;
                                if (remaining % 2 == 0)
                                {
                                    x = remaining / 2;
                                    y = yTry;
                                    found = true;
                                    break;
                                }
                                if (yTry == 0) break;
                            }
                            //-----------------------------------------

                            bool hasNoExcluded = JoinedTrainer.All(name => !ExcludeList.Contains(name));

                            string PadDisplay(string s, int padWidth)
                            {
                                int width = s.Sum(c => (c <= 0x7F ? 1 : 2));
                                return s + new string(' ', Math.Max(0, padWidth - width));
                            }

                            if (hasNoExcluded)
                            {
                                Console.WriteLine($"{SeedData.Year}, {SeedData.Month}, {SeedData.Day}, {SeedData.Hour}, {SeedData.Minute}, {SeedData.Second}, 0x{SeedData.VCount:X2}, 0x{SeedData.Timer0:X4}, 0x{SeedData.Seed:X16}");
                                writer.WriteLine($"{SeedData.Year}, {SeedData.Month}, {SeedData.Day}, {SeedData.Hour}, {SeedData.Minute}, {SeedData.Second}, 0x{SeedData.VCount:X2}, 0x{SeedData.Timer0:X4}, 0x{SeedData.Seed:X16}");

                                if (found)
                                {
                                    Console.WriteLine($"Offset：{(count - 1)}, 受付：{x}, ボックス：{y}");
                                    writer.WriteLine($"Offset：{(count - 1)}, 受付：{x}, ボックス：{y}");
                                }
                                else 
                                {
                                    Console.WriteLine($"Offset：{(count - 1)}");
                                    writer.WriteLine($"Offset：{(count - 1)}");
                                }
                                Console.WriteLine($"PWTRNG：0x{PWTSeed:X16}");
                                writer.WriteLine($"PWTRNG：0x{PWTSeed:X16}");

                                for (int i = 0; i < 4; i++)
                                {
                                    string half1 = PadDisplay(WorldLeaders[i], 10);
                                    string half2 = PadDisplay(WorldLeaders[i + 4], 10);

                                    Console.WriteLine($"    {half1}{half2}");
                                    writer.WriteLine($"    {half1}{half2}");
                                }
                                Flag += 1;

                                Console.WriteLine("");
                                writer.WriteLine("");
                            }
                            if (CountFlag > 0 && CountFlag == Flag) return;
                        }
                    }
                    Console.WriteLine("======================================");
                }

            }while (Console.ReadKey().Key == ConsoleKey.R);
        }
        
        static ulong NextSeed(ulong PWTSeed)
        {
            ulong a = 0x5D588B656C078965;
            ulong b = 0x269EC3;
            ulong result = (a * PWTSeed + b) & 0xFFFFFFFFFFFFFFFF;
            return result;

        }
    }

}
