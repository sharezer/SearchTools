using UnityEngine;
using System.Collections;
using System.Text;
using System.Security.Cryptography;
using System;
using System.IO;

namespace LsSearch
{
    class RegisterHelper
    {
        private static string guid_key = "LSUUID";
        public static bool isRegister()
        {
            string guid = PlayerPrefs.GetString(guid_key);

            if (string.IsNullOrEmpty(guid))
                return false;

            guid = EncAndDec.DecodeClientLocal(guid);
            if (guid == SystemInfo.deviceUniqueIdentifier)
                return true;
            else
                return false;
        }

        public static void DoRegister()
        {
            string guid = SystemInfo.deviceUniqueIdentifier;
            string result = EncAndDec.EncodeClientLocal(guid);
            PlayerPrefs.SetString(guid_key, result);
        }
    }

    /// <summary>
    /// 加密/解密类
    /// </summary>
    public class EncAndDec
    {
        //加密/解密钥匙
        const string KEY_64 = "sharezer";//注意了，是8个字符，64位    
        const string IV_64 = "86987785";//注意了，是8个字符，64位

        const string ClientLocal_KEY_64 = "sharezer";
        const string ClientLocal_IV_64 = "86987785";

        /// <summary>
        /// 加密的方法，通过2个密匙进行加密
        /// </summary>
        /// <param name="data">加密的数据</param>
        /// <returns>返回加密后的字符串</returns>
        public static string Encode(string data)
        {
            EncAndDec ed = new EncAndDec();
            return ed.Encode(data, KEY_64, IV_64);
        }
        /// <summary>
        /// 解密的方法
        /// </summary>
        /// <param name="data">解密的数据</param>
        /// <returns>返回加密前的字符串</returns>
        public static string Decode(string data)
        {
            EncAndDec ed = new EncAndDec();
            return ed.Decode(data, KEY_64, IV_64);
        }

        /// <summary>
        /// 客户本地加密的方法，通过2个密匙进行加密
        /// </summary>
        /// <param name="data">加密的数据</param>
        /// <returns>返回加密后的字符串</returns>
        public static string EncodeClientLocal(string data)
        {
            EncAndDec ed = new EncAndDec();
            return ed.Encode(data, ClientLocal_KEY_64, ClientLocal_IV_64);
        }
        /// <summary>
        /// 客户本地解密的方法
        /// </summary>
        /// <param name="data">解密的数据</param>
        /// <returns>返回加密前的字符串</returns>
        public static string DecodeClientLocal(string data)
        {
            EncAndDec ed = new EncAndDec();
            return ed.Decode(data, ClientLocal_KEY_64, ClientLocal_IV_64);
        }

        #region DEC加密的方法
        /// <summary>
        /// 加密的方法，通过2个密匙进行加密
        /// </summary>
        /// <param name="data">通过Md5加密一次</param>
        /// <param name="KEY_64"></param>
        /// <param name="IV_64"></param>
        /// <returns></returns>
        private string Encode(string data, string KEY_64, string IV_64)
        {

            KEY_64 = ToMD5(KEY_64);
            IV_64 = ToMD5(IV_64);
            byte[] byKey = System.Text.ASCIIEncoding.ASCII.GetBytes(KEY_64);
            byte[] byIV = System.Text.ASCIIEncoding.ASCII.GetBytes(IV_64);

            DESCryptoServiceProvider cryptoProvider = new DESCryptoServiceProvider();
            int i = cryptoProvider.KeySize;
            MemoryStream ms = new MemoryStream();
            CryptoStream cst = new CryptoStream(ms, cryptoProvider.CreateEncryptor(byKey, byIV), CryptoStreamMode.Write);

            StreamWriter sw = new StreamWriter(cst);
            sw.Write(data);
            sw.Flush();
            cst.FlushFinalBlock();
            sw.Flush();
            return Convert.ToBase64String(ms.GetBuffer(), 0, (int)ms.Length);

        }
        /// <summary>
        /// 解密的方法（）
        /// </summary>
        /// <param name="data"></param>
        /// <param name="KEY_64"></param>
        /// <param name="IV_64"></param>
        /// <returns></returns>
        private string Decode(string data, string KEY_64, string IV_64)
        {
            KEY_64 = ToMD5(KEY_64);
            IV_64 = ToMD5(IV_64);
            byte[] byKey = System.Text.ASCIIEncoding.ASCII.GetBytes(KEY_64);
            byte[] byIV = System.Text.ASCIIEncoding.ASCII.GetBytes(IV_64);

            byte[] byEnc;
            try
            {
                byEnc = Convert.FromBase64String(data);
            }
            catch
            {
                return null;
            }

            DESCryptoServiceProvider cryptoProvider = new DESCryptoServiceProvider();
            MemoryStream ms = new MemoryStream(byEnc);
            CryptoStream cst = new CryptoStream(ms, cryptoProvider.CreateDecryptor(byKey, byIV), CryptoStreamMode.Read);
            StreamReader sr = new StreamReader(cst);
            return sr.ReadToEnd();
        }
        #endregion

        #region MD5加密
        /// <summary>
        /// 转换MD5密码
        /// </summary>
        /// <param name="pass"></param>
        /// <returns></returns>
        public static string ToMD5(string KEY)
        {
            byte[] result = Encoding.Default.GetBytes(KEY);
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] output = md5.ComputeHash(result);

            string KEY_64 = BitConverter.ToString(output).Replace("-", "").Substring(0, 8);
            return KEY_64;

        }
        #endregion

    }

}
