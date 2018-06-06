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

        private void button1_Click(object sender, EventArgs e) {
            RakumaAPI api = new RakumaAPI("RakumaEmail", "RakumaPassword");
            api.tryRakumaLogin();
            RakumaItem item = new RakumaItem();
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

        }

        private void button2_Click(object sender, EventArgs e) {
            FrilAPI api = new FrilAPI("FrilEmail", "FrilPassword");
            api.tryFrilLogin();
            //api.Sell();
            var item = api.getItemDetailInfo("101773596");
            Console.WriteLine(item);
        }
    }
}
