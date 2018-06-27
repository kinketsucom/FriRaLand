using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;

namespace FriLand {
    public partial class TestForm : Form {
        public TestForm() {
            InitializeComponent();
        }

        public static string mail = "";
        public static string pass = "";

        private void button1_Click(object sender, EventArgs e) {
        }

        private void TestForm_Load(object sender, EventArgs e) {
            
            //FrilAPI api = new FrilAPI(mail, pass);
            //api.tryFrilLogin(cc);
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
