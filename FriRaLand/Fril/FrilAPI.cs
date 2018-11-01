using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Codeplex.Data;
using System.IO;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Security.Cryptography;
using System.Windows.Forms;
using RakuLand.DBHelper;

namespace RakuLand {
    public class FrilAPI {
        private const string USER_AGENT = "Mozilla/5.0 (iPhone; CPU iPhone OS 10_3_3 like Mac OS X) AppleWebKit/603.3.8 (KHTML, like Gecko) Mobile/14G60 Fril/7.2.0";
        private string proxy;
        private const string XPLATFORM = "android";
        private const string XAPPVERSION = "600";

        public string global_refresh_token; //未使用

        //GET,POSTのRequestのResponse
        private class FrilRawResponse {
            public bool error = false;
            public string response = "";
            public FrilRawResponse() { }
            public FrilRawResponse(string response, bool error=false) {
            this.response = response; this.error = error;
            }

        }
        public class Bank {
            public string bankid;
            public string kind;
            public string branch_id;
            public string account_number;
            public string family_name;
            public string first_name;
            public string birthday;
        }

        public class RakumaNotificationResponse{
            public DateTime created_at;
            public string id;//これがlatest_id
            public string image_url;
            public string item_id;
            public string message;
            public string type_id;
            //1:meのいいね,2:meのコメント,10,取引メッセ,25楽間からのメッセ？（振込手続き完了）,26:（ポイント有効期限が近づく）
            public DateTime updated_at;
            public string url;
            public FrilAPI api;
        }



        //通知のunreadcountをもらう
        public Dictionary<string,double> getNotificationCount() {
            Dictionary<string, double> rst = new Dictionary<string, double>();
            try {
                string url = string.Format("https://api.fril.jp/api/v4/notifications/unread_count?auth_token={0}&me_latest_id={1}&official_latest_id={2}&order_latest_id={3}", this.account.auth_token, this.account.nortification_needed_info.me_latest_id, this.account.nortification_needed_info.official_latest_id, this.account.nortification_needed_info.order_latest_id);
                var param = new Dictionary<string, string>();
                FrilRawResponse rawres = getFrilAPI(url, param, this.account.cc, false);
                if (rawres.error) throw new Exception("getNotificationCount Error");
                dynamic resjson = DynamicJson.Parse(rawres.response);
                rst.Add("count", resjson.count);
                rst.Add("me", resjson.me);
                rst.Add("official", resjson.official);
                rst.Add("order", resjson.order);
                Log.Logger.Info("ラクマからの通知カウントの取得に成功");
                return rst;
            } catch (Exception ex) {
                Log.Logger.Error("ラクマからの通知カウントの取得に失敗");
                Console.WriteLine(ex);
                return rst;
            }
        }
        //通知を取得する
        public List<RakumaNotificationResponse> getNotifications(bool get_all=false) {
            List<RakumaNotificationResponse> rst = new List<RakumaNotificationResponse>();
            try {
                //meの部分
                string url = string.Format("https://api.fril.jp/api/v4/notifications?auth_token={0}&type=me",this.account.auth_token);
                var param = new Dictionary<string, string>();
                FrilRawResponse rawres = getFrilAPI(url, param,this.account.cc,false);
                if (rawres.error) throw new Exception("getNotifications Error");
                dynamic resjson = DynamicJson.Parse(rawres.response);
                Dictionary<string,double> dic = getNotificationCount();//表示件数を決定するdic
                int count = 0;
                foreach (var data in resjson.notifications) {
                    if (!get_all) { //全部取らないとき
                       if (count >= (int)dic["me"]) break;
                    }
                    if (data.type_id != 1) {
                        RakumaNotificationResponse notification = new RakumaNotificationResponse();
                        string time = data.created_at.Substring(0, data.created_at.IndexOf("+"));
                        time = time.Replace("-", "/");
                        time = time.Replace("T", " ");
                        notification.created_at = DateTime.Parse(time);
                        notification.id = data.id.ToString();
                        if (data.image_url == null) notification.image_url = "";
                        else notification.image_url = data.image_url.ToString();
                        if (data.item_id == null) notification.item_id = "";
                        else notification.item_id = data.item_id.ToString();
                        notification.message = data.message.ToString();
                        notification.type_id = data.type_id.ToString();
                        time = data.updated_at.Substring(0, data.updated_at.IndexOf("+"));
                        time = time.Replace("-", "/");
                        time = time.Replace("T", " ");
                        notification.updated_at = DateTime.Parse(time);
                        notification.url = data.url.ToString();
                        notification.api = this;
                        if (int.Parse(notification.id) > this.account.nortification_needed_info.me_latest_id) {
                            account.nortification_needed_info.me_latest_id = int.Parse(notification.id);
                            new AccountDBHelper().updateAccount(this.account.DBId, this.account);
                        }
                        count += 1;
                        rst.Add(notification);
                    }
                }


                //orderの部分
                url = string.Format("https://api.fril.jp/api/v4/notifications?auth_token={0}&type=order", this.account.auth_token);
                param = new Dictionary<string, string>();
                rawres = getFrilAPI(url, param, this.account.cc, false);
                if (rawres.error) throw new Exception("getNotifications Error");
                resjson = DynamicJson.Parse(rawres.response);
                count = 0;
                foreach (var data in resjson.notifications) {
                    if (!get_all) { //全部取らないとき
                        if (count >= (int)dic["order"]) break;
                    }
                    RakumaNotificationResponse notification = new RakumaNotificationResponse();
                    string time = data.created_at.Substring(0, data.created_at.IndexOf("+"));
                    time = time.Replace("-", "/");
                    time = time.Replace("T", " ");
                    notification.created_at = DateTime.Parse(time);
                    notification.id = data.id.ToString();
                    if (data.image_url == null) notification.image_url = "";
                    else notification.image_url = data.image_url.ToString();
                    if (data.item_id == null) notification.item_id = "";
                    else notification.item_id = data.item_id.ToString();
                    notification.message = data.message.ToString();
                    notification.type_id = data.type_id.ToString();
                    time = data.updated_at.Substring(0, data.updated_at.IndexOf("+"));
                    time = time.Replace("-", "/");
                    time = time.Replace("T", " ");
                    notification.updated_at = DateTime.Parse(time);
                    notification.url = data.url.ToString();
                    notification.api = this;
                    if (int.Parse(notification.id) > this.account.nortification_needed_info.order_latest_id) {
                        account.nortification_needed_info.order_latest_id = int.Parse(notification.id);
                        new AccountDBHelper().updateAccount(this.account.DBId, this.account);
                    }
                    count += 1;
                    rst.Add(notification);
                }














                Log.Logger.Info("ラクマからの通知の取得に成功");
                return rst;
            } catch (Exception ex) {
                Log.Logger.Error("ラクマからの通知の取得に失敗");
                Console.WriteLine(ex);
                return rst;
            }
        }


        #region  FIXME:ラクマ用につくりかえようとしているもの
        public void GetBankInfo() {//銀行口座情報を取得する
            Dictionary<string, string> param = new Dictionary<string, string>();
            param.Add("auth_token", this.account.auth_token);
            string url = "https://api.fril.jp/api/bank";
            FrilRawResponse res = postFrilAPI(url, param, this.account.cc);
            dynamic resjson = DynamicJson.Parse(res.response);
            if (!resjson.result) return;//口座情報がない
            try {
                this.account.bank_info.account_number = resjson.bank.account_number;
                this.account.bank_info.bank_code_id = resjson.bank.bank_code_id;
                this.account.bank_info.branch_code = resjson.bank.branch_code;
                this.account.bank_info.branch_name = resjson.bank.branch_name;
                this.account.bank_info.code = resjson.bank.code;
                this.account.bank_info.deposit_type = resjson.bank.deposit_type;
                this.account.bank_info.first_name = resjson.bank.first_name;
                this.account.bank_info.id = resjson.bank.id;
                this.account.bank_info.last_name = resjson.bank.last_name;
                this.account.bank_info.name = resjson.bank.name;
            } catch(Exception ex) {
                Log.Logger.Error(ex);
                Console.WriteLine(ex);
            }
        }

        public void GetBalanceInfo() {//収益情報を取得する
            Dictionary<string, string> param = new Dictionary<string, string>();
            param.Add("auth_token", this.account.auth_token);
            string url = "https://api.fril.jp/api/balance/show";
            FrilRawResponse res = postFrilAPI(url, param, this.account.cc);
            dynamic resjson = DynamicJson.Parse(res.response);
            if (!resjson.result) return;//収益情報がない
            try {
                this.account.balance_info.balance = resjson.balance;
                this.account.balance_info.bank = resjson.bank;
                this.account.balance_info.idverify_pending =resjson.idverify_pending;
                this.account.balance_info.point = resjson.point;
                this.account.balance_info.result = resjson.result;
                this.account.balance_info.withdrawal = resjson.withdrawal;
            } catch (Exception ex) {
                Log.Logger.Error(ex);
                Console.WriteLine(ex);
            }

        }


        #endregion


        public class Address {
            public bool is_default;
            public string zip_code1;
            public string zip_code2;
            public string family_name;
            public string first_name;
            public string family_name_kana;
            public string first_name_kana;
            public string prefecture;
            public string city;
            public string city_normalize;
            public string address1;
            public string address1_crc;
            public string address2;
            public string telephone;
            public string telephone_normalize;
            public long id;
            public override string ToString() {
                return zip_code1 + "-" + zip_code2 + " " + prefecture + city + address1 + address2 + " " + family_name + first_name;
            }
            static public bool compare(Address a, Address b) {
                //id, is_default以外がすべて同じならtrue;
                return (a.zip_code1 == b.zip_code1 && a.family_name == b.family_name && a.first_name == b.first_name && a.family_name_kana == b.family_name_kana
                    && a.first_name_kana == b.first_name_kana && a.prefecture == b.prefecture && a.city == b.city && a.address1 == b.address1 && a.address2 == b.address2 && a.telephone == b.telephone);
            }
        }
        public class Uriage {
            public int current_sales;
            public int payment_fee;
        }

