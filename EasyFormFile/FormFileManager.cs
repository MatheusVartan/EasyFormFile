using System;
using System.IO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EasyFormFile
{
    /// <summary>
    /// A static class that handle IFormFiles.
    /// </summary>
    public static class FormFileManager
    {
        /// <summary>
        /// Static String containing the type of IFormFile content. The content is required to convert a byte[] to IFormFile.
        /// </summary>
        public static string ContentType { get; set; }

        /// <summary>
        /// Converts a collection of IFormFile in a collection of byte[]
        /// </summary>
        /// <param name="iFormFiles">The collection of IFormFile to be converted</param>
        /// <returns>Returns a collection of byte[]</returns>
        public static ICollection<byte[]> ToByteArrays(this ICollection<IFormFile> iFormFiles)
        {
            ICollection<byte[]> byteArrayCollection = new List<byte[]>();
            foreach (var iFormFile in iFormFiles)
                byteArrayCollection.Add(iFormFile.ToByteArray());
            return byteArrayCollection;
        }

        /// <summary>
        /// Converts a IFormFile to byte[]
        /// </summary>
        /// <param name="iFormFile">The IFormFile to be converted</param>
        /// <returns>Returns a byte[]</returns>
        public static byte[] ToByteArray(this IFormFile iFormFile)
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
                iFormFile.OpenReadStream().CopyTo(memoryStream);
                ContentType = iFormFile.ContentType;
                return memoryStream.ToArray();
            }
        }

        /// <summary>
        /// Asynchronously converts a collection of IFormFile in a collection of byte[]
        /// </summary>
        /// <param name="iFormFiles">The collection of IFormFile to be converted</param>
        /// <returns>Returns a collection of byte[]</returns>
        public static async Task<ICollection<byte[]>> ToByteArraysAsync(this ICollection<IFormFile> iFormFiles)
        {
            ICollection<byte[]> byteArrayCollection = new List<byte[]>();
            foreach (var iFormFile in iFormFiles)
                byteArrayCollection.Add(await iFormFile.ToByteArrayAsync());
            return byteArrayCollection;
        }

        /// <summary>
        /// Asynchronously converts a IFormFile to byte[]
        /// </summary>
        /// <param name="iFormFile">The IFormFile to be converted</param>
        /// <returns>Returns a byte[]</returns>
        public static async Task<byte[]> ToByteArrayAsync(this IFormFile iFormFile)
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
                await iFormFile.OpenReadStream().CopyToAsync(memoryStream);
                ContentType = iFormFile.ContentType;
                return memoryStream.ToArray();
            }
        }

        /// <summary>
        /// Creates physical files from a collection of IFormFile
        /// </summary>
        /// <param name="iFormFiles">The collection of IFormFile used to create the physical files</param>
        /// <param name="path">The path where the files will be created</param>
        public static void ToFiles(this ICollection<IFormFile> iFormFiles, string path)
        {
            foreach (var iFormFile in iFormFiles)
                iFormFile.ToFile(path);
        }

        /// <summary>
        /// Creates physical file from IFormFile
        /// </summary>
        /// <param name="iFormFile">The IFormFile used to create the physical file</param>
        /// <param name="path">The path where the file will be created</param>
        public static void ToFile(this IFormFile iFormFile, string path)
        {
            using (FileStream fileStream = new FileStream(path, FileMode.Create))
            {
                iFormFile.CopyTo(fileStream);
            }
        }

        /// <summary>
        /// Asynchronously creates physical files from a collection of IFormFile
        /// </summary>
        /// <param name="iFormFiles">The collection of IFormFile used to create the physical files</param>
        /// <param name="path">The path where the files will be created</param>
        public async static Task ToFilesAsync(this ICollection<IFormFile> iFormFiles, string path)
        {
            foreach (var iFormFile in iFormFiles)
                await iFormFile.ToFileAsync(path);
        }

        /// <summary>
        /// Asynchronously creates physical file from IFormFile
        /// </summary>
        /// <param name="iFormFile">The IFormFile used to create the physical file</param>
        /// <param name="path">The path where the file will be created</param>
        public static async Task ToFileAsync(this IFormFile iFormFile, string path)
        {
            using (FileStream fileStream = new FileStream(path, FileMode.Create))
            {
                await iFormFile.CopyToAsync(fileStream);
            }
        }

        /// <summary>
        /// Converts a byte[] to FileStreamResult
        /// </summary>
        /// <param name="byteArray">The byte[] to be converted</param>
        /// <param name="contentType">The content type of the byte[] needed to convertion</param>
        /// <returns>Returns a FileStreamResult</returns>
        public static FileStreamResult ToFileStreamResult(this byte[] byteArray, string contentType)
        {
            using (MemoryStream ms = new MemoryStream(byteArray))
            {
                return new FileStreamResult(ms, contentType);
            }
        }

        /// <summary>
        /// Converts a FileStreamResult to IFormFile
        /// </summary>
        /// <param name="fileStreamResult">The FileStreamResult to be converted</param>
        /// <param name="name">Set the name from the Content-Disposition header.</param>
        /// <param name="fileName">Set the file name from the Content-Disposition header.</param>
        /// <returns>Returns a IFormFile</returns>
        public static IFormFile ToIFormFile(this FileStreamResult fileStreamResult, string name = "name", string fileName = "fileName")
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
                fileStreamResult.FileStream.CopyTo(memoryStream);
                return new FormFile(memoryStream, 0, memoryStream.Length, name, fileName);
            }
        }

        /// <summary>
        /// Converts a byte[] to IFormFile
        /// </summary>
        /// <param name="byteArray">The byte[] to be converted</param>
        /// <param name="contentType">The content type of the byte[] needed to convertion</param>
        /// <param name="name">Set the name from the Content-Disposition header.</param>
        /// <param name="fileName">Set the file name from the Content-Disposition header.</param>
        /// <returns></returns>
        public static IFormFile ToIFormFile(this byte[] byteArray, string contentType, string name = "name", string fileName = "fileName")
        {
            return byteArray.ToFileStreamResult(contentType).ToIFormFile(name, fileName);
        }

        /// <summary>
        /// Returns a collection of byte[] strings to a collection of base64 strings.
        /// </summary>
        /// <param name="byteArrays">The collection of byte[] to be converted</param>
        /// <returns>Returns a collection of base64 strings</returns>
        public static ICollection<string> ToBase64Strings(this ICollection<byte[]> byteArrays)
        {
            ICollection<string> baseStringCollection = new List<string>();
            foreach (byte[] byteArray in byteArrays)
                baseStringCollection.Add(byteArray.ToBase64String());
            return baseStringCollection;
        }

        /// <summary>
        /// Asynchronously converts a collection of byte[] strings to a collection of base64 strings.
        /// </summary>
        /// <param name="byteArrays">The collection of byte[] to be converted</param>
        /// <returns>Returns a collection of base64 strings</returns>
        public static async Task<ICollection<string>> ToBase64StringsAsync(this ICollection<byte[]> byteArrays)
        {
            ICollection<string> baseStringCollection = new List<string>();
            foreach (byte[] byteArray in byteArrays)
                baseStringCollection.Add(await byteArray.ToBase64StringAsync());
            return baseStringCollection;
        }

        /// <summary>
        /// Converts a byte[] to a base64 string.
        /// </summary>
        /// <param name="byteArray">The byte[] to be converted.</param>
        /// <returns>Returns a byte[].</returns>
        public static string ToBase64String(this byte[] byteArray)
        {
            return Convert.ToBase64String(byteArray);
        }

        /// <summary>
        /// Asynchronously converts a byte[] to a base64 string.
        /// </summary>
        /// <param name="byteArray">The byte[] to be converted.</param>
        /// <returns>Returns a byte[].</returns>
        public static async Task<string> ToBase64StringAsync(this byte[] byteArray)
        {
            return await Task.FromResult(Convert.ToBase64String(byteArray));
        }

        /// <summary>
        /// Converts a collection of base64 string to a collection of byte[].
        /// </summary>
        /// <param name="baseStrings">The collection of base64 strings to be converted.</param>
        /// <returns>Return a collection of byte[]</returns>
        public static ICollection<byte[]> ToByteArrays(this ICollection<string> baseStrings)
        {
            ICollection<byte[]> byteArrayCollection = new List<byte[]>();
            foreach (string baseString in baseStrings)
                byteArrayCollection.Add(baseString.ToByteArray());
            return byteArrayCollection;
        }

        /// <summary>
        /// Asynchronously converts a collection of base64 string to a collection of byte[].
        /// </summary>
        /// <param name="baseStrings">The collection of base64 strings to be converted.</param>
        /// <returns>Return a collection of byte[]</returns>
        public static async Task<ICollection<byte[]>> ToByteArraysAsync(this ICollection<string> baseStrings)
        {
            ICollection<byte[]> byteArrayCollection = new List<byte[]>();
            foreach (string baseString in baseStrings)
                byteArrayCollection.Add(await baseString.ToByteArrayAsync());
            return byteArrayCollection;
        }

        /// <summary>
        /// Converts a base64 string to a byte[].
        /// </summary>
        /// <param name="baseString">The base64 string to be converted.</param>
        /// <returns>Returns a byte[].</returns>
        public static byte[] ToByteArray(this string baseString)
        {
            return Convert.FromBase64String(baseString);
        }

        /// <summary>
        /// Asynchronously converts a base64 string to a byte[].
        /// </summary>
        /// <param name="baseString">The base64 string to be converted.</param>
        /// <returns>Returns a byte[].</returns>
        public static async Task<byte[]> ToByteArrayAsync(this string baseString)
        {
            return await Task.FromResult(Convert.FromBase64String(baseString));
        }
    }
}