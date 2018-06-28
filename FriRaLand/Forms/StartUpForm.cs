using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.IO;
using Codeplex.Data;
using RakuLand.DBHelper;

namespace RakuLand.Forms {
    public partial class StartUpForm : Form {
        static public VersionInfo versioninfo;
        static public Notification notification;
        static public bool notifyFlag = false;//新しいものがあればtrue
        public StartUpForm() {
            InitializeComponent();
            this.TopMost = true;
            //インスタンスを作成した際に最新バージョン・お知らせを取得する
            loadVersionAndNotification();
        }
        public static void loadVersionAndNotification() {
            versioninfo = getLatestVersion(MainForm.ProductKind);
            notification = getLatestNotification(MainForm.ProductKind);
            if (!string.IsNullOrEmpty(versioninfo.version) && (compareVersion(getNowVersion(), versioninfo.version) < 0 || notification.created_at > Settings.getLatestOsiraseCreatedTime())) {
                notifyFlag = true;
                new SettingsDBHelper().updateSettings(Common.last_osirase_createdTime, notification.created_at.ToString());
            } else {
                notifyFlag = false;
            }
        }
        private void startUpWindow_Load(object sender, EventArgs e) {
            nowVersionLabel.Text = getNowVersion().ToString();
            if (!string.IsNullOrEmpty(versioninfo.version)) {
                this.versionNumLabel.Text = versioninfo.version.ToString();
                this.richTextBox1.Text = versioninfo.detail;
                if (compareVersion(getNowVersion(), versioninfo.version) >= 0) {
                    this.button1.Text = "最新です";
                    this.button1.Enabled = false;
                }
            } else {
                this.richTextBox1.Text = "バージョン情報の取得に失敗しました";
                this.button1.Enabled = false;
            }
            if (notification.created_at != new DateTime(1970, 1, 1)) {
                this.notificationDateLabel.Text = notification.created_at.ToString();
                this.richTextBox2.Text = notification.message;
            } else {
                this.richTextBox2.Text = "お知らせの取得に失敗しました";
            }
        }
        public static string getNowVersion() {
            //現在のバージョン情報を取得
            System.Diagnostics.FileVersionInfo ver = System.Diagnostics.FileVersionInfo.GetVersionInfo(System.Reflection.Assembly.GetExecutingAssembly().Location);
            string versionstr = ver.ProductVersion;
            //バージョンはx.x.x.xのようになっている、はじめのx.xだけを切り出し
            int firstDotIndex = versionstr.IndexOf('.');
            int secondDotIndex = versionstr.IndexOf('.', firstDotIndex + 1);
            string res = versionstr.Substring(0, secondDotIndex);
            return res;
        }
        public static int compareVersion(string a, string b) {
            //x.xの形になっているバージョン文字列を受け取って大小比較を行う
            //a < bなら-1を返す, a=bなら0を返す, a > bなら1を返す
            //バージョン同士の比較なので普通の小数点の大小関係とは異なることに注意
            int a_firstDotIndex = a.IndexOf('.');
            int a1 = int.Parse(a.Substring(0, a_firstDotIndex));
            int a2 = int.Parse(a.Substring(a_firstDotIndex + 1, a.Length - a_firstDotIndex - 1));
            int b_firstDotIndex = b.IndexOf('.');
            int b1 = int.Parse(b.Substring(0, b_firstDotIndex));
            int b2 = int.Parse(b.Substring(b_firstDotIndex + 1, b.Length - b_firstDotIndex - 1));
            if (a1 < b1) return -1;
            if (a1 > b1) return 1;
            if (a2 < b2) return -1;
            if (a2 > b2) return 1;
            return 0;
        }
        public class VersionInfo {
            public string version;
            public string detail;
            public string filename;
            public VersionInfo(string v, string d) { this.version = v; this.detail = d; }
        }
        public class Notification {
            public string message;
            public DateTime created_at;
            public Notification() {
                this.created_at = new DateTime(1970, 1, 1);
                this.message = "";
            }
        }
        public static VersionInfo getLatestVersion(string kind) {
            VersionInfo res = new VersionInfo("", "");
            try {
                string response = doGet("http://160.16.69.60/api/versioninfo", new Dictionary<string, string>());
                dynamic resjson = DynamicJson.Parse(response);
                int num = 0;
                foreach (var info in resjson) {
                    if (info.kind == kind) {
                        if (num == 0 || compareVersion(res.version, info.version) < 0) {
                            res.version = info.version;
                            res.detail = info.detail;
                            res.filename = info.filename;
                        }
                        num++;
                    }
                }
            } catch (Exception ex) {
                Log.Logger.Error("最新バージョンの取得に失敗: " + ex.Message);
            }
            return res;
        }
        //新しいものから順にすべての更新履歴を取得する
        private static List<VersionInfo> getAllVersion(string kind) {
            List<VersionInfo> res = new List<VersionInfo>();
            try {
                string response = doGet("http://160.16.69.60/api/versioninfo", new Dictionary<string, string>());
                dynamic resjson = DynamicJson.Parse(response);
                foreach (var info in resjson) {
                    if (info.kind == kind) {
                        VersionInfo v = new VersionInfo("", "");
                        v.version = info.version;
                        v.detail = info.detail;
                        v.filename = info.filename;
                        res.Add(v);
                    }
                }
            } catch (Exception ex) {
                Log.Logger.Error("全てのバージョンの取得に失敗: " + ex.Message);
            }
            return res;
        }
        public static Notification getLatestNotification(string kind) {
            Notification res = new Notification();
            try {
                DateTime latest = new DateTime(1970, 1, 1);
                string response = doGet("http://160.16.69.60/api/notifications", new Dictionary<string, string>());
                dynamic resjson = DynamicJson.Parse(response);
                foreach (var n in resjson) {
                    if (n.kind == kind) {
                        DateTime dt = Common.getDateFromUnixTimeStamp((long)n.created_at);
                        if (latest < dt) {
                            res.created_at = dt;
                            res.message = n.message;
                        }
                    }
                }
            } catch (Exception ex) {
                Log.Logger.Error("新着情報の取得に失敗: " + ex.Message);
            }
            return res;
        }
        public static List<Notification> getAllNotification(string kind) {
            List<Notification> res = new List<Notification>();
            try {
                DateTime latest = new DateTime(1970, 1, 1);
                string response = doGet("http://160.16.69.60/api/notifications", new Dictionary<string, string>());
                dynamic resjson = DynamicJson.Parse(response);
                foreach (var n in resjson) {
                    if (n.kind == kind) {
                        Notification nf = new Notification();
                        DateTime dt = Common.getDateFromUnixTimeStamp((long)n.created_at);
                        nf.created_at = dt;
                        nf.message = n.message;
                        res.Add(nf);
                    }
                }
            } catch (Exception ex) {
                Log.Logger.Error("新着情報の取得に失敗: " + ex.Message);
            }
            return res;
        }
        private static string doGet(string url, Dictionary<string, string> param) {
            try {
                //url = Uri.EscapeUriString(url);//日本語などを％エンコードする
                //パラメータをURLに付加 ?param1=val1&param2=val2...
                url += "?";
                List<string> paramstr = new List<string>();
                foreach (KeyValuePair<string, string> p in param) {
                    string k = Uri.EscapeUriString(p.Key);
                    string v = Uri.EscapeUriString(p.Value);
                    paramstr.Add(k + "=" + v);
                }
                url += string.Join("&", paramstr);
                //HttpWebRequestの作成
                HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
                req.Method = "GET";
                //結果取得
                string content = "";
                var task = Task.Factory.StartNew(() => executeGetRequest(req));
                task.Wait(10000);
                if (task.IsCompleted)
                    content = task.Result;
                else
                    throw new Exception("Timed out");
                if (string.IsNullOrEmpty(content)) throw new Exception("webrequest error");
                return content;
            } catch (Exception e) {
                return "";
            }
        }
        private static string executeGetRequest(HttpWebRequest req) {
            try {
                HttpWebResponse webres = (HttpWebResponse)req.GetResponse();
                Stream s = webres.GetResponseStream();
                StreamReader sr = new StreamReader(s);
                string content = sr.ReadToEnd();
                return content;
            } catch {
                return "";
            }
        }