        public Dictionary<string, string> getMainBankDictionary() {
            Dictionary<string, string> param = new Dictionary<string, string>();
            param.Add("auth_token", this.account.auth_token);
            string url = "https://api.mercari.jp/master/get_banks";//FIXME:Frilのものにかえる
            FrilRawResponse res = getFrilAPI(url, param,this.account.cc);
            if (res.error) return null;
            Dictionary<string, string> rst = new Dictionary<string, string>();
            try {
                dynamic resjson = DynamicJson.Parse(res.response);
                var main_bank = resjson.data.main_bank;
                foreach (var bank in main_bank) {
                    rst[bank.name] = bank.code;
                }
                return rst;
            } catch {
                return null;
            }
        }
        public Dictionary<string, Dictionary<string, string>> getOtherBankDictionary() {
            Dictionary<string, string> param = new Dictionary<string, string>();
            param.Add("auth_token", this.account.auth_token);
            string url = "https://api.mercari.jp/master/get_banks";//FIXME:Frilのものにかえる
            FrilRawResponse res = getFrilAPI(url, param,this.account.cc);
            if (res.error) return null;
            Dictionary<string, Dictionary<string, string>> rst = new Dictionary<string, Dictionary<string, string>>();
            try {
                dynamic resjson = DynamicJson.Parse(res.response);
                var initials = resjson.data.initials;
                foreach (var initial in initials) { //あ行,か行・・
                    var datas = initial.datas;
                    foreach (var data in datas) { //あ, い...
                        Dictionary<string, string> tmp = new Dictionary<string, string>();
                        string initial_str = data.initial; //あ. い
                        var datadatas = data.datas;
                        foreach (var datadata in datadatas) {
                            string name = datadata.name;
                            string code = datadata.code;
                            tmp[name] = code;
                        }
                        rst[initial_str] = tmp;
                    }
                }
                return rst;
            } catch {
                return null;
            }
        }
        public Uriage getCurrentSales() {
            Dictionary<string, string> param = new Dictionary<string, string>();
            param.Add("auth_token", this.account.auth_token);
            string url = "https://api.mercari.jp/sales/get_current_sales";//FIXME:Frilのものにかえる
            FrilRawResponse res = getFrilAPI(url, param, this.account.cc);
            if (res.error) {
                Log.Logger.Error("現在の売り上げ取得失敗");
                return null;
            }
            try {
                dynamic resjson = DynamicJson.Parse(res.response);
                Uriage rst = new Uriage();
                rst.current_sales = (int)resjson.data.current_sales;
                rst.payment_fee = (int)resjson.data.payment_fee;
                Log.Logger.Info("現在の売り上げ取得成功");
                return rst;
            } catch {
                Log.Logger.Error("現在の売り上げ取得失敗");
                return null;
            }
        }
        public Bank getBankAccounts() {
            try {
                Dictionary<string, string> param = new Dictionary<string, string>();
                param.Add("_access_token", this.account.auth_token);
                string url = "https://api.mercari.jp/bank_accounts/get";//FIXME:Frilのものにかえる
                FrilRawResponse res = getFrilAPI(url, param,this.account.cc);
                if (res.error) throw new Exception("アカウントの口座情報取得失敗");
                Bank rst = new Bank();
                dynamic json = DynamicJson.Parse(res.response);
                var bank_account = json.data.bank_account;
                rst.bankid = bank_account.bank_id;
                rst.kind = bank_account.kind;
                rst.branch_id = bank_account.branch_id;
                rst.account_number = bank_account.account_number;
                rst.family_name = bank_account.family_name;
                rst.first_name = bank_account.first_name;
                rst.birthday = json.data.anti_social.birthday;
                Log.Logger.Info("アカウントの口座情報取得成功");
                return rst;
            } catch {
                Log.Logger.Error("アカウントの口座情報取得失敗");
                return null;
            }
        }

        public Address getAddressWithBank() {
            try {
                Dictionary<string, string> param = new Dictionary<string, string>();
                param.Add("auth_token", this.account.auth_token);
                string url = "https://api.mercari.jp/bank_accounts/get";//FIXME:Frilのものにかえる
                FrilRawResponse res = getFrilAPI(url, param,this.account.cc);
                if (res.error) throw new Exception("アカウントの口座情報取得失敗");
                Address rst = new Address();
                dynamic json = DynamicJson.Parse(res.response);
                var address = json.data.anti_social.address;
                rst = new Address();
                rst.address1 = address.address1;
                rst.address1_crc = address.address1_crc;
                rst.address2 = address.address2;
                rst.city = address.city;
                rst.city_normalize = address.city_normalize;
                rst.family_name = address.family_name;
                rst.family_name_kana = address.family_name_kana;
                rst.first_name = address.first_name;
                rst.first_name_kana = address.first_name_kana;
                rst.id = (long)address.id;
                rst.is_default = (bool)address.is_default;
                rst.prefecture = address.prefecture;
                rst.telephone = address.telephone;
                rst.telephone_normalize = address.telephone_normalize;
                rst.zip_code1 = address.zip_code1;
                rst.zip_code2 = address.zip_code2;
                Log.Logger.Info("アカウントの口座紐付き住所取得成功");
                return rst;
            } catch {
                Log.Logger.Error("アカウントの口座紐付き住所取得失敗");
                return null;
            }
        }

        public Common.Account account;



        //public FrilAPI(string email, string password) {
        //    this.account = new Common.Account();
        //    this.account.kind = Common.Account.Fril_Account;
        //    this.account.email = email;
        //    this.account.password = password;
        //}
        public FrilAPI(Common.Account account) {
            this.account = account;
        }
        //public FrilAPI(Common.Account account) {//Mainformのaaccountから取ってくるよう
        //    this.account = new Common.Account();
        //    this.account.kind = Common.Account.Fril_Account;
        //    this.account.email = account.email;
        //    this.account.password = account.password;
        //    this.account.fril_fril_auth_token = accountaccount.auth_token;
        //}
        public struct TradingStatus {
            public const string Wait_Paymet = "wait_payment";
            public const string Wait_Shipping = "wait_shipping";
            public const string Wait_Review = "wait_review";
            public const string Wait_Done = "wait_done";
            public const string Done = "done";
        }


        //get_items でのOption
        private class GetItemsOption {
            public string sellerid = "";
            public List<int> status_list = new List<int>();
            public Dictionary<string, string> ToPairList() {
                //空文字列あるいは空リストの場合そのオプションはなし
                Dictionary<string, string> rst = new Dictionary<string, string>();
                if (this.sellerid != "") rst.Add("seller_id", this.sellerid);
                if (this.status_list.Count != 0) rst.Add("status", FrilItem.ItemStatusListTostring(this.status_list));
                return rst;
            }
        }

