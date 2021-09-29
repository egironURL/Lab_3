using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Compresion.Clases;

namespace AppConsola
{
    class Program
    {
        static void Main(string[] args)
        {
            var fi1 = new FileInfo(@"C:\Users\IT\Documents\EG\Cadena.txt");
            var fi2 = new FileInfo(@"C:\Users\IT\Documents\EG\Cadena.lzw");

            LZW compresion = new LZW(fi1, fi2);
            //compresion.Comprimir();
            compresion.CrearArchivoCompresion();

            Console.ReadLine();
        }
    }
}
