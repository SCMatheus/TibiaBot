IronOCR  - The OCR & Tesseract Library for .NET 
=============================================================
Quickstart:  https://ironsoftware.com/csharp/ocr/
	
	

Installation for .Net Framework 4 and above
=============================================================
- Include all dlls in net40 folder into your project
- Add Assembly references to:
	* System.Configuration
	* System.Drawing


Installation for .Net Standard & .Net Core 2.0+  & .Net 5
=============================================================
- Include all dlls in netstandard2.0 folder into your project
- Add a Nuget Package Reference to:
	* System.Drawing.Common 4.7 or higher


Compatibility
=============================================================
Supports applications and websites developed in 
- .Net FrameWork 4 (and above) for Windows and Azure
- .Net Core 2, 3 (and above) for Windows, Linux, MacOs and Azure
- .Net 5


C# Code Example
=============================================================
```
using IronOcr;
var Ocr = new IronTesseract();
using (var Input = new OcrInput("image.png"))
{
    // Input.Deskew();  // use if image not straight
    // Input.DeNoise(); // use if image contains digital noise
    var Result = Ocr.Read(Input);
    Console.WriteLine(Result.Text);
}
```

Documentation
=============================================================

- Code Samples				:	https://ironsoftware.com/csharp/ocr/examples/simple-csharp-ocr-tesseract/
- MSDN Class Reference		:	https://ironsoftware.com/csharp/ocr/object-reference/
- Tutorials					:	https://ironsoftware.com/csharp/ocr/tutorials/how-to-read-text-from-an-image-in-csharp-net/
- Support					:	developers@ironsoftware.com
