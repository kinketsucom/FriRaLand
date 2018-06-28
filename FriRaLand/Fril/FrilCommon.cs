using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Codeplex.Data;
using System.Drawing;


namespace RakuLand {
    class FrilCommon {
        public static Dictionary<string, string> conditionTypeFril = new Dictionary<string, string>();
        public static Dictionary<string, string> shippingPayersFril = new Dictionary<string, string>();
        public static Dictionary<string, string> shippingMethodsSellerFril = new Dictionary<string, string>();
        public static Dictionary<string, string> shippingMethodsBuyerFril = new Dictionary<string, string>();
        public static Dictionary<string, string> shippingFromAreas = new Dictionary<string, string>();
        public static Dictionary<string, string> shippingDurations = new Dictionary<string, string>();
        public static Dictionary<string, string> cancelOption = new Dictionary<string, string>();
        public static Dictionary<int, int> ItemConditionIdToSelectedIndex = new Dictionary<int, int>();
        public static List<ItemCondition> ItemConditionList = new List<ItemCondition>();
        public static Dictionary<int, int> ShippingPayerIdToSelectedIndexDictionary = new Dictionary<int, int>();
        public static List<ShippingPayer> ShippingPayerList = new List<ShippingPayer>();
        public static Dictionary<int, int> ShippingMethodIdToSelectedIndex = new Dictionary<int, int>();
        public static Dictionary<int, int> PrefectureIdToSelectedIndex = new Dictionary<int, int>();
        public static List<Prefecture> PrefectureList = new List<Prefecture>();
        public static Dictionary<int, int> ShippingDurationIdToSelectedIndexDictionary = new Dictionary<int, int>();
        public static List<ShippingDuration> ShippingDurationList = new List<ShippingDuration>();


