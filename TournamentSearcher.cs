using PWT_RNG;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime;
using System.Text;
using System.Threading.Tasks;

namespace PWT_test
{
    internal class TournamentSearcher
    {
        static void Main(string[] args)
        {

            {
                Console.WriteLine("======================================");
                Console.Write("PWT RNG Tool\n");
                Console.Write("Initial SEED : 0x");   // 初期SEED入力

                    if (uint.TryParse(Console.ReadLine(), System.Globalization.NumberStyles.HexNumber, null, out uint Seed))
                    {
                        Console.Write("Entry Count：");  //受付回数入力
                        if (uint.TryParse(Console.ReadLine(), out uint count) && count > 0)
                        {
                            string outputPath = $"PWT RNG.txt";
                            using (StreamWriter writer = new(outputPath))
                            {

                                var PWT = new PWTSeed(Seed, count);
                                ulong PWTSeed = PWT.PWTRNG();
                                Console.WriteLine($"PWTSeed：0x{PWTSeed:X16}");

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

                                string[] TrainerName = {"チェレン","ホミカ","アーティ","カミツレ","ヤーコン","フウロ","ジャガ","シズイ","ベル","ポッド",
                                    "コーン","デント","アロエ","ハチク","グリーン","ワタル","ダイゴ","ミクリ","シロナ","アデク",
                                    "タケシ","カスミ","マチス","エリカ","ナツメ","カツラ","サカキ","ハヤト","ツクシ","アカネ",
                                    "マツバ","シジマ","ミカン","ヤナギ","イブキ","アンズ","ツツジ","トウキ","テッセン","アスナ",
                                    "センリ","ナギ","フウ","ラン","アダン","ヒョウタ","ナタネ","メリッサ","スモモ","マキシ",
                                    "トウガン","スズナ","デンジ","レッド",};

                                List<string> JoinedTrainer = JoinIDs.ConvertAll(id => TrainerName[(ulong)id]);

                                Console.WriteLine("参加トレーナー");

                                for (int i = 0; i < JoinedTrainer.Count; i++)
                                {
                                    Console.WriteLine($"{i + 1}.{JoinedTrainer[i]}");
                                }

                                Console.WriteLine($"PWTSeed：0x{PWTSeed:X16}");

                                Console.WriteLine("======================================");
                            }
                        }
                    }
            }
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