        //成功: itemID 失敗: null
        public string Sell(FrilItem item,CookieContainer cc) {
            try {
                //商品情報をまず送る
                string url = "https://api.fril.jp/api/items/request";
                Dictionary<string, string> param = new Dictionary<string, string>();
                param.Add("auth_token", this.account.auth_token);
                param.Add("carriage", item.carriage.ToString());
                param.Add("category", item.category_id.ToString());
                param.Add("delivery_area", item.d_area.ToString());
                param.Add("delivery_date", item.d_date.ToString());
                param.Add("delivery_method", item.d_method.ToString());
                param.Add("detail", item.detail);
                param.Add("item_id", "0");
                param.Add("p_category", item.category_p_id.ToString());
                param.Add("request_required", "0");
                param.Add("sell_price", item.s_price.ToString());
                if (item.size_id <= 0) {
                    param.Add("size", "19999");
                    param.Add("size_name", "なし");
                } else { 
                    param.Add("size", item.size_id.ToString());
                    param.Add("size_name", item.size_name);
                }
                param.Add("status", item.status.ToString());
                param.Add("title", item.item_name);
                //ブランドidは指定のものを追加か、パラメータまったく含めないの二択
                //それ以外はクラッシュ
                if (item.brand_id > 0) { 
                    param.Add("brand", item.brand_id.ToString());
                }                
                //パラメータ表示
                foreach(var val in param) {
                    Console.WriteLine(val.Key + ":" + val.Value);
                }


                var rawres = postFrilAPI(url, param, cc);
                if (rawres.error) throw new Exception("商品情報の送信に失敗");
                if (string.IsNullOrEmpty(rawres.response)) {
                    MessageBox.Show("連続出品しすぎか、アカウントが何らかの理由で制限があります。\n公式webでアカウントの確認をしてください。");
                    throw new Exception("アカウントに制限ありの可能性or連続出品制限"); }
                string res = "";
                //itemIDをとりだして画像を送る
                string item_id = ((long)DynamicJson.Parse(rawres.response).item_id).ToString();
                int total_img_num = 0;
                foreach (var imagepath in item.imagepaths) if (string.IsNullOrEmpty(imagepath) == false) total_img_num++;
                for (int num = 1; num <= total_img_num; num++) {
                    string image_url = "https://api.fril.jp/api/items/request_img";
                    Dictionary<string, string> req_img_param = new Dictionary<string, string>();
                    req_img_param.Add("auth_token", this.account.auth_token);
                    req_img_param.Add("item_id", item_id);
                    req_img_param.Add("current_num", num.ToString());
                    req_img_param.Add("total_num", total_img_num.ToString());
                    res = postMultipartFril(image_url, req_img_param, item.imagepaths[num - 1],cc);
                }
                Console.WriteLine(res);
                Log.Logger.Info("商品出品成功: " + item_id);
                return item_id;
            }
            catch (Exception ex) {
                Console.WriteLine(ex);
                Log.Logger.Error("商品出品失敗: " + ex.Message);
                return null;
            }
        }
        public bool tryFrilLogin(CookieContainer cc) {
            /*revert,android_id,device_id,app_generated_idなどのパラメタはなくてもOKなので送らない*/
            Dictionary<string, string> param = new Dictionary<string, string>();
            param.Add("email", this.account.email);
            param.Add("password", this.account.password);
            //installation_idをランダムに作成し新しい端末でのログインとする
            string rand1 = Guid.NewGuid().ToString("N").Replace("-", "").Substring(0, 8);
            string rand2 = Guid.NewGuid().ToString("N").Replace("-", "").Substring(0, 4);
            string rand3 = Guid.NewGuid().ToString("N").Replace("-", "").Substring(0, 4);
            string rand4 = Guid.NewGuid().ToString("N").Replace("-", "").Substring(0, 12);
            string dummy_installation_id = string.Join("-", new string[] { rand1, rand2, rand3, rand4 });
            param.Add("installation_id", dummy_installation_id);
            string url = "https://api.fril.jp/api/v4/users/sign_in";
            FrilRawResponse rawres = postFrilAPI(url, param, cc);
            if (rawres.error) return false;
            try {
                dynamic resjson = DynamicJson.Parse(rawres.response);
                this.account.auth_token = resjson.auth_token;
                this.account.expirationDate = DateTime.Now.AddDays(90.0);
                this.account = getProfile(account,cc);
                Console.WriteLine("フリルログイン成功");
                Log.Logger.Info("フリルログイン成功");
                return true;
            }
            catch (Exception e) {
                Log.Logger.Info("フリルログイン失敗");
                return false;
            }
        }
        private Common.Account getProfile(Common.Account account,CookieContainer cc) {
            Dictionary<string, string> param = new Dictionary<string, string>();
            param.Add("auth_token", account.auth_token);
            FrilRawResponse res = getFrilAPI("http://api.fril.jp/api/v2/users", param,cc);
            var json = DynamicJson.Parse(res.response);
            account.nickname = json.user.screen_name;   
            account.userId = ((long)json.user.id).ToString();
            return account;
        }
        public FrilItem getItemDetailInfo(string item_id,CookieContainer cc)
        {
            Dictionary<string, string> param = new Dictionary<string, string>();
            param.Add("auth_token", this.account.auth_token);
            param.Add("item_id", item_id);
            string url = "http://api.fril.jp/api/v3/items/show";
            FrilRawResponse rawres = getFrilAPI(url, param,cc);
            if (rawres.error)
            {
                Log.Logger.Error(string.Format("フリル商品詳細情報取得失敗 id:{0}", item_id));
                return null;
            }
            dynamic resjson = DynamicJson.Parse(rawres.response);
            FrilItem item = new FrilItem(resjson.item);
            return item;
        }
        //ブラウザの商品ページから商品IDを得る
        //成功:商品ID 失敗null
        public string getItemIDFromBrowserItemURL(string browserItemURL,CookieContainer cc)
        {
            string urlHeader = "https://item.fril.jp/";
            try
            {
                if (browserItemURL.IndexOf(urlHeader) < 0)
                {
                    throw new Exception();
                }
                string hashstr = browserItemURL.Substring(urlHeader.Length, browserItemURL.Length - urlHeader.Length);
                Dictionary<string, string> param = new Dictionary<string, string>();
                param.Add("hash", hashstr);
                string url = "http://api.fril.jp/api/v3/items/show/open";
                FrilRawResponse rawres = getFrilAPI(url, param, cc);
                if (rawres.error) throw new Exception();
                dynamic resjson = DynamicJson.Parse(rawres.response);
                return ((long)resjson.item.info.item_id).ToString();
            }
            catch (Exception ex)
            {
                Log.Logger.Error("商品ページURLから商品ID取得に失敗:" + browserItemURL);
                return null;
            }
        }
        //ユーザが出品している商品をすべて取得
        public List<FrilItem> getSellingItem(string userId,string get_item_type ,CookieContainer cc) {
            List<FrilItem> rst = new List<FrilItem>();
            bool has_next = true;
            string max_id = "0"; //二回目以降で「この商品IDより後」の商品を取得する
            do {
                Dictionary<string, string> param = new Dictionary<string, string>();
                param.Add("auth_token", this.account.auth_token);
                param.Add("status", get_item_type);
                string url = "https://api.fril.jp/api/v3/items/sell";
                FrilRawResponse rawres = getFrilAPI(url, param,cc);
                if (rawres.error) {
                    Log.Logger.Error("フリル出品中商品の取得に失敗: UserID: " + this.account.userId);
                    return rst;
                }
                dynamic resjson = DynamicJson.Parse(rawres.response);
                foreach (dynamic data in resjson.items) {
                    FrilItem item = new FrilItem();
                    item.item_status_in_fril = resjson.type;
                    item.item_id = ((long)data.item_id).ToString();
                    item.imageurls[0] = data.img_url;
                    item.item_name = data.item_name;
                    //item.detail = data.item_detail;//FIXIT:商品説明のとりかたわかんないっす・・・
                    item.s_price = (int)data.price;
                    item.t_status = (int)data.t_status;
                    item.user_id = ((long)data.user_id).ToString();
                    item.brand_id = ((data.brand_id == null) ? -1 : (int)item.brand_id);
                    item.i_brand_id = ((data.i_brand_id == null) ? -1 : (int)item.i_brand_id);
                    item.screen_name = data.screen_name;
                    item.created_at = DateTime.Parse(data.created_at);
                    item.likes_count = (int)data.like_count;
                    item.comments_count = (int)data.comment_count;
                    item.status_message = get_item_type;
                    item.screen_name = this.account.nickname;
                    
                    rst.Add(item);
                    max_id = item.item_id;

                }
                has_next = false;//FIXME:ここわからなかったんでfalseいれてるだけで絶対ダメ
                //has_next = (bool)resjson.paging.has_next;//FIXME:ここもわからませんでした
            } while (has_next);
            return rst;
        }
        //FrilAPIをGETでたたく
        private FrilRawResponse getFrilAPI(string url, Dictionary<string, string> param, CookieContainer cc, bool webmode = false) {
            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
            //ストップウォッチを開始する
            sw.Start();
            FrilRawResponse res = new FrilRawResponse();
            try {
                //url = Uri.EscapeDataString(url);//日本語などを％エンコードする
                //パラメータをURLに付加 ?param1=val1&param2=val2...
                if (param.Count != 0) {
                    url += "?";
                    List<string> paramstr = new List<string>();
                    foreach (KeyValuePair<string, string> p in param) {
                        string k = Uri.EscapeDataString(p.Key);
                        string v = Uri.EscapeDataString(p.Value);
                        paramstr.Add(k + "=" + v);
                    }
                    url += string.Join("&", paramstr);
                }
                //HttpWebRequestの作成
                HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
                req.CookieContainer = cc;
                req.UserAgent = FrilAPI.USER_AGENT;
                req.Method = "GET";
                //webモードのときはauth_tokenをヘッダにいれる
                if (webmode && !string.IsNullOrEmpty(this.account.auth_token)) req.Headers.Add("Authorization", this.account.auth_token);
                //プロキシの設定
                if (string.IsNullOrEmpty(this.proxy) == false) {
                    System.Net.WebProxy proxy = new System.Net.WebProxy(this.proxy);
                    req.Proxy = proxy;
                }
                //結果取得
                string content = "";
                var task = Task.Factory.StartNew(() => executeGetRequest(req));
                task.Wait(10000);
                if (task.IsCompleted) {
                    res = task.Result;
                } else
                    throw new Exception("Timed out");
                if (res.error) throw new Exception("webrequest error");
                Log.Logger.Info("FrilGETリクエスト成功");
                return res;
            }
            catch (Exception e) {
                Log.Logger.Error("FrilGETリクエスト失敗:" + res.response);
                Console.WriteLine(e);
                return res;
            }
        }
        private FrilRawResponse executeGetRequest(HttpWebRequest req) {
            try {
                HttpWebResponse webres = (HttpWebResponse)req.GetResponse();
                Stream s = webres.GetResponseStream();
                StreamReader sr = new StreamReader(s);
                string content = sr.ReadToEnd();
                return new FrilRawResponse(content, false);
            }
            catch {
                return new FrilRawResponse("", true);
            }
        }
        private FrilRawResponse executePostRequest(HttpWebRequest req, byte[] bytes) {
            try {
                using (Stream requestStream = req.GetRequestStream()) {
                    requestStream.Write(bytes, 0, bytes.Length);
                }
                //結果取得
                string result = "";
                using (Stream responseStream = req.GetResponse().GetResponseStream()) {
                    using (StreamReader streamReader = new StreamReader(responseStream, Encoding.GetEncoding("UTF-8"))) {
                        return new FrilRawResponse(streamReader.ReadToEnd(), false);
                    }
                }
            }
            catch {
                return new FrilRawResponse("", true);
            }
        }
        //FrilAPIをDELETEでたたく
        private FrilRawResponse deleteFrilAPI(string url, Dictionary<string, string> param, CookieContainer cc, bool webmode = false) {
            FrilRawResponse res = new FrilRawResponse();
            try {
                string text = "";
                List<string> paramstr = new List<string>();
                int num = 0;
                foreach (KeyValuePair<string, string> p in param) {
                    string k = Uri.EscapeDataString(p.Key);
                    string v = Uri.EscapeDataString(p.Value);
                    if (num != 0) text += "&";
                    text = text + (k + "=" + v);
                    num++;
                }
                byte[] bytes = Encoding.ASCII.GetBytes(text);
                HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
                req.UserAgent = FrilAPI.USER_AGENT;
                req.Method = "DELETE";
                //リクエストヘッダを付加
                req.Headers.Add("Accept-Encoding", "br, gzip, deflate");
                req.ContentType = "application/x-www-form-urlencoded";
                req.Accept = "*/*";
                req.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
                req.ContentLength = (long)bytes.Length;
                //webモードのときはauth_tokenをヘッダにいれる
                if (webmode && !string.IsNullOrEmpty(this.account.auth_token)) req.Headers.Add("Authorization", this.account.auth_token);
                //クッキーコンテナの追加
                req.CookieContainer = cc;
                //プロキシの設定
                if (string.IsNullOrEmpty(this.proxy) == false) {
                    System.Net.WebProxy proxy = new System.Net.WebProxy(this.proxy);
                    req.Proxy = proxy;
                }
                //タイムアウト設定
                req.Timeout = 5000;
                //POST
                string content = "";
                var task = Task.Factory.StartNew(() => executePostRequest(ref req, bytes));
                task.Wait(10000);
                if (task.IsCompleted) {
                    content = task.Result;
                    Console.WriteLine(content);
                } else {
                    throw new Exception("Timed out");
                }
                if (res.error) throw new Exception("webrequest error");
                //if (res.Contains("false")) throw new Exception("item result false");
                res.error = false;
                res.response = content;
                Log.Logger.Error("FrilDELETEリクエスト失敗" + res.response);
                req.Abort();
                return res;
            } catch (Exception e) {

                Console.WriteLine(e);
                Log.Logger.Error("FrilDELETEリクエスト失敗");
                return res;
            }
        }
        //FrilAPIをPOSTでたたく
        private FrilRawResponse postFrilAPI(string url, Dictionary<string, string> param,CookieContainer cc, bool webmode = false) {
            FrilRawResponse res = new FrilRawResponse();
            try {
                string text = "";
                List<string> paramstr = new List<string>();
                int num = 0;
                foreach (KeyValuePair<string, string> p in param) {
                    string k = Uri.EscapeDataString(p.Key);
                    string v = Uri.EscapeDataString(p.Value);
                    if (num != 0) text += "&";
                    text = text + (k + "=" + v);
                    num++;
                }
                byte[] bytes = Encoding.ASCII.GetBytes(text);
                HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
                req.UserAgent = FrilAPI.USER_AGENT;
                req.Method = "POST";
                //リクエストヘッダを付加
                req.Headers.Add("Accept-Encoding", "br, gzip, deflate");
                req.ContentType = "application/x-www-form-urlencoded";
                req.Accept = "*/*";
                req.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
                req.ContentLength = (long)bytes.Length;
                //webモードのときはauth_tokenをヘッダにいれる
                if (webmode && !string.IsNullOrEmpty(this.account.auth_token)) req.Headers.Add("Authorization", this.account.auth_token);
                //クッキーコンテナの追加
                req.CookieContainer = cc;
                //プロキシの設定
                if (string.IsNullOrEmpty(this.proxy) == false) {
                    System.Net.WebProxy proxy = new System.Net.WebProxy(this.proxy);
                    req.Proxy = proxy;
                }
                //タイムアウト設定
                req.Timeout = 5000;
                //POST
                string content = "";
                var task = Task.Factory.StartNew(() => executePostRequest(ref req, bytes));
                task.Wait(100000);
                if (task.IsCompleted) {
                    content = task.Result;
                    Console.WriteLine(content);
                } else {
                    throw new Exception("Timed out");
                }
                if (res.error) throw new Exception("webrequest error");
                //if (res.Contains("false")) throw new Exception("item result false");
                res.error = false;
                res.response = content;
                Log.Logger.Error("FrilPOSTリクエスト失敗" + res.response);
                req.Abort();
                return res;
            }
            catch (Exception e) {
                
                Console.WriteLine(e);
                Log.Logger.Error("FrilPOSTリクエスト失敗");
                return res;
            }
        }
        //FrilAPIをMultiPartPOSTでたたく
        private FrilRawResponse postFrilAPIwithMultiPart(string url, Dictionary<string, string> param, Dictionary<int, string> imagedic) {
            FrilRawResponse res = new FrilRawResponse();

            Encoding encoding = Encoding.GetEncoding("UTF-8");
            string text = Environment.TickCount.ToString();
            byte[] bytes = encoding.GetBytes("\r\n");
            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
            httpWebRequest.UserAgent = FrilAPI.USER_AGENT;
            httpWebRequest.Headers.Set("X-PLATFORM", FrilAPI.XPLATFORM);
            httpWebRequest.Headers.Set("X-APP-VERSION", FrilAPI.XAPPVERSION);
            httpWebRequest.Method = "POST";
            httpWebRequest.ContentType = "multipart/form-data; boundary=" + text;
            string text2 = "";
            foreach (KeyValuePair<string, string> current in param) {
                text2 = string.Concat(new string[]
                {
                    text2,
                    "--",
                    text,
                    "\r\nContent-Disposition: form-data; name=\"",
                    current.Key,
                    "\"\r\n\r\n",
                    current.Value,
                    "\r\n"
                });
            }
            byte[] bytes2 = encoding.GetBytes(text2);
            List<byte[]> list = new List<byte[]>();
            long num = 0L;
            foreach (KeyValuePair<int, string> imagekey in imagedic) {
                string text3 = imagekey.Value;
                Path.GetFileName(text3);
                string s = string.Concat(new object[]
                {
                    "--",
                    text,
                    "\r\nContent-Disposition: form-data; name=\"photo_",
                    imagekey.Key,
                    "\"; filename=\"photo_",
                    imagekey.Key,
                    ".jpg\"\r\nContent-Type: image/jpeg\r\n\r\n"
                });
                list.Add(encoding.GetBytes(s));
                num += (long)encoding.GetBytes(s).Length + new FileInfo(text3).Length;
            }
            byte[] bytes3 = encoding.GetBytes("--" + text + "--\r\n");
            httpWebRequest.ContentLength = (long)bytes2.Length + num + (long)(bytes.Length * imagedic.Count) + (long)bytes3.Length;
            string result = "";
            try {
                using (Stream requestStream = httpWebRequest.GetRequestStream()) {
                    requestStream.Write(bytes2, 0, bytes2.Length);
                    int j = 0;
                    foreach (KeyValuePair<int, string> imagekey in imagedic) {
                        using (FileStream fileStream = new FileStream(imagekey.Value, FileMode.Open, FileAccess.Read)) {
                            requestStream.Write(list[j], 0, list[j].Length);
                            byte[] array = new byte[4096];
                            while (true) {
                                int num3 = fileStream.Read(array, 0, array.Length);
                                if (num3 == 0) {
                                    break;
                                }
                                requestStream.Write(array, 0, num3);
                            }
                            requestStream.Write(bytes, 0, bytes.Length);
                        }
                        j++;
                    }
                    requestStream.Write(bytes3, 0, bytes3.Length);
                    WebResponse response = httpWebRequest.GetResponse();
                    string text4 = "";
                    using (Stream responseStream = response.GetResponseStream()) {
                        using (StreamReader streamReader = new StreamReader(responseStream, Encoding.GetEncoding("UTF-8"))) {
                            text4 = streamReader.ReadToEnd();
                        }
                    }
                    res.response = text4;
                    res.error = false;
                }
            } catch (WebException ex) {
                if (ex.Status != WebExceptionStatus.ProtocolError) {
                    throw ex;
                }
                HttpWebResponse httpWebResponse = (HttpWebResponse)ex.Response;
                using (Stream responseStream2 = httpWebResponse.GetResponseStream()) {
                    using (StreamReader streamReader2 = new StreamReader(responseStream2, Encoding.UTF8)) {
                        res.response = streamReader2.ReadToEnd();
                        res.error = true;
                    }
                }
              
            }
            return res;
        }
        private string executePostRequest(ref HttpWebRequest req, byte[] bytes) {
            try {
                using (Stream requestStream = req.GetRequestStream()) {
                    requestStream.Write(bytes, 0, bytes.Length);
                }
                //結果取得
                using (Stream responseStream = req.GetResponse().GetResponseStream()) {
                    using (StreamReader streamReader = new StreamReader(responseStream, Encoding.GetEncoding("UTF-8"))) {
                        return streamReader.ReadToEnd();
                    }
                }
            }
            catch(Exception ex) {
                Console.WriteLine(ex);
                if (ex.Message.Contains("500")) {
                    Console.WriteLine("500エラーメッセボックスだします");
                    MessageBox.Show("500エラーです。");
                  
                }
                if (ex.Message.Contains("403")){
                    Console.WriteLine("403エラーメッセボックスだします");
                    MessageBox.Show("403エラーです。");
                }
                return "";
            }
        }
        private string postMultipartFril(string url, Dictionary<string, string> param, string file,CookieContainer cc) {
            Encoding encoding = Encoding.GetEncoding("UTF-8");
            string text = Environment.TickCount.ToString();
            byte[] bytes = encoding.GetBytes("\r\n");
            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
            httpWebRequest.UserAgent = USER_AGENT;
            httpWebRequest.CookieContainer = cc;
            httpWebRequest.Method = "POST";
            httpWebRequest.ContentType = "multipart/form-data; boundary=" + text;
            string text2 = "";
            foreach (KeyValuePair<string, string> current in param) {
                text2 = string.Concat(new string[]
				{
					text2,
					"--",
					text,
					"\r\nContent-Disposition: form-data; name=\"",
					current.Key,
					"\"\r\n\r\n",
					current.Value,
					"\r\n"
				});
            }
            long num = 0L;
            Path.GetFileName(file);
            string s = "--" + text + "\r\nContent-Disposition: form-data; name=\"image\"; filename=\"image.jpg\"\r\nContent-Type: image/jpeg\r\n\r\n";
            byte[] bytes2 = encoding.GetBytes(s);
            num += (long)encoding.GetBytes(s).Length + new FileInfo(file).Length;
            byte[] bytes3 = encoding.GetBytes(text2);
            byte[] bytes4 = encoding.GetBytes("--" + text + "--\r\n");
            httpWebRequest.ContentLength = (long)bytes3.Length + num + (long)bytes.Length + (long)bytes4.Length;
            string result;
            using (Stream requestStream = httpWebRequest.GetRequestStream()) {
                requestStream.Write(bytes3, 0, bytes3.Length);
                using (FileStream fileStream = new FileStream(file, FileMode.Open, FileAccess.Read)) {
                    requestStream.Write(bytes2, 0, bytes2.Length);
                    byte[] array = new byte[4096];
                    while (true) {
                        int num2 = fileStream.Read(array, 0, array.Length);
                        if (num2 == 0) {
                            break;
                        }
                        requestStream.Write(array, 0, num2);
                    }
                    requestStream.Write(bytes, 0, bytes.Length);
                }
                requestStream.Write(bytes4, 0, bytes4.Length);
                WebResponse webResponse = null;
                string text3 = "";
                try {
                    webResponse = httpWebRequest.GetResponse();
                    Log.Logger.Info("access info:url->" + url);
                }
                catch (WebException ex) {
                    webResponse = (HttpWebResponse)ex.Response;
                    Log.Logger.Info("access info:url->" + url + " message->" + ex.Message);
                }
                finally {
                    using (Stream responseStream = webResponse.GetResponseStream()) {
                        using (StreamReader streamReader = new StreamReader(responseStream, Encoding.GetEncoding("UTF-8"))) {
                            text3 = streamReader.ReadToEnd();
                        }
                    }
                }
                result = text3;
            }
            return result;
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
                } else {
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
            } catch (Exception ex) {
                Log.Logger.Error(ex.Message);
                Log.Logger.Error("出品用画像の作成に失敗");
                return "";
            }
        }
        //public bool updateProfilePhoto(string new_imagepath,CookieContainer cc) {
        //    string url = string.Format("https://api.mercari.jp/users/update_profile?_fril_auth_token={0}&_global_fril_auth_token={1}", this.account.auth_token);//FIXIT:frilにかえる
        //    Dictionary<string, string> param = new Dictionary<string, string>();
        //    param.Add("name", this.account.nickname);
        //    param.Add("introduction", this.getProfileIntroduction(cc));
        //    FrilRawResponse res = updateProfilePhotoPost(url, param, new_imagepath);
        //    return !res.error;
        //}
        //public string getProfileIntroduction(CookieContainer cc) {
        //    Dictionary<string, string> param = new Dictionary<string, string>();
        //    param.Add("_user_format", "profile");
        //    param.Add("_fril_auth_token", this.account.auth_token);
        //    string url = "https://api.mercari.jp/users/get_profile";//FIXIT:フリルのものに変える
        //    FrilRawResponse res = getFrilAPI(url, param ,cc);
        //    if (res.error) {
        //        return "";
        //    }
        //    dynamic resjson = DynamicJson.Parse(res.response);
        //    try {
        //        return (string)resjson.data.introduction;
        //    } catch (Exception e) {
        //        return "";
        //    }
        //}
        //プロフィール更新用に
        private FrilRawResponse updateProfilePhotoPost(string url, Dictionary<string, string> param, string imagepath) {
            FrilRawResponse res = new FrilRawResponse();

            Encoding encoding = Encoding.GetEncoding("UTF-8");
            string text = Environment.TickCount.ToString();
            byte[] bytes = encoding.GetBytes("\r\n");
            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
            httpWebRequest.UserAgent = FrilAPI.USER_AGENT;
            httpWebRequest.Headers.Set("X-PLATFORM", FrilAPI.XPLATFORM);
            httpWebRequest.Headers.Set("X-APP-VERSION", FrilAPI.XAPPVERSION);
            httpWebRequest.Method = "POST";
            httpWebRequest.ContentType = "multipart/form-data; boundary=" + text;
            string text2 = "";
            foreach (KeyValuePair<string, string> current in param) {
                text2 = string.Concat(new string[]
                {
                    text2,
                    "--",
                    text,
                    "\r\nContent-Disposition: form-data; name=\"",
                    current.Key,
                    "\"\r\n\r\n",
                    current.Value,
                    "\r\n"
                });
            }
            byte[] bytes2 = encoding.GetBytes(text2);

            long num = 0L;
            int num2 = 1;
            string text3 = imagepath;
            Path.GetFileName(text3);
            string s = string.Concat(new object[]
                {
                    "--",
                    text,
                    "\r\nContent-Disposition: form-data; name=\"photo\"; filename=\"photo.jpg\"\r\nContent-Type: image/jpeg\r\n\r\n"
                });
            byte[] bytedata = encoding.GetBytes(s);
            num += (long)encoding.GetBytes(s).Length + new FileInfo(text3).Length;
            num2++;
            byte[] bytes3 = encoding.GetBytes("--" + text + "--\r\n");
            httpWebRequest.ContentLength = (long)bytes2.Length + num + (long)(bytes.Length) + (long)bytes3.Length;
            string result = "";
            try {
                using (Stream requestStream = httpWebRequest.GetRequestStream()) {
                    requestStream.Write(bytes2, 0, bytes2.Length);
                    using (FileStream fileStream = new FileStream(imagepath, FileMode.Open, FileAccess.Read)) {
                        requestStream.Write(bytedata, 0, bytedata.Length);
                        byte[] array = new byte[4096];
                        while (true) {
                            int num3 = fileStream.Read(array, 0, array.Length);
                            if (num3 == 0) {
                                break;
                            }
                            requestStream.Write(array, 0, num3);
                        }
                        requestStream.Write(bytes, 0, bytes.Length);
                    }
                    requestStream.Write(bytes3, 0, bytes3.Length);
                    WebResponse response = httpWebRequest.GetResponse();
                    string text4 = "";
                    using (Stream responseStream = response.GetResponseStream()) {
                        using (StreamReader streamReader = new StreamReader(responseStream, Encoding.GetEncoding("UTF-8"))) {
                            text4 = streamReader.ReadToEnd();
                        }
                    }
                    res.response = text4;
                    res.error = false;
                }
            } catch (WebException ex) {
                if (ex.Status != WebExceptionStatus.ProtocolError) {
                    throw ex;
                }
                HttpWebResponse httpWebResponse = (HttpWebResponse)ex.Response;
                using (Stream responseStream2 = httpWebResponse.GetResponseStream()) {
                    using (StreamReader streamReader2 = new StreamReader(responseStream2, Encoding.UTF8)) {
                        res.response = streamReader2.ReadToEnd();
                        res.error = false;
                    }
                }
            }
            return res;
        }

