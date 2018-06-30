using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Codeplex.Data;
using System.Net;
using System.IO;
using System.Drawing;        //json

namespace RakuLand {
    public class FrilItem {
        public string buyer_simei;
        public System.Drawing.Image Image { get; set; } //アプリケーション内使用用パラメータ//表示用の画像
        public string parent_id;
        public string child_id;
        public int zaikonum;
        public int DBId; //DB上でのId;
        public DateTime created_date;
        public string bikou;
        public bool address_copyed;

        public string item_id;
        public string item_name;
        public string detail;
        public int s_price;
        public int status;
        public int t_status;//FIXMEOPTION:取引状態を表しているはず0:selling,3:trading(相手の決済終了),4:受け取り確認待ち,5:相手が評価をしたあと
        public int carriage;//出品負担者
        public int d_method;//delivery_method
        public int d_date;//delivery_date
        public int d_area;//delivary_from_area;
        public string user_id = "";//出品者IDとして用いている
        public DateTime created_at;//ex)2017-09-27T09:12:57+09:00
        public string screen_name = ""; //出品者アカウント名
        public int category_id;
        public int category_p_id;//category2をあてている
        public int size_id; //19999=>なし
        public string size_name;
        public int brand_id; //null
        public int i_brand_id;
        public int comments_count;
        public int likes_count;
        public string[] imageurls = new string[]{"","","",""}; //画像URL
        public string[] imagepaths = new string[] { "", "", "", "" }; //ローカルの画像パス
        public int item_pv; //なにこれ？？たぶんアクセス数
        public int category_level1_id;
        public int category_level2_id;
        public int category_level3_id;
        public int category_level4_id;


        private static readonly DateTime UNIX_EPOCH = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc); //UnixTimeの開始時刻
        public string buyer_name = ""; //購入者アカウント名
        public string buyer_address = "";
        public DateTime buyer_comment_time = UNIX_EPOCH;
        public DateTime seller_comment_time = UNIX_EPOCH;
        public int transaction_message_num = -1;
        public DateTime buyer_transaction_message_time = UNIX_EPOCH;
        public DateTime seller_transaction_message_time = UNIX_EPOCH;


        //public int num_likes { get; set; } //いいね数
        //public int num_comments { get; set; }//コメント数
        public long pager_id; //FIXITOPTION:不必要なパラメータの可能性がある//商品ページのインデックス?,get_itemsで60件以上あるときは最後のitemのpager_idを使って2回目以降叩く
        public string status_message { get; set; } //表示用の状態「出品中・停止中・支払い待ち・発送待ち・評価待ち」
        public string created_str { get; set; }//FIXME:なにこれ

        //public  FrilSeller seller;

        public bool is_sellitem = false;
        public bool is_buyitem = false;
        public struct StatusMessage {
            public const string on_sale = "出品中";
            public const string trading = "取引中";
            public const string wait_payment = "支払い待ち";
            public const string wait_shipping = "発送待ち";
            public const string wait_review = "受取待ち";
            public const string wait_done = "未評価";
            public const string done = "取引完了";
            public const string sold_out = "売却済";
            public const string stop = "公開停止中";
        }


        public string item_status_in_fril;//商品がsellingかtradingかsoldか







        public FrilItem() {

        }
        public FrilItem Clone() {
            return (FrilItem)MemberwiseClone();
        }
        public FrilItem(dynamic json) {
            try {
                //info
                var info = json.info;
                //this.seller = new FrilSeller(json.seller);
                this.item_id = ((long)info.item_id).ToString();
                this.item_name = info.item_name;
                this.detail = info.detail;
                this.s_price = (int)info.s_price;
                this.status = (int)info.status;
                this.t_status = (int)info.t_status;
                this.carriage = (int)info.carriage;
                this.d_method = (int)info.d_method;
                this.d_date = (int)info.d_date;
                this.d_area = (int)info.d_area;
                this.created_at = DateTime.Parse((string)info.created_at);
                this.user_id = ((long)info.user_id).ToString();
                this.screen_name = info.screen_name;
                this.category_id = (int)info.category_id;
                this.category_p_id = (int)info.category_p_id;
                this.size_id = (int)info.size_id;
                this.size_name = info.size_name;
                this.brand_id = (info.brand_id == null) ? -1 : (int)info.brand_id;
                this.i_brand_id = (info.i_brand_id == null) ? -1 : (int)info.i_brand_id;
                
                
                
                
                //image
                for (int i = 0; i < 4; i++) imageurls[i] = "";
                int num = 0;
                foreach (var image in json.imgs) {
                    imageurls[num++] = image.file_name;
                }
                //comment,like
                this.comments_count = (int)json.comments_count;
                this.likes_count = (int)json.likes_count;
            }
            catch (Exception ex) {
                Log.Logger.Error("フリル商品jsonパース失敗" + ex.Message);
            }
        }
        //商品のstatusのリストからリクエスト用の文字列を作成する
        static public string ItemStatusListTostring(List<int> op) {
            List<string> t = new List<string>();
            foreach (int o in op) {
                if (o == 0) t.Add("stop");
                if (o == 1) t.Add("on_sale");
                if (o == 2) t.Add("trading");
                if (o == 3) t.Add("sold_out");
            }
            if (t.Count == 0) return "";
            else return string.Join(",", t);
        }


        public void loadImageFromFile() {
            try {
                if (string.IsNullOrEmpty(this.imagepaths[0])) return;
                var bitmap = new Bitmap(this.imagepaths[0]);
                this.Image = FrilCommon.ResizeImage(bitmap, 50, 50);
                bitmap.Dispose();
            } catch(Exception ex) {
                this.Image = null;
                Console.WriteLine(ex);
            }
        }
        //表で表示するための画像を読み込む
        public void loadImageFromThumbnail() {
            try {
                using (WebClient webClient = new WebClient()) {
                    this.imageurls[0] = this.imageurls[0].Replace("f=webp", "f=jpeg");
                    Stream stream = webClient.OpenRead(this.imageurls[0]);
                    var bitmap = new Bitmap(stream);
                    this.Image = FrilCommon.ResizeImage(bitmap, 50, 50);
                    bitmap.Dispose();
                }
            } catch (Exception ex) {
                this.Image = null;
                Console.WriteLine(ex);
            }
        }









    }
}
