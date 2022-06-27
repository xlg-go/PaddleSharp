using OpenCvSharp;
using Sdcb.PaddleOCR.KnownModels;

namespace Sdcb.PaddleOCR.Tests
{
    public class UnitTest1
    {
        [Fact]
        public async Task FastCheckOCR()
        {
            OCRModel model = KnownOCRModel.PPOcrV3;
            await model.EnsureAll();

            byte[] sampleImageData;
            string sampleImageUrl = @"https://www.tp-link.com.cn/content/images2017/gallery/4288_1920.jpg";
            using (HttpClient http = new HttpClient())
            {
                Console.WriteLine("Download sample image from: " + sampleImageUrl);
                sampleImageData = await http.GetByteArrayAsync(sampleImageUrl);
            }

            using (PaddleOcrAll all = new PaddleOcrAll(model.RootDirectory, model.KeyPath)
            {
                AllowRotateDetection = true, /* 允许识别有角度的文字 */
                Enable180Classification = false, /* 允许识别旋转角度大于90度的文字 */
            })
            {
                // Load local file by following code:
                // using (Mat src2 = Cv2.ImRead(@"C:\test.jpg"))
                using (Mat src = Cv2.ImDecode(sampleImageData, ImreadModes.Color))
                {
                    PaddleOcrResult result = all.Run(src);
                    Console.WriteLine("Detected all texts: \n" + result.Text);
                    foreach (PaddleOcrResultRegion region in result.Regions)
                    {
                        Console.WriteLine($"Text: {region.Text}, Score: {region.Score}, RectCenter: {region.Rect.Center}, RectSize:    {region.Rect.Size}, Angle: {region.Rect.Angle}");
                    }
                }
            }
        }
    }
}