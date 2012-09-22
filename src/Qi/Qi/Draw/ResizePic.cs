using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;

namespace Qi.Draw
{
    /// <summary>
    /// 
    /// </summary>
    public class ResizePic
    {
        /// <summary>
        /// 
        /// </summary>
        public int MaxWidth { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int MaxHeight { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="originalImagePath"></param>
        /// <param name="thumNailPath"></param>
        /// <returns></returns>
        public void Resize(string originalImagePath, string thumNailPath)
        {
            Image originalImage = Image.FromFile(originalImagePath);


            int targetWidth;
            int targetHight;

            if (originalImage.Height >= originalImage.Width)
            {
                if (originalImage.Height < MaxHeight)
                {
                    File.Copy(originalImagePath, thumNailPath);
                }
                targetHight = MaxHeight;
                targetWidth = originalImage.Width*targetHight/originalImage.Height;
            }
            else
            {
                if (originalImage.Width < MaxWidth)
                {
                    File.Copy(originalImagePath, thumNailPath);
                }
                targetWidth = MaxWidth;
                targetHight = originalImage.Height*targetWidth/originalImage.Width;
            }


            //新建一个bmp图片
            var bitmap = new Bitmap(targetWidth, targetWidth);

            //新建一个画板
            Graphics graphic = Graphics.FromImage(bitmap);

            //设置高质量查值法
            graphic.InterpolationMode = InterpolationMode.High;

            //设置高质量，低速度呈现平滑程度
            graphic.SmoothingMode = SmoothingMode.HighQuality;

            //清空画布并以透明背景色填充
            graphic.Clear(Color.Transparent);

            //在指定位置并且按指定大小绘制原图片的指定部分
            graphic.DrawImage(originalImage, new Rectangle(0, 0, targetWidth, targetHight),
                              new Rectangle(0, 0, originalImage.Width, originalImage.Height),
                              GraphicsUnit.Pixel);
            bitmap.Save(thumNailPath, ImageFormat.Png);
        }
    }
}