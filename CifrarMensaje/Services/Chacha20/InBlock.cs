using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CifrarMensaje.Services.Chacha20
{
    public class InBlock
    {
        private readonly uint[] constants = { 0x61707865, 0x3320646e, 0x79622d32, 0x6b206574 };
        private readonly byte[] nonce = new byte[12] {
            0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 
            0x00, 0x00, 0x00, 0x01   
         };

        /*

            +---------+---------+---------+---------+
            | Const   | Const   | Const   | Const   |
            +---------+---------+---------+---------+
            | Key[0]  | Key[1]  | Key[2]  | Key[3]  |
            +---------+---------+---------+---------+
            | Key[4]  | Key[5]  | Key[6]  | Key[7]  |
            +---------+---------+---------+---------+
            | Counter | Nonce[0]| Nonce[1]| Nonce[2]|
            +---------+---------+---------+---------+


        */

        // El bloque debe ser 512 bit o lo que es igual 16 bytes
        public uint[] InitializeBlock(byte[] key)
        {
            //La clave secreta corresponde a 256 bits
            //Nonce 64 bits
            //Contador 64 bits

            uint[] block = new uint[16];

            block[0] = constants[0];
            block[1] = constants[1];
            block[2] = constants[2];
            block[3] = constants[3];
            block[4] = BitConverter.ToUInt32(key, 0);
            block[5] = BitConverter.ToUInt32(key, 4);
            block[6] = BitConverter.ToUInt32(key, 8);
            block[7] = BitConverter.ToUInt32(key, 12);
            block[8] = BitConverter.ToUInt32(key, 16);
            block[9] = BitConverter.ToUInt32(key, 20);
            block[10] = BitConverter.ToUInt32(key, 24);
            block[11] = BitConverter.ToUInt32(key, 28);
            block[12] = 0; // Contador inicial a 0
            block[13] = BitConverter.ToUInt32(nonce, 0);
            block[14] = BitConverter.ToUInt32(nonce, 4);
            block[15] = BitConverter.ToUInt32(nonce, 8);

            return block;
        }
    }
}