        #region FIXMEOPTION:もしかしたら不要
        //public bool Edit(FrilItem item, string[] imagelocation) {
        //    FrilRawResponse res = new FrilRawResponse();
        //    Dictionary<string, string> dictionary = new Dictionary<string, string>();
        //    try {
        //        string url = string.Format("https://api.mercari.jp/items/edit?_access_token={0}&_global_access_token={1}", this.account.auth_token);

        //        /*手数料を計算する*/
        //        int sales_fee = GetSalesFee(item.s_price, item.category_id);

        //        //dictionary.Add("_ignore_warning", "false");
        //        dictionary.Add("id", item.item_id);
        //        dictionary.Add("category_id", item.category_id.ToString());
        //        dictionary.Add("description", item.detail);
        //        //dictionary.Add("exhibit_token", MercariAPI.getCsrfToken());
        //        dictionary.Add("item_condition", item.status.ToString());
        //        dictionary.Add("name", item.item_name);
        //        dictionary.Add("price", item.s_price.ToString());
        //        dictionary.Add("sales_fee", sales_fee.ToString());
        //        dictionary.Add("shipping_duration", item.d_date.ToString());
        //        dictionary.Add("shipping_from_area", item.d_area.ToString());
        //        dictionary.Add("shipping_payer", item.carriage.ToString());
        //        dictionary.Add("shipping_method", item.d_method.ToString());
        //        if (item.size_id > 0) dictionary.Add("size", item.size_id.ToString());
        //        if (item.brand_id > 0) dictionary.Add("brand_name", item.brand_id.ToString());
        //        //既にアップロード済みの画像のときはファイルを一時ファイルにダウンロード
        //        Dictionary<int, string> dic = new Dictionary<int, string>();
        //        if (!string.IsNullOrEmpty(imagelocation[0])) {
        //            if (imagelocation[0].IndexOf("https://static-mercari-jp") >= 0) {//Frilのものにかえる
        //                Common.DownloadFileTo(imagelocation[0], "tmp/tmp1.jpg");
        //                dic.Add(1, "tmp/tmp1.jpg");
        //            } else {
        //                dic.Add(1, imagelocation[0]);
        //            }
        //        }
        //        if (!string.IsNullOrEmpty(imagelocation[1])) {
        //            if (imagelocation[1].IndexOf("https://static-mercari-jp") >= 0) {
        //                Common.DownloadFileTo(imagelocation[1], "tmp/tmp2.jpg");
        //                dic.Add(2, "tmp/tmp2.jpg");
        //            } else {
        //                dic.Add(2, imagelocation[1]);
        //            }
        //        }
        //        if (!string.IsNullOrEmpty(imagelocation[2])) {
        //            if (imagelocation[2].IndexOf("https://static-mercari-jp") >= 0) {
        //                Common.DownloadFileTo(imagelocation[2], "tmp/tmp3.jpg");
        //                dic.Add(3, "tmp/tmp3.jpg");
        //            } else {
        //                dic.Add(3, imagelocation[2]);
        //            }
        //        }
        //        if (!string.IsNullOrEmpty(imagelocation[3])) {
        //            if (imagelocation[3].IndexOf("https://static-mercari-jp") >= 0) {
        //                Common.DownloadFileTo(imagelocation[3], "tmp/tmp4.jpg");
        //                dic.Add(4, "tmp/tmp4.jpg");
        //            } else {
        //                dic.Add(4, imagelocation[3]);
        //            }
        //        }
        //        res = postFrilAPIwithMultiPart(url, dictionary, dic);
        //        if (res.error) throw new Exception();
        //        return true;
        //    } catch (Exception e) {
        //        Log.Logger.Error("商品の編集に失敗 以下エラー内容詳細" + item.item_id);
        //        if (res.error) {
        //            Log.Logger.Error("res.errorがtrue");

