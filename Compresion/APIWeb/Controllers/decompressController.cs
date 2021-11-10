using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Compresion;
using Newtonsoft.Json;

namespace APIWeb.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class decompressController : ControllerBase
    {
        // POST  /api/decompress
        [HttpPost]
        public async Task<FileResult> Download()
        {
            var files = Request.Form.Files;
            FileInfo fileInfoDecompress = null;

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

                        fileInfoDecompress = new FileInfo(filePath);
                    }
                    if (fileInfoDecompress != null)
                    {
                        Compresion.Clases.LZW compresion = new Compresion.Clases.LZW(null, fileInfoDecompress);
                        List<byte> ArrayDecompressFile = compresion.Descomprimir();
                        var ArrayBytesDecompress = ArrayDecompressFile.ToArray();
                        string fileName = BuscarNombreOriginal(files[0].FileName);                       

                        return File(ArrayBytesDecompress, "application/octet-stream", fileName);
                    }
                }
            }

            return null;
        }

        // //////////////////////////////////////////COMPRESIONES//////////////////////////////////////////
        public string BuscarNombreOriginal(string _fileNameCompress)
        {
            List<Compresion.Clases.Compresion> compresions = new List<Compresion.Clases.Compresion>();
            var CurrentDirectory = Directory.GetCurrentDirectory();
            string pathfileCompressions = CurrentDirectory + "\\App_Data\\fileCompressions.json";

            StreamReader r = new StreamReader(pathfileCompressions);
            string jsonString = r.ReadToEnd();
            r.Close();

            if (jsonString != string.Empty)
            {
                compresions = JsonConvert.DeserializeObject<List<Compresion.Clases.Compresion>>(jsonString);
                for (int x = 0; x < compresions.Count; x++)
                {
                    if (compresions.ElementAt(x).PathFileHUFF == _fileNameCompress)
                    {
                        string fileName = compresions.ElementAt(x).FileName;
                        return fileName;
                    }
                }
            }

            string Name = _fileNameCompress.Replace(".lzw", ".txt");
            return Name;
        }
    }
}