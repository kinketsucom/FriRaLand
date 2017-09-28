using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FriRaLand {
    class RakumaItem {
        public string productId;
        public string sellerId;
        public string sellerNickname;
        public int sellerEvaluationTotal;
        public int sellerSellingCount;
        public string purchaserId;
        public string productName;
        public int sellingPrice;
        public int systemFee;
        public int sellingStatus;
        public string descriptionText;
        public int brandId;
        public string brandName;
        public int sizeId;
        public string sizeTitle;
        public int conditionType;
        public int postageType;
        public int deliveryMethod;
        public int deliveryTerm;
        public int prefectureCode;
        public int likeCount;
        public int commentCount;
        public DateTime updateTime;
        public int ownLikeFlag;
        public DateTime registartionTime;
        public int[] categoryId = new int[3]; //0:Level3 1:Level2 2;Level1

        public string[] imageurls = new string[4]; //画像URL
        public string[] imagepaths = new string[4]; //ローカルの画像パス
        public RakumaItem() {

        }
        public RakumaItem(dynamic json) {
            var info = json.productInfo;
            this.productId = info.profuctId;
            this.sellerId = info.sellerId;
            this.sellerNickname = info.sellerNickname;
            this.sellerEvaluationTotal = (int)info.sellerEvaluationTotal;
            this.sellerSellingCount = (int)info.sellerSellingCount;
            this.purchaserId = info.purchaserId;
            this.productName = info.productName;
            this.sellingPrice = (int)info.sellingPrice;
            this.systemFee = (int)info.systemFee;
            this.sellingStatus = (int)info.sellingStatus;
            this.descriptionText = info.descriptionText;
            this.brandId = (int)info.brandId;
            this.brandName = info.brandName;
            this.sizeId = (int)info.sizeId;
            this.sizeTitle = info.sizeTitle;
            this.conditionType = (int)info.conditionType;
            this.postageType = (int)info.postageType;
            this.deliveryMethod = (int)info.deliveryMethod;
            this.deliveryTerm = (int)info.deliveryTerm;
            this.prefectureCode = (int)info.prefectureCode;
            this.likeCount = (int)info.likeCount;
            this.commentCount = (int)info.commentCount;
            this.updateTime = Common.getDateFromUnixTimeStamp((long)info.updateTime);
            this.ownLikeFlag = (int)info.ownLikeFlag;

            var categoryList = json.categoryList;
            for (int i = 0; i < 3; i++) this.categoryId[i] = -1;
            int num = 0;
            foreach (var category in categoryList) {
                this.categoryId[num++] = (int)category.categoryId;
            }
            this.registartionTime = Common.getDateFromUnixTimeStamp((long)info.registrationTime);
            for (int i = 0; i < 4; i++) this.imageurls[i] = "";
            num = 0;
            foreach (var imageinfo in json.imageList) {
                this.imageurls[num++] = imageinfo.image;
            }
        }
    }
}
