using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.IO;
using System.Numerics;

namespace BBS
{
    class Program
    {
        static BigInteger[] GetIntFromFile(string filepath)
        {
            var fileStream = new FileStream(filepath, FileMode.Open, FileAccess.Read);
            using (var streamReader = new StreamReader(fileStream, Encoding.UTF8))
            {
                BigInteger p = BigInteger.Parse(streamReader.ReadLine());
                BigInteger q = BigInteger.Parse(streamReader.ReadLine());
                return new BigInteger[] { p, q };
            }
            throw new Exception("С файлом : " + filepath + " возникла проблема");
        }


        static byte[] GetSeriesFromFile(string filepath)
        {
            var fileStream = new FileStream(filepath, FileMode.Open, FileAccess.Read);
            List<byte> series = new List<byte>();
            using (var streamReader = new StreamReader(fileStream, Encoding.UTF8))
            {
                while (!streamReader.EndOfStream)
                {
                    series.Add(byte.Parse(streamReader.ReadLine()));
                }
                return series.ToArray();
            }

            throw new Exception("С файлом: " + filepath + " возникла проблема");
        }


        static void ShowArray<T>(T[] arr)
        {
            for (int i = 0; i < arr.Length - 1; i++)
            {
                Console.Write(arr[i]);
                Console.Write(", ");
            }
            Console.Write(arr[arr.Length - 1]);
        }

        static void PutIntToFile(string filepath, byte[] seqs)
        {
            var fileStream = new FileStream(filepath, FileMode.OpenOrCreate, FileAccess.Write);
            using (var streamWriter = new StreamWriter(fileStream, Encoding.UTF8))
            {
                foreach (var el in seqs)
                {
                    streamWriter.WriteLine(el.ToString());
                }
                return;
            }
            throw new Exception("С файлом: " + filepath + " возникла проблема");

        }

        static BigInteger GCD(BigInteger x, BigInteger y)
        {
            while (y != 0)
            {
                var t = y;
                y = x % y;
                x = t;
            }
            return x;
        }


        static byte[] BBS(BigInteger p, BigInteger q, int m)
        {
            if (p % 4 != 3 || q % 4 != 3)
                throw new Exception("Выбранный простые числа не подходят");
            BigInteger N = p * q;
            Random random = new Random();
            byte[] series = new byte[m];
            int s = random.Next(1, (int)N - 1);
            while (GCD(s, N) != 1)
            {
                s = random.Next(1, (int)N - 1);
            }
            BigInteger u = BigInteger.ModPow(s, 2, N);
            for (int i = 0; i < m; i++)
            {
                series[i] = (byte)(u % 2);
                u = BigInteger.ModPow(u, 2, N);
            }
            return series;
        }



        static bool TestOnFrequance(string filename, bool show = true)
        {
            byte[] series = GetSeriesFromFile(filename);
            int sum = 0;
            for (int i = 0; i < series.Length; i++)
            {
                sum += 2 * series[i] - 1;
            }
            double s = Math.Abs(sum) / Math.Sqrt(series.Length);
            if (show)
            {

                Console.Write("\ns = ");
                Console.Write(s);
                Console.WriteLine();
                if (s <= 1.82138636)
                {
                    Console.WriteLine("Последовательность прошла проверку частотности");
                }
                else Console.WriteLine("Последовательность не прошла проверку частотности");

            }
            return s <= 1.82138636;
        }

        static bool TestOnSubseq(string filename, bool show = true)
        {
            byte[] series = GetSeriesFromFile(filename);
            int piSum = 0;
            for (int i = 0; i < series.Length; i++)
            {
                piSum += series[i];
            }
            double piS = (piSum + 0.0) / series.Length;
            int Vn = 1;
            for (int i = 1; i < series.Length - 1; i++)
            {
                Vn += series[i] == series[i + 1] ? 1 : 0;
            }
            double s = Math.Abs(Vn - 2 * piS * series.Length * (1 - piS)) /
                (2 * Math.Sqrt(2 * series.Length) * piS * (1 - piS));
            if (show)
            {

                Console.Write("\ns = ");
                Console.Write(s);
                Console.WriteLine();
                if (s <= 1.82138636)
                {
                    Console.WriteLine("Последовательность прошла проверку на подпоследовательность");
                }
                else Console.WriteLine("Последовательность не прошла проверку на подпоследовательность");

            }
            return s <= 1.82138636;
        }


        static bool TestOnDispersion(string filename, bool show = true)
        {

            byte[] series = GetSeriesFromFile(filename);
            int[] Ss = new int[series.Length + 2];
            Ss[0] = 0;
            for (int i = 0; i < series.Length; i++)
            {
                for (int j = i; j >= 0; j--)
                {
                    Ss[i + 1] += 2 * series[j] - 1;
                }
            }
            int k = 0;
            for (int i = 0; i < Ss.Length; i++)
            {
                if (Ss[i] == 0) k++;
            }
            int L = k - 1;
            int[] ksij = new int[19];
            double[] Ys = new double[19];
            for (int j = -9; j <= 9; j++)
            {
                if (j == 0) continue;
                for (int i = 0; i < Ss.Length; i++)
                {
                    if (Ss[i] == j) ksij[j + 9]++;
                }
                Ys[j + 9] = Math.Abs(ksij[j + 9] - L) /
                    Math.Sqrt(2 * L * (4 * Math.Abs(j) - 2));
            }
            bool result = true;
            for (int i = 0; i < Ys.Length; i++)
            {
                if (Ys[i] > 1.82138636) result = false;
            }
            if (show)
            {
                Console.Write("\nS' = ");
                ShowArray(Ss);
                Console.Write("\nL = ");
                Console.Write(L);
                Console.Write("\nksij = ");
                ShowArray(ksij);
                Console.Write("\nYj = ");
                ShowArray(Ys);
                Console.WriteLine();
            }
            if (result)
            {
                Console.WriteLine("Последовательность прошла проверку на  произвольные отклонения");
            }
            else Console.WriteLine("Последовательность не прошла проверку на  произвольные отклонения");
            return result;
        }

        static void Main()
        {
            string filepathDir = @"D:\Projects\CSharp\BBSU\BBSU\";
            string filepathOut = filepathDir + @"OUT.txt";
            string filepathIn = filepathDir + @"IN.txt";
            BigInteger[] pq = GetIntFromFile(filepathIn);
            byte[] series = BBS(pq[0], pq[1], 200);
            //PutIntToFile(filepathOut, series);

            byte[] test = GetSeriesFromFile(filepathOut);
            ShowArray(test);
            TestOnFrequance(filepathOut);
            TestOnSubseq(filepathOut);
            TestOnDispersion(filepathOut);
            Console.ReadKey();
            Console.ReadLine();
        }
    }
}