        //        } else {
        //            Log.Logger.Error("res.errorはfalse");
        //        }
        //        if (!string.IsNullOrEmpty(res.response)) Log.Logger.Error("response: " + res.response);
        //        else Log.Logger.Error("res.responseはnull");

        //        Log.Logger.Error("出品内容:");
        //        Log.Logger.Error(dictionary);
        //        return false;
        //    }
        //}
        #endregion
        //最新の手数料のレートに応じて手数料を求める
        //手数料取得失敗時は負の値が返る
        public int GetSalesFee(int price, int category_id) {
            try {
                Dictionary<string, string> param = new Dictionary<string, string>(); //GetTokenParamListForFrilAPI();
                FrilRawResponse rawres = getFrilAPI("https://api.mercari.jp/sales_fee/get", param, this.account.cc);//FIXITOPTION:Frilのものに変更しないと
                dynamic resjson = DynamicJson.Parse(rawres.response);
                object[] sales_cond = resjson.data.parameters;
                /*カテゴリを再優先, 次に金額の条件を満たすか*/
                for (int i = 0; i < sales_cond.Length; i++) {
                    dynamic cond = sales_cond[i];
                    /*カテゴリIDの条件があってそれを満たさない場合は次の条件へ*/
                    if (cond.category_id() && (int)cond.category_id != category_id) continue;
                    /*金額の条件を満たしていれば手数料決定*/
                    int min_price = cond.min_price() ? (int)cond.min_price : -1;
                    int max_price = cond.max_price() ? (int)cond.max_price : -1;
                    int fixed_fee = cond.fixed_fee() ? (int)cond.fixed_fee : 0;
                    if (min_price <= price && price <= max_price) {
                        /*手数料計算*/
                        double rate = cond.rate() ? (double)cond.rate : -1;
                        int fee = (int)Math.Floor(rate * price) + fixed_fee;
                        return fee;
                    }
                }
                return (int)((double)price * 0.1); /*どの条件にもマッチしなかった場合はとりあえず10%返す*/
            } catch (Exception e) {
                Log.Logger.Error(string.Format("手数料計算に失敗:価格:{0},カテゴリID{1}", price, category_id));
                return (int)((double)price * 0.1); ;
            }
        }
        //特定のitemIDの商品情報を取得
        //このAPIではコメントなどの情報も取得できるが現時点では取り出していない
        public FrilItem GetItemInfobyItemIDWithDetail(string item_id) {
            try {
                Dictionary<string, string> param = new Dictionary<string, string>(); //GetTokenParamListForFrilAPI();
                param.Add("auth_token", this.account.auth_token);
                param.Add("item_id", item_id);
                FrilRawResponse rawres = getFrilAPI("http://api.fril.jp/api/v3/items/show", param,this.account.cc);//FIXITOPTION;Frilのものにかえる
                //Logger.info(rawres.response);
                dynamic resjson = DynamicJson.Parse(rawres.response);
                dynamic iteminfo = resjson.item;
                FrilItem item = new FrilItem(iteminfo);//caegory_klevelは取得できない
                return item;
            } catch (Exception e) {
                Console.WriteLine(e);
                Log.Logger.Error(e);
                return null;
            }
        }
        //商品を削除する.要ログイン
        //返り値: 成功:true 失敗:false
        //public bool Cancel(FrilItem item,account) {
        //    return Cancel(item.item_id,account);
        //}
        //FIXITOPTION:おそらく不要です
        //public bool Cancel(string item_id,Common.Account account) {
        //    try {
        //        string url = "https://api.fril.jp/api/items/delete";
        //        Dictionary<string, string> param = new Dictionary<string, string>();
        //        param.Add("item_id", item_id);
        //        param.Add("auth_token", account.auth_token);
        //        FrilRawResponse rawres = postFrilAPI(url, param,this.account.cc);
        //        if (rawres.error) return false;
        //        /*グローバルアクセストークンを更新*/
        //        dynamic resjson = DynamicJson.Parse(rawres.response);
        //        bool result = resjson.result;
        //        if (!result) throw new Exception();
        //        Log.Logger.Info("商品の削除に成功");
        //        return true;
        //    } catch (Exception ex) {
        //        Log.Logger.Error("商品の削除に失敗");
        //        Console.WriteLine(ex);
        //        return false;
        //    }
        //}
        public bool Stop(string item_id) {
            try {
                string url = string.Format("https://api.mercari.jp/items/update_status?_access_token={0}&_global_access_token={1}", this.account.auth_token);
                Dictionary<string, string> param = new Dictionary<string, string>();
                param.Add("item_id", item_id);
                param.Add("status", "stop");

                FrilRawResponse rawres = postFrilAPI(url, param,this.account.cc);
                if (rawres.error) return false;
                /*グローバルアクセストークンを更新*/
                dynamic resjson = DynamicJson.Parse(rawres.response);
                string result = resjson.result;
                if (result != "OK") throw new Exception();
                Log.Logger.Info("商品の出品停止に成功");
                return true;
            } catch (Exception e) {
                Log.Logger.Error("商品の出品停止に失敗");
                return false;
            }
        }
        //商品を削除して出品する.要ログイン
        //返り値: 成功:新しい商品オブジェクト 失敗:null
        public FrilItem CancelandSell(string item_id) {
            FrilItem item = GetItemInfobyItemIDWithDetail(item_id);
            Cancel(item.item_id,account); //削除失敗した場合も出品はする
            return null;
        }
        //特定のuserIdの商品をすべて取得
        //List<int> status_option := 商品の状態1:on_sale 2:trading 3:sold_out
        public List<FrilItem> GetAllItemsWithSellers(string sellerid, List<int> status_list, bool notall = false, bool detailflag = false) {
            GetItemsOption option = new GetItemsOption();
            option.sellerid = sellerid;
            option.status_list = status_list;
            return GetItems(option, notall, detailflag);
        }
        public bool Cancel(string item_id, Common.Account account) {
            try {
                string url = "https://api.fril.jp/api/items/delete";
                Dictionary<string, string> param = new Dictionary<string, string>();
                param.Add("item_id", item_id);
                param.Add("auth_token", account.auth_token);
                FrilRawResponse rawres = postFrilAPI(url, param, this.account.cc);
                if (rawres.error) return false;
                /*グローバルアクセストークンを更新*/
                dynamic resjson = DynamicJson.Parse(rawres.response);
                bool result = resjson.result;
                if (!result) throw new Exception();
                Log.Logger.Info("商品の削除に成功");
                return true;
            } catch (Exception ex) {
                Log.Logger.Error("商品の削除に失敗");
                Console.WriteLine(ex);
                return false;
            }
        }


