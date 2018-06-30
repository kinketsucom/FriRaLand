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
    public partial class ItemRegisterFromExcelForm : Form {
        private string message = "";
        private MainForm mainform;
        private int excel_rownum;
        private FrilItemDBHelper itemDBHelper;
        public ItemRegisterFromExcelForm(MainForm mainform) {
            this.mainform = mainform;
            this.itemDBHelper = new FrilItemDBHelper();

            InitializeComponent();
        }

        private void ItemRegisterFromExcelForm_FormClosing(object sender, FormClosingEventArgs e) {
            //実行中は閉じれない
            if (BackgroundWorker1.IsBusy) e.Cancel = true;
            this.mainform.OnBackFromItemExhibitForm();
        }

        private void ItemRegisterFromExcelForm_Load(object sender, EventArgs e) {

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
                /*ICell cell2 = row2.GetCell(num);
                string text = "";
                try {
                    if (cell2 != null) {
                        text = cell2.RichStringCellValue.ToString();
                    }
                } catch (Exception) {
                    message += i.ToString() + "行目の親番号が正しくありません。\n";
                    flag = true;
                    continue;
                }
                num++;
                cell2 = row2.GetCell(num);
                string text2 = "";
                try {
                    if (cell2 != null) {
                        text2 = cell2.RichStringCellValue.ToString();
                    }
                } catch (Exception) {
                    message += i.ToString() + "行目の子番号が正しくありません。\n";
                    flag = true;
                    continue;
                }
                num++;*/
                ICell cell2 = row2.GetCell(num);
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
                num++;
                cell2 = row2.GetCell(num);
                string text19 = "";
                try {
                    text19 = cell2.RichStringCellValue.ToString();
                } catch (Exception) {
                    message += i.ToString() + "行目の購入申請が正しくありません。" + Environment.NewLine;
                    flag = true;
                    continue;
                }
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
                if (!(text3 == "") || !(text4 == "") || !(text5 == "") || !(text6 == "") || !(text7 == "") || !(text8 == "") || !(text9 == "") || !(text10 == "") || !(text11 == "") || !(text12 == "") || !(text14 == "") || !(text15 == "") || !(text16 == "") || !(text17 == "") || !(text18 == "") || !(text19 == "") || !(text20 == "")) {
                    /*string parent_id = text.Trim();
                    string child_id = text2.Trim();
                    if (parent_id == "" && child_id != "") {
                        message += i.ToString() + "行目の商品の親IDが入力されていません。" + Environment.NewLine;
                        flag = true;
                    } else if (parent_id != "" && child_id == "") {
                        message += i.ToString() + "行目の商品の子IDが入力されていません。" + Environment.NewLine;
                        flag = true;
                    } else*/
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
                                bool buy_request = (text19 == "あり");
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
                                        if (now_shipping_duration_id < 0) {
                                            message += i.ToString() + "行目の発送までの日数が正しくありません。" + Environment.NewLine;
                                            flag = true;
                                        } else if (text20 == "0") {
                                            message += i.ToString() + "行目の販売価格が入力されていません。" + Environment.NewLine;
                                            flag = true;
                                        } else if (!int.TryParse(text18, out now_price)) {
                                            message += i.ToString() + "行目の販売価格が整数ではありません。" + Environment.NewLine;
                                            flag = true;
                                        } else if (now_price < 300 || now_price > 990000) {
                                            message += i.ToString() + "行目の販売価格が300～990,000の間で入力されていません。" + Environment.NewLine;
                                            flag = true;
                                        } else {
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
                                            exhibitItem.brand_id = now_brandid;
                                            exhibitItem.status = now_item_condition_id;
                                            exhibitItem.carriage = now_shipping_payer_id;
                                            exhibitItem.d_method = now_shipping_method_id;
                                            exhibitItem.d_area = now_shipping_area_id;
                                            exhibitItem.d_date = now_shipping_duration_id;
                                            exhibitItem.s_price = now_price;
                                            /*//親IDと子IDが既に存在する場合は上書き
                                            int existItemDBId = itemfamilyDBHelper.getItemDBIdFromParentChild(parent_id, child_id);
                                            if (existItemDBId >= 0) {
                                                //DBを更新する
                                                this.DBHelper.updateItem(existItemDBId, exhibitItem);
                                            } else {*/
                                            //DBに追加する
                                            this.itemDBHelper.addItem(exhibitItem); ;
                                            //DBIDを取得
                                            int addItemDBId = this.itemDBHelper.getItemDBId(exhibitItem);
                                            /*//親IDと子IDがあればDBに追加
                                            if (parent_id != "" && child_id != "") {
                                                int zaikonum = zaikoDBHelper.getZaikoNum(parent_id);
                                                if (zaikonum < 0) zaikoDBHelper.updateZaikoInfo(parent_id, 0);
                                                itemfamilyDBHelper.addItemFamily(parent_id, child_id, addItemDBId);
                                            }*/
                                            /*}*/
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
            if (BackgroundWorker1.IsBusy)
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

                //BackgroundWorkerのProgressChangedイベントが発生するようにする デザイナで設定
                //BackgroundWorker1.WorkerReportsProgress = true;
                //DoWorkで取得できるパラメータ(10)を指定して、処理を開始する
                //パラメータが必要なければ省略できる
                BackgroundWorker1.RunWorkerAsync(filename);
            }

        }
        private BackgroundWorker bgWorker;
        private void BackgroundWorker1_DoWork(object sender, DoWorkEventArgs e) {
            this.bgWorker = (BackgroundWorker)sender;
            //パラメータを取得する
            string filename = (string)e.Argument;
            e.Result = readExcelFile(filename);
            //ProgressChangedで取得できる結果を設定する
        }

        private void BackgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e) {
            //ProgressBar1の値を変更する
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
    }
}