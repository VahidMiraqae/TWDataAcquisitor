using System;
using System.Threading.Tasks;

namespace TWDataAcquisitor
{
    internal interface IOcrProcess
    {
        Task<string> Ocr(byte[] imageBytes); 
    }
}