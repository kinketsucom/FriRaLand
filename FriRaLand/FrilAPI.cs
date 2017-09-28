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
        private const string USER_AGENT = "Mozilla/5.0 (iPad; COU OS 10_3_2 like Mac OS X) AppleWebKit/603.2.4 (KHTML, like Gecko) Mobile/14F89 Fril/6.7.1";
        private string cookie_fril_session;
        private string proxy;
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
        //成功: itemID 失敗: null
        public string Sell(FrilItem item) {
            try {
                //商品情報をまず送る
                string url = "https://api.fril.jp/api/items/request";
                Dictionary<string, string> param = new Dictionary<string, string>();
                param.Add("auth_token", this.account.auth_token);
                param.Add("item_id", "0");
                param.Add("brand", item.brand_id.ToString());
                param.Add("carriage", item.carriage.ToString());
                param.Add("category", item.category_id.ToString());
                param.Add("delivery_area", item.d_area.ToString());
                param.Add("delivery_date", item.d_date.ToString());
                param.Add("delivery_method", item.d_method.ToString());
                param.Add("detail", item.detail);
                param.Add("p_category", item.category_p_id.ToString());
                param.Add("request_required", "0");
                param.Add("sell_price", item.s_price.ToString());
                param.Add("size", item.size_id.ToString());
                param.Add("size_name", item.size_name);
                param.Add("status", item.status.ToString());
                param.Add("title", item.item_name);
                var rawres = postFrilAPI(url, param);
                if (rawres.error) throw new Exception("商品情報の送信に失敗");
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
                    res = postMultipartFril(image_url, req_img_param, item.imagepaths[num - 1]);
                }
                Console.WriteLine(res);
                Log.Logger.Info("商品出品成功: " + item_id);
                return item_id;
            }
            catch (Exception ex) {
                Log.Logger.Error("商品出品失敗: " + ex.Message);
                return null;
            }
        }
        public bool tryFrilLogin() {
            /*revert,android_id,device_id,app_generated_idなどのパラメタはなくてもOKなので送らない*/
            Dictionary<string, string> param = new Dictionary<string, string>();
            param.Add("email", this.account.email);
            param.Add("password", this.account.password);
            string url = "https://api.fril.jp/api/v4/users/sign_in";
            FrilRawResponse rawres = postFrilAPI(url, param);
            if (rawres.error) return false;
            try {
                dynamic resjson = DynamicJson.Parse(rawres.response);
                this.account.auth_token = resjson.auth_token;
                ////Logger.info("ログイン成功");
                return true;
            }
            catch (Exception e) {
                ////Logger.info("ログイン失敗");
                return false;
            }
        }
        //FrilAPIをPOSTでたたく
        private FrilRawResponse postFrilAPI(string url, Dictionary<string, string> param) {
            FrilRawResponse res = new FrilRawResponse();
            try {
                string text = "";
                List<string> paramstr = new List<string>();
                int num = 0;
                foreach (KeyValuePair<string, string> p in param) {
                    string k = Uri.EscapeUriString(p.Key);
                    string v = Uri.EscapeUriString(p.Value);
                    if (num != 0) text += "&";
                    text = text + (k + "=" + v);
                    num++;
                }
                byte[] bytes = Encoding.ASCII.GetBytes(text);
                HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
                req.UserAgent = FrilAPI.USER_AGENT;
                req.Method = "POST";
                //リクエストヘッダを付加
                req.ContentType = "application/x-www-form-urlencoded; charset=utf-8";
                req.Accept = "application/json";
                req.ContentLength = (long)bytes.Length;
                //クッキーコンテナの追加
                req.CookieContainer = this.cc;// new CookieContainer();
                //クッキーの追加
                //if (!string.IsNullOrEmpty(this.cookie_fril_session)) req.CookieContainer.Add(new Uri(url), new Cookie("_fril_session", this.cookie_fril_session));
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
                if (task.IsCompleted)
                    content = task.Result;
                else
                    throw new Exception("Timed out");
                if (string.IsNullOrEmpty(content)) throw new Exception("webrequest error");
                res.error = false;
                res.response = content;
                //クッキー更新
                foreach(Cookie c in req.CookieContainer.GetCookies(new Uri(url))){
                    if(c.Name.ToString() == "_fril_session") this.cookie_fril_session = c.Value.ToString();
                }
                //Log.//Logger.Info("MercariPOSTリクエスト成功");
                req.Abort();
                return res;
            }
            catch (Exception e) {
                return res;
                //Log.//Logger.Error("MercariPOSTリクエスト成功");
            }
        }
        private string executePostRequest(ref HttpWebRequest req, byte[] bytes) {
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
        private CookieContainer cc = new CookieContainer();
        private string postMultipartFril(string url, Dictionary<string, string> param, string file) {
            Encoding encoding = Encoding.GetEncoding("UTF-8");
            string text = Environment.TickCount.ToString();
            byte[] bytes = encoding.GetBytes("\r\n");
            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
            httpWebRequest.UserAgent = USER_AGENT;
            httpWebRequest.CookieContainer = this.cc;
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
