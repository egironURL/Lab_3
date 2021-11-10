using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Compresion.Clases;

namespace Compresion.Interfaces
{
    interface ICompresion
    {
        List<byte> Comprimir();

        List<byte> Descomprimir();
    }
}
