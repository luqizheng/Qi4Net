using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
namespace Qi.Secret
{
    public static class DesHelper
    {
        public static byte[] EncryptByDes(this string content, Encoding getByteFunc, string rgbKey, string rgbIv)
        {
            if (rgbIv.Length < 8)
                throw new ArgumentOutOfRangeException("rgbKey", "rgbIv need to more than 8");
            if (rgbKey.Length < 8)
                throw new ArgumentOutOfRangeException("rgbIv", "rgbIv need to more than 8");
            using (var encrp = new DESCryptoServiceProvider())
            {
                using (var ms = new MemoryStream())
                {
                    byte[] inData = Encoding.Default.GetBytes(content);
                    ICryptoTransform encode = encrp.CreateEncryptor(getByteFunc.GetBytes(rgbKey),
                                                                    getByteFunc.GetBytes(rgbIv));
                    using (var cs = new CryptoStream(ms, encode, CryptoStreamMode.Write))
                    {
                        cs.Write(inData, 0, inData.Length);
                        cs.FlushFinalBlock();
                    }
                    return ms.ToArray();
                }
            }
        }

        public static byte[] DecryptByDes(this byte[] inData, Encoding getByteFunc, string rgbKey, string rgbIv)
        {
            if (rgbIv.Length < 8)
                throw new ArgumentOutOfRangeException("rgbKey", "rgbIv need to more than 8");
            if (rgbKey.Length < 8)
                throw new ArgumentOutOfRangeException("rgbIv", "rgbIv need to more than 8");
            using (var encrp = new DESCryptoServiceProvider())
            {
                using (var ms = new MemoryStream())
                {
                    ICryptoTransform encode = encrp.CreateDecryptor(getByteFunc.GetBytes(rgbKey),
                                                                    getByteFunc.GetBytes(rgbIv));
                    using (var cs = new CryptoStream(ms, encode, CryptoStreamMode.Write))
                    {
                        cs.Write(inData, 0, inData.Length);
                        cs.FlushFinalBlock();
                    }
                    return ms.ToArray();
                }
            }
        }

        public static void EncryptFileByDes(this FileInfo sourceFile, string destFile, string rgbKey,
                                            string rgbIv)
        {
            sourceFile.EncryptFileByDes(destFile, Encoding.Default, rgbKey, rgbIv);
        }

        public static void EncryptFileByDes(this FileInfo sourceFile, string destFile, Encoding getByteFunc,
                                            string rgbKey,
                                            string rgbIv)
        {
            if (!sourceFile.Exists)
                throw new FileNotFoundException("File not fine", sourceFile.FullName);

            byte[] btKey = getByteFunc.GetBytes(rgbKey);
            byte[] btIV = getByteFunc.GetBytes(rgbIv);
            var des = new DESCryptoServiceProvider();
            byte[] btFile = File.ReadAllBytes(sourceFile.FullName);

            using (var fs = new FileStream(destFile, FileMode.Create, FileAccess.Write))
            {
                using (var cs = new CryptoStream(fs, des.CreateEncryptor(btKey, btIV), CryptoStreamMode.Write))
                {
                    cs.Write(btFile, 0, btFile.Length);
                    cs.FlushFinalBlock();
                }
            }
        }

        public static void DecryptFileByDes(this FileInfo sourceFile, string destFile, string rgbKey, string rgbIv)
        {
            sourceFile.DecryptFileByDes(destFile, Encoding.Default, rgbKey, rgbIv);
        }

        public static void DecryptFileByDes(this FileInfo sourceFile, string destFile, Encoding getByteFunc,
                                            string rgbKey, string rgbIv)
        {
            if (!sourceFile.Exists) throw new FileNotFoundException("Fiel not file.", sourceFile.FullName);

            byte[] btKey = Encoding.Default.GetBytes(rgbKey);
            byte[] btIV = Encoding.Default.GetBytes(rgbIv);
            var des = new DESCryptoServiceProvider();
            byte[] btFile = File.ReadAllBytes(sourceFile.FullName);

            using (var fs = new FileStream(destFile, FileMode.Create, FileAccess.Write))
            {
                using (var cs = new CryptoStream(fs, des.CreateDecryptor(btKey, btIV), CryptoStreamMode.Write))
                {
                    cs.Write(btFile, 0, btFile.Length);
                    cs.FlushFinalBlock();
                }
            }
        }
    }
}