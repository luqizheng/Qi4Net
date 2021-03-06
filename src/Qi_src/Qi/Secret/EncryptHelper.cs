﻿using System;
using System.Security.Cryptography;
using System.Text;

namespace Qi.Secret
{
    /// <summary>
    /// 
    /// </summary>
    public static class EncryptHelper
    {
        #region Md5
        /// <summary>
        /// 
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public static byte[] Md5Utf8(this string content)
        {
            return Encrypt(content, Encoding.UTF8, new MD5CryptoServiceProvider());
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public static byte[] Md5Utf7(this string content)
        {
            return Encrypt(content, Encoding.UTF7, new MD5CryptoServiceProvider());
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public static byte[] Md5ASCII(this string content)
        {
            return Encrypt(content, Encoding.ASCII, new MD5CryptoServiceProvider());
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public static byte[] Md5Unicode(this string content)
        {
            return Encrypt(content, Encoding.Unicode, new MD5CryptoServiceProvider());
        }

        #endregion

        #region Sha1

        /// <summary>
        ///用sha1 utf8进行加密
        /// </summary>
        public static byte[] Sha1Utf8(this string content)
        {
            return Encrypt(content, Encoding.UTF8, new SHA1CryptoServiceProvider());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public static byte[] Sha1ASCII(this string content)
        {
            return Encrypt(content, Encoding.ASCII, new SHA1CryptoServiceProvider());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public static byte[] Sha1Unicode(this string content)
        {
            return Encrypt(content, Encoding.Unicode, new SHA1CryptoServiceProvider());
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public static byte[] Sha1Utf7(this string content)
        {
            return Encrypt(content, Encoding.UTF7, new SHA1CryptoServiceProvider());
        }

        #endregion

        #region sha256

        /// <summary>
        /// 
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public static byte[] Sha256Utf8(this string content)
        {
            return Encrypt(content, Encoding.UTF8, new SHA256CryptoServiceProvider());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public static byte[] Sha256ASCII(this string content)
        {
            return Encrypt(content, Encoding.ASCII, new SHA256CryptoServiceProvider());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public static byte[] Sha256Unicode(this string content)
        {
            return Encrypt(content, Encoding.Unicode, new SHA256CryptoServiceProvider());
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public static byte[] Sha256Utf7(this string content)
        {
            return Encrypt(content, Encoding.UTF7, new SHA256CryptoServiceProvider());
        }

        #endregion

        #region sha384

        /// <summary>
        /// 
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public static byte[] Sha384Utf8(this string content)
        {
            return Encrypt(content, Encoding.UTF8, new SHA384CryptoServiceProvider());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public static byte[] Sha384ASCII(this string content)
        {
            return Encrypt(content, Encoding.ASCII, new SHA384CryptoServiceProvider());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public static byte[] Sha384Unicode(this string content)
        {
            return Encrypt(content, Encoding.Unicode, new SHA384CryptoServiceProvider());
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public static byte[] Sha384Utf7(this string content)
        {
            return Encrypt(content, Encoding.UTF7, new SHA384CryptoServiceProvider());
        }

        #endregion

        #region sha512

        /// <summary>
        /// 
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public static byte[] Sha512Utf8(this string content)
        {
            return Encrypt(content, Encoding.UTF8, new SHA512CryptoServiceProvider());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public static byte[] Sha512ASCII(this string content)
        {
            return Encrypt(content, Encoding.ASCII, new SHA512CryptoServiceProvider());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public static byte[] Sha512Unicode(this string content)
        {
            return Encrypt(content, Encoding.Unicode, new SHA512CryptoServiceProvider());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public static byte[] Sha512Utf7(this string content)
        {
            return Encrypt(content, Encoding.UTF7, new SHA512CryptoServiceProvider());
        }

        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="content">加密的方法</param>
        /// <param name="getBytesFunc">把content转换为byte的方法</param>
        /// <param name="hashAlgorithm">加密的方式</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">Content is null</exception>
        ///  <exception cref="ArgumentNullException">getBytesFunc is null</exception>
        ///  <exception cref="ArgumentNullException">hashAlgorithm is null</exception>
        public static byte[] Encrypt(string content, Encoding getBytesFunc, HashAlgorithm hashAlgorithm)
        {
            if (content == null) throw new ArgumentNullException("content");
            if (getBytesFunc == null) throw new ArgumentNullException("getBytesFunc");
            if (hashAlgorithm == null) throw new ArgumentNullException("hashAlgorithm");
            byte[] iput = getBytesFunc.GetBytes(content);
            return hashAlgorithm.ComputeHash(iput);
        }
    }
}