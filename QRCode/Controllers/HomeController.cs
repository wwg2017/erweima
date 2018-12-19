using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ThoughtWorks.QRCode.Codec;
using ThoughtWorks.QRCode.Codec.Data;

namespace QRCode.Controllers
{
    public class HomeController : Controller
    {
        #region 二维码
        //
        // GET: /Home/
        public ActionResult Index()
        {
            return View();
        }

        //生成二维码方法一
        private void CreateCode_Simple(string nr)
        {
            QRCodeEncoder qrCodeEncoder = new QRCodeEncoder();
            qrCodeEncoder.QRCodeEncodeMode = QRCodeEncoder.ENCODE_MODE.BYTE;
            qrCodeEncoder.QRCodeScale = 4;
            qrCodeEncoder.QRCodeVersion = 8;
            qrCodeEncoder.QRCodeErrorCorrect = QRCodeEncoder.ERROR_CORRECTION.M;
            //System.Drawing.Image image = qrCodeEncoder.Encode("4408810820 深圳－广州 小江");
            System.Drawing.Image image = qrCodeEncoder.Encode(nr);
            string filename = DateTime.Now.ToString("yyyymmddhhmmssfff").ToString() + ".jpg";
            string filepath = Server.MapPath(@"~\Upload") + "\\" + filename;
            System.IO.FileStream fs = new System.IO.FileStream(filepath, System.IO.FileMode.OpenOrCreate, System.IO.FileAccess.Write);
            image.Save(fs, System.Drawing.Imaging.ImageFormat.Jpeg);

            fs.Close();
            image.Dispose();
            //二维码解码
            var codeDecoder = CodeDecoder(filepath);
        }

        /// <summary>
        /// 生成二维码
        /// </summary>
        /// <param name="strData">要生成的文字或者数字，支持中文。如： "4408810820 深圳－广州" 或者：4444444444</param>
        /// <param name="qrEncoding">三种尺寸：BYTE ，ALPHA_NUMERIC，NUMERIC</param>
        /// <param name="level">大小：L M Q H</param>
        /// <param name="version">版本：如 8</param>
        /// <param name="scale">比例：如 4</param>
        /// <returns></returns>
        public ActionResult CreateCode_Choose(string strData, string qrEncoding, string level, int version, int scale)
        {
            QRCodeEncoder qrCodeEncoder = new QRCodeEncoder();
            string encoding = qrEncoding;
            switch (encoding)
            {
                case "Byte":
                    qrCodeEncoder.QRCodeEncodeMode = QRCodeEncoder.ENCODE_MODE.BYTE;
                    break;
                case "AlphaNumeric":
                    qrCodeEncoder.QRCodeEncodeMode = QRCodeEncoder.ENCODE_MODE.ALPHA_NUMERIC;
                    break;
                case "Numeric":
                    qrCodeEncoder.QRCodeEncodeMode = QRCodeEncoder.ENCODE_MODE.NUMERIC;
                    break;
                default:
                    qrCodeEncoder.QRCodeEncodeMode = QRCodeEncoder.ENCODE_MODE.BYTE;
                    break;
            }

            qrCodeEncoder.QRCodeScale = scale;
            qrCodeEncoder.QRCodeVersion = version;
            switch (level)
            {
                case "L":
                    qrCodeEncoder.QRCodeErrorCorrect = QRCodeEncoder.ERROR_CORRECTION.L;
                    break;
                case "M":
                    qrCodeEncoder.QRCodeErrorCorrect = QRCodeEncoder.ERROR_CORRECTION.M;
                    break;
                case "Q":
                    qrCodeEncoder.QRCodeErrorCorrect = QRCodeEncoder.ERROR_CORRECTION.Q;
                    break;
                default:
                    qrCodeEncoder.QRCodeErrorCorrect = QRCodeEncoder.ERROR_CORRECTION.H;
                    break;
            }
            //文字生成图片
            Image image = qrCodeEncoder.Encode("http://www.baidu.com");
            string filename = DateTime.Now.ToString("yyyymmddhhmmssfff").ToString() + ".jpg";
            string filepath = Server.MapPath(@"~\Upload") + "\\" + filename;
            //如果文件夹不存在，则创建
            //if (!Directory.Exists(filepath))
            //    Directory.CreateDirectory(filepath);
            System.IO.FileStream fs = new System.IO.FileStream(filepath, System.IO.FileMode.OpenOrCreate, System.IO.FileAccess.Write);
            image.Save(fs, System.Drawing.Imaging.ImageFormat.Jpeg);
            fs.Close();
            image.Dispose();
            ImageUtility imgun = new ImageUtility();
            Bitmap map = new Bitmap(Image.FromFile(Server.MapPath("/Upload/t.jpg")));

            Bitmap maps = new Bitmap(Image.FromFile(filepath));

            Bitmap myImage = new Bitmap(imgun.MergeQrImg(maps, map));


            var s = Server.MapPath("~") + "Upload\\1.jpg";
            myImage.Save(s);
            return Content(@"/Upload/1.jpg");
        }

