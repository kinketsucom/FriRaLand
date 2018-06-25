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

namespace FriRaLand {
    public class FrilAPI {
        private const string USER_AGENT = "Mozilla/5.0 (iPhone; CPU iPhone OS 10_3_3 like Mac OS X) AppleWebKit/603.3.8 (KHTML, like Gecko) Mobile/14G60 Fril/7.2.0";
        private string proxy;
        private const string XPLATFORM = "android";
        private const string XAPPVERSION = "600";

        public string global_refresh_token; //未使用
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
        //public FrilAPI(Common.Account account) {//Mainformのaaccountから取ってくるよう
        //    this.account = new Common.Account();
        //    this.account.kind = Common.Account.Fril_Account;
        //    this.account.email = account.email;
        //    this.account.password = account.password;
        //    this.account.fril_fril_auth_token = accountaccount.auth_token;
        //}



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

        //商品の出品.要ログイン
        //返り値: 成功:新しい商品オブジェクト 失敗:null
        public FrilItem Sell(FrilItem item, string[] imagelocation) {
            FrilRawResponse res = new FrilRawResponse();
            Dictionary<string, string> dictionary = new Dictionary<string, string>();
            try {
                string url = string.Format("https://api.mercari.jp/sellers/sell?_access_token={0}&_global_access_token={1}", this.account.auth_token);//FIXIT:Frilのものに変える

                /*手数料を計算する*/
                int sales_fee = GetSalesFee(item.s_price, item.category_id);

                dictionary.Add("_ignore_warning", "false");
                dictionary.Add("category_id", item.category_id.ToString());
                dictionary.Add("description", item.detail);
                dictionary.Add("exhibit_token", FrilAPI.getCsrfToken());
                dictionary.Add("item_condition", item.status.ToString());
                dictionary.Add("name", item.item_name);
                dictionary.Add("price", item.s_price.ToString());
                dictionary.Add("sales_fee", sales_fee.ToString());
                dictionary.Add("shipping_duration", item.d_date.ToString());
                dictionary.Add("shipping_from_area", item.d_area.ToString());
                dictionary.Add("shipping_payer", item.carriage.ToString());
                dictionary.Add("shipping_method", item.d_method.ToString());
                if (item.size_id > 0) dictionary.Add("size", item.size_id.ToString());
                if (item.brand_id > 0) dictionary.Add("brand_name", item.brand_id.ToString());
                Dictionary<int, string> dic = new Dictionary<int, string>();
                if (!string.IsNullOrEmpty(imagelocation[0])) dic.Add(1, imagelocation[0]);
                if (!string.IsNullOrEmpty(imagelocation[1])) dic.Add(2, imagelocation[1]);
                if (!string.IsNullOrEmpty(imagelocation[2])) dic.Add(3, imagelocation[2]);
                if (!string.IsNullOrEmpty(imagelocation[3])) dic.Add(4, imagelocation[3]);

                res = postFrilAPIwithMultiPart(url, dictionary, dic);
                if (res.error) throw new Exception();
                dynamic resjson = DynamicJson.Parse(res.response);
                FrilItem rstitem = new FrilItem(resjson.data);
                return rstitem;
            } catch (Exception e) {
                Log.Logger.Error("商品の出品に失敗 以下エラー内容詳細" + item.item_id);
                if (res.error) {
                    Log.Logger.Error("res.errorがtrue");

                } else {
                    Log.Logger.Error("res.errorはfalse");
                }
                if (!string.IsNullOrEmpty(res.response)) Log.Logger.Error("response: " + res.response);
                else Log.Logger.Error("res.responseはnull");

                Log.Logger.Error("出品内容:");
                Log.Logger.Error(dictionary);
                return null;
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
        public bool updateProfilePhoto(string new_imagepath,CookieContainer cc) {
            string url = string.Format("https://api.mercari.jp/users/update_profile?_fril_auth_token={0}&_global_fril_auth_token={1}", this.account.auth_token);//FIXIT:frilにかえる
            Dictionary<string, string> param = new Dictionary<string, string>();
            param.Add("name", this.account.nickname);
            param.Add("introduction", this.getProfileIntroduction(cc));
            FrilRawResponse res = updateProfilePhotoPost(url, param, new_imagepath);
            return !res.error;
        }
        public string getProfileIntroduction(CookieContainer cc) {
            Dictionary<string, string> param = new Dictionary<string, string>();
            param.Add("_user_format", "profile");
            param.Add("_fril_auth_token", this.account.auth_token);
            string url = "https://api.mercari.jp/users/get_profile";//FIXIT:フリルのものに変える
            FrilRawResponse res = getFrilAPI(url, param ,cc);
            if (res.error) {
                return "";
            }
            dynamic resjson = DynamicJson.Parse(res.response);
            try {
                return (string)resjson.data.introduction;
            } catch (Exception e) {
                return "";
            }
        }
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


        public bool Edit(FrilItem item, string[] imagelocation) {
            FrilRawResponse res = new FrilRawResponse();
            Dictionary<string, string> dictionary = new Dictionary<string, string>();
            try {
                string url = string.Format("https://api.mercari.jp/items/edit?_access_token={0}&_global_access_token={1}", this.account.auth_token);

                /*手数料を計算する*/
                int sales_fee = GetSalesFee(item.s_price, item.category_id);

                //dictionary.Add("_ignore_warning", "false");
                dictionary.Add("id", item.item_id);
                dictionary.Add("category_id", item.category_id.ToString());
                dictionary.Add("description", item.detail);
                //dictionary.Add("exhibit_token", MercariAPI.getCsrfToken());
                dictionary.Add("item_condition", item.status.ToString());
                dictionary.Add("name", item.item_name);
                dictionary.Add("price", item.s_price.ToString());
                dictionary.Add("sales_fee", sales_fee.ToString());
                dictionary.Add("shipping_duration", item.d_date.ToString());
                dictionary.Add("shipping_from_area", item.d_area.ToString());
                dictionary.Add("shipping_payer", item.carriage.ToString());
                dictionary.Add("shipping_method", item.d_method.ToString());
                if (item.size_id > 0) dictionary.Add("size", item.size_id.ToString());
                if (item.brand_id > 0) dictionary.Add("brand_name", item.brand_id.ToString());
                //既にアップロード済みの画像のときはファイルを一時ファイルにダウンロード
                Dictionary<int, string> dic = new Dictionary<int, string>();
                if (!string.IsNullOrEmpty(imagelocation[0])) {
                    if (imagelocation[0].IndexOf("https://static-mercari-jp") >= 0) {//FIXIT:Frilのものにかえる
                        Common.DownloadFileTo(imagelocation[0], "tmp/tmp1.jpg");
                        dic.Add(1, "tmp/tmp1.jpg");
                    } else {
                        dic.Add(1, imagelocation[0]);
                    }
                }
                if (!string.IsNullOrEmpty(imagelocation[1])) {
                    if (imagelocation[1].IndexOf("https://static-mercari-jp") >= 0) {
                        Common.DownloadFileTo(imagelocation[1], "tmp/tmp2.jpg");
                        dic.Add(2, "tmp/tmp2.jpg");
                    } else {
                        dic.Add(2, imagelocation[1]);
                    }
                }
                if (!string.IsNullOrEmpty(imagelocation[2])) {
                    if (imagelocation[2].IndexOf("https://static-mercari-jp") >= 0) {
                        Common.DownloadFileTo(imagelocation[2], "tmp/tmp3.jpg");
                        dic.Add(3, "tmp/tmp3.jpg");
                    } else {
                        dic.Add(3, imagelocation[2]);
                    }
                }
                if (!string.IsNullOrEmpty(imagelocation[3])) {
                    if (imagelocation[3].IndexOf("https://static-mercari-jp") >= 0) {
                        Common.DownloadFileTo(imagelocation[3], "tmp/tmp4.jpg");
                        dic.Add(4, "tmp/tmp4.jpg");
                    } else {
                        dic.Add(4, imagelocation[3]);
                    }
                }
                res = postFrilAPIwithMultiPart(url, dictionary, dic);
                if (res.error) throw new Exception();
                return true;
            } catch (Exception e) {
                Log.Logger.Error("商品の編集に失敗 以下エラー内容詳細" + item.item_id);
                if (res.error) {
                    Log.Logger.Error("res.errorがtrue");

                } else {
                    Log.Logger.Error("res.errorはfalse");
                }
                if (!string.IsNullOrEmpty(res.response)) Log.Logger.Error("response: " + res.response);
                else Log.Logger.Error("res.responseはnull");

                Log.Logger.Error("出品内容:");
                Log.Logger.Error(dictionary);
                return false;
            }
        }
        //最新の手数料のレートに応じて手数料を求める
        //手数料取得失敗時は負の値が返る
        public int GetSalesFee(int price, int category_id) {
            try {
                CookieContainer cc = new CookieContainer();//FIXIT:これいるんかなあ？意味のないクッキーかも？
                Dictionary<string, string> param = GetTokenParamListForFrilAPI();
                FrilRawResponse rawres = getFrilAPI("https://api.mercari.jp/sales_fee/get", param,cc);//FIXIT:Frilのものに変更しないと
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
        //access_tokenとglobal_access_tokenのはいったListを返す関数
        private Dictionary<string, string> GetTokenParamListForFrilAPI() {
            Dictionary<string, string> param = new Dictionary<string, string>();
            param.Add("_fril_auth_token", this.account.auth_token);
            return param;
        }
        //特定のitemIDの商品情報を取得
        //このAPIではコメントなどの情報も取得できるが現時点では取り出していない
        public FrilItem GetItemInfobyItemIDWithDetail(string item_id) {
            try {
                CookieContainer cc = new CookieContainer();//FIXIT:不要なクッキーコンテナかどうか調べる必要がある
                Dictionary<string, string> param = GetTokenParamListForFrilAPI();
                param.Add("id", item_id);
                FrilRawResponse rawres = getFrilAPI("https://api.mercari.jp/items/get", param,cc);//FIXIT;Frilのものにかえる
                //Logger.info(rawres.response);
                dynamic resjson = DynamicJson.Parse(rawres.response);
                dynamic iteminfo = resjson.data;
                FrilItem item = new FrilItem(iteminfo);
                return item;
            } catch (Exception e) {
                //e.printStackTrace();
                return null;
            }
        }
        //商品を削除する.要ログイン
        //返り値: 成功:true 失敗:false
        //public bool Cancel(FrilItem item,account) {
        //    return Cancel(item.item_id,account);
        //}
        public bool Cancel(string item_id,Common.Account account) {
            try {
                string url = "https://api.fril.jp/api/items/delete";//FIXIT:Frilのものにかえる
                Dictionary<string, string> param = new Dictionary<string, string>();
                param.Add("item_id", item_id);
                param.Add("auth_token", account.auth_token);
                CookieContainer cc = new CookieContainer();//FIXIT:不要なクッキーコンテナの可能性がある。
                FrilRawResponse rawres = postFrilAPI(url, param,cc);
                if (rawres.error) return false;
                /*グローバルアクセストークンを更新*/
                dynamic resjson = DynamicJson.Parse(rawres.response);
                string result = resjson.result;
                if (result != "OK") throw new Exception();
                Log.Logger.Info("商品の削除に成功");
                return true;
            } catch (Exception e) {
                Log.Logger.Error("商品の削除に失敗");
                return false;
            }
        }
        public bool Stop(string item_id) {
            try {
                CookieContainer cc = new CookieContainer();//FIXIT:不要なクッキーコンテナの可能性がある。
                string url = string.Format("https://api.mercari.jp/items/update_status?_access_token={0}&_global_access_token={1}", this.account.auth_token);
                Dictionary<string, string> param = new Dictionary<string, string>();
                param.Add("item_id", item_id);
                param.Add("status", "stop");

                FrilRawResponse rawres = postFrilAPI(url, param,cc);
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
        //コンディションに応じて商品を取得する
        //一度のリクエストで取れるのは最大で60個 60個を超える場合は複数回APIを叩いて結果を取得する.
        private List<FrilItem> GetItems(GetItemsOption option, bool notall, bool detailflag) {
            Dictionary<string, string> default_param = GetTokenParamListForFrilAPI();
            default_param = default_param.Concat(option.ToPairList()).ToDictionary(x => x.Key, x => x.Value);
            default_param.Add("limit", "60");
            List<FrilItem> res = new List<FrilItem>();

            //60個以上あるか
            Boolean has_next = false;
            //2回目以降でつかうmax_pager_id
            string max_pager_id = "";
            CookieContainer cc = new CookieContainer();//FIXIT:不必要なクッキーコンテナの可能性がある
            do {
                Dictionary<string, string> param = new Dictionary<string, string>();
                param = param.Concat(default_param).ToDictionary(x => x.Key, x => x.Value);
                if (max_pager_id != "") param.Add("max_pager_id", max_pager_id);
                FrilRawResponse rawres = getFrilAPI("http://api.fril.jp/api/v3/items/show", param,cc);//FIXIT:Frilのものにかえる

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
        //コメント用の構造体
        public struct Comment {
            public DateTime created;
            public string nickname;
            public string message;
            public string userid;
            public string id;
        }
        //商品のコメントを取得する
        public List<Comment> GetComments(string itemid) {
            var res = new List<Comment>();
            try {
                Dictionary<string, string> param = GetTokenParamListForFrilAPI();
                param.Add("item_id", itemid);
                CookieContainer cc = new CookieContainer();//不必要なクッキーコンテナの可能性がある。
                FrilRawResponse rawres = getFrilAPI("https://api.mercari.jp/comments/gets", param,cc);//FIXIT:Frilのものに変える
                dynamic resjson = DynamicJson.Parse(rawres.response);
                int commentnum = ((object[])resjson.data).Length;
                for (int i = 0; i < commentnum; i++) {
                    Comment c = new Comment();
                    c.nickname = resjson.data[i].user.name;
                    c.userid = ((long)resjson.data[i].user.id).ToString();
                    c.id = ((long)resjson.data[i].id).ToString();
                    c.message = resjson.data[i].message;
                    c.created = Common.getDateFromUnixTimeStamp((long)resjson.data[i].created);
                    res.Add(c);
                }
                Log.Logger.Info("コメント取得成功: " + itemid);
                return res;
            } catch (Exception e) {
                Log.Logger.Error("コメント取得失敗: " + itemid);
                return res;
            }
        }






    }
}