        //コンディションに応じて商品を取得する
        //一度のリクエストで取れるのは最大で60個 60個を超える場合は複数回APIを叩いて結果を取得する.
        //FIXIT:この60とかいう数字コメントアウトしてるいま
        private List<FrilItem> GetItems(GetItemsOption option, bool notall, bool detailflag) {
            Dictionary<string, string> default_param = new Dictionary<string, string>(); //GetTokenParamListForFrilAPI();
            default_param = default_param.Concat(option.ToPairList()).ToDictionary(x => x.Key, x => x.Value);
            default_param.Add("limit", "60");
            List<FrilItem> res = new List<FrilItem>();

            //60個以上あるか
            Boolean has_next = false;//FIXME:has_nextのやつかんがえないとね
            //2回目以降でつかうmax_pager_id
            string max_pager_id = "";
            do {
                Dictionary<string, string> param = new Dictionary<string, string>();
                param = param.Concat(default_param).ToDictionary(x => x.Key, x => x.Value);
                if (max_pager_id != "") param.Add("max_pager_id", max_pager_id);
                param.Add("auth_token", this.account.auth_token);
                FrilRawResponse rawres = getFrilAPI("http://api.fril.jp/api/v3/items/show", param, this.account.cc);

                if (rawres.error) return res;
                try {
                    dynamic resjson = DynamicJson.Parse(rawres.response);
                    object[] datas = resjson.data;
                    has_next = resjson.meta.has_next;
                    //detailflagが立っていれば 1件ずつデータとりだし1つずつAPIを叩いて詳しい情報を取得する
                    if (detailflag) {
                        for (int i = 0; i < datas.Length; i++) {
                            string item_id = ((dynamic)datas[i]).id;
                            FrilItem item = GetItemInfobyItemIDWithDetail(item_id);
                            res.Add(item);
                            max_pager_id = item.pager_id.ToString();
                        }
                    } else {
                        foreach (object data in datas) {
                            FrilItem item = new FrilItem((dynamic)data);
                            res.Add(item);
                            max_pager_id = item.pager_id.ToString();
                        }
                    }
                } catch (Exception e) {
                    //e.printStackTrace();
                    return res;
                }
            } while (has_next == true && notall == false);
            return res;
        }
        private static string getCsrfToken() {
            byte[] array = new byte[16];
            new RNGCryptoServiceProvider().GetBytes(array);
            StringBuilder stringBuilder = new StringBuilder();
            for (int i = 0; i < array.Length; i++) {
                stringBuilder.AppendFormat("{0:x2}", array[i]);
            }
            return stringBuilder.ToString();
        }
        //現在のUNIXタイムスタンプを取得
        private static string getUNIXTimeStamp() {
            long unixTimestamp = (long)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalMilliseconds;
            return unixTimestamp.ToString();
        }
        //取引メッセージを取得する
        public List<Comment> GetTransactionMessages(string itemid) {
            List<Comment> res = new List<Comment>();
            try {
                Dictionary<string, string> param = new Dictionary<string, string>();
                string url = "https://web.fril.jp/transaction";
                param.Add("item_id", itemid);
                FrilRawResponse rawres = getFrilAPI(url, param,this.account.cc, true);
                Log.Logger.Info("取引メッセージの取得に成功");
                res = GetTransactionCommentFromHTML(rawres.response);//パース
                return res;
            } catch (Exception ex) {
                Log.Logger.Error(ex.Message);
                Log.Logger.Error("取引メッセージの取得に失敗");
                return res;
            }
        }