        /// <summary>
        /// 二维码解码
        /// </summary>
        /// <param name="filePath">图片路径</param>
        /// <returns></returns>
        public string CodeDecoder(string filePath)
        {
            if (!System.IO.File.Exists(filePath))
                return null;
            Bitmap myBitmap = new Bitmap(Image.FromFile(filePath));

           
            QRCodeDecoder decoder = new QRCodeDecoder();
            string decodedString = decoder.decode(new QRCodeBitmapImage(myBitmap));
            return decodedString;
        }
        #endregion

        #region 添加自己图片
        #region 修理图片方法一（未被用到）
        public class QRCodeHelper
        {
            public static Bitmap GetThumbnail(Bitmap b, int destHeight, int destWidth)
            {
                System.Drawing.Image imgSource = b;
                System.Drawing.Imaging.ImageFormat thisFormat = imgSource.RawFormat;
                int sW = 0, sH = 0;
                // 按比例缩放  
                int sWidth = imgSource.Width;
                int sHeight = imgSource.Height;
                if (sHeight > destHeight || sWidth > destWidth)
                {
                    if ((sWidth * destHeight) > (sHeight * destWidth))
                    {
                        sW = destWidth;
                        sH = (destWidth * sHeight) / sWidth;
                    }
                    else
                    {
                        sH = destHeight;
                        sW = (sWidth * destHeight) / sHeight;
                    }
                }
                else
                {
                    sW = sWidth;
                    sH = sHeight;
                }
                Bitmap outBmp = new Bitmap(destWidth, destHeight);
                Graphics g = Graphics.FromImage(outBmp);
                g.Clear(Color.Transparent);
                // 设置画布的描绘质量  
                g.CompositingQuality = CompositingQuality.HighQuality;
                g.SmoothingMode = SmoothingMode.HighQuality;
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.DrawImage(imgSource, new Rectangle((destWidth - sW) / 2, (destHeight - sH) / 2, sW, sH), 0, 0, imgSource.Width, imgSource.Height, GraphicsUnit.Pixel);
                g.Dispose();
                // 以下代码为保存图片时，设置压缩质量  
                EncoderParameters encoderParams = new EncoderParameters();
                long[] quality = new long[1];
                quality[0] = 100;
                EncoderParameter encoderParam = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, quality);
                encoderParams.Param[0] = encoderParam;
                imgSource.Dispose();
                return outBmp;
            }           
        }
        #endregion
        #region 方法二修图片
        public  class ImageUtility
        {
            #region 合并用户QR图片和用户头像
            /// <summary> 
            /// 合并用户QR图片和用户头像 
            /// </summary> 
            /// <param name="qrImg">QR图片</param> 
            /// <param name="headerImg">用户头像</param> 
            /// <param name="n">缩放比例</param> 
            /// <returns></returns> 
            public  Bitmap MergeQrImg(Bitmap qrImg, Bitmap headerImg, double n = 0.23)
            {
                int margin = 10;
                float dpix = qrImg.HorizontalResolution;
                float dpiy = qrImg.VerticalResolution;
                var _newWidth = (10 * qrImg.Width - 46 * margin) * 1.0f / 46;
                var _headerImg = ZoomPic(headerImg, _newWidth / headerImg.Width);
                //处理头像 
                int newImgWidth = _headerImg.Width + margin;
                Bitmap headerBgImg = new Bitmap(newImgWidth, newImgWidth);
                headerBgImg.MakeTransparent();
                Graphics g = Graphics.FromImage(headerBgImg);
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                g.Clear(Color.Transparent);
                Pen p = new Pen(new SolidBrush(Color.White));
                Rectangle rect = new Rectangle(0, 0, newImgWidth - 1, newImgWidth - 1);
                using (GraphicsPath path = CreateRoundedRectanglePath(rect, 7))
                {
                    g.DrawPath(p, path);
                    g.FillPath(new SolidBrush(Color.White), path);
                }
                //画头像 
                Bitmap img1 = new Bitmap(_headerImg.Width, _headerImg.Width);
                Graphics g1 = Graphics.FromImage(img1);
                g1.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                g1.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                g1.Clear(Color.Transparent);
                Pen p1 = new Pen(new SolidBrush(Color.Gray));
                Rectangle rect1 = new Rectangle(0, 0, _headerImg.Width - 1, _headerImg.Width - 1);
                using (GraphicsPath path1 = CreateRoundedRectanglePath(rect1, 7))
                {
                    g1.DrawPath(p1, path1);
                    TextureBrush brush = new TextureBrush(_headerImg);
                    g1.FillPath(brush, path1);
                }
                g1.Dispose();
                PointF center = new PointF((newImgWidth - _headerImg.Width) / 2, (newImgWidth - _headerImg.Height) / 2);
                g.DrawImage(img1, center.X, center.Y, _headerImg.Width, _headerImg.Height);
                g.Dispose();
                Bitmap backgroudImg = new Bitmap(qrImg.Width, qrImg.Height);
                backgroudImg.MakeTransparent();
                backgroudImg.SetResolution(dpix, dpiy);
                headerBgImg.SetResolution(dpix, dpiy);
                Graphics g2 = Graphics.FromImage(backgroudImg);
                g2.Clear(Color.Transparent);
                g2.DrawImage(qrImg, 0, 0);
                PointF center2 = new PointF((qrImg.Width - headerBgImg.Width) / 2, (qrImg.Height - headerBgImg.Height) / 2);
                g2.DrawImage(headerBgImg, center2);
                g2.Dispose();
                return backgroudImg;
            }
            #endregion

