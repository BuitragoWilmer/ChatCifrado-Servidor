using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CifrarMensaje.Services.Chacha20
{
    internal class Chacha20
    {
        private const int ROUNDS = 20;
        private int _round = 0;
        private const string filePath = "chacha20_log.txt";
        private static uint ROTL(uint a, int b)
        {
            return (a << b) | (a >> (32 - b));
        }

        private static void QR(ref uint a, ref uint b, ref uint c, ref uint d)
        {
            a += b; d ^= a; d = ROTL(d, 16);
            c += d; b ^= c; b = ROTL(b, 12);
            a += b; d ^= a; d = ROTL(d, 8);
            c += d; b ^= c; b = ROTL(b, 7);
        }

        public uint[] ChaChaBlock( uint[] inBlock)
        {

            uint[] x = new uint[16];
            uint[] outBlock = new uint[16];
            Array.Copy(inBlock, x, 16); // Copy input to x

            // 10 loops × 2 rounds/loop = 20 rounds
            for (int i = 0; i < ROUNDS; i += 2)
            {
                LogRoundsToFile(x, $"Ronda {_round + 1}:");
                // Odd round
                QR(ref x[0], ref x[4], ref x[8], ref x[12]); // column 1
                QR(ref x[1], ref x[5], ref x[9], ref x[13]); // column 2
                QR(ref x[2], ref x[6], ref x[10], ref x[14]); // column 3
                QR(ref x[3], ref x[7], ref x[11], ref x[15]); // column 4

                // Even round
                QR(ref x[0], ref x[5], ref x[10], ref x[15]); // diagonal 1 (main diagonal)
                QR(ref x[1], ref x[6], ref x[11], ref x[12]); // diagonal 2
                QR(ref x[2], ref x[7], ref x[8], ref x[13]); // diagonal 3
                QR(ref x[3], ref x[4], ref x[9], ref x[14]); // diagonal 4

            }

            for (int i = 0; i < 16; ++i)
            {
                outBlock[i] = x[i] + inBlock[i];
            }

            LogRoundsToFile(x, $"Bloque final");

            return outBlock;
        }

        private  void LogRoundsToFile(uint[] block, string mensaje)
        {
            using (StreamWriter writer = new StreamWriter(filePath, true))
            {

                    writer.WriteLine(mensaje);


                    // Escribir la matriz en formato de 4 columnas y 4 filas
                    for (int i = 0; i < 16; i += 4)
                    {
                        writer.WriteLine($"{block[i]:x8}  {block[i + 1]:x8}  {block[i + 2]:x8}  {block[i + 3]:x8}");
                }

                writer.WriteLine(); // Espacio entre rondas
                _round++;

            }
        }
    }
}