        private void button1_Click(object sender, EventArgs e) {
            if (!string.IsNullOrEmpty(StartUpForm.versioninfo.filename)) {
                string filepath = StartUpForm.versioninfo.filename;
                //ファイルを保存する画面を開く
                SaveFileDialog sfd = new SaveFileDialog();
                sfd.FileName = filepath;
                sfd.Title = "保存先のファイルを選択してください";
                //ダイアログを表示する
                if (sfd.ShowDialog() == DialogResult.OK) {
                    if (backgroundWorker1.IsBusy) {
                        MessageBox.Show("現在ダウンロード中です", MainForm.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    string savefilename = sfd.FileName;
                    backgroundWorker1.RunWorkerAsync(new string[] { filepath, savefilename });
                }
            }
        }

        private void backgroundWorker1_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e) {
            string[] arguments = (string[])e.Argument;
            string fileurl = "http://160.16.69.60/upload/" + arguments[0];
            string savefilename = arguments[1];
            System.Net.WebClient wc = new System.Net.WebClient();
            wc.DownloadFile(fileurl, savefilename);
            wc.Dispose();
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e) {
            MessageBox.Show("ダウンロードが完了しました。\n"
                + "ダウンロードしたzipファイルを解凍し、以前のバージョンのdatabase.dbファイルをコピーしてデータを引き継いでください",
                MainForm.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void button2_Click(object sender, EventArgs e) {
            //過去すべての更新履歴をテキストファイルに保存する
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "テキストファイル(*.txt)|*.*";
            sfd.FileName = "更新履歴.txt";
            sfd.Title = "保存先を選択してください";
            try {
                if (sfd.ShowDialog() == DialogResult.OK) {

                    System.IO.StreamWriter sw = new System.IO.StreamWriter(sfd.FileName, false, System.Text.Encoding.GetEncoding("shift_jis"));
                    foreach (var v in getAllVersion(MainForm.ProductKind)) {
                        sw.WriteLine(v.detail);
                        sw.WriteLine("");
                    }
                    //閉じる
                    sw.Close();
                    MessageBox.Show("保存が完了しました:" + sfd.FileName);
                }
            } catch (Exception ex) {
                MessageBox.Show("保存に失敗しました");
            }
        }

        private void button3_Click(object sender, EventArgs e) {
            //過去すべてのお知らせをテキストファイルに保存する
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "テキストファイル(*.txt)|*.*";
            sfd.FileName = "お知らせ履歴.txt";
            sfd.Title = "保存先を選択してください";
            try {
                if (sfd.ShowDialog() == DialogResult.OK) {

                    System.IO.StreamWriter sw = new System.IO.StreamWriter(sfd.FileName, false, System.Text.Encoding.GetEncoding("shift_jis"));
                    foreach (var v in getAllNotification(MainForm.ProductKind)) {
                        sw.WriteLine(v.message);
                        sw.WriteLine("");
                    }
                    //閉じる
                    sw.Close();
                    MessageBox.Show("保存が完了しました:" + sfd.FileName);
                }
            } catch (Exception ex) {
                MessageBox.Show("保存に失敗しました");
            }
        }
    }
}
