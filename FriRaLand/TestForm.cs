using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FriRaLand {
    public partial class TestForm : Form {
        public TestForm() {
            InitializeComponent();
        }

        public static string mail = "";
        public static string pass = "";

        private void button1_Click(object sender, EventArgs e) {
            api.tryRakumaLogin();
            item.brandId = 1573;
            item.categoryId = new int[3]{281, 32, 3};
            item.conditionType = 1;
            item.deliveryMethod = 1;
            item.deliveryTerm = 3;
            item.descriptionText = "商品説明";
            item.prefectureCode = 27;
            item.productName = "商品名";
            item.sellingPrice = 9999;
            item.imagepaths[0] = @"画像パス";
            api.Sell(item);
        }

        private void TestForm_Load(object sender, EventArgs e) {
            //FrilAPI api = new FrilAPI(mail, pass);
            //api.tryFrilLogin();
        }

        private void button2_Click(object sender, EventArgs e) {
            //FrilAPI api = new FrilAPI(mail,pass);
            //api.tryFrilLogin(cc);
            ////api.Sell();
            //var itemid = api.getItemIDFromBrowserItemURL("https://item.fril.jp/63c796255a32a4b40bf96ceb6a0f9aa8");
            //var item = api.getItemDetailInfo(itemid);
            //Console.WriteLine(item.brand_id);
            //Console.WriteLine(item.detail);

        }



    }
}