        public static void init() {

            //Masterデータの読み込み
            getFrilCategories();
            getFrilSizeGroup();
            getFrilBrands();
            conditionTypeFril = new Dictionary<string, string>();
            conditionTypeFril.Add("新品、未使用", "5");
            conditionTypeFril.Add("未使用に近い", "4");
            conditionTypeFril.Add("目立った傷や汚れなし", "6");
            conditionTypeFril.Add("やや傷や汚れあり", "3");
            conditionTypeFril.Add("傷や汚れあり", "2");
            conditionTypeFril.Add("全体的に状態が悪い", "1");
            shippingPayersFril = new Dictionary<string, string>();
            shippingPayersFril.Add("送料込み(あなたが負担)", "1");
            shippingPayersFril.Add("着払い(購入者が負担)", "2");
            shippingMethodsSellerFril = new Dictionary<string, string>();
            shippingMethodsSellerFril.Add("未定", "9");
            shippingMethodsSellerFril.Add("かんたんフリルパック", "15");
            shippingMethodsSellerFril.Add("普通郵便", "1");
            shippingMethodsSellerFril.Add("レターパックライト", "4");
            shippingMethodsSellerFril.Add("レターパックプラス", "3");
            shippingMethodsSellerFril.Add("クリックポスト", "11");
            shippingMethodsSellerFril.Add("宅急便コンパクト", "14");
            shippingMethodsSellerFril.Add("ゆうパック元払い", "2");
            shippingMethodsSellerFril.Add("ヤマト宅急便", "6");
            shippingMethodsSellerFril.Add("ゆうパケット", "17");
            shippingMethodsSellerFril.Add("ゆうメール元払い", "12");
            shippingMethodsSellerFril.Add("スマートレター", "16");
            shippingMethodsBuyerFril = new Dictionary<string, string>();
            shippingMethodsBuyerFril.Add("未定", "9");
            shippingMethodsBuyerFril.Add("ゆうパック着払い", "8");
            shippingMethodsBuyerFril.Add("ヤマト宅急便", "6");
            shippingMethodsBuyerFril.Add("ゆうパケット", "17");
            shippingMethodsBuyerFril.Add("ゆうメール着払い", "13");
            shippingFromAreas = new Dictionary<string, string>();
            shippingFromAreas.Add("北海道", "1");
            shippingFromAreas.Add("青森県", "2");
            shippingFromAreas.Add("岩手県", "3");
            shippingFromAreas.Add("宮城県", "4");
            shippingFromAreas.Add("秋田県", "5");
            shippingFromAreas.Add("山形県", "6");
            shippingFromAreas.Add("福島県", "7");
            shippingFromAreas.Add("茨城県", "8");
            shippingFromAreas.Add("栃木県", "9");
            shippingFromAreas.Add("群馬県", "10");
            shippingFromAreas.Add("埼玉県", "11");
            shippingFromAreas.Add("千葉県", "12");
            shippingFromAreas.Add("東京都", "13");
            shippingFromAreas.Add("神奈川県", "14");
            shippingFromAreas.Add("新潟県", "15");
            shippingFromAreas.Add("富山県", "16");
            shippingFromAreas.Add("石川県", "17");
            shippingFromAreas.Add("福井県", "18");
            shippingFromAreas.Add("山梨県", "19");
            shippingFromAreas.Add("長野県", "20");
            shippingFromAreas.Add("岐阜県", "21");
            shippingFromAreas.Add("静岡県", "22");
            shippingFromAreas.Add("愛知県", "23");
            shippingFromAreas.Add("三重県", "24");
            shippingFromAreas.Add("滋賀県", "25");
            shippingFromAreas.Add("京都府", "26");
            shippingFromAreas.Add("大阪府", "27");
            shippingFromAreas.Add("兵庫県", "28");
            shippingFromAreas.Add("奈良県", "29");
            shippingFromAreas.Add("和歌山県", "30");
            shippingFromAreas.Add("鳥取県", "31");
            shippingFromAreas.Add("島根県", "32");
            shippingFromAreas.Add("岡山県", "33");
            shippingFromAreas.Add("広島県", "34");
            shippingFromAreas.Add("山口県", "35");
            shippingFromAreas.Add("徳島県", "36");
            shippingFromAreas.Add("香川県", "37");
            shippingFromAreas.Add("愛媛県", "38");
            shippingFromAreas.Add("高知県", "39");
            shippingFromAreas.Add("福岡県", "40");
            shippingFromAreas.Add("佐賀県", "41");
            shippingFromAreas.Add("長崎県", "42");
            shippingFromAreas.Add("熊本県", "43");
            shippingFromAreas.Add("大分県", "44");
            shippingFromAreas.Add("宮崎県", "45");
            shippingFromAreas.Add("鹿児島県", "46");
            shippingFromAreas.Add("沖縄県", "47");
            shippingDurations = new Dictionary<string, string>();
            shippingDurations.Add("1～2日で発送", "1");
            shippingDurations.Add("2～3日で発送", "2");
            shippingDurations.Add("4～7日で発送", "3");
            cancelOption = new Dictionary<string, string>();
            cancelOption.Add("見る", "1");
            cancelOption.Add("見ない", "2");
        }
        #region　いらないとおもいました
        //public struct RakumaCategory {
        //    public int id;
        //    public int type;
        //    public string name;
        //    public int parent;
        //    public int brandType;
        //    public int sizeType;
        //    public int hasChild;
        //    public int level;
        //    public int seq;
        //}
        //public static List<RakumaCategory> rakuma_categories = new List<RakumaCategory>();
        //public static Dictionary<int, List<RakumaCategory>> rakuma_categoryDictionary = new Dictionary<int, List<RakumaCategory>>();
        //public static Dictionary<int, List<RakumaSize>> rakuma_sizeDictionary = new Dictionary<int, List<RakumaSize>>();//sizeType(CategoryID) -> List<RakumaSize>
        //static void getRakumaCategories() {
        //    //rakuma_categories = new List<RakumaCategory>();
        //    rakuma_categoryDictionary = new Dictionary<int, List<RakumaCategory>>();
        //    string jsonstr = "";
        //    using (WebClient webClient = new WebClient()) {
        //        byte[] bytes = webClient.DownloadData("http://www.rakufuri.com/data/category.json");
        //        jsonstr = Encoding.UTF8.GetString(bytes);
        //    }
        //    dynamic json = DynamicJson.Parse(jsonstr);
        //    foreach (var data in json) {
        //        RakumaCategory c = new RakumaCategory();
        //        c.id = (int)data.id;
        //        c.type = (int)data.type;
        //        c.name = data.name;
        //        c.parent = (int)data.parent;
        //        c.brandType = (int)data.brandType;
        //        c.sizeType = (int)data.sizeType;
        //        c.hasChild = (int)data.hasChild;
        //        c.level = (int)data.level;
        //        c.seq = (int)data.seq;
        //        //rakuma_categories.Add(c);
        //        //dictionary作成
        //        if (rakuma_categoryDictionary.ContainsKey(c.parent) == false) rakuma_categoryDictionary[c.parent] = new List<RakumaCategory>();
        //        rakuma_categoryDictionary[c.parent].Add(c);
        //    }
        //    Log.Logger.Info("ラクマカテゴリリスト読み込み完了");
        //}
        //public struct RakumaSize {
        //    public int id;
        //    public int type;
        //    public string title;
        //    public int seq;
        //}
        //static void getRakumaSizeTypes() {
        //    rakuma_sizeDictionary = new Dictionary<int, List<RakumaSize>>();
        //    string jsonstr = "";
        //    using (WebClient webClient = new WebClient()) {
        //        byte[] bytes = webClient.DownloadData("http://www.rakufuri.com/data/size.json");
        //        jsonstr = Encoding.UTF8.GetString(bytes);
        //    }
        //    dynamic json = DynamicJson.Parse(jsonstr);
        //    foreach (var data in json) {
        //        RakumaSize s = new RakumaSize();
        //        s.id = (int)data.id;
        //        s.type = (int)data.type;
        //        s.title = data.title;
        //        s.seq = (int)data.seq;
        //        //dictionary作成
        //        if (rakuma_sizeDictionary.ContainsKey(s.type) == false) rakuma_sizeDictionary[s.type] = new List<RakumaSize>();
        //        rakuma_sizeDictionary[s.type].Add(s);
        //    }
        //    Log.Logger.Info("ラクマサイズリスト読み込み完了");
        //}

        #endregion
        public struct FrilCategory {
            public int id;
            public int parent_id;//0なら根カテゴリ
            public string name;
            public int grand_parent_id;//0なら根カテゴリ
            public int size_group_id;//jsonではnull, プログラム上では-1でグループ無し
            public List<int> child_ids;
        }
        public struct Brand {
            public int id;
            public string name_ja;
            public string is_deleted; // "no"
        }
        public struct SizeInfo {
            public int id;
            public string name;
            public string display_order;
            public string item_size_group_id;
            public string is_deleted;
        }
        public struct ItemCondition {
            public int id;
            public string name;
        }
        public struct ShippingPayer {
            public int id;
            public string name;
            public string code;
        }
        public struct Prefecture {
            public int id;
            public int area_group_id;
            public string name;
        }
        public struct ShippingDuration {
            public int id;
            public string name;
        }
        //public static List<FrilCategory> fril_categories = new List<FrilCategory>();
        public static Dictionary<int, List<FrilCategory>> fril_categoryDictionary = new Dictionary<int, List<FrilCategory>>();
        //public static List<FrilSizeGroup> fril_sizegroups = new List<FrilSizeGroup>();
        public static Dictionary<int, List<FrilSizeInfo>> fril_default_sizeInfoDictionary = new Dictionary<int, List<FrilSizeInfo>>();
        public static List<FrilBrand> fril_brands = new List<FrilBrand>();
        static void getFrilCategories() {
            //fril_categories = new List<FrilCategory>();
            fril_categoryDictionary = new Dictionary<int, List<FrilCategory>>();
            string jsonstr = "";
            using (WebClient webClient = new WebClient()) {
                byte[] bytes = webClient.DownloadData("https://api.fril.jp/api/v3/initial_data");
                jsonstr = Encoding.UTF8.GetString(bytes);
            }
            dynamic json = DynamicJson.Parse(jsonstr);
            foreach (var data in json.categories) {
                FrilCategory c = new FrilCategory();
                c.id = (int)data.id;
                c.parent_id = (int)data.parent_id;
                c.name = data.name;
                c.grand_parent_id = (int)data.grand_parent_id;
                c.child_ids = new List<int>();
                foreach (var child in data.child_ids) {
                    c.child_ids.Add((int)child);
                }
                c.size_group_id = (data.size_group_id == null) ? -1 : (int)data.size_group_id;
                //fril_categories.Add(c);
                //dictionary作成
                if (fril_categoryDictionary.ContainsKey(c.parent_id) == false) fril_categoryDictionary[c.parent_id] = new List<FrilCategory>();
                fril_categoryDictionary[c.parent_id].Add(c);
            }
            Log.Logger.Info("フリルカテゴリリスト読み込み完了");
        }

        public struct FrilSizeGroup {
            public int id;
            public string name;
            public bool hidden;
            public List<FrilSizeType> size_types;
            public List<FrilSizeInfo> default_size_list; //FrilSizeType.name = "デフォルト"になっているもの、なければ0個目のsize
        }
        public struct FrilSizeType {
            public string name;
            public List<FrilSizeInfo> sizes;
        }
        public struct FrilSizeInfo {
            public int id;
            public string name;
        }
        public static void getFrilSizeGroup() {
            //fril_sizegroups = new List<FrilSizeGroup>();
            fril_default_sizeInfoDictionary = new Dictionary<int, List<FrilSizeInfo>>();
            string jsonstr = "";
            using (WebClient webClient = new WebClient()) {
                byte[] bytes = webClient.DownloadData("https://api.fril.jp/api/v3/initial_data");
                jsonstr = Encoding.UTF8.GetString(bytes);
            }
            dynamic json = DynamicJson.Parse(jsonstr);
            foreach (var sizegroup in json.size_groups) {
                FrilSizeGroup sg = new FrilSizeGroup();
                sg.id = (int)sizegroup.id;
                sg.name = sizegroup.name;
                sg.hidden = (bool)sizegroup.hidden;
                sg.size_types = new List<FrilSizeType>();
                sg.default_size_list = new List<FrilSizeInfo>();
                foreach (var sz in sizegroup.size_types) {
                    FrilSizeType size_type = new FrilSizeType();
                    size_type.name = sz.name;
                    size_type.sizes = new List<FrilSizeInfo>();
                    foreach (var si in sz.sizes) {
                        FrilSizeInfo size_info = new FrilSizeInfo();
                        size_info.id = (int)si.id;
                        size_info.name = si.name;
                        size_type.sizes.Add(size_info);
                    }
                    if (sz.name == "デフォルト") sg.default_size_list = size_type.sizes; 
                    sg.size_types.Add(size_type);
                }
                if (sg.default_size_list.Count == 0) sg.default_size_list = sg.size_types[0].sizes;
                //fril_sizegroups.Add(sg)
                fril_default_sizeInfoDictionary[sg.id] = sg.default_size_list;
            }
            Log.Logger.Info("フリルサイズデータ読み込み完了");
        }
        public struct FrilBrand {
            public int id;
            public string name;
            public string kana_name;
            public int confirm_flag;
        }
        public static void getFrilBrands() {
            fril_brands = new List<FrilBrand>();
            string jsonstr = "";
            using (WebClient webClient = new WebClient()) {
                byte[] bytes = webClient.DownloadData("https://api.fril.jp/api/v3/brands");
                jsonstr = Encoding.UTF8.GetString(bytes);
            }
            dynamic json = DynamicJson.Parse(jsonstr);
            foreach (var bd in json.brands) {
                var brand = new FrilBrand();
                brand.id = (int)bd.id;
                brand.name = bd.name;
                brand.kana_name = bd.kana_name;
                brand.confirm_flag = (int)bd.confirm_flag;
                fril_brands.Add(brand);
            }
            Log.Logger.Info("フリルブランドデータ読み込み完了");
        }

        public static Bitmap ResizeImage(Bitmap image, double dw, double dh) {
            double hi;
            double imagew = image.Width;
            double imageh = image.Height;

            if ((dh / dw) <= (imageh / imagew)) {
                hi = dh / imageh;
            } else {
                hi = dw / imagew;
            }
            int w = (int)(imagew * hi);
            int h = (int)(imageh * hi);

            Bitmap result = new Bitmap(w, h);
            Graphics g = Graphics.FromImage(result);
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            g.DrawImage(image, 0, 0, result.Width, result.Height);

            return result;
        }

















    }
}
