using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FriRaLand {
    class Common {
        public class Account {
            public const int Fril_Account = 0;
            public const int Rakuma_Account = 1;
            public int DBId;
            public int kind;
            public string email;
            public string password;
            public string fril_auth_token;
            public string rakuma_access_token;
            public string nickname;
            public string userId;
            public DateTime expirationDate;
        }
        public static DateTime UNIX_EPOCH = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc); //UnixTimeの開始時刻
        static public DateTime getDateFromUnixTimeStamp(long timestamp) {
            try {
                return UNIX_EPOCH.AddSeconds(timestamp).ToLocalTime();
            }
            catch {
                return UNIX_EPOCH.ToLocalTime();
            }
        }
        static public DateTime getDateFromUnixTimeStamp(string timestamp) {
            try {
                return getDateFromUnixTimeStamp(long.Parse(timestamp));
            }
            catch {
                return UNIX_EPOCH.ToLocalTime();
            }
        }
        public static string getExhibitionImageFromPath(string path) {
            try {
                //imgフォルダがなかったら作成
                if (string.IsNullOrEmpty(path)) return "";
                if (System.IO.Directory.Exists("img") == false) System.IO.Directory.CreateDirectory(@"img");
                Bitmap bitmap = new Bitmap(path);
                int startposx = 0;
                int startposy = 0;
                int drawwidth = 0;
                int drawheight = 0;
                if (bitmap.Height > bitmap.Width) {
                    startposx = (int)((720.0 - ((double)bitmap.Width * ((double)720 / (double)bitmap.Height))) / 2.0);
                    drawwidth = 720 - startposx * 2;
                    drawheight = 720;
                }
                else {
                    startposy = (int)((720.0 - ((double)bitmap.Height * ((double)720 / (double)bitmap.Width))) / 2.0);
                    drawwidth = 720;
                    drawheight = 720 - startposy * 2;
                }
                Bitmap bitmap2 = new Bitmap(720, 720);
                Graphics graphics = Graphics.FromImage(bitmap2);
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.FillRectangle(Brushes.White, 0, 0, 720, 720);
                graphics.DrawImage(bitmap, startposx, startposy, drawwidth, drawheight);
                graphics.Dispose();
                string str = DateTime.Now.ToString("yyyyMMddhhmmssfff") + ".jpg";
                string result;
                bitmap2.Save("img/" + str, ImageFormat.Jpeg);
                bitmap2.Dispose();
                result = "img/" + str;
                Log.Logger.Info("出品用画像の作成に成功");
                return result;
            }
            catch (Exception ex) {
                Log.Logger.Error(ex.Message);
                Log.Logger.Error("出品用画像の作成に失敗");
                return "";
            }
        }
    }
}
