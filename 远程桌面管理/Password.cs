using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace 远程桌面管理
{
    internal class Password
    {
        private static byte[] s_aditionalEntropy = null;         //附加的加密因子，自定义

        public string Build(string plainText)
        {
            byte[] secret = Encoding.Unicode.GetBytes(plainText);
            byte[] encryptedSecret = Protect(secret);

            string res = string.Empty;
            foreach (byte b in encryptedSecret)
            {
                res += b.ToString("X2");                          //炒鸡坑爹的，转换16进制的一定要用2位，不然就像我一样被坑了半个多月
            }
            return res;
        }

        //加密方法
        private byte[] Protect(byte[] data)
        {
            try
            {
                // Encrypt the data using DataProtectionScope.CurrentUser. The result can be decrypted
                //  only by the same current user.
                return ProtectedData.Protect(data, s_aditionalEntropy, DataProtectionScope.LocalMachine);
            }
            catch (CryptographicException e)
            {
                return null;
            }
        }

        //解密方法
        private byte[] Unprotect(byte[] data)
        {
            try
            {
                //Decrypt the data using DataProtectionScope.CurrentUser.
                return ProtectedData.Unprotect(data, s_aditionalEntropy, DataProtectionScope.LocalMachine);
            }
            catch (CryptographicException e)
            {
                return null;
            }
        }
    }
}