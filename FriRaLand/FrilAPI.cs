using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Codeplex.Data;
using System.IO;

namespace FriRaLand {
    class FrilAPI {
        private const string USER_AGENT = "Mozilla/5.0 (iPhone; CPU iPhone OS 10_3_3 like Mac OS X) AppleWebKit/603.3.8 (KHTML, like Gecko) Mobile/14G60 Fril/7.2.0";
        private string proxy;
        //private CookieContainer cc = new CookieContainer();

        //GET,POSTのRequestのResponse
        private class FrilRawResponse {
            public bool error = true;
            public string response = "";
        }
        
        public Common.Account account;
        public FrilAPI(string email, string password) {
            this.account = new Common.Account();
            this.account.kind = Common.Account.Fril_Account;
            this.account.email = email;
            this.account.password = password;
        }
        public FrilAPI(Common.Account account) {
            this.account = account;
        }
       

        //成功: itemID 失敗: null
        public string Sell(FrilItem item,CookieContainer cc) {
            try {
                //商品情報をまず送る
                string url = "https://api.fril.jp/api/items/request";
                Dictionary<string, string> param = new Dictionary<string, string>();
                param.Add("auth_token", this.account.fril_auth_token);
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
                param.Add("size", item.size_id.ToString());
                param.Add("size_name", item.size_name);
                param.Add("status", item.status.ToString());
                param.Add("title", item.item_name);
                if (item.brand_id.ToString()!="0") { //FIXIT:ブランドが必要なカテゴリではブランドidがないとクラッシュ
                    param.Add("brand", item.brand_id.ToString());
                }                
                //パラメータ表示
                foreach(var val in param) {
                    Console.WriteLine(val.Key + ":" + val.Value);
                }


                var rawres = postFrilAPI(url, param, cc);
                if (rawres.error) throw new Exception("商品情報の送信に失敗");
                string res = "";
                //itemIDをとりだして画像を送る
                string item_id = ((long)DynamicJson.Parse(rawres.response).item_id).ToString();
                int total_img_num = 0;
                foreach (var imagepath in item.imagepaths) if (string.IsNullOrEmpty(imagepath) == false) total_img_num++;
                for (int num = 1; num <= total_img_num; num++) {
                    string image_url = "https://api.fril.jp/api/items/request_img";
                    Dictionary<string, string> req_img_param = new Dictionary<string, string>();
                    req_img_param.Add("auth_token", this.account.fril_auth_token);
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
            string url = "https://api.fril.jp/api/v4/users/sign_in";
            FrilRawResponse rawres = postFrilAPI(url, param, cc);
            if (rawres.error) return false;
            try {
                dynamic resjson = DynamicJson.Parse(rawres.response);
                this.account.fril_auth_token = resjson.auth_token;
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
            param.Add("auth_token", account.fril_auth_token);
            FrilRawResponse res = getFrilAPI("http://api.fril.jp/api/v2/users", param,cc);
            var json = DynamicJson.Parse(res.response);
            account.nickname = json.user.screen_name;   
            account.userId = ((long)json.user.id).ToString();
            return account;
        }


        public FrilItem getItemDetailInfo(string item_id,CookieContainer cc)
        {
            Dictionary<string, string> param = new Dictionary<string, string>();
            param.Add("auth_token", this.account.fril_auth_token);
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
        public List<FrilItem> getSellingItem(string userId,CookieContainer cc) {
            List<FrilItem> rst = new List<FrilItem>();
            bool has_next = true;
            string max_id = "0"; //二回目以降で「この商品IDより後」の商品を取得する
            do {
                Dictionary<string, string> param = new Dictionary<string, string>();
                param.Add("include_sold_out", "0");
                param.Add("limit", "60");
                param.Add("max_id", max_id);
                param.Add("user_id", userId);
                string url = "https://api.fril.jp/api/v3/items/list";
                FrilRawResponse rawres = getFrilAPI(url, param,cc);
                if (rawres.error) {
                    Log.Logger.Error("フリル出品中商品の取得に失敗: UserID: " + this.account.userId);
                    return rst;
                }
                dynamic resjson = DynamicJson.Parse(rawres.response);
                foreach (dynamic data in resjson.items) {
                    FrilItem item = new FrilItem();
                    item.item_id = ((long)data.item_id).ToString();
                    item.imageurls[0] = data.img_url;
                    item.item_name = data.item_name;
                    item.detail = data.item_detail;
                    item.s_price = (int)data.price;
                    item.t_status = (int)data.t_status;
                    item.user_id = ((long)data.user_id).ToString();
                    item.brand_id = ((data.brand_id == null) ? -1 : (int)item.brand_id);
                    item.i_brand_id = ((data.i_brand_id == null) ? -1 : (int)item.i_brand_id);
                    item.screen_name = data.screen_name;
                    item.created_at = DateTime.Parse(data.created_at);
                    item.likes_count = (int)data.like_count;
                    item.comments_count = (int)data.comment_count;
                    rst.Add(item);
                    max_id = item.item_id;
                }
                has_next = (bool)resjson.paging.has_next;
            } while (has_next);
            return rst;
        }
        //FrilAPIをGETでたたく
        private FrilRawResponse getFrilAPI(string url, Dictionary<string, string> param, CookieContainer cc) {
            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
            //ストップウォッチを開始する
            sw.Start();
            FrilRawResponse res = new FrilRawResponse();
            try {
                //url = Uri.EscapeDataString(url);//日本語などを％エンコードする
                //パラメータをURLに付加 ?param1=val1&param2=val2...
                url += "?";
                List<string> paramstr = new List<string>();
                foreach (KeyValuePair<string, string> p in param) {
                    string k = Uri.EscapeDataString(p.Key);
                    string v = Uri.EscapeDataString(p.Value);
                    paramstr.Add(k + "=" + v);
                }
                url += string.Join("&", paramstr);
                //HttpWebRequestの作成
                HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
                req.CookieContainer = cc;




                req.UserAgent = FrilAPI.USER_AGENT;
                req.Method = "GET";
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
                    content = task.Result;
                } else
                    throw new Exception("Timed out");
                if (string.IsNullOrEmpty(content)) throw new Exception("webrequest error");
                res.error = false;
                res.response = content;
                Log.Logger.Info("FrilGETリクエスト成功");
                return res;
            }
            catch (Exception e) {
                Log.Logger.Error("FrilGETリクエスト失敗");
                return res;
            }
        }
        private string executeGetRequest(HttpWebRequest req) {
            try {
                HttpWebResponse webres = (HttpWebResponse)req.GetResponse();
                Stream s = webres.GetResponseStream();
                StreamReader sr = new StreamReader(s);
                string content = sr.ReadToEnd();
                return content;
            }
            catch {
                return "";
            }
        }
        private string executePostRequest(HttpWebRequest req, byte[] bytes) {
            try {
                using (Stream requestStream = req.GetRequestStream()) {
                    requestStream.Write(bytes, 0, bytes.Length);
                }
                //結果取得
                string result = "";
                using (Stream responseStream = req.GetResponse().GetResponseStream()) {
                    using (StreamReader streamReader = new StreamReader(responseStream, Encoding.GetEncoding("UTF-8"))) {
                        return streamReader.ReadToEnd();
                    }
                }
            }
            catch {
                return "";
            }
        }
        //FrilAPIをPOSTでたたく
        private FrilRawResponse postFrilAPI(string url, Dictionary<string, string> param,CookieContainer cc) {
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

                //クッキーコンテナの追加
                req.CookieContainer = cc;
                // クッキー確認
                //foreach (Cookie c in cc.GetCookies(new Uri("https://api.fril.jp/api/"))) {
                //    Console.WriteLine("クッキー名:" + c.Name.ToString());
                //    Console.WriteLine("値:" + c.Value.ToString());
                //    Console.WriteLine("ドメイン名:" + c.Domain.ToString());
                //}
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
                if (string.IsNullOrEmpty(content)) throw new Exception("webrequest error");
                if (content.Contains("false")) throw new Exception("item result false");
                res.error = false;
                res.response = content;
                Log.Logger.Info("FrilPOSTリクエスト成功");
                req.Abort();
                return res;
            }
            catch (Exception e) {
                
                Console.WriteLine(e);
                Log.Logger.Error("FrilPOSTリクエスト失敗");
                return res;
            }
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
            catch {
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
    }
}