        //取引htmlを取得する
        public string GetTransactionPage(string itemid) {
            string res="";
            try {
                Dictionary<string, string> param = new Dictionary<string, string>();
                string url = "https://web.fril.jp/transaction";
                param.Add("item_id", itemid);
                FrilRawResponse rawres = getFrilAPI(url, param, this.account.cc, true);
                Log.Logger.Info("transactionの取得に成功");
                res = rawres.response;
                return res;
            } catch (Exception ex) {
                Log.Logger.Error(ex.Message);
                Log.Logger.Error("取引メッセージの取得に失敗");
                return res;
            }
        }


        //購入者情報の構造体
        public struct TransactionInfo {
            public string transaction_id;
            public string buyername;
            public string buyerid;
            public string address;
            public string status;
            public string zipcode;
            public string address1;
            public string address2;
            public string sellerid;
            public string buyer_screen_name;
            //public DateTime created;
            public string created;
            public bool error;
        }
        //購入者氏名及び商品発送先住所を取得する
        public TransactionInfo GetTransactionInfo(string item_id) {
            TransactionInfo res = new TransactionInfo();
            try {
                string html = GetTransactionPage(item_id);
                res = GetAddressFromHTML(html);
                Dictionary<string, string> param = new Dictionary<string, string>();
                if (res.error) throw new Exception();
                //構造体から情報取り出し
                //string buyername = resjson.data.family_name + " " + resjson.data.first_name;
                //string zipcode = resjson.data.zip_code1 + "-" + resjson.data.zip_code2;
                //string prefecture = resjson.data.prefecture;
                //string city = resjson.data.city;
                //string address1 = resjson.data.address1;
                //string address2 = resjson.data.address2;
                //string result_address = zipcode + Environment.NewLine + prefecture + city + address1 + Environment.NewLine + address2;
                //res.buyerid = ((long)resjson.data.buyer_id).ToString();
                //res.sellerid = ((long)resjson.data.seller_id).ToString();
                //res.buyername = buyername;
                //res.address = result_address;
                //res.status = resjson.data.status;
                //res.transaction_id = ((long)resjson.data.id).ToString();
                //res.zipcode = zipcode;
                //res.address1 = prefecture + city + address1;
                //res.address2 = address2;
                //createdプロパティはバージョンによってUNIXスタンプ返すものと文字列返すものがある！
                /*long created = (long)resjson.data.created;
                DateTime UNIX_EPOCH = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc); //UnixTimeの開始時刻
                res.created = UNIX_EPOCH.AddSeconds(created).ToLocalTime();*/
                //res.created = resjson.data.created;
                Log.Logger.Info("取引情報の取得に成功");
                return res;
            } catch (Exception ex) {
                Log.Logger.Error(ex.Message);
                Log.Logger.Error("取引情報の取得に失敗");
                return res;
            }
        }

        private TransactionInfo GetAddressFromHTML(string html) {
            try {
                
                TransactionInfo transactionInfo = new TransactionInfo();
                int num = 0;
                num = html.IndexOf("<p class=\"caption-text\">配送先住所</p>", num);
                num = html.IndexOf("<p>", num) + "<p>".Length;
                int num2 = html.IndexOf("<br />", num);
                transactionInfo.zipcode = html.Substring(num, num2 - num);
                num = num2;
                num = html.IndexOf("<br />", num) + "<br />".Length;
                num2 = html.IndexOf("<br />", num);
                transactionInfo.address = html.Substring(num, num2 - num);
                transactionInfo.address = transactionInfo.address.Replace(" ", "").Replace("\n", "");
                num = num2 + "<br />".Length;
                num2 = html.IndexOf("</p>", num);
                transactionInfo.buyername = html.Substring(num, num2 - num);
                transactionInfo.buyername = transactionInfo.buyername.Replace(" ", "").Replace("\n", "");
                num = num2;
                num = html.IndexOf("<div class=\"black-text\">", num)+ "<div class=\"black-text\">".Length;
                num = html.IndexOf(">", num) + ">".Length;
                num2 = html.IndexOf("</a>", num);
                transactionInfo.buyer_screen_name = html.Substring(num, num2 - num);
                transactionInfo.buyer_screen_name = transactionInfo.buyer_screen_name.Replace(" ", "").Replace("\n", "");

                //TODO:ほかにもなんか必要ならここからぶちこめる
                transactionInfo.error = false;
                return transactionInfo;

            } catch (Exception ex) {
                TransactionInfo transactionInfo = new TransactionInfo();
                Console.WriteLine(ex);
                Log.Logger.Error(ex);
                transactionInfo.error = true;
                return transactionInfo;
            }
        }

        //コメント用の構造体
        public struct Comment {
            public string id;
            public string item_id;
            public string user_id;
            public string comment;
            public DateTime created_at;
            public string screen_name;
            public string profile_img;//REVIEW:一応書いておいたけどラクランドでは使わない
        }
        //商品のコメントを取得する
        public List<Comment> GetComments(string item_id) {
            var res = new List<Comment>();
            try {
                Dictionary<string, string> param = new Dictionary<string, string>(); //GetTokenParamListForFrilAPI();
                param.Add("auth_token", this.account.auth_token);
                param.Add("item_id", item_id);
 
                FrilRawResponse rawres = getFrilAPI("https://api.fril.jp/api/v3/comments", param,this.account.cc);
                dynamic resjson = DynamicJson.Parse(rawres.response);
                int commentnum = ((object[])resjson.comments).Length;
                for (int i = 0; i < commentnum; i++) {
                    Comment c = new Comment();
                    c.id = ((long)resjson.comments[i].id).ToString();
                    c.item_id = ((long)resjson.comments[i].item_id).ToString();
                    c.user_id = ((long)resjson.comments[i].user_id).ToString();
                    c.comment = resjson.comments[i].comment;
                    string time = resjson.comments[i].created_at;
                    time = time.Substring(0, time.IndexOf("+"));
                    time = time.Replace("-", "/");
                    time = time.Replace("T", " ");
                    c.created_at = DateTime.Parse(time);
                    c.screen_name = resjson.comments[i].screen_name;
                    
                    res.Add(c);
                }
                Log.Logger.Info("コメント取得成功: " + item_id);
                return res;
            } catch (Exception ex) {
                Log.Logger.Error("コメント取得失敗: " + item_id);
                Console.WriteLine(ex);
                return res;
            }
        }
        //コメントを追加
        public bool AddComment(string item_id, string message) {
            try {
                Dictionary<string, string> param = new Dictionary<string, string>();
                string url = "https://api.fril.jp/api/v3/comments/";
                param.Add("auth_token", this.account.auth_token);
                param.Add("comment", message);
                param.Add("item_id", item_id);
                //param.Add("status", "1"); //?たぶんいらん
                FrilRawResponse rawres = postFrilAPI(url, param,this.account.cc);
                if (rawres.error) throw new Exception();
                Log.Logger.Info("コメント追加成功");
                return true;
            } catch (Exception ex) {
                Log.Logger.Error("コメント追加失敗: " + item_id);
                Log.Logger.Error(ex.Message);
                return false;
            }
        }
        //コメントを削除
        public bool DeleteComment(string itemid, string commentid) {
            try {
                Dictionary<string, string> param = new Dictionary<string, string>();
                string url = "https://api.fril.jp/api/v3/comments/";
                url += commentid+"?";
                param.Add("auth_token", this.account.auth_token);
                FrilRawResponse rawres = deleteFrilAPI(url, param,this.account.cc);
                if (rawres.error) throw new Exception();
                Log.Logger.Info("コメント削除成功");
                return true;
            } catch (Exception ex) {
                Log.Logger.Error("コメント削除失敗: " + itemid);
                Log.Logger.Error(ex.Message);
                return false;
            }
        }
        //取引メッセージを送信する
        public void SendTransactionMessage(string itemid, string comment , Dictionary<string,string>param_dic) {
            try {
                Dictionary<string, string> param = new Dictionary<string, string>();
                string url = "https://api.fril.jp/api/order/comment/add";//これメッセ送るよう
                param = param_dic;//TODO:これやってるいみある？？
                param.Add("comment", comment);
                FrilRawResponse rawres = postFrilAPI(url, param,this.account.cc);
                if (rawres.error) throw new Exception();
                Log.Logger.Info("取引メッセージ送信成功");
            } catch (Exception ex) {
                Log.Logger.Error(ex.Message);
                Log.Logger.Error("取引メッセージ送信失敗");
                Console.WriteLine(ex);
            }
        }
        //商品の発送通知を行う
        public bool SendItemShippedNotification(string itemid,Dictionary<string,string> param) {
            try {
                string url = "https://web.fril.jp/v2/order/shipping";
                FrilRawResponse rawres = postFrilAPI(url, param,this.account.cc);
                if (rawres.error) throw new Exception();
                Log.Logger.Info("商品の発送通知に成功");
                return true;
            } catch (Exception ex) {
                Log.Logger.Error(ex.Message);
                Log.Logger.Error("商品の発送通知に失敗");
            }
            return true;
        }

