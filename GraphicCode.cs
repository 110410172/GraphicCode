using System;
using System.Drawing;
using System.Security.Cryptography;
using System.Text;

namespace GraphicCode
{
    /// <summary>
    /// 图片码
    /// 用于验证身份时的反暴力破解
    /// </summary>
    public class GraphicCode
    {
        /// <summary>
        /// 密钥
        /// </summary>
        private string key = "code#8k3i2abz";

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key">密钥</param>
        public GraphicCode(string key)
        {
            this.key = key;
        }

        /// <summary>
        /// 检查验证码是否正确
        /// </summary>
        /// <param name="code">验证码</param>
        /// <param name="encryptCode">加过密的验证码</param>
        /// <returns>正确true,错误false</returns>
        public bool Validate(string code, string encryptCode)
        {
            try
            {
                //var d = Encrypter.EncryptByMD5(key + code);
                if (EncryptByMD5(key + code) != encryptCode) return false;
            }
            catch (Exception e)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// 创建图片码 
        /// </summary>
        /// <returns>返回codeEncrypted(加密后的校验码), codeImage(base64型Image字符).</returns>
        public GraphicCodeModel Make()
        {
            string checkCode = this.RandomCode();
            var base64Img = this.CodeImage(checkCode);
            return new GraphicCodeModel(base64Img, EncryptByMD5(key + checkCode));
        }

        /// <summary>
        /// 生成验证码
        /// </summary>
        /// <param name="code">需要转换成图片的编码</param>
        /// <returns>返回strbaser64类型验证码</returns>
        private String CodeImage(string checkCode)
        {
            //drawing包
            //https://github.com/CoreCompat/CoreCompat
            //创建一个位图对象，.net core 需要安装System.Drawing.common
            var image = new Bitmap((int)Math.Ceiling((checkCode.Length * 12.5)), 22);
            //创建Graphics对象
            var g = Graphics.FromImage(image);
            var ms = new System.IO.MemoryStream();
            try
            {
                //生成随机生成器
                Random random = new Random();
                //清空图片背景色
                g.Clear(Color.White);
                //画图片的背景噪音线
                for (int i = 0; i < 2; i++)
                {
                    int x1 = random.Next(image.Width);
                    int x2 = random.Next(image.Width);
                    int y1 = random.Next(image.Height);
                    int y2 = random.Next(image.Height);
                    g.DrawLine(new Pen(Color.Black), x1, y1, x2, y2);
                }
                Font font = new System.Drawing.Font("Arial", 12, (FontStyle.Bold));
                System.Drawing.Drawing2D.LinearGradientBrush brush = new System.Drawing.Drawing2D.LinearGradientBrush(new Rectangle(0, 0, image.Width, image.Height), Color.Blue, Color.DarkRed, 1.2f, true);
                g.DrawString(checkCode, font, brush, 2, 2);

                //画图片的前景噪音点
                for (int i = 0; i < 100; i++)
                {
                    int x = random.Next(image.Width);
                    int y = random.Next(image.Height);
                    image.SetPixel(x, y, Color.FromArgb(random.Next()));
                }
                //画图片的边框线
                g.DrawRectangle(new Pen(Color.Silver), 0, 0, image.Width - 1, image.Height - 1);

                image.Save(ms, System.Drawing.Imaging.ImageFormat.Gif);

                byte[] arr = new byte[ms.Length];
                ms.Position = 0;
                ms.Read(arr, 0, (int)ms.Length);
                ms.Close();
                String strbaser64 = Convert.ToBase64String(arr);

                return strbaser64;

            }
            finally
            {
                g.Dispose();
                image.Dispose();
            }
        }

        private string EncryptByMD5(string input)
        {
            MD5 md5Hasher = MD5.Create();
            byte[] data = md5Hasher.ComputeHash(Encoding.UTF8.GetBytes(input));

            StringBuilder sBuilder = new StringBuilder();
            //将每个字节转为16进制
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }

            return sBuilder.ToString();
        }

        private string RandomCode()
        {
            //创建整型型变量
            int number;
            //创建字符型变量
            char code;
            //创建字符串变量并初始化为空
            string checkCode = String.Empty;
            //创建Random对象
            Random random = new Random();
            //使用For循环生成4个数字
            for (int i = 0; i < 4; i++)
            {
                //生成一个随机数
                number = random.Next();
                //将数字转换成为字符型
                code = (char)('0' + (char)(number % 10));
                checkCode += code.ToString();
            }
            //返回字符串
            return checkCode;
        }

    }

    public class GraphicCodeModel
    {
        /// <summary>
        /// 图片码实体类构造
        /// </summary>
        /// <param name="codeImage">base64型Image字符 </param>
        /// <param name="codeEncrypted">加过密的数字</param>
        public GraphicCodeModel(string codeImage, string codeEncrypted)
        {
            this.CodeImage = codeImage;
            this.CodeEncrypted = codeEncrypted;
        }

        /// <summary>
        /// base64型Image字符 
        /// </summary>
        public string CodeImage { get; private set; }
        /// <summary>
        /// 加过密的数字
        /// </summary>
        public string CodeEncrypted { get; private set; }
    }
}
