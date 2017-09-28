using Codeplex.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace FriRaLand {
    class RakumaAPI {
        private string RAKUMA_USER_AGENT = "RakumaApp/iOS/1.7.4/iPhone8,1/OS10.3.2";
        private string RAKUMA_LOGIN_USER_AGENT = "Rakuma/1.7.4 CFNetwork/811.5.4 Darwin/16.6.0";
        private string RAKUTEN_USER_AGENT = "Mozilla/5.0 (iPhone; CPU iPhone OS 10_3_2 like Mac OS X) AppleWebKit/603.2.4 (KHTML, like Gecko) Mobile/14F89 RakumaApp/iOS/1.7.5.1/iPhone8,1/OS10.3.2";
        private string cookie_rakuma_session;
        private string proxy;
        private class RakumaRawResponse {
            public bool error = true;
            public string response = "";
        }
        public Common.Account account;
        public RakumaAPI(string email, string password) {
            this.account = new Common.Account();
            this.account.kind = Common.Account.Rakuma_Account;
            this.account.email = email;
            this.account.password = password;
        }
        public bool tryRakumaLogin() {
            /*revert,android_id,device_id,app_generated_idなどのパラメタはなくてもOKなので送らない*/
            Dictionary<string, string> param = new Dictionary<string, string>();
            param.Add("client_id", "fleama");
            param.Add("client_secret", "C7nWk2WQDPHoeojHvqO0t3Zra_NImSWViDIaLMdOBz4R");
            param.Add("grant_type", "password");
            param.Add("scope", "90days@Access,90days@Refresh,memberinfo_read_name,memberinfo_read_pointsummary,tokencheck");
            param.Add("username", this.account.email);
            param.Add("password", this.account.password);
            string url = "https://24x7.app.rakuten.co.jp/engine/token";
            RakumaRawResponse rawres = postRakumaAPI(url, param, RAKUMA_LOGIN_USER_AGENT);
            if (rawres.error) return false;
            try {
                dynamic resjson = DynamicJson.Parse(rawres.response);
                this.account.rakuma_access_token = resjson.access_token;
                this.account.expirationDate = DateTime.Now.AddDays(90.0);
                Log.Logger.Info("ラクマログイン成功");
                return true;
            }
            catch (Exception e) {
                Log.Logger.Info("ラクマログイン失敗");
                return false;
            }
        }
        public string Sell(RakumaItem item) {
            try {
                Dictionary<string, string> param = new Dictionary<string, string>();
                //カテゴリ決定
                int category_id = -1;
                foreach (var id in item.categoryId) if (id != -1) category_id = id;
                param.Add("brand_id", item.brandId.ToString());
                param.Add("category_id", category_id.ToString());
                param.Add("condition_type", item.conditionType.ToString());
                param.Add("delivery_method", item.deliveryMethod.ToString());
                param.Add("delivery_term", item.deliveryTerm.ToString());
                param.Add("description_text", item.descriptionText);
                param.Add("device_type", "1");
                param.Add("postage_type", "1");
                param.Add("prefecture_code", item.prefectureCode.ToString());
                param.Add("product_name", item.productName);
                param.Add("selling_price", item.sellingPrice.ToString());
                List<string> imagePathList = new List<string>();
                foreach (var path in item.imagepaths) if (string.IsNullOrEmpty(path) == false) imagePathList.Add(path);
                string rst = postMultipartRakuma("https://api.rakuma.rakuten.co.jp/selling-api/rest/product/register", param, imagePathList.ToArray());
                dynamic resjson = DynamicJson.Parse(rst);
                string new_access_token = resjson.accessToken;
                string itemid = null;
                if (resjson.success == "出品しました") {
                    itemid = resjson.productId;
                }
                return itemid;
            }catch (Exception ex){
                Log.Logger.Error("ラクマへの出品失敗: " + ex.Message);
                return null;
            }
        }
        //FrilAPIをPOSTでたたく
        private RakumaRawResponse postRakumaAPI(string url, Dictionary<string, string> param, string user_agent) {
            RakumaRawResponse res = new RakumaRawResponse();
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
                req.UserAgent = user_agent;
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
                foreach (Cookie c in req.CookieContainer.GetCookies(new Uri(url))) {
                    //if (c.Name.ToString() == "_fril_session") this.cookie_fril_session = c.Value.ToString();
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
        public string postMultipartRakuma(string url, Dictionary<string, string> param, string[] files) {
            Encoding encoding = Encoding.GetEncoding("UTF-8");
            string text = Environment.TickCount.ToString();
            byte[] bytes = encoding.GetBytes("\r\n");
            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
            httpWebRequest.UserAgent = RAKUMA_USER_AGENT;
            httpWebRequest.CookieContainer = this.cc;
            if (string.IsNullOrEmpty(this.account.rakuma_access_token) == false) {
                httpWebRequest.Headers.Set("access_token", this.account.rakuma_access_token);
            }
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
            List<byte[]> list = new List<byte[]>();
            long num = 0L;
            int num2 = 1;
            for (int i = 0; i < files.Length; i++) {
                string text3 = files[i];
                Path.GetFileName(text3);
                string s = string.Concat(new object[]
				{
					"--",
					text,
					"\r\nContent-Disposition: form-data; name=\"product_images\"; filename=\"image",
					num2,
					".jpg\"\r\nContent-Type: image/jpeg\r\n\r\n"
				});
                list.Add(encoding.GetBytes(s));
                num += (long)encoding.GetBytes(s).Length + new FileInfo(text3).Length;
                num2++;
            }
            for (int j = num2; j <= 4; j++) {
                text2 = string.Concat(new object[]
				{
					text2,
					"--",
					text,
					"\r\nContent-Disposition: form-data; name=\"product_images\"; filename=\"image",
					j,
					".jpg\"\r\nContent-Type: image/jpeg\r\n\r\n\r\n"
				});
            }
            byte[] bytes2 = encoding.GetBytes(text2);
            byte[] bytes3 = encoding.GetBytes("--" + text + "--\r\n");
            httpWebRequest.ContentLength = (long)bytes2.Length + num + (long)(bytes.Length * files.Length) + (long)bytes3.Length;
            string result;
            using (Stream requestStream = httpWebRequest.GetRequestStream()) {
                requestStream.Write(bytes2, 0, bytes2.Length);
                for (int k = 0; k < files.Length; k++) {
                    using (FileStream fileStream = new FileStream(files[k], FileMode.Open, FileAccess.Read)) {
                        requestStream.Write(list[k], 0, list[k].Length);
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
                }
                requestStream.Write(bytes3, 0, bytes3.Length);
                WebResponse webResponse = null;
                string text4 = "";
                try {
                    webResponse = httpWebRequest.GetResponse();
                    Log.Logger.Info("access info:url->" + url);
                }
                catch (WebException ex) {
                    webResponse = (HttpWebResponse)ex.Response;
                    Log.Logger.Error("access error:url->" + url + " message->" + ex.Message);
                }
                finally {
                    using (Stream responseStream = webResponse.GetResponseStream()) {
                        using (StreamReader streamReader = new StreamReader(responseStream, Encoding.GetEncoding("UTF-8"))) {
                            text4 = streamReader.ReadToEnd();
                        }
                    }
                }
                result = text4;
            }
            return result;
        }
    }
}