        //相手を評価する
        public bool SendReview(string itemid, string message = "") {
            try {
                string html = this.GetTransactionPage(itemid);
                Dictionary<string, string> param = GetEvaluationFromHTML(html);
                string url = "https://web.fril.jp/v2/order/review";
                param.Add("review", "1");
                param.Add("comment", message);
               FrilRawResponse rawres = postFrilAPI(url, param,this.account.cc,true);
                if (rawres.error) throw new Exception();
                Log.Logger.Info("購入者の評価に成功");
                Console.WriteLine("購入者の評価に成功");
                return true;
            } catch (Exception ex) {
                Log.Logger.Info("購入者の評価に失敗");
                Console.WriteLine(ex);
                return false;
            }
        }
        private Dictionary<string, string> GetEvaluationFromHTML(string html) {
            Dictionary<string, string> dictionary = new Dictionary<string, string>();
            int num = 0;
            num = html.IndexOf("<form id=\"review-form\"", num);
            int num2 = html.IndexOf("</form>", num);
            string text = html.Substring(num, num2 - num);
            num = 0;
            while (text.IndexOf("<input type=\"hidden\"", num) >= 0) {
                num = text.IndexOf("<input type=\"hidden\"", num);
                num2 = text.IndexOf("/>", num) + "/>".Length;
                string text2 = text.Substring(num, num2 - num);
                num = num2;
                int num3 = text2.IndexOf("name=\"") + "name=\"".Length;
                int num4 = text2.IndexOf("\"", num3);
                string key = text2.Substring(num3, num4 - num3);
                num3 = text2.IndexOf("value=\"") + "value=\"".Length;
                num4 = text2.IndexOf("\"", num3);
                string value = text2.Substring(num3, num4 - num3);
                dictionary.Add(key, value);
            }
            num = 0;
            num = text.IndexOf("<input name=\"utf8\"", num);
            num2 = text.IndexOf("/>", num) + "/>".Length;
            string text3 = text.Substring(num, num2 - num);
            num = text3.IndexOf("value=\"") + "value=\"".Length;
            num2 = text3.IndexOf("\"", num);
            string value2 = text3.Substring(num, num2 - num);
            dictionary.Add("utf8", value2);
            return dictionary;
        }
    


        //購入された取引をキャンセルする
        public bool cancelTransaction(string itemid) {
            //TransactionInfo info = GetTransactionInfo(itemid);
            //try {
            //    Dictionary<string, string> param = new Dictionary<string, string>();
            //    string url = "https://api.mercari.jp/transactions/cancel";
            //    param.Add("transaction_evidence_id", info.transaction_id);
            //    param.Add("_platform", "android");
            //    param.Add("_app_version", XAPPVERSION);
            //    param.Add("t", FrilAPI.getUNIXTimeStamp());
            //    param.Add("_access_token", this.account.auth_token);
            //    FrilRawResponse rawres = postFrilAPI(url, param,this.account.cc);
            //    if (rawres.error) throw new Exception();
            //    Log.Logger.Info("取引のキャンセルに成功");
            //    return true;
            //} catch (Exception ex) {
            //    Log.Logger.Info("取引のキャンセルに失敗: " + ex.Message);
                return false;
            //}
        }
        private List<Comment> GetTransactionCommentFromHTML(string html) {
            List<Comment> list = new List<Comment>();
            int num = 0;
            num = html.IndexOf(">取引メッセージ<", num) + "> 取引メッセージ <".Length;
            int num2 = html.IndexOf("<form", num);
            string text = html.Substring(num, num2 - num);
            num = 0;
            List<string> list2 = new List<string>();
            while (text.IndexOf("<div class=\"row\">", num) >= 0) {
                num = text.IndexOf("<div class=\"row\">", num) + "<div class=\"row\">".Length;
                num2 = text.IndexOf("<div class=\"row\">", num);
                if (num2 < 0) {
                    break;
                }
                list2.Add(text.Substring(num, num2 - num));
                num = num2;
            }
            foreach (string text2 in list2) {
                Comment comment = new Comment();
                if (text2.IndexOf("<div class=\"col s2\">") < text2.IndexOf("<div class=\"col s10\">")) {
                    comment.screen_name = "出品者";
                } else {
                    comment.screen_name = "購入者";
                }
                num = 0;
                num = text2.IndexOf(" <p class=\"small-text\">", num) + " <p class=\"small-text\">".Length;
                num2 = text2.IndexOf("</p>", num);
                string text3 = text2.Substring(num, num2 - num);
                text3 = text3.Replace("\n", "");
                comment.comment = text3.Replace("<br />", "\n");
                num = 0;
                num = text2.IndexOf("<p class=\"small-text right-align\">", num) + "<p class=\"small-text right-align\">".Length;
                num2 = text2.IndexOf("</p>", num);
                string created_at = text2.Substring(num, num2 - num);
                comment.created_at = DateTime.Parse(created_at);
                list.Add(comment);
            }
            return list;
        }

        #region 引き出し

        //引き出しリクエストページを取得する
        public bool BankUpdate() {
            try {
                Dictionary<string, string> param = new Dictionary<string, string>();
                string url = "https://api.fril.jp/api/bank/update";
                param.Add("account_number", this.account.bank_info.account_number);
                param.Add("auth_token", this.account.auth_token);
                param.Add("bank_id", this.account.bank_info.id.ToString());
                param.Add("branch_code", this.account.bank_info.branch_code);
                param.Add("deposit_type", this.account.bank_info.deposit_type.ToString());
                param.Add("first_name", this.account.bank_info.first_name);
                param.Add("last_name", this.account.bank_info.last_name);
                FrilRawResponse rawres = postFrilAPI(url, param, this.account.cc, false);
                if (rawres.error) throw new Exception();
                Log.Logger.Info("口座情報updateに成功");
                Console.WriteLine("口座情報updateに成功");
                return true;
            } catch (Exception ex) {
                Log.Logger.Info("口座情報updateに失敗");
                Console.WriteLine(ex);
                return false;
            }
        }

        //出品中商品修正
        public bool ReviseSellingItem(Dictionary<string,string>param) {
            try {
                string url = "https://api.fril.jp/api/items/request";
                FrilRawResponse rawres = postFrilAPI(url, param, this.account.cc, false);
                if (rawres.error) throw new Exception();
                Log.Logger.Info("アイテム修正に成功");
                Console.WriteLine("アイテム修正に成功");
                return true;
            } catch (Exception ex) {
                Log.Logger.Info("アイテム修正に失敗");
                Console.WriteLine(ex);
                return false;
            }
        }


        //引き出す
        public bool Withdraw(int amount) {
            try {
                string html = this.GetWithdrawPage();
                Dictionary<string, string> param = GetWithdrawParamListFromHTML(html);
                param.Add("balance", this.account.balance_info.balance.ToString());
                param.Add("amount", amount.ToString());//FIXIT:残す金額を決めて入れる
                param.Add("bank_id", this.account.bank_info.id.ToString());
                param.Add("action", "");
                string url = "https://web.fril.jp/balance/withdrawal";
                FrilRawResponse rawres = postFrilAPI(url, param, this.account.cc, true);
                if (rawres.error) throw new Exception();
                Log.Logger.Info("出金に成功");
                Console.WriteLine("出金に成功");
                return true;
            } catch (Exception ex) {
                Log.Logger.Info("出金に失敗");
                Console.WriteLine(ex);
                return false;
            }
        }

        //引き出しhtmlを取得する
        public string GetWithdrawPage() {
            string res = "";
            try {
                Dictionary<string, string> param = new Dictionary<string, string>();
                string url = "https://web.fril.jp/balance/withdrawal/request";
                FrilRawResponse rawres = getFrilAPI(url, param, this.account.cc, true);
                Log.Logger.Info("transactionの取得に成功");
                res = rawres.response;
                return res;
            } catch (Exception ex) {
                Log.Logger.Error(ex.Message);
                Log.Logger.Error("取引メッセージの取得に失敗");
                return res;
            }
        }
        //引き出しhtmlから必要情報をとってくる
        private Dictionary<string,string> GetWithdrawParamListFromHTML(string html) {
            Dictionary<string, string> param = new Dictionary<string, string>();
            // HtmlDocumentオブジェクトを構築する
            var doc = new HtmlAgilityPack.HtmlDocument();
            doc.LoadHtml(html);
            //utf-8
            string xpath = @"//*[@id=""home""]/div/form/input[1]";
            var collection = doc.DocumentNode.SelectNodes(xpath);
            string text = collection[0].GetAttributeValue("value", "");
            param.Add("utf_8", text);
            //authenticity_token
            xpath = @"//*[@id=""home""]/div/form/input[2]";
            collection = doc.DocumentNode.SelectNodes(xpath);
            text = collection[0].GetAttributeValue("value", "");
            param.Add("authenticity_token",text);
            
            //xpath = @"//*[@id=""home""]/div/form/input[1]";
            //int num = 0;
            ////utf-8
            //num = html.IndexOf("<input name=\"utf8\" type=\"hidden\" value=\"", num) + "<input name =\"utf8\" type=\"hidden\" value=\"".Length;
            //int num2 = html.IndexOf("\" > ", num);
            //text = html.Substring(num, num2 - num);
            //param.Add("utf-8", text);
            ////authenticity_token
            //num = 4;
            //List<string> list2 = new List<string>();
            //while (text.IndexOf("<div class=\"row\">", num) >= 0) {
            //    num = text.IndexOf("<div class=\"row\">", num) + "<div class=\"row\">".Length;
            //    num2 = text.IndexOf("<div class=\"row\">", num);
            //    if (num2 < 0) {
            //        break;
            //    }
            //    list2.Add(text.Substring(num, num2 - num));
            //    num = num2;
            //}
            //foreach (string text2 in list2) {
            //    Comment comment = new Comment();
            //    if (text2.IndexOf("<div class=\"col s2\">") < text2.IndexOf("<div class=\"col s10\">")) {
            //        comment.screen_name = "出品者";
            //    } else {
            //        comment.screen_name = "購入者";
            //    }
            //    num = 0;
            //    num = text2.IndexOf(" <p class=\"small-text\">", num) + " <p class=\"small-text\">".Length;
            //    num2 = text2.IndexOf("</p>", num);
            //    string text3 = text2.Substring(num, num2 - num);
            //    text3 = text3.Replace("\n", "");
            //    comment.comment = text3.Replace("<br />", "\n");
            //    num = 0;
            //    num = text2.IndexOf("<p class=\"small-text right-align\">", num) + "<p class=\"small-text right-align\">".Length;
            //    num2 = text2.IndexOf("</p>", num);
            //    string created_at = text2.Substring(num, num2 - num);
            //    comment.created_at = DateTime.Parse(created_at);
            //    list.Add(comment);
            //}
            return param;
        }

        #endregion



    }
}
