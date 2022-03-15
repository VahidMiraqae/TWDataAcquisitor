using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tesseract;

namespace TWDataAcquisitor
{
    public class OcrClass1 : IOcrProcess
    {
        private TesseractEngine _engine;

        public OcrClass1()
        {
            _engine = new TesseractEngine(@"./tessdata", "eng", EngineMode.Default);
        }


        public Task<string> Ocr(byte[] imageBytes)
        {
            return Task.Run(() =>
            {
                using (var img = Pix.LoadFromMemory(imageBytes))
                {
                    using (var page = _engine.Process(img))
                    {
                        var text = page.GetText();
                        return text;
                    }
                }
            });
        }
    }
}
