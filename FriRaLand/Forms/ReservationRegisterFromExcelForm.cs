using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using RakuLand.DBHelper;

namespace RakuLand.Forms {
    public partial class ReservationRegisterFromExcelForm : Form {
        private string message = "";
        private MainForm mainform;
        private FrilItemDBHelper itemDBHelper = new FrilItemDBHelper();
        private ReservationDBHelper reservationDBHelper = new ReservationDBHelper();
        private Dictionary<string, int> cashItem = new Dictionary<string, int>();
        private bool debugmode = true;
        private int excel_rownum = 0;
        public ReservationRegisterFromExcelForm(MainForm mainform) {
            this.mainform = mainform;
            InitializeComponent();
        }

        private void BackgroundWorker1_DoWork(object sender, DoWorkEventArgs e) {
            this.bgWorker = (BackgroundWorker)sender;
            //パラメータを取得する
            string filename = (string)e.Argument;
            e.Result = readExcelFile(filename);
        }
        public int readExcelFile(string fileName) {
            FileStream fileStream;
            try {
                fileStream = File.OpenRead(fileName);
            } catch (Exception) {
                int result = 1;
                return result;
            }
            XSSFWorkbook xSSFWorkbook = new XSSFWorkbook(fileStream);
            ISheet sheet = xSSFWorkbook.GetSheet("tmp");
            if (sheet == null) {
                return 2;
            }
            /*IRow row = sheet.GetRow(443);
            if (row == null) {
                return 2;
            }
            ICell cell = row.GetCell(0);
            if (cell == null) {
                return 2;
            }
            if (cell.NumericCellValue != 3.0) {
                return 2;
            }*/
            ISheet sheet2 = xSSFWorkbook.GetSheet("item");
            if (sheet2 == null) {
                return 2;
            }
            int i = 0;
            bool flag = false;
            Dictionary<string, int> cashAccount = new Dictionary<string, int>();
            List<Common.Account> AccountList = AccountManageForm.accountLoader();

            while (i < excel_rownum) {
                //ProgressChangedイベントハンドラを呼び出し、
                //コントロールの表示を変更する
                bgWorker.ReportProgress((i + 1) * 100 / excel_rownum);
                #region excel_read
                i++;
                IRow row2 = sheet2.GetRow(i);
                if (row2 == null) {
                    break;
                }
                int num = 1;
                ICell cell2 = row2.GetCell(num);
                string text2 = "";
                try {
                    if (cell2 != null) {
                        text2 = cell2.RichStringCellValue.ToString();
                    }
                } catch (Exception) {
                    message += i.ToString() + "行目の出品アカウントが入力されていません。\n";
                    flag = true;
                    continue;
                }
                num++;
                cell2 = row2.GetCell(num);
                string text3 = "";
                try {
                    if (cell2 != null) {
                        text3 = cell2.RichStringCellValue.ToString();
                    }
                } catch (Exception) {
                    message += i.ToString() + "行目の商品名が正しくありません。" + Environment.NewLine;
                    flag = true;
                    continue;
                }
                num++;
                cell2 = row2.GetCell(num);
                string text4 = "";
                try {
                    if (cell2 != null) {
                        text4 = cell2.RichStringCellValue.ToString();
                    }
                } catch (Exception) {
                    message += i.ToString() + "行目の商品の説明が正しくありません。" + Environment.NewLine;
                    flag = true;
                    continue;
                }
                num++;
                cell2 = row2.GetCell(num);
                string text5 = "";
                try {
                    if (cell2 != null) {
                        text5 = cell2.RichStringCellValue.ToString();
                    }
                } catch (Exception) {
                    message += i.ToString() + "行目の商品画像1が正しくありません。" + Environment.NewLine;
                    flag = true;
                    continue;
                }
                num++;
                cell2 = row2.GetCell(num);
                string text6 = "";
                try {
                    if (cell2 != null) {
                        text6 = cell2.RichStringCellValue.ToString();
                    }
                } catch (Exception) {
                    message += i.ToString() + "行目の商品画像2が正しくありません。" + Environment.NewLine;
                    flag = true;
                    continue;
                }
                num++;
                cell2 = row2.GetCell(num);
                string text7 = "";
                try {
                    if (cell2 != null) {
                        text7 = cell2.RichStringCellValue.ToString();
                        if (debugmode) text7 = "";

                    }
                } catch (Exception) {
                    message += i.ToString() + "行目の商品画像3が正しくありません。" + Environment.NewLine;
                    flag = true;
                    continue;
                }
                num++;
                cell2 = row2.GetCell(num);
                string text8 = "";
                try {
                    if (cell2 != null) {
                        text8 = cell2.RichStringCellValue.ToString();
                        if (debugmode) text8 = "";

                    }
                } catch (Exception) {
                    message += i.ToString() + "行目の商品画像4が正しくありません。" + Environment.NewLine;
                    flag = true;
                    continue;
                }
                num++;
                cell2 = row2.GetCell(num);
                string text9 = "";
                try {
                    if (cell2 != null) {
                        text9 = cell2.RichStringCellValue.ToString();
                    }
                } catch (Exception) {
                    message += i.ToString() + "行目のカテゴリー1が正しくありません。" + Environment.NewLine;
                    flag = true;
                    continue;
                }
                num++;
                cell2 = row2.GetCell(num);
                string text10 = "";
                try {
                    if (cell2 != null) {
                        text10 = cell2.RichStringCellValue.ToString();
                    }
                } catch (Exception) {
                    message += i.ToString() + "行目のカテゴリー2が正しくありません。" + Environment.NewLine;
                    flag = true;
                    continue;
                }
                num++;
                cell2 = row2.GetCell(num);
                string text11 = "";
                try {
                    if (cell2 != null) {
                        text11 = cell2.RichStringCellValue.ToString();
                    }
                } catch (Exception) {
                    message += i.ToString() + "行目のカテゴリー3が正しくありません。" + Environment.NewLine;
                    flag = true;
                    continue;
                }
                num++;
                cell2 = row2.GetCell(num);
                string text12 = "";
                try {
                    if (cell2 != null) {
                        text12 = cell2.RichStringCellValue.ToString();
                    }
                } catch (Exception) {
                    message += i.ToString() + "行目のサイズが正しくありません。" + Environment.NewLine;
                    flag = true;
                    continue;
                }
                num++;
                cell2 = row2.GetCell(num);
                string text13 = "";
                try {
                    if (cell2 != null) {
                        text13 = cell2.RichStringCellValue.ToString();
                    }
                } catch (Exception) {
                    message += i.ToString() + "行目のブランドが正しくありません。" + Environment.NewLine;
                    flag = true;
                    continue;
                }
                num++;
                cell2 = row2.GetCell(num);
                string text14 = "";
                try {
                    if (cell2 != null) {
                        text14 = cell2.RichStringCellValue.ToString();
                    }
                } catch (Exception) {
                    message += i.ToString() + "行目の配送料の負担が正しくありません。" + Environment.NewLine;
                    flag = true;
                    continue;
                }
                num++;
                cell2 = row2.GetCell(num);
                string text15 = "";
                try {
                    if (cell2 != null) {
                        text15 = cell2.RichStringCellValue.ToString();
                    }
                } catch (Exception) {
                    message += i.ToString() + "行目の配送の方法が正しくありません。" + Environment.NewLine;
                    flag = true;
                    continue;
                }
                num++;
                cell2 = row2.GetCell(num);
                string text16 = "";
                try {
                    if (cell2 != null) {
                        text16 = cell2.RichStringCellValue.ToString();
                    }
                } catch (Exception) {
                    message += i.ToString() + "行目の商品の状態が正しくありません。" + Environment.NewLine;
                    flag = true;
                    continue;
                }
                num++;
                cell2 = row2.GetCell(num);
                string text17 = "";
                try {
                    if (cell2 != null) {
                        text17 = cell2.RichStringCellValue.ToString();
                    }
                } catch (Exception) {
                    message += i.ToString() + "行目の発送元の地域が正しくありません。" + Environment.NewLine;
                    flag = true;
                    continue;
                }
                num++;
                cell2 = row2.GetCell(num);
                string text18 = "";
                try {
                    if (cell2 != null) {
                        text18 = cell2.RichStringCellValue.ToString();
                    }
                } catch (Exception) {
                    message += i.ToString() + "行目の発送までの日数が正しくありません。" + Environment.NewLine;
                    flag = true;
                    continue;
                }
                /*num++;
                cell2 = row2.GetCell(num);
                string text19 = "";
                try {
                    text19 = cell2.RichStringCellValue.ToString();
                } catch (Exception) {
                    message += i.ToString() + "行目の購入申請が正しくありません。" + Environment.NewLine;
                    flag = true;
                    continue;
                }*/
                num++;
                cell2 = row2.GetCell(num);
                string text20 = "0";
                try {
                    text20 = cell2.NumericCellValue.ToString();
                } catch (Exception) {
                    message += i.ToString() + "行目の販売価格が正しくありません。" + Environment.NewLine;
                    flag = true;
                    continue;
                }
                num++;
                cell2 = row2.GetCell(num);
                string text21 = "0";
                try {
                    text21 = cell2.NumericCellValue.ToString();
                } catch (Exception) {
                    message += i.ToString() + "行目の出品時間（年）が正しくありません。" + Environment.NewLine;
                    flag = true;
                    continue;
                }
                num++;
                cell2 = row2.GetCell(num);
                string text22 = "0";
                try {
                    text22 = cell2.NumericCellValue.ToString();
                } catch (Exception) {
                    message += i.ToString() + "行目の出品時間（月）が正しくありません。" + Environment.NewLine;
                    flag = true;
                    continue;
                }
                num++;
                cell2 = row2.GetCell(num);
                string text23 = "0";
                try {
                    text23 = cell2.NumericCellValue.ToString();
                } catch (Exception) {
                    message += i.ToString() + "行目の出品時間（日）が正しくありません。" + Environment.NewLine;
                    flag = true;
                    continue;
                }
                num++;
                cell2 = row2.GetCell(num);
                string text24 = "";
                try {
                    text24 = cell2.NumericCellValue.ToString();
                } catch (Exception) {
                    message += i.ToString() + "行目の出品時間（時）が正しくありません。" + Environment.NewLine;
                    flag = true;
                    continue;
                }
                num++;
                cell2 = row2.GetCell(num);
                string text25 = "";
                try {
                    text25 = cell2.NumericCellValue.ToString();
                } catch (Exception) {
                    message += i.ToString() + "行目の出品時間（分）が正しくありません。" + Environment.NewLine;
                    flag = true;
                    continue;
                }
                num++;
                cell2 = row2.GetCell(num);
                string text26 = "0";
                try {
                    text26 = cell2.NumericCellValue.ToString();
                } catch (Exception) {
                    //message += i.ToString() + "行目の取消時間（年）が正しくありません。" + Environment.NewLine ;
                    //flag = true;
                    //continue;
                }
                num++;
                cell2 = row2.GetCell(num);
                string text27 = "0";
                try {
                    text27 = cell2.NumericCellValue.ToString();
                } catch (Exception) {
                    //message += i.ToString() + "行目の取消時間（月）が正しくありません。" + Environment.NewLine ;
                    //flag = true;
                    //continue;
                }
                num++;
                cell2 = row2.GetCell(num);
                string text28 = "0";
                try {
                    text28 = cell2.NumericCellValue.ToString();
                } catch (Exception) {
                    //message += i.ToString() + "行目の取消時間（日）が正しくありません。" + Environment.NewLine ;
                    //flag = true;
                    //continue;
                }
                num++;
                cell2 = row2.GetCell(num);
                string text29 = "";
                try {
                    text29 = cell2.NumericCellValue.ToString();
                } catch (Exception) {
                    //message += i.ToString() + "行目の取消時間（時）が正しくありません。" + Environment.NewLine ;
                    //flag = true;
                    //continue;
                }
                num++;
                cell2 = row2.GetCell(num);
                string text30 = "";
                try {
                    text30 = cell2.NumericCellValue.ToString();
                } catch (Exception) {
                    //message += i.ToString() + "行目の取消時間（分）が正しくありません。" + Environment.NewLine ;
                    //flag = true;
                    //continue;
                }
                num++;
                cell2 = row2.GetCell(num);
                string text31 = "";
                try {
                    if (cell2 != null) {
                        text31 = cell2.RichStringCellValue.ToString();
                    }
                } catch (Exception) {
                    //message += i.ToString() + "行目の取消いいねが正しくありません。" + Environment.NewLine ;
                    //flag = true;
                    //continue;
                }
                num++;
                cell2 = row2.GetCell(num);
                string text32 = "";
                try {
                    if (cell2 != null) {
                        text32 = cell2.RichStringCellValue.ToString();
                    }
                } catch (Exception) {
                    //message += i.ToString() + "行目の取消コメントが正しくありません。" + Environment.NewLine ;
                    //flag = true;
                    //continue;
                }
                num++;
                cell2 = row2.GetCell(num);
                string text33 = "0";
                try {
                    text33 = cell2.NumericCellValue.ToString();
                } catch (Exception) {
                    //message += i.ToString() + "行目の取消時間（年）が正しくありません。" + Environment.NewLine ;
                    //flag = true;
                    //continue;
                }
                num++;
                cell2 = row2.GetCell(num);
                string text34 = "0";
                try {
                    text34 = cell2.NumericCellValue.ToString();
                } catch (Exception) {
                    //message += i.ToString() + "行目の取消時間（月）が正しくありません。" + Environment.NewLine ;
                    //flag = true;
                    //continue;
                }
                num++;
                cell2 = row2.GetCell(num);
                string text35 = "0";
                try {
                    text35 = cell2.NumericCellValue.ToString();
                } catch (Exception) {
                    //message += i.ToString() + "行目の取消時間（日）が正しくありません。" + Environment.NewLine ;
                    //flag = true;
                    //continue;
                }
                num++;
                cell2 = row2.GetCell(num);
                string text36 = "";
                try {
                    text36 = cell2.NumericCellValue.ToString();
                } catch (Exception) {
                    //message += i.ToString() + "行目の取消時間（時）が正しくありません。" + Environment.NewLine ;
                    //flag = true;
                    //continue;
                }
                num++;
                cell2 = row2.GetCell(num);
                string text37 = "";
                try {
                    text37 = cell2.NumericCellValue.ToString();
                } catch (Exception) {
                    //message += i.ToString() + "行目の取消時間（分）が正しくありません。" + Environment.NewLine ;
                    //flag = true;
                    //continue;
                }
                num++;
                cell2 = row2.GetCell(num);
                string text38 = "";
                try {
                    if (cell2 != null) {
                        text38 = cell2.RichStringCellValue.ToString();
                    }
                } catch (Exception) {
                    //message += i.ToString() + "行目の取消いいねが正しくありません。" + Environment.NewLine ;
                    //flag = true;
                    //continue;
                }
                num++;
                cell2 = row2.GetCell(num);
                string text39 = "";
                try {
                    if (cell2 != null) {
                        text39 = cell2.RichStringCellValue.ToString();
                    }
                } catch (Exception) {
                    //message += i.ToString() + "行目の取消コメントが正しくありません。" + Environment.NewLine ;
                    //flag = true;
                    //continue;
                }
                #endregion
                Image img1, img2, img3, img4;
                img1 = img2 = img3 = img4 = null;
                if (text5 != "") {
                    try {
                        img1 = Image.FromFile(text5);
                    } catch {

                    }
                }
                if (text6 != "") {
                    try {
                        img2 = Image.FromFile(text6);
                    } catch {

                    }
                }
                if (text7 != "") {
                    try {
                        img3 = Image.FromFile(text7);
                    } catch {

                    }
                }
                if (text8 != "") {
                    try {
                        img4 = Image.FromFile(text8);
                    } catch {

                    }
                }
                FrilCommon.FrilCategory now_category;
                int now_sizeid, now_shipping_payer_id, now_shipping_area_id, now_shipping_method_id, now_brandid;
                int now_shipping_duration_id, now_price, now_item_condition_id;
                int now_category_level1_id, now_category_level2_id, now_category_level3_id, now_category_level4_id;
                now_category_level1_id = now_category_level2_id = now_category_level3_id = now_category_level4_id = -1;
                now_sizeid = -1; //サイズ選択しない場合は-1
                now_brandid = -1; //サイズ選択しない場合は-1
                if (!(text2 == "") || !(text3 == "") || !(text4 == "") || !(text5 == "") || !(text6 == "") || !(text7 == "") || !(text8 == "") || !(text9 == "") || !(text10 == "") || !(text11 == "") || !(text12 == "") || !(text14 == "") || !(text15 == "") || !(text16 == "") || (!(text17 == "0") && !(text17 == "")) || (!(text18 == "0") && !(text18 == "")) || !(text20 == "") || !(text21 == "") || !(text22 == "") || !(text23 == "") || !(text24 == "") || !(text25 == "")) {
                    int accountDBId = -1;
                    if (!cashAccount.ContainsKey(text2)) {
                        accountDBId = new AccountDBHelper().getAccountDBId(text2);
                        cashAccount[text2] = accountDBId;
                    } else {
                        accountDBId = cashAccount[text2];
                    }
                    if (accountDBId < 0) {
                        message += i.ToString() + "行目のアカウントがツールに登録されていません。" + Environment.NewLine;
                        flag = true;
                    }
                    if (text3 == "") {
                        message += i.ToString() + "行目の商品名が入力されていません。" + Environment.NewLine;
                        flag = true;
                    } else if (text3.Length > 40) {
                        message += i.ToString() + "行目の商品名が40文字を超えています。" + Environment.NewLine;
                        flag = true;
                    } else if (text4 == "") {
                        message += i.ToString() + "行目の商品説明が入力されていません。" + Environment.NewLine;
                        flag = true;
                    } else if (text4.Length > 1000) {
                        message += i.ToString() + "行目の商品説明が1000文字を超えています。" + Environment.NewLine;
                        flag = true;
                    } else if (text5 == "" && text6 == "" && text7 == "" && text8 == "") {
                        message += i.ToString() + "行目の画像が選択されていません。" + Environment.NewLine;
                        flag = true;
                    } else if (text5 != "" && !File.Exists(text5)) {
                        message += i.ToString() + "行目の画像1のファイルがありません。" + Environment.NewLine;
                        flag = true;
                    } else if (img1 != null && img1.RawFormat.Equals(System.Drawing.Imaging.ImageFormat.Jpeg) == false) {
                        message += i.ToString() + "行目の画像1のファイル形式がJPGではありません。" + Environment.NewLine;
                        flag = true;
                    } else if (text6 != "" && !File.Exists(text6)) {
                        message += i.ToString() + "行目の画像2のファイルがありません。" + Environment.NewLine;
                        flag = true;
                    } else if (img2 != null && img2.RawFormat.Equals(System.Drawing.Imaging.ImageFormat.Jpeg) == false) {
                        message += i.ToString() + "行目の画像2のファイル形式がJPGではありません。" + Environment.NewLine;
                        flag = true;
                    } else if (text7 != "" && !File.Exists(text7)) {
                        message += i.ToString() + "行目の画像3のファイルがありません。" + Environment.NewLine;
                        flag = true;
                    } else if (img3 != null && img3.RawFormat.Equals(System.Drawing.Imaging.ImageFormat.Jpeg) == false) {
                        message += i.ToString() + "行目の画像3のファイル形式がJPGではありません。" + Environment.NewLine;
                        flag = true;
                    } else if (text8 != "" && !File.Exists(text8)) {
                        message += i.ToString() + "行目の画像4のファイルがありません。" + Environment.NewLine;
                        flag = true;
                    } else if (img4 != null && img4.RawFormat.Equals(System.Drawing.Imaging.ImageFormat.Jpeg) == false) {
                        message += i.ToString() + "行目の画像4のファイル形式がJPGではありません。" + Environment.NewLine;
                        flag = true;
                    } else if (text9 == "" || text10 == "") {
                        message += i.ToString() + "行目のカテゴリーが選択されていません。" + Environment.NewLine;
                        flag = true;
                    } else if (FrilCommon.getCategoryFromName(0, text9) == null) {
                        //親が0のカテゴリ一（つまりレベル1のカテゴリ一覧内になければエラー
                        message += i.ToString() + "行目のカテゴリー1が正しくありません。" + Environment.NewLine;
                        flag = true;
                    } else {
                        var category_level1 = FrilCommon.getCategoryFromName(0, text9);
                        now_category_level1_id = category_level1.Value.id;
                        now_category = category_level1.Value;
                        text10 = text10.Replace("〜", "~");
                        var category_level2 = FrilCommon.getCategoryFromName(category_level1.Value.id, text10);
                        if (category_level2 == null) {
                            message += i.ToString() + "行目のカテゴリー2が正しくありません。" + Environment.NewLine;
                            flag = true;
                        } else {
                            now_category = category_level2.Value;
                            now_category_level2_id = category_level2.Value.id;
                            //レベル3が存在していてかつ名前が存在しないならアウト
                            text11 = text11.Replace("〜", "~");
                            var category_level3 = FrilCommon.getCategoryFromName(category_level2.Value.id, text11);
                            if (category_level3 == null) {
                                message += i.ToString() + "行目のカテゴリー3が正しくありません。" + Environment.NewLine;
                                flag = true;
                            } else {
                                if (category_level3.HasValue) {
                                    now_category = category_level3.Value;
                                    now_category_level3_id = category_level3.Value.id;
                                }
                                if (text12 != "") {
                                    //サイズを選択しないといけないかつサイズを正しく選択していないならエラー
                                    if (FrilCommon.hasSizeOption(now_category)) {
                                        now_sizeid = FrilCommon.getSizeIdFromName(now_category, text12);
                                        if (now_sizeid < 0) {
                                            message += i.ToString() + "行目のサイズが正しくありません。" + Environment.NewLine;
                                            flag = true;
                                        }
                                    }
                                }
                                if (text13 != "") {
                                    //ブランド情報があるけどブランド情報のIDが見つからなければ
                                    now_brandid = FrilCommon.getBrandIdFromName(text13);
                                    if (now_brandid < 0) {
                                        message += i.ToString() + "行目のブランドが正しくありません。" + Environment.NewLine;
                                        flag = true;
                                    }
                                }
                                now_item_condition_id = FrilCommon.getItemConditionIdFromName(text16);
                                now_shipping_payer_id = FrilCommon.getShippingPayerIdFromName(text14);
                                now_shipping_method_id = FrilCommon.getShippingMethodIdFromName(now_shipping_payer_id, text15);
                                now_shipping_area_id = FrilCommon.getShippingAreaIdFromName(text17);
                                now_shipping_duration_id = FrilCommon.getShippingDurationIdFromName(text18);
                                //bool buy_request = (text19 == "あり");
                                if (text16 == "") {
                                    message += i.ToString() + "行目の商品の状態が入力されていません。" + Environment.NewLine;
                                    flag = true;
                                } else if (now_item_condition_id < 0) {
                                    message += i.ToString() + "行目の商品の状態が正しくありません。" + Environment.NewLine;
                                    flag = true;
                                } else if (text14 == "") {
                                    message += i.ToString() + "行目の配送量の負担が入力されていません。" + Environment.NewLine;
                                    flag = true;
                                } else if (now_shipping_payer_id < 0) {
                                    message += i.ToString() + "行目の配送量の負担が正しくありません。" + Environment.NewLine;
                                    flag = true;
                                } else if (text15 == "") {
                                    message += i.ToString() + "行目の配送の方法が入力されていません。" + Environment.NewLine;
                                    flag = true;
                                } else {
                                    if (now_shipping_method_id < 0) {
                                        message += i.ToString() + "行目の配送の方法が正しくありません。" + Environment.NewLine;
                                        flag = true;
                                    } else if (text17 == "") {
                                        message += i.ToString() + "行目の発送元の地域が入力されていません。" + Environment.NewLine;
                                        flag = true;
                                    } else if (now_shipping_area_id < 0) {
                                        message += i.ToString() + "行目の発送元の地域が正しくありません。" + Environment.NewLine;
                                        flag = true;
                                    } else if (text18 == "") {
                                        message += i.ToString() + "行目の発送までの日数が入力されていません。" + Environment.NewLine;
                                        flag = true;
                                    } else {
                                        int num4, num5, num6, num7;
                                        if (now_shipping_duration_id < 0) {
                                            message += i.ToString() + "行目の発送までの日数が正しくありません。" + Environment.NewLine;
                                            flag = true;
                                        } else if (text20 == "0") {
                                            message += i.ToString() + "行目の販売価格が入力されていません。" + Environment.NewLine;
                                            flag = true;
                                        } else if (!int.TryParse(text20, out now_price)) {
                                            message += i.ToString() + "行目の販売価格が整数ではありません。" + Environment.NewLine;
                                            flag = true;
                                        } else if (now_price < 300 || now_price > 990000) {
                                            message += i.ToString() + "行目の販売価格が300～990,000の間で入力されていません。" + Environment.NewLine;
                                            flag = true;
                                        } else if (text21 == "0") {
                                            message += i.ToString() + "行目の出品時間（年）が入力されていません。" + Environment.NewLine;
                                            flag = true;
                                        } else if (text22 == "0") {
                                            message += i.ToString() + "行目の出品時間（月）が入力されていません。" + Environment.NewLine;
                                            flag = true;
                                        } else if (!int.TryParse(text22, out num4)) {
                                            message += i.ToString() + "行目の出品時間（月）が数値ではありません。" + Environment.NewLine;
                                            flag = true;
                                        } else if (num4 < 1 || num4 > 12) {
                                            message += i.ToString() + "行目の出品時間（月）が正しくありません。" + Environment.NewLine;
                                            flag = true;
                                        } else if (text23 == "0") {
                                            message += i.ToString() + "行目の出品時間（日）が入力されていません。" + Environment.NewLine;
                                            flag = true;
                                        } else if (!int.TryParse(text23, out num5)) {
                                            message += i.ToString() + "行目の出品時間（日）が数値ではありません。" + Environment.NewLine;
                                            flag = true;
                                        } else if (num5 < 1 || num5 > 31) {
                                            message += i.ToString() + "行目の出品時間（日）が正しくありません。" + Environment.NewLine;
                                            flag = true;
                                        } else if (text24 == "") {
                                            message += i.ToString() + "行目の出品時間（時）が入力されていません。" + Environment.NewLine;
                                            flag = true;
                                        } else if (!int.TryParse(text24, out num6)) {
                                            message += i.ToString() + "行目の出品時間（時）が数値ではありません。" + Environment.NewLine;
                                            flag = true;
                                        } else if (num6 < 0 || num6 > 23) {
                                            message += i.ToString() + "行目の出品時間（時）が正しくありません。" + Environment.NewLine;
                                            flag = true;
                                        } else if (text25 == "") {
                                            message += i.ToString() + "行目の出品時間（分）が入力されていません。" + Environment.NewLine;
                                            flag = true;
                                        } else if (!int.TryParse(text25, out num7)) {
                                            message += i.ToString() + "行目の出品時間（分）が数値ではありません。" + Environment.NewLine;
                                            flag = true;
                                        } else if (num7 < 0 || num7 > 59) {
                                            message += i.ToString() + "行目の出品時間（分）が正しくありません。" + Environment.NewLine;
                                            flag = true;
                                        } else {
                                            int num8 = 0;
                                            if (text27 != "" && text27 != "0" && !int.TryParse(text27, out num8)) {
                                                message += i.ToString() + "行目の取消時間（月）が数値ではありません。" + Environment.NewLine;
                                                flag = true;
                                            } else if ((text27 != "" && text27 != "0" && num8 < 1) || num8 > 12) {
                                                message += i.ToString() + "行目の取消時間（月）が正しくありません。" + Environment.NewLine;
                                                flag = true;
                                            } else {
                                                int num9 = 0;
                                                if (text28 != "" && text28 != "0" && !int.TryParse(text28, out num9)) {
                                                    message += i.ToString() + "行目の取消時間（日）が数値ではありません。" + Environment.NewLine;
                                                    flag = true;
                                                } else if ((text28 != "" && text28 != "0" && num9 < 1) || num9 > 31) {
                                                    message += i.ToString() + "行目の取消時間（日）が正しくありません。" + Environment.NewLine;
                                                    flag = true;
                                                } else {
                                                    int num10 = 0;
                                                    if (text29 != "" && !int.TryParse(text29, out num10)) {
                                                        message += i.ToString() + "行目の取消時間（時）が数値ではありません。" + Environment.NewLine;
                                                        flag = true;
                                                    } else if ((text29 != "" && num10 < 0) || num10 > 23) {
                                                        message += i.ToString() + "行目の取消時間（時）が正しくありません。" + Environment.NewLine;
                                                        flag = true;
                                                    } else {
                                                        int num11 = 0;
                                                        if (text30 != "" && !int.TryParse(text30, out num11)) {
                                                            message += i.ToString() + "行目の取消時間（分）が数値ではありません。" + Environment.NewLine;
                                                            flag = true;
                                                        } else if ((text30 != "" && num11 < 0) || num11 > 59) {
                                                            message += i.ToString() + "行目の取消時間（分）が正しくありません。" + Environment.NewLine;
                                                            flag = true;
                                                        } else {
                                                            string[] cancelOption = FrilCommon.getCancelOption();
                                                            if (text31 != "" && Array.IndexOf<string>(cancelOption, text31) < 0) {
                                                                message += i.ToString() + "行目の取消いいねが正しくありません。" + Environment.NewLine;
                                                                flag = true;
                                                            } else if (text32 != "" && Array.IndexOf<string>(cancelOption, text32) < 0) {
                                                                message += i.ToString() + "行目の取消コメントが正しくありません。" + Environment.NewLine;
                                                                flag = true;
                                                            } else {
                                                                int num12 = 0;
                                                                if (text34 != "" && text34 != "0" && !int.TryParse(text34, out num12)) {
                                                                    message += i.ToString() + "行目の取消時間2（月）が数値ではありません。" + Environment.NewLine;
                                                                    flag = true;
                                                                } else if ((text34 != "" && text34 != "0" && num12 < 1) || num12 > 12) {
                                                                    message += i.ToString() + "行目の取消時間2（月）が正しくありません。" + Environment.NewLine;
                                                                    flag = true;
                                                                } else {
                                                                    int num13 = 0;
                                                                    if (text35 != "" && text35 != "0" && !int.TryParse(text35, out num13)) {
                                                                        message += i.ToString() + "行目の取消時間2（日）が数値ではありません。" + Environment.NewLine;
                                                                        flag = true;
                                                                    } else if ((text35 != "" && text35 != "0" && num13 < 1) || num13 > 31) {
                                                                        message += i.ToString() + "行目の取消時間2（日）が正しくありません。" + Environment.NewLine;
                                                                        flag = true;
                                                                    } else {
                                                                        int num14 = 0;
                                                                        if (text36 != "" && !int.TryParse(text36, out num14)) {
                                                                            message += i.ToString() + "行目の取消時間2（時）が数値ではありません。" + Environment.NewLine;
                                                                            flag = true;
                                                                        } else if ((text36 != "" && num14 < 0) || num14 > 23) {
                                                                            message += i.ToString() + "行目の取消時間2（時）が正しくありません。" + Environment.NewLine;
                                                                            flag = true;
                                                                        } else {
                                                                            int num15 = 0;
                                                                            if (text37 != "" && !int.TryParse(text37, out num15)) {
                                                                                message += i.ToString() + "行目の取消時間2（分）が数値ではありません。" + Environment.NewLine;
                                                                                flag = true;
                                                                            } else if ((text37 != "" && num15 < 0) || num15 > 59) {
                                                                                message += i.ToString() + "行目の取消時間2（分）が正しくありません。" + Environment.NewLine;
                                                                                flag = true;
                                                                            } else {
                                                                                if (text38 != "" && Array.IndexOf<string>(cancelOption, text38) < 0) {
                                                                                    message += i.ToString() + "行目の取消いいね2が正しくありません。" + Environment.NewLine;
                                                                                    flag = true;
                                                                                } else if (text39 != "" && Array.IndexOf<string>(cancelOption, text39) < 0) {
                                                                                    message += i.ToString() + "行目の取消コメント2が正しくありません。" + Environment.NewLine;
                                                                                    flag = true;
                                                                                } else {
                                                                                    #region addItemToDB
                                                                                    var exhibitItem = new FrilItem();
                                                                                    exhibitItem.item_name = text3;
                                                                                    exhibitItem.detail = text4;
                                                                                    if (this.checkBox1.Checked == false) {
                                                                                        exhibitItem.imagepaths[0] = text5;
                                                                                        exhibitItem.imagepaths[1] = text6;
                                                                                        exhibitItem.imagepaths[2] = text7;
                                                                                        exhibitItem.imagepaths[3] = text8;
                                                                                    } else {
                                                                                        //トリミング
                                                                                        exhibitItem.imagepaths[0] = Common.getExhibitionImageFromPath(text5);
                                                                                        exhibitItem.imagepaths[1] = Common.getExhibitionImageFromPath(text6);
                                                                                        exhibitItem.imagepaths[2] = Common.getExhibitionImageFromPath(text7);
                                                                                        exhibitItem.imagepaths[3] = Common.getExhibitionImageFromPath(text8);
                                                                                    }
                                                                                    exhibitItem.category_level1_id = now_category_level1_id;
                                                                                    exhibitItem.category_level2_id = now_category_level2_id;
                                                                                    exhibitItem.category_level3_id = now_category_level3_id;
                                                                                    exhibitItem.category_level4_id = now_category_level4_id;
                                                                                    exhibitItem.size_id = now_sizeid;
                                                                                    // exhibitItem.size_name = 
                                                                                    exhibitItem.brand_id = now_brandid;
                                                                                    exhibitItem.status = now_item_condition_id;
                                                                                    exhibitItem.carriage = now_shipping_payer_id;
                                                                                    exhibitItem.d_method = now_shipping_method_id;
                                                                                    exhibitItem.d_area = now_shipping_area_id;
                                                                                    exhibitItem.d_date = now_shipping_duration_id;
                                                                                    exhibitItem.s_price = now_price;
                                                                                    #endregion
                                                                                    #region UnkoSystem
                                                                                    exhibitItem.category_p_id = exhibitItem.category_level2_id;
                                                                                    exhibitItem.category_id = exhibitItem.category_level3_id;
                                                                                    //exhibitItem.
                                                                                    #endregion
                                                                                    #region addReservationToDB
                                                                                    //既におなじ商品がDBにあるかチェックする
                                                                                    int addItemDBId = this.checkCashItem(exhibitItem);
                                                                                    try {
                                                                                        if (addItemDBId == 0) {
                                                                                            //DBにないので追加
                                                                                            this.itemDBHelper.addItem(exhibitItem); ;
                                                                                            addItemDBId = this.itemDBHelper.getItemDBId(exhibitItem);
                                                                                        }
                                                                                    } catch (Exception) {
                                                                                        continue;
                                                                                    }
                                                                                    ReservationSettingForm.ReservationSetting reservation = new ReservationSettingForm.ReservationSetting();
                                                                                    reservation.itemDBId = addItemDBId;
                                                                                    reservation.accountDBId = accountDBId;
                                                                                    reservation.exhibitDate = DateTime.Parse(string.Concat(new string[] { text21, "/", text22, "/", text23, " ", text24, ":", text25, ":00" }));
                                                                                    if (text26 != "0" && text26 != "" && text27 != "0" && text27 != "" && text28 != "" && text29 != "" && text30 != "") {
                                                                                        //自動削除する
                                                                                        reservation.deleteDate = DateTime.Parse(string.Concat(new string[] { text26, "/", text27, "/", text28, " ", text29, ":", text30, ":00" }));
                                                                                    } else {
                                                                                        //自動削除しない
                                                                                        reservation.deleteDate = DateTime.Parse(ReservationSettingForm.ReservationSetting.dafaultDate);
                                                                                    }
                                                                                    if (text33 != "0" && text33 != "" && text34 != "0" && text34 != "" && text35 != "" && text36 != "" && text37 != "") {
                                                                                        //自動削除2する
                                                                                        reservation.deleteDate2 = DateTime.Parse(string.Concat(new string[] { text33, "/", text34, "/", text35, " ", text36, ":", text37, ":00" }));
                                                                                    } else {
                                                                                        //自動削除2しない
                                                                                        reservation.deleteDate2 = DateTime.Parse(ReservationSettingForm.ReservationSetting.dafaultDate);
                                                                                    }
                                                                                    reservation.consider_favorite = false;
                                                                                    if (text31 == "見る") {
                                                                                        reservation.consider_favorite = true;
                                                                                    }
                                                                                    reservation.consider_comment = false;
                                                                                    if (text32 == "見る") {
                                                                                        reservation.consider_comment = true;
                                                                                    }
                                                                                    reservation.consider_favorite2 = false;
                                                                                    if (text38 == "見る") {
                                                                                        reservation.consider_favorite2 = true;
                                                                                    }
                                                                                    reservation.consider_comment2 = false;
                                                                                    if (text39 == "見る") {
                                                                                        reservation.consider_comment2 = true;
                                                                                    }
                                                                                    this.reservationDBHelper.addReservation(reservation);
                                                                                    #endregion
                                                                                }
                                                                            }
                                                                        }
                                                                    }
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            if (flag) {
                return 3;
            }
            return 0;
        }
        private void button1_Click(object sender, EventArgs e) {
            //処理が行われているときは、何もしない
            if (BackgroundWorker1.IsBusy)// || BackgroundWorker2.IsBusy)
                return;

            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "エクセルファイル(*.xlsx)|*.xlsx";
            ofd.Title = "エクセルファイルを選択してください";
            //ダイアログボックスを閉じる前に現在のディレクトリを復元するようにする
            ofd.RestoreDirectory = true;
            //ダイアログを表示する
            if (ofd.ShowDialog() == DialogResult.OK) {
                string filename = ofd.FileName;
                button1.Enabled = false;
                checkBox1.Enabled = false;
                numericUpDown1.Enabled = false;
                //コントロールを初期化する
                ProgressBar1.Minimum = 0;
                ProgressBar1.Maximum = 100;
                ProgressBar1.Value = 0;
                this.excel_rownum = (int)this.numericUpDown1.Value;
                this.message = "";
                this.richTextBox1.Text = "";
                this.groupBox1.Enabled = false;
                if (this.radioButton1.Checked) {
                    BackgroundWorker1.RunWorkerAsync(filename); //商品情報を含めて登録
                } else {
                    //BackgroundWorker2.RunWorkerAsync(filename); //親ID子IDを含めて登録
                }
            }
        }
        private BackgroundWorker bgWorker;
        private void BackgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e) {
            ProgressBar1.Value = e.ProgressPercentage;
        }

        private void BackgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e) {
            this.ProgressBar1.Value = 100;
            int result = (int)e.Result;
            this.richTextBox1.Text = message;
            //if (e.Error != null) {
            switch (result) {
                case 0:
                    MessageBox.Show("全て正常に処理完了", "結果", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    break;
                case 3:
                    MessageBox.Show("一部の商品が正しく処理できませんでした", "結果", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    break;
                default:
                    MessageBox.Show("ファイル形式が不正またはファイルを開けません。\nエクセルでファイルを開いたまま読み込まないでください", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    break;
            }
            //Button1を有効に戻す
            button1.Enabled = true;
            this.checkBox1.Enabled = true;
            this.numericUpDown1.Enabled = true;
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e) {
            if (this.checkBox1.Checked) {
                MessageBox.Show("画像は自動でトリミングされます。\nトリミングした画像はimgフォルダ内に保存されます");
            }
        }

        private void ReservationRegisterFromExcelForm_FormClosing(object sender, FormClosingEventArgs e) {
            if (BackgroundWorker1.IsBusy) e.Cancel = true;
            this.mainform.OnBackFromReservationSettingForm();
        }

        //キャッシュを利用して、商品がDBに既にあればそのDBIdを返す. DBになければ0を返す
        private int checkCashItem(FrilItem exhibitItem) {
            int num = 0;
            //商品情報をKeyにしてキャッシュに追加する
            string key = string.Concat(new string[]
            {
                exhibitItem.item_name,
                exhibitItem.detail,
                exhibitItem.imagepaths[0],
                exhibitItem.imagepaths[1],
                exhibitItem.imagepaths[2],
                exhibitItem.imagepaths[3],
                exhibitItem.category_level1_id.ToString(),
                exhibitItem.category_level2_id.ToString(),
                exhibitItem.category_level3_id.ToString(),
                exhibitItem.category_level4_id.ToString(),
                exhibitItem.size_id.ToString(),
                exhibitItem.brand_id.ToString(),
                exhibitItem.status.ToString(),
                exhibitItem.carriage.ToString(),
                exhibitItem.d_method.ToString(),
                exhibitItem.d_area.ToString(),
                exhibitItem.d_date.ToString(),
                exhibitItem.s_price.ToString()
            });
            try {
                num = this.cashItem[key];
            } catch (KeyNotFoundException) {
                try {
                    num = this.itemDBHelper.getItemDBId(exhibitItem);
                    if (num != 0) {
                        this.cashItem.Add(key, num);
                    }
                } catch (Exception) {
                    return 0;
                }
            }
            return num;
        }
    }
}