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

        public static System.Random random = new System.Random(1000);
        public static string ikkatu_shukkin_interval = "ikkatu_shukkin_interval";
        public static string ikkatu_hassou_interval = "ikkatu_hassou_interval";
        public static string ikkatu_hyouka_interval = "ikkatu_hyouka_interval";
        public static string ikkatu_shuppin_interval = "ikkatu_shuppin_interval";
        public static string ikkatu_torikesi_interval = "ikkatu_torikesi_interval";
        public static string reexhibit_check_interval = "reexhibit_check_interval";
        public static string check_kengai_status_onexhibit = "check_kengai_status_onexhibit";
        public static string auto_check_kengai_interval = "auto_check_kengai_interval";
        public static string kengai_notexhibit_border = "kengai_notexhibit_border";
        public static string comment_template = "comment_template";
        public static string transaction_message_template = "transaction_message_template";
        public static string buyer_review_message_template = "buyer_review_message_template";
        public static string seller_review_message_template = "seller_review_message_template";
        public static string shipping_notification_comment_message_template = "shipping_notification_comment_message_template";
        public static string comment_template_title = "comment_template_title";
        public static string transaction_message_template_title = "transaction_message_template_title";
        public static string buyer_review_message_template_title = "buyer_review_message_template_title";
        public static string seller_review_message_template_title = "seller_review_message_template_title";
        public static string shipping_notification_comment_message_template_title = "shipping_notification_comment_message_template_title";
        public static string use_proxy_manual = "use_proxy_manual";
        public static string use_proxy_auto = "use_proxy_auto";
        public static string donot_auto_token_refresh = "donot_auto_token_refresh";
        public static string ikkatu_shukkin_password = "ikkatu_shukkin_password";
        public static string autobuy_url = "autobuy_url";
        public static string autobuy_num = "autobuy_num";
        public static string autobuy_interval = "autobuy_interval";
        public static string daily_exhibit_start_hour = "daily_exhibit_start_hour";
        public static string daily_exhibit_start_minute = "daily_exhibit_start_minute";
        public static string daily_exhibit_interval_day = "daily_exhibit_interval_day";
        public static string daily_exhibit_interval_hour = "daily_exhibit_interval_hour";
        public static string daily_exhibit_interval_minute = "daily_exhibit_interval_minute";
        public static string daily_exhibit_interval_random_day1 = "daily_exhibit_interval_random_day1";
        public static string daily_exhibit_interval_random_hour1 = "daily_exhibit_interval_random_hour1";
        public static string daily_exhibit_interval_random_minute1 = "daily_exhibit_interval_random_minute1";
        public static string daily_exhibit_interval_random_day2 = "daily_exhibit_interval_random_day2";
        public static string daily_exhibit_interval_random_hour2 = "daily_exhibit_interval_random_hour2";
        public static string daily_exhibit_interval_random_minute2 = "daily_exhibit_interval_random_minute2";
        public static string daily_exhibit_interval_israndom = "daily_exhibit_interval_israndom";
        public static string daily_exhibit_delete_day = "daily_exhibit_delete_day";
        public static string daily_exhibit_delete_hour = "daily_exhibit_delete_hour";
        public static string daily_exhibit_delete_minute = "daily_exhibit_delete_minute";
        public static string daily_exhibit_consider_favorite = "daily_exhibit_consider_favorite";
        public static string daily_exhibit_consider_comment = "daily_exhibit_consider_comment";
        public static string daily_exhibit_delete_day2 = "daily_exhibit_delete_day2";
        public static string daily_exhibit_delete_hour2 = "daily_exhibit_delete_hour2";
        public static string daily_exhibit_delete_minute2 = "daily_exhibit_delete_minute2";
        public static string daily_exhibit_consider_favorite2 = "daily_exhibit_consider_favorite2";
        public static string daily_exhibit_consider_comment2 = "daily_exhibit_consider_comment2";
        public static string daily_exhibit_reexhibit_flag = "daily_exhibit_reexhibit_flag";
        public static string daily_exhibit_use_delete = "daily_exhibit_use_delete";
        public static string daily_exhibit_use_delete2 = "daily_exhibit_use_delete2";
        public static string use_notification = "use_notification";
        public static string notification_interval = "notification_interval";
        public static string last_notification_date = "last_notification_date";
        public static string item_name_header_list = "item_name_header_list";
        public static string item_name_max_length = "item_name_max_length";
        public static string auto_send_shipping_notification_on_fake_buy = "auto_send_shipping_notification_on_fake_buy";
        public static string auto_review_buyer_on_review_seller = "auto_review_buyer_on_review_seller";
        public static string save_message_window_size = "save_message_window_size";
        public static string message_window_size_width = "message_window_size_width";
        public static string message_window_size_height = "message_window_size_height";
        public static string last_osirase_createdTime = "last_osirase_createdTime";
        public static string autobuy_start_shipping_notification_interval = "autobuy_start_shipping_notification_interval";
        public static string nesage_enabled = "nesage_enabled";
        public static string nesage_groupkind_id = "nesage_groupkind_id";
        public static string nesage_hour = "nesage_hour";
        public static string nesage_minute = "nesage_minute";
        public static string nesage_kaisu = "nesage_kaisu";
        public static string nesage_like_lowerlimit = "nesage_like_lower_limit";
        public static string nesage_rate = "nesage_rate";
        public static string nesage_ng_list = "nesage_ng_list";
        public static string nesage_interval = "nesage_interval";
        public static string manual_neage_nesage_method = "manual_neage_nesage_method";
        public static string manual_neage_nesage_fixprice = "manual_neage_nesage_fixprice";
        public static string manual_neage_nesage_rate = "manual_neage_nesage_rate";





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
        static public string getUnixTimeStampFromDate(DateTime dt) {
            dt = dt.ToUniversalTime();
            Int32 unixTimestamp = (Int32)(dt.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
            return unixTimestamp.ToString();
        }
        // Base64エンコード
        public static string Base64Encode(string str) {
            try {
                var enc = new UTF8Encoding();
                byte[] bytes = enc.GetBytes(str);
                return Convert.ToBase64String(bytes);
            } catch (Exception ex) {
                Log.Logger.Error("Base64EncodeError");
                return "";
            }
        }

        // Base64デコード
        public static string Base64Decode(string str) {
            try {
                var enc = new UTF8Encoding();
                byte[] bytes = Convert.FromBase64String(str);
                return enc.GetString(bytes);
            } catch (Exception ex) {
                Log.Logger.Error("Base64DecodeError");
                return "";
            }
        }








    }
}
