using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace APIWeb.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class compressionsController : ControllerBase
    {
        // GET  /api/compressions
        [HttpGet]
        public string JsonCompressions()
        {
            var CurrentDirectory = Directory.GetCurrentDirectory();
            string pathfileCompressions = CurrentDirectory + "\\App_Data\\fileCompressions.json";
            string jsonString = string.Empty;

            if (!System.IO.File.Exists(pathfileCompressions))
            {
                System.IO.File.Create(pathfileCompressions);
            }
            else
            {
                StreamReader r = new StreamReader(pathfileCompressions);
                jsonString = r.ReadToEnd();
                r.Close();
            }

            return jsonString;
        }
    }
}