using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Compresion.Interfaces;

namespace Compresion.Clases
{
    public class LZW
    {
        Dictionary<string, int> Diccionario { get; set; }

        private double CompressRatio { get; set; }

        private double CompressFactor { get; set; }

        private int ReductionPer { get; set; }

        private FileInfo originalFile { get; set; }

        private FileInfo compressFile { get; set; }

        public LZW(FileInfo _originalFile, FileInfo _compressFile)
        {
            this.originalFile = _originalFile;
            this.compressFile = _compressFile;
        }

        public List<byte> Comprimir()
        {
            List<byte> listaByteCompresion = new List<byte>();

            List<int> listaCharArchivo = LecturaArchivoOrigin();
            Dictionary<string, int> Diccionario = CrearDiccionario(listaCharArchivo);
            List<int> listaTextoCompresion = CrearListaCompresion(Diccionario, listaCharArchivo);

            listaByteCompresion = CrearListaByteCompresion(Diccionario, listaTextoCompresion);
            ObtenerCompressRatio(listaByteCompresion.Count, listaCharArchivo.Count);
            ObtenerCompressFactor(listaCharArchivo.Count, listaByteCompresion.Count);

            return listaByteCompresion;
        }

        private List<int> LecturaArchivoOrigin()
        {
            List<int> ListaASCII = new List<int>();

            int letter = 0;
            FileStream stream = originalFile.Open(FileMode.Open, FileAccess.Read);
            BinaryReader reader = new BinaryReader(stream);
            while (letter != -1)
            {
                try
                {
                    letter = reader.ReadByte();
                }
                catch
                {
                    letter = reader.Read();
                }

                if (letter != -1)
                {

                    ListaASCII.Add(letter);
                    Console.Write((char)letter);
                }
            }
            reader.Close();
            stream.Close();

            return ListaASCII;
        }

        private Dictionary<string, int> CrearDiccionario(List<int> _listaCharArchivo)
        {
            Dictionary<string, int> Diccionario = new Dictionary<string, int>();

            int NoASCII = -1, Cont = 1;
            string caracter = string.Empty;
            for (int x = 0; x < _listaCharArchivo.Count; x++)
            {
                NoASCII = _listaCharArchivo.ElementAt(x);
                caracter = Convert.ToString((char)NoASCII);
                if (!Diccionario.ContainsKey(caracter))
                {
                    Diccionario.Add(caracter, Cont);
                    Cont++;
                }
            }
            //var sortedDict = from entry in Diccionario orderby entry.Key ascending select entry;

            return Diccionario;
        }

        private List<int> CrearListaCompresion(Dictionary<string, int> _Diccionario, List<int> _listaCharArchivo)
        {
            Dictionary<string, int> Diccionario = new Dictionary<string, int>();
            List<int> listaCompresion = new List<int>();

            string caracter = string.Empty, nuevo = string.Empty;
            int NoASCII = -1, previo = 0;
            int Cont = Diccionario.Count + 1;

            for (int x = 0; x < _Diccionario.Count; x++)
            {
                Diccionario.Add(_Diccionario.ElementAt(x).Key, _Diccionario.ElementAt(x).Value);
            }

            for (int y = 0; y < _listaCharArchivo.Count; y++)
            {
                NoASCII = _listaCharArchivo.ElementAt(y);
                caracter = Convert.ToString((char)NoASCII);
                nuevo = nuevo + caracter;
                if (!Diccionario.ContainsKey(nuevo))
                {
                    Diccionario.Add(nuevo, Cont);
                    string anterior = nuevo.Remove(nuevo.Length - 1);
                    nuevo = Convert.ToString(nuevo.Last());
                    Diccionario.TryGetValue(anterior, out previo);
                    listaCompresion.Add(previo);
                    Cont++;
                }
            }
            listaCompresion.Add(Diccionario.Count);

            return listaCompresion;
        }

        private List<byte> CrearListaByteCompresion(Dictionary<string, int> _Diccionario, List<int> _listaCompresion)
        {
            List<byte> ListaByteCompresion = new List<byte>();
            string textoComprimido = string.Empty;
            string strbyte = string.Empty;
            int maxDiccionario = _listaCompresion.Last();
            int bitLong = Convert.ToInt16(Math.Log(maxDiccionario, 2));
            _listaCompresion.RemoveAt(_listaCompresion.Count - 1);

            ListaByteCompresion.Add(Convert.ToByte(bitLong));
            ListaByteCompresion.Add(Convert.ToByte(_Diccionario.Count));

            for (int x = 0; x < _Diccionario.Count; x++)
            {
                ListaByteCompresion.Add(Convert.ToByte(_Diccionario.ElementAt(x).Key.ElementAt(0)));
            }



            for (int y = 0; y < _listaCompresion.Count; y++)
            {
                strbyte = Convert.ToString(_listaCompresion.ElementAt(y), 2);
                strbyte = strbyte.PadLeft(bitLong, '0');
                textoComprimido = textoComprimido + strbyte;
            }

            if((textoComprimido.Length % 8) != 0)
            {
                int completar = ((textoComprimido.Length / 8) + 1) * 8 - textoComprimido.Length;
                for (int i = 0; i < completar; i++)
                {
                    textoComprimido = textoComprimido + '0';
                }
            }

            strbyte = string.Empty;
            for (int z = 0; z < textoComprimido.Length; z++)
            {
                strbyte = strbyte + textoComprimido.ElementAt(z);
                if(strbyte.Length == 8)
                {
                    ListaByteCompresion.Add(Convert.ToByte(strbyte, 2));
                    strbyte = string.Empty;
                }
            }

            return ListaByteCompresion;
        }

        private void ObtenerCompressRatio(int _noBytesCompress, int _noBytesOrigin)
        {
            this.CompressRatio = Convert.ToDouble(Decimal.Divide(_noBytesCompress, _noBytesOrigin));
            this.CompressRatio = Math.Round(this.CompressRatio, 2);

            this.ReductionPer = 100 - Convert.ToInt32(this.CompressRatio * 100);
        }

        private void ObtenerCompressFactor(int _noBytesOrigin, int _noBytesCompress)
        {
            this.CompressFactor = Convert.ToDouble(Decimal.Divide(_noBytesOrigin, _noBytesCompress));
            this.CompressFactor = Math.Round(this.CompressFactor, 2);
        }

        public void CrearArchivoCompresion()
        {
            if (File.Exists(compressFile.FullName))
            {
                File.Delete(compressFile.FullName);
            }

            //File.Create(PathFileHUFF);
            List<byte> _FileCompress = Comprimir();
            FileStream stream = new FileStream(compressFile.FullName, FileMode.Create, FileAccess.Write);
            BinaryWriter writer = new BinaryWriter(stream);

            for (int x = 0; x < _FileCompress.Count; x++)
            {
                writer.Write(_FileCompress.ElementAt(x));
            }

            writer.Close();
            stream.Close();

            Console.WriteLine("\n\nARCHIVO COMPRIMIDO EXITOSAMENTE");
            Console.WriteLine("\nCompress Ratio: " + CompressRatio);
            Console.WriteLine("\nCompress Factor: " + CompressFactor);
            Console.WriteLine("\nReductionPer: " + ReductionPer);
        }
    }
}