            #region 图形处理
            /// <summary> 
            /// 创建圆角矩形 
            /// </summary> 
            /// <param name="rect">区域</param> 
            /// <param name="cornerRadius">圆角角度</param> 
            /// <returns></returns> 
            private GraphicsPath CreateRoundedRectanglePath(Rectangle rect, int cornerRadius)
            {
                //下午重新整理下，圆角矩形 
                GraphicsPath roundedRect = new GraphicsPath();
                roundedRect.AddArc(rect.X, rect.Y, cornerRadius * 2, cornerRadius * 2, 180, 90);
                roundedRect.AddLine(rect.X + cornerRadius, rect.Y, rect.Right - cornerRadius * 2, rect.Y);
                roundedRect.AddArc(rect.X + rect.Width - cornerRadius * 2, rect.Y, cornerRadius * 2, cornerRadius * 2, 270, 90);
                roundedRect.AddLine(rect.Right, rect.Y + cornerRadius * 2, rect.Right, rect.Y + rect.Height - cornerRadius * 2);
                roundedRect.AddArc(rect.X + rect.Width - cornerRadius * 2, rect.Y + rect.Height - cornerRadius * 2, cornerRadius * 2, cornerRadius * 2, 0, 90);
                roundedRect.AddLine(rect.Right - cornerRadius * 2, rect.Bottom, rect.X + cornerRadius * 2, rect.Bottom);
                roundedRect.AddArc(rect.X, rect.Bottom - cornerRadius * 2, cornerRadius * 2, cornerRadius * 2, 90, 90);
                roundedRect.AddLine(rect.X, rect.Bottom - cornerRadius * 2, rect.X, rect.Y + cornerRadius * 2);
                roundedRect.CloseFigure();
                return roundedRect;
            }
            /// <summary> 
            /// 图片按比例缩放 
            /// </summary> 
            private Image ZoomPic(Image initImage, double n)
            {
                //缩略图宽、高计算 
                double newWidth = initImage.Width;
                double newHeight = initImage.Height;
                newWidth = n * initImage.Width;
                newHeight = n * initImage.Height;
                //生成新图 
                //新建一个bmp图片 
                System.Drawing.Image newImage = new System.Drawing.Bitmap((int)newWidth, (int)newHeight);
                //新建一个画板 
                System.Drawing.Graphics newG = System.Drawing.Graphics.FromImage(newImage);
                //设置质量 
                newG.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                newG.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                //置背景色 
                newG.Clear(Color.Transparent);
                //画图 
                newG.DrawImage(initImage, new System.Drawing.Rectangle(0, 0, newImage.Width, newImage.Height), new System.Drawing.Rectangle(0, 0, initImage.Width, initImage.Height), System.Drawing.GraphicsUnit.Pixel);
                newG.Dispose();
                return newImage;
            }
            #endregion
        }
        #endregion
        #endregion
    }
}