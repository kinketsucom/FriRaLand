using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FriRaLand.Models {
    public class FrilItemDB {
        public int Id { get;set; }//データベース用Id

        public string item_id{get;set;}
        public string item_name{get;set;}
        public string detail{get;set;}
        public int s_price{get;set;}
        public int status{get;set;}
        public int t_status{get;set;}
        public int carriage{get;set;}
        public int d_method{get;set;}//delivery_method
        public int d_date{get;set;}//delivery_date
        public int d_area{get;set;}//delivary_from_area{get;set;}
        public string user_id{get;set;}//出品者ID
        public DateTime created_at{get;set;}//ex)2017-09-27T09:12:57+09:00
        public string screen_name{get;set;} //出品者アカウント名
        public int category_id{get;set;}
        public int category_p_id{get;set;}//parent?
        public int size_id{get;set;} //19999=>なし
        public string size_name{get;set;}
        public int brand_id{get;set;} //null
        public int i_brand_id{get;set;}
        public int commens_count{get;set;}
        public int likes_count{get;set;}
        public string[] imageurls{get;set;} //画像URL
        public string[] imagepaths{get;set;} //ローカルの画像パス
    }
}
