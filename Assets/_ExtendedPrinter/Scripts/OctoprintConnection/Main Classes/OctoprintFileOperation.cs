using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OctoPrintLib.File;

namespace OctoPrintLib.Operations
{
    /// <summary>
    /// Tracks Files, can delete, upload and slice.
    /// </summary>
    public class OctoprintFileOperation : OctoprintConnection
    {
        /// <summary>
        /// Initializes a Filetracker, this shouldn't be done directly and is part of the Connection it needs anyway
        /// </summary>
        /// <param name="server">The Octoprint connection it connects to.</param>
        public OctoprintFileOperation(OctoprintServer server) : base(server)
        {
        }


        /// <summary>
        /// Downloads a file from Octoprint
        /// </summary>
        /// <param name="remoteLocation">Localtion of file on Octoprint (location without base URL)</param>
        /// <param name="localDownloadPath">Full path (with</param>
        public async Task<bool> DownloadFileAsync(string remoteLocation, string localDownloadPath)
        {
            using (WebClient webclient = new WebClient())
            {
                webclient.Headers.Add("X-API-Key", server.ApplicationKey);

                try
                {
                    var str = "http://" + server.DomainNmaeOrIp + "/downloads/files/local/" + remoteLocation;
                    await webclient.DownloadFileTaskAsync(new Uri(str), localDownloadPath);
                    //SetDownloadedFileLocalInformation(downloadPath);
                }
                catch (Exception e)
                {
                    return false;
                }

                return true;
            }
        }
        /// <summary>
        /// Gets all the files on the Server
        /// </summary>
        public async Task<OctoprintFileResponse> GetFilesAsync()
        {
            string jobInfo = await GetAsync("api/files");
            return JsonConvert.DeserializeObject<OctoprintFileResponse>(jobInfo);

        }

        
        /// <summary>
        /// Retrieve all files in the folder
        /// </summary>
        /// <param name="path"> folder</param>
        /// <returns></returns>
        public async Task<FolderInformation> GetFileInfosInFolderAsync(string location, string path ="")
        {
            string jobInfo = "";
            try
            {
                //var url = "api/files/" + location;
                //if(path != string.Empty)
                //{
                //    url += "/" + path;
                //}
                var url = "api/files?force=true";

                jobInfo = await GetAsync(url);
            }
            catch (WebException e)
            {
                switch (((HttpWebResponse)e.Response).StatusCode)
                {
                    case HttpStatusCode.NotFound:
                        Debug.WriteLine("searched for a file that wasn't there at " + path);
                        return null;
                }
            }
            return JsonConvert.DeserializeObject<FolderInformation>(jobInfo);
        }

        /// <summary>
        /// Retrieve a specific file’s information
        /// </summary>
        /// <param name="fileName"> the path of the file including its name and extension</param>
        /// <returns></returns>
        public async Task<OctoprintFile> GetFileInfoAsync(string fileName, string location = "local")
        {
            string jobInfo = "";
            try
            {
                jobInfo = await GetAsync("api/files/" + location + "/" + fileName);
            }
            catch (WebException e)
            {
                switch (((HttpWebResponse)e.Response).StatusCode)
                {
                    case HttpStatusCode.NotFound:
                        Debug.WriteLine("searched for a file that wasn't there at " + fileName);
                        return null;
                }
            }
            return JsonConvert.DeserializeObject<OctoprintFile>(jobInfo);
        }

        /// <summary>
        /// Selects the File for printing
        /// </summary>
        /// <returns>The Http Result</returns>
        /// <param name="path">The path of the file that should be selected.</param>
        /// <param name="location">The location (local or sdcard) where this File should be. Normally local</param>
        /// <param name="print">If set, defines if the GCode should be printed directly after being selected. null means false</param>
        public string Select(string path, bool print = false, string location = "local")
        {
            JObject data = new JObject
            {
                { "command", "select" },
                { "print", print}
            };

            try
            {
                return PostJson("api/files/" + location + "/" + path, data);
            }
            catch (WebException e)
            {
                switch (((HttpWebResponse)e.Response).StatusCode)
                {
                    case HttpStatusCode.Conflict:
                        return "409 The Printer is propably not operational";
                    default:
                        return "unknown webexception occured";
                }

            }
        }

        

        /// <summary>
        /// Deletes a File
        /// </summary>
        /// <returns>The Http Result</returns>
        /// <param name="location">Location of the File to delete, sdcard or local</param>
        /// <param name="path">The path of the File to delete.</param>
        public async Task<string> DeleteAsync(string location, string path)
        {
            try
            {
                return await base.DeleteAsync("api/files/" + location + "/" + path);
            }
            catch (WebException e)
            {
                switch (((HttpWebResponse)e.Response).StatusCode)
                {
                    case HttpStatusCode.Conflict:
                        return "409 The file is currently in use by a Slicer or a Printer";
                    case HttpStatusCode.NotFound:
                        return "404 did not find the file";
                    default:
                        return "unknown webexception occured";
                }

            }


        }

        /// <summary>
        /// Creates a folder, if a subfolder should be created, create it with slashes and the path before it.
        /// </summary>
        /// <returns>The Http Result</returns>
        /// <param name="path">The Path of the Folder including the new folder name.</param>
        public async Task<string> CreateFolderAsync(string path)
        {
            string foldername = path.Split('/')[path.Split('/').Length - 1];
            path = path.Substring(0, path.Length - foldername.Length);

            MultipartFormDataContent multipartContent = new MultipartFormDataContent();
            multipartContent.Add(new StringContent(foldername), "foldername");
            multipartContent.Add(new StringContent(path), "path");

            return await PostMultipartAsync("api/files/local", multipartContent);

        }

        

        //public async Task<string>  UploadFileAsync(string localFullFilePath,  string remoteLocation = "local", bool select = false, bool print = false)
        //{
        //    var fileData = await System.IO.File.ReadAllBytesAsync(localFullFilePath); //check if file exists
        //    var filename = Path.GetFileName(localFullFilePath);
        //    MultipartFormDataContent multipartContent = new MultipartFormDataContent();
        //    multipartContent.Add(new StreamContent(new MemoryStream(fileData)), "file", filename);
        //    if (select) multipartContent.Add(new StringContent(select.ToString()), "select");
        //    if (print) multipartContent.Add(new StringContent(print.ToString()), "print");


        //    return await PostMultipartAsync("api/files/"+ remoteLocation, multipartContent);
        //}
    }
}

