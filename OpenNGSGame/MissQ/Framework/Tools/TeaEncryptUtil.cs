namespace MissQ.Tools
{
    public class TeaEncryptUtil
    {
        public const int SALT_LEN = 2;
        public const int ZERO_LEN = 7;

        /*pKey为16byte*/
        /*
        输入:pInBuf为密文格式,nInBufLen为pInBuf的长度是8byte的倍数; *pOutBufLen为接收缓冲区的长度
        特别注意*pOutBufLen应预置接收缓冲区的长度!
        输出:pOutBuf为明文(Body),pOutBufLen为pOutBuf的长度,至少应预留nInBufLen-10;
        返回值:如果格式正确返回TRUE;
        */
        /*TEA解密算法,CBC模式*/
        /*密文格式:PadLen(1byte)+Padding(var,0-7byte)+Salt(2byte)+Body(var byte)+Zero(7byte)*/
        public static bool oi_symmetry_decrypt2(byte[] pInBuf, int nInBufLen, byte[] pKey, byte[] pOutBuf, ref int pOutBufLen)
        {

            int nPadLen, nPlainLen;
            byte[] dest_buf = new byte[8];
            byte[] zero_buf = new byte[8];
            byte[] iv_pre_crypt = new byte[8];
            byte[] iv_cur_crypt = new byte[8];

            int dest_i, i, j;
            //byte[] pInBufBoundary;
            int nBufPos = 0;

            int inBufPos = 0;


            if ((nInBufLen % 8) != 0 || (nInBufLen < 16)) return false;


            TeaDecryptECB(pInBuf, inBufPos, pKey, dest_buf, nBufPos);

            nPadLen = dest_buf[0] & 0x7/*只要最低三位*/;

            /*密文格式:PadLen(1byte)+Padding(var,0-7byte)+Salt(2byte)+Body(var byte)+Zero(7byte)*/
            i = nInBufLen - 1/*PadLen(1byte)*/- nPadLen - SALT_LEN - ZERO_LEN; /*明文长度*/
            if (pOutBufLen < i || (i < 0)) return false;
            pOutBufLen = i;

            //pInBufBoundary = pInBuf + nInBufLen; /*输入缓冲区的边界，下面不能pInBuf>=pInBufBoundary*/


            for (i = 0; i < 8; i++)
                zero_buf[i] = 0;

            System.Buffer.BlockCopy(zero_buf, 0, iv_pre_crypt, 0, 8);
            System.Buffer.BlockCopy(pInBuf, inBufPos, iv_cur_crypt, 0, 8);

            inBufPos += 8;
            nBufPos += 8;

            dest_i = 1; /*dest_i指向dest_buf下一个位置*/


            /*把Padding滤掉*/
            dest_i += nPadLen;

            /*dest_i must <=8*/

            /*把Salt滤掉*/
            for (i = 1; i <= SALT_LEN;)
            {
                if (dest_i < 8)
                {
                    dest_i++;
                    i++;
                }
                else if (dest_i == 8)
                {
                    /*解开一个新的加密块*/

                    /*改变前一个加密块的指针*/
                    System.Buffer.BlockCopy(iv_cur_crypt, 0, iv_pre_crypt, 0, 8);
                    System.Buffer.BlockCopy(pInBuf, inBufPos, iv_cur_crypt, 0, 8);

                    /*异或前一块明文(在dest_buf[]中)*/
                    for (j = 0; j < 8; j++)
                    {
                        if ((nBufPos + j) >= nInBufLen)
                            return false;
                        dest_buf[j] ^= pInBuf[inBufPos + j];
                    }

                    /*dest_i==8*/
                    TeaDecryptECB(dest_buf, 0, pKey, dest_buf, 0);

                    /*在取出的时候才异或前一块密文(iv_pre_crypt)*/


                    inBufPos += 8;
                    nBufPos += 8;

                    dest_i = 0; /*dest_i指向dest_buf下一个位置*/
                }
            }

            /*还原明文*/

            int outBufPos = 0;

            nPlainLen = pOutBufLen;
            while (nPlainLen > 0)
            {
                if (dest_i < 8)
                {
                    int outPos = outBufPos++;
                    if (pOutBuf != null && pOutBuf.Length >= outPos)
                    {
                        pOutBuf[outBufPos++] = (byte)(dest_buf[dest_i] ^ iv_pre_crypt[dest_i]);
                    }                    
                    dest_i++;
                    nPlainLen--;
                }
                else if (dest_i == 8)
                {
                    /*dest_i==8*/

                    /*改变前一个加密块的指针*/
                    System.Buffer.BlockCopy(iv_cur_crypt, 0, iv_pre_crypt, 0, 8);
                    System.Buffer.BlockCopy(pInBuf, inBufPos, iv_cur_crypt, 0, 8);

                    /*解开一个新的加密块*/

                    /*异或前一块明文(在dest_buf[]中)*/
                    for (j = 0; j < 8; j++)
                    {
                        if ((nBufPos + j) >= nInBufLen)
                            return false;
                        dest_buf[j] ^= pInBuf[j + inBufPos];
                    }

                    TeaDecryptECB(dest_buf, 0, pKey, dest_buf, 0);

                    /*在取出的时候才异或前一块密文(iv_pre_crypt)*/


                    inBufPos += 8;
                    nBufPos += 8;

                    dest_i = 0; /*dest_i指向dest_buf下一个位置*/
                }
            }

            /*校验Zero*/
            for (i = 1; i <= ZERO_LEN;)
            {
                if (dest_i < 8)
                {
                    if ((dest_buf[dest_i] ^ iv_pre_crypt[dest_i]) != 0) return false;
                    dest_i++;
                    i++;
                }
                else if (dest_i == 8)
                {
                    /*改变前一个加密块的指针*/
                    System.Buffer.BlockCopy(iv_cur_crypt, 0, iv_pre_crypt, 0, 8);
                    System.Buffer.BlockCopy(pInBuf, inBufPos, iv_cur_crypt, 0, 8);

                    /*解开一个新的加密块*/

                    /*异或前一块明文(在dest_buf[]中)*/
                    for (j = 0; j < 8; j++)
                    {
                        if ((nBufPos + j) >= nInBufLen)
                            return false;
                        dest_buf[j] ^= pInBuf[j + inBufPos];
                    }

                    TeaDecryptECB(dest_buf, 0, pKey, dest_buf, 0);

                    /*在取出的时候才异或前一块密文(iv_pre_crypt)*/


                    inBufPos += 8;
                    nBufPos += 8;
                    dest_i = 0; /*dest_i指向dest_buf下一个位置*/
                }

            }

            return true;
        }



        static int rand()
        {
            return 0;
        }


        /*pKey为16char*/
        /*
        输入:nInBufLen为需加密的明文部分(Body)长度;
        输出:返回为加密后的长度(是8char的倍数);
        */
        /*TEA加密算法,CBC模式*/
        /*密文格式:PadLen(1char)+Padding(var,0-7char)+Salt(2char)+Body(var char)+Zero(7char)*/
        public static int oi_symmetry_encrypt2_len(int nInBufLen)
        {

            int nPadSaltBodyZeroLen/*PadLen(1char)+Salt+Body+Zero的长度*/;
            int nPadlen;

            /*根据Body长度计算PadLen,最小必需长度必需为8char的整数倍*/
            nPadSaltBodyZeroLen = nInBufLen/*Body长度*/+ 1 + SALT_LEN + ZERO_LEN/*PadLen(1char)+Salt(2char)+Zero(7char)*/;
            nPadlen = nPadSaltBodyZeroLen % 8;
            if (0 != nPadlen) /*len=nSaltBodyZeroLen%8*/
            {
                /*模8余0需补0,余1补7,余2补6,...,余7补1*/
                nPadlen = 8 - nPadlen;
            }

            return nPadSaltBodyZeroLen + nPadlen;
        }


        /*pKey为16char*/
        /*
            输入:pInBuf为需加密的明文部分(Body),nInBufLen为pInBuf长度;
            输出:pOutBuf为密文格式,pOutBufLen为pOutBuf的长度是8char的倍数;
        */
        /*TEA加密算法,CBC模式*/
        /*密文格式:PadLen(1char)+Padding(var,0-7char)+Salt(2char)+Body(var char)+Zero(7char)*/
        public static void oi_symmetry_encrypt2(byte[] pInBuf, int nInBufLen, byte[] pKey, byte[] pOutBuf, ref int pOutBufLen)
        {

            int nPadSaltBodyZeroLen/*PadLen(1char)+Salt+Body+Zero的长度*/;
            int nPadlen;
            byte[] src_buf = new byte[8];
            byte[] iv_plain = new byte[8];
            byte[] iv_crypt = new byte[8];
            int src_i, i, j;

            int outBufPos = 0;

            /*根据Body长度计算PadLen,最小必需长度必需为8char的整数倍*/
            nPadSaltBodyZeroLen = nInBufLen/*Body长度*/+ 1 + SALT_LEN + ZERO_LEN/*PadLen(1char)+Salt(2char)+Zero(7char)*/;
            nPadlen = nPadSaltBodyZeroLen % 8;
            if (0 != nPadlen) /*len=nSaltBodyZeroLen%8*/
            {
                /*模8余0需补0,余1补7,余2补6,...,余7补1*/
                nPadlen = 8 - nPadlen;
            }

            /*srand( (unsigned)time( NULL ) ); 初始化随机数*/
            /*加密第一块数据(8char),取前面10char*/
            src_buf[0] = (byte)((((byte)(rand()) & 0x0f8/*最低三位存PadLen,清零*/) | nPadlen));
            src_i = 1; /*src_i指向src_buf下一个位置*/

            while (nPadlen-- > 0)
                src_buf[src_i++] = (byte)rand(); /*Padding*/

            /*come here, src_i must <= 8*/

            for (i = 0; i < 8; i++)
                iv_plain[i] = 0;

            System.Buffer.BlockCopy(iv_plain, 0, iv_crypt, 0, 8);

            pOutBufLen = 0; /*init OutBufLen*/

            for (i = 1; i <= SALT_LEN;) /*Salt(2char)*/
            {
                if (src_i < 8)
                {
                    src_buf[src_i++] = (byte)rand();
                    i++; /*i inc in here*/
                }

                if (src_i == 8)
                {
                    /*src_i==8*/

                    for (j = 0; j < 8; j++) /*加密前异或前8个char的密文(iv_crypt指向的)*/
                        src_buf[j] ^= iv_crypt[j];

                    /*pOutBuffer、pInBuffer均为8char, pKey为16char*/
                    /*加密*/
                    TeaEncryptECB(src_buf, 0, pKey, pOutBuf, outBufPos);

                    if (pOutBuf != null)
                    {
                        for (j = 0; j < 8; j++) /*加密后异或前8个char的明文(iv_plain指向的)*/
                            pOutBuf[j + outBufPos] ^= iv_plain[j];
                    }

                    /*保存当前的iv_plain*/
                    for (j = 0; j < 8; j++)
                        iv_plain[j] = src_buf[j];

                    /*更新iv_crypt*/
                    src_i = 0;
                    System.Buffer.BlockCopy(pOutBuf, outBufPos, iv_crypt, 0, 8);
                    pOutBufLen += 8;
                    outBufPos += 8;
                }
            }


            int inBufPos = 0;

            /*src_i指向src_buf下一个位置*/

            while (nInBufLen > 0)
            {
                if (src_i < 8)
                {
                    src_buf[src_i++] = pInBuf[inBufPos++];
                    nInBufLen--;
                }

                if (src_i == 8)
                {
                    /*src_i==8*/

                    for (j = 0; j < 8; j++) /*加密前异或前8个char的密文(iv_crypt指向的)*/
                        src_buf[j] ^= iv_crypt[j];
                    /*pOutBuffer、pInBuffer均为8char, pKey为16char*/
                    TeaEncryptECB(src_buf, 0, pKey, pOutBuf, outBufPos);

                    for (j = 0; j < 8; j++) /*加密后异或前8个char的明文(iv_plain指向的)*/
                        pOutBuf[outBufPos + j] ^= iv_plain[j];

                    /*保存当前的iv_plain*/
                    for (j = 0; j < 8; j++)
                        iv_plain[j] = src_buf[j];

                    src_i = 0;
                    System.Buffer.BlockCopy(pOutBuf, outBufPos, iv_crypt, 0, 8);
                    pOutBufLen += 8;
                    outBufPos += 8;
                }
            }

            /*src_i指向src_buf下一个位置*/

            for (i = 1; i <= ZERO_LEN;)
            {
                if (src_i < 8)
                {
                    src_buf[src_i++] = 0;
                    i++; /*i inc in here*/
                }

                if (src_i == 8)
                {
                    /*src_i==8*/

                    for (j = 0; j < 8; j++) /*加密前异或前8个char的密文(iv_crypt指向的)*/
                        src_buf[j] ^= iv_crypt[j];
                    /*pOutBuffer、pInBuffer均为8char, pKey为16char*/
                    TeaEncryptECB(src_buf, 0, pKey, pOutBuf, outBufPos);

                    for (j = 0; j < 8; j++) /*加密后异或前8个char的明文(iv_plain指向的)*/
                        pOutBuf[j + outBufPos] ^= iv_plain[j];

                    /*保存当前的iv_plain*/
                    for (j = 0; j < 8; j++)
                        iv_plain[j] = src_buf[j];

                    src_i = 0;
                    System.Buffer.BlockCopy(pOutBuf, outBufPos, iv_crypt, 0, 8);
                    pOutBufLen += 8;
                    outBufPos += 8;
                }
            }

        }




        const uint DELTA = 0x9e3779b9;

        const int ROUNDS = 16;
        const int LOG_ROUNDS = 4;


        /*pOutBuffer、pInBuffer均为8char, pKey为16char*/
        static void TeaEncryptECB(byte[] pInBuf, int inBufPos, byte[] pKey, byte[] pOutBuf, int outBufIndex)
        {
            uint y, z;
            uint sum;
            uint[] k = new uint[4];
            int i;


            /*plain-text is TCP/IP-endian;*/

            /*GetBlockBigEndian(in, y, z);*/
            y = (uint)System.Net.IPAddress.NetworkToHostOrder(System.BitConverter.ToInt32(pInBuf, 0));
            z = (uint)System.Net.IPAddress.NetworkToHostOrder(System.BitConverter.ToInt32(pInBuf, 4));
            /*TCP/IP network char order (which is big-endian).*/

            for (i = 0; i < 4; i++)
            {
                /*now key is TCP/IP-endian;*/
                k[i] = (uint)System.Net.IPAddress.NetworkToHostOrder(System.BitConverter.ToInt32(pKey, i * 4));
            }

            sum = 0;
            for (i = 0; i < ROUNDS; i++)
            {
                sum += DELTA;
                y += ((z << 4) + k[0]) ^ (z + sum) ^ ((z >> 5) + k[1]);
                z += ((y << 4) + k[2]) ^ (y + sum) ^ ((y >> 5) + k[3]);
            }

            System.BitConverter.GetBytes(System.Net.IPAddress.HostToNetworkOrder((int)y)).CopyTo(pOutBuf, outBufIndex);
            System.BitConverter.GetBytes(System.Net.IPAddress.HostToNetworkOrder((int)z)).CopyTo(pOutBuf, outBufIndex + 4);

            /*now encrypted buf is TCP/IP-endian;*/
        }


        /*pOutBuffer、pInBuffer均为8byte, pKey为16byte*/
        static void TeaDecryptECB(byte[] pInBuf, int inBufIndex, byte[] pKey, byte[] pOutBuf, int outBufIndex)
        {
            uint y, z, sum;
            uint[] k = new uint[4];
            int i;

            /*now encrypted buf is TCP/IP-endian;*/
            /*TCP/IP network byte order (which is big-endian).*/

            y = (uint)System.Net.IPAddress.NetworkToHostOrder(System.BitConverter.ToInt32(pInBuf, 0));
            z = (uint)System.Net.IPAddress.NetworkToHostOrder(System.BitConverter.ToInt32(pInBuf, 4));


            for (i = 0; i < 4; i++)
            {
                /*key is TCP/IP-endian;*/
                k[i] = (uint)System.Net.IPAddress.NetworkToHostOrder(System.BitConverter.ToInt32(pKey, i * 4));
            }

            sum = DELTA << LOG_ROUNDS;
            for (i = 0; i < ROUNDS; i++)
            {
                z -= (y << 4) + k[2] ^ y + sum ^ (y >> 5) + k[3];
                y -= (z << 4) + k[0] ^ z + sum ^ (z >> 5) + k[1];
                sum -= DELTA;
            }

            System.BitConverter.GetBytes(System.Net.IPAddress.HostToNetworkOrder((int)y)).CopyTo(pOutBuf, outBufIndex);
            System.BitConverter.GetBytes(System.Net.IPAddress.HostToNetworkOrder((int)z)).CopyTo(pOutBuf, outBufIndex + 4);

            /*now plain-text is TCP/IP-endian;*/
        }
    }
}