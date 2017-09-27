using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Codeplex.Data;

namespace FriRaLand {
    class FrilRaCommon {
        public static Dictionary<string, string> conditionTypeRakuma = new Dictionary<string, string>();
        public static Dictionary<string, string> conditionTypeFril = new Dictionary<string, string>();
        public static Dictionary<string, string> shippingPayersRakuma = new Dictionary<string, string>();
        public static Dictionary<string, string> shippingPayersFril = new Dictionary<string, string>();
        public static Dictionary<string, string> shippingMethodsSellerRakuma = new Dictionary<string, string>();
        public static Dictionary<string, string> shippingMethodsSellerFril = new Dictionary<string, string>();
        public static Dictionary<string, string> shippingMethodsBuyerRakuma = new Dictionary<string, string>();
        public static Dictionary<string, string> shippingMethodsBuyerFril = new Dictionary<string, string>();
        public static Dictionary<string, string> shippingFromAreas = new Dictionary<string, string>();
        public static Dictionary<string, string> shippingDurations = new Dictionary<string, string>();
        public static Dictionary<string, string> cancelOption = new Dictionary<string, string>();

        public static void init() {
            //Masterデータの読み込み
            getCategories();
            getSizeGroup();
            getBrands();
            conditionTypeRakuma = new Dictionary<string, string>();
            conditionTypeRakuma.Add("新品、未使用", "1");
            conditionTypeRakuma.Add("未使用に近い", "2");
            conditionTypeRakuma.Add("目立った傷や汚れなし", "3");
            conditionTypeRakuma.Add("やや傷や汚れあり", "4");
            conditionTypeRakuma.Add("傷や汚れあり", "5");
            conditionTypeRakuma.Add("全体的に状態が悪い", "6");
            conditionTypeFril = new Dictionary<string, string>();
            conditionTypeFril.Add("新品、未使用", "5");
            conditionTypeFril.Add("未使用に近い", "4");
            conditionTypeFril.Add("目立った傷や汚れなし", "6");
            conditionTypeFril.Add("やや傷や汚れあり", "3");
            conditionTypeFril.Add("傷や汚れあり", "2");
            conditionTypeFril.Add("全体的に状態が悪い", "1");
            shippingPayersRakuma = new Dictionary<string, string>();
            shippingPayersRakuma.Add("送料込み(出品者負担)", "2");
            shippingPayersRakuma.Add("着払い(購入者負担)", "1");
            shippingPayersFril = new Dictionary<string, string>();
            shippingPayersFril.Add("送料込み(あなたが負担)", "1");
            shippingPayersFril.Add("着払い(購入者が負担)", "2");
            shippingMethodsSellerRakuma = new Dictionary<string, string>();
            shippingMethodsSellerRakuma.Add("未定", "1");
            shippingMethodsSellerRakuma.Add("ラクマ定額パック（日本郵便）", "14");
            shippingMethodsSellerRakuma.Add("ラクマ定額パック（ヤマト運輸）", "12");
            shippingMethodsSellerRakuma.Add("クリックポスト", "10");
            shippingMethodsSellerRakuma.Add("ゆうメール", "2");
            shippingMethodsSellerRakuma.Add("ゆうパケット", "13");
            shippingMethodsSellerRakuma.Add("レターパック", "4");
            shippingMethodsSellerRakuma.Add("普通郵便（定型、定型外）", "5");
            shippingMethodsSellerRakuma.Add("ゆうパック", "7");
            shippingMethodsSellerRakuma.Add("宅急便コンパクト", "11");
            shippingMethodsSellerRakuma.Add("宅急便", "6");
            shippingMethodsSellerRakuma.Add("はこBOON", "9");
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
            shippingMethodsBuyerRakuma = new Dictionary<string, string>();
            shippingMethodsBuyerRakuma.Add("未定", "1");
            shippingMethodsBuyerRakuma.Add("ゆうメール", "5");
            shippingMethodsBuyerRakuma.Add("ゆうパック", "4");
            shippingMethodsBuyerRakuma.Add("宅急便", "3");
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
        public struct Category {
            public int id;
            public int parent_id;//0なら根カテゴリ
            public string name;
            public int grand_parent_id;//0なら根カテゴリ
            public int size_group_id;//jsonではnull, プログラム上では-1でグループ無し
            public List<int> child_ids;
        }
        public static List<Category> categories = new List<Category>();
        public static List<SizeGroup> sizegroups = new List<SizeGroup>();
        public static List<Brand> brands = new List<Brand>();
        static void getCategories() {
            categories = new List<Category>();
            string jsonstr = "";
            using (WebClient webClient = new WebClient()) {
                byte[] bytes = webClient.DownloadData("https://api.fril.jp/api/v3/initial_data");
                jsonstr = Encoding.UTF8.GetString(bytes);
            }
            dynamic json = DynamicJson.Parse(jsonstr);
            foreach (var data in json.categories) {
                Category c = new Category();
                c.id = (int)data.id;
                c.parent_id = (int)data.parent_id;
                c.name = data.name;
                c.grand_parent_id = (int)data.grand_parent_id;
                c.child_ids = new List<int>();
                foreach (var child in data.child_ids) {
                    c.child_ids.Add((int)child);
                }
                c.size_group_id = (data.size_group_id == null) ? -1 : (int)data.size_group_id;
                categories.Add(c);
            }
            Log.Logger.Info("カテゴリリスト読み込み完了");
        }

        public struct SizeGroup {
            public int id;
            public string name;
            public bool hidden;
            public List<SizeType> size_types;
        }
        public struct SizeType {
            public string name;
            public List<SizeInfo> sizes;
        }
        public struct SizeInfo {
            public int id;
            public string name;
        }
        public static void getSizeGroup() {
            sizegroups = new List<SizeGroup>();
            string jsonstr = "";
            using (WebClient webClient = new WebClient()) {
                byte[] bytes = webClient.DownloadData("https://api.fril.jp/api/v3/initial_data");
                jsonstr = Encoding.UTF8.GetString(bytes);
            }
            dynamic json = DynamicJson.Parse(jsonstr);
            foreach (var sizegroup in json.size_groups) {
                SizeGroup sg = new SizeGroup();
                sg.id = (int)sizegroup.id;
                sg.name = sizegroup.name;
                sg.hidden = (bool)sizegroup.hidden;
                sg.size_types = new List<SizeType>();
                foreach (var sz in sizegroup.size_types) {
                    SizeType size_type = new SizeType();
                    size_type.name = sz.name;
                    size_type.sizes = new List<SizeInfo>();
                    foreach (var si in sz.sizes) {
                        SizeInfo size_info = new SizeInfo();
                        size_info.id = (int)si.id;
                        size_info.name = si.name;
                        size_type.sizes.Add(size_info);
                    }
                    sg.size_types.Add(size_type);
                }
                sizegroups.Add(sg);
            }
            Log.Logger.Info("サイズデータ読み込み完了");
        }
        public struct Brand {
            public int id;
            public string name;
            public string kana_name;
            public int confirm_flag;
        }
        public static void getBrands() {
            brands = new List<Brand>();
            string jsonstr = "";
            using (WebClient webClient = new WebClient()) {
                byte[] bytes = webClient.DownloadData("https://api.fril.jp/api/v3/brands");
                jsonstr = Encoding.UTF8.GetString(bytes);
            }
            dynamic json = DynamicJson.Parse(jsonstr);
            foreach (var bd in json.brands) {
                var brand = new Brand();
                brand.id = (int)bd.id;
                brand.name = bd.name;
                brand.kana_name = bd.kana_name;
                brand.confirm_flag = (int)bd.confirm_flag;
                brands.Add(brand);
            }
            Log.Logger.Info("ブランドデータ読み込み完了");
        }
    }
}
