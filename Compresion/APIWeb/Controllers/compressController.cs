using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Compresion;
using System.Text;
using Newtonsoft.Json;

namespace APIWeb.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class compressController : ControllerBase
    {
        // POST  /api/compress/{name}
        [HttpPost("{name}")]
        public async Task<FileResult> Download(string name)
        {
            var files = Request.Form.Files;
            var fileName = name + ".lzw";
            FileInfo fileInfoCompress = null;

            if (files.Count == 1)
            {
                foreach (var formFile in files)
                {
                    if (formFile.Length > 0)
                    {
                        var filePath = Path.GetTempFileName();

                        using (var stream = System.IO.File.Create(filePath))
                        {
                            await formFile.CopyToAsync(stream);
                        }

                        fileInfoCompress = new FileInfo(filePath);
                    }
                    if (fileInfoCompress != null)
                    {
                        Compresion.Clases.LZW compresion = new Compresion.Clases.LZW(fileInfoCompress, null);
                        List<byte> ArrayCompressFile = compresion.Comprimir();
                        var ArrayBytesCompress = ArrayCompressFile.ToArray();

                        Compresion.Clases.Compresion dataCompress = new Compresion.Clases.Compresion();
                        dataCompress = compresion.ObtenerDatosCompresion();
                        dataCompress.FileName = files[0].FileName;
                        dataCompress.PathFileHUFF = fileName;
                        SerializeJsonFile(dataCompress);

                        return File(ArrayBytesCompress, "application/octet-stream", fileName);
                    }
                }
            }

            return null;
        }

        // //////////////////////////////////////////COMPRESIONES//////////////////////////////////////////
        public void SerializeJsonFile(Compresion.Clases.Compresion _dataCompress)
        {
            List<Compresion.Clases.Compresion> compresions = new List<Compresion.Clases.Compresion>();
            var CurrentDirectory = Directory.GetCurrentDirectory();
            string pathfileDirectory = CurrentDirectory + "\\App_Data";
            string pathfileCompressions = CurrentDirectory + "\\App_Data\\fileCompressions.json";

            if (!System.IO.Directory.Exists(pathfileDirectory))
            {
                DirectoryInfo di = Directory.CreateDirectory(pathfileDirectory);
            }
            if (!System.IO.File.Exists(pathfileCompressions))
            {
                System.IO.File.Create(pathfileCompressions);
            }
            StreamReader r = new StreamReader(pathfileCompressions);
            string jsonString = r.ReadToEnd();
            if (jsonString != string.Empty)
            {
                compresions = JsonConvert.DeserializeObject<List<Compresion.Clases.Compresion>>(jsonString);
            }
            r.Close();

            compresions.Add(_dataCompress);
            StreamWriter sw = new StreamWriter(pathfileCompressions);

            string compressJsonFile = JsonConvert.SerializeObject(compresions, Formatting.Indented);
            sw.WriteLine(compressJsonFile);
            sw.Close();
        }
    }
}