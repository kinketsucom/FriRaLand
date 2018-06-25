using FriRaLand.DBHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FriRaLand {



    class ExhibitService {
        //商品が売れた際のDB操作
        public static void updateDBOnSold(FrilAPI api, FrilItem soldFrilItem) {
            var shuppinrirekiDic = new ShuppinRirekiDBHelper().loadRirekiDictionary();
            if (shuppinrirekiDic.ContainsKey(soldFrilItem.item_id) == false) return;
            int itemDBId = shuppinrirekiDic[soldFrilItem.item_id].itemDBId;
            Common.Account account = new AccountDBHelper().getAccountFromSellerid(soldFrilItem.user_id.ToString());
            updateDBOnSold(account, api, soldFrilItem, itemDBId);
        }
        public static void updateDBOnSold(Common.Account account, FrilAPI api, FrilItem soldFrilItem, int merlandItemDBId) {
            ItemFamilyDBHelper itemfamilyDBHelper = new ItemFamilyDBHelper();
            AccountDBHelper accountDBHelper = new AccountDBHelper();
            ShuppinRirekiDBHelper shuppinRirekiDBHelper = new ShuppinRirekiDBHelper();
            ExhibitLogDBHelper exhibitLogDBHelper = new ExhibitLogDBHelper();
            ZaikoDBHelper zaikoDBHelper = new ZaikoDBHelper();
            var itemParentDic = new ItemFamilyDBHelper().getItemIdToParentIdDictionary();
            //出品履歴に該当商品のデータが存在しない場合は既に処理完了しているのでDB操作を行わない
            if (shuppinRirekiDBHelper.existShuppinRireki(soldFrilItem.item_id) == false) return;

            //集計ログに「販売」を追加                      
            var itemfamily = itemfamilyDBHelper.getItemFamilyFromItemDBId(merlandItemDBId); //商品の在庫familyを取得 
            exhibitLogDBHelper.addSoldLog(api.nickname, soldFrilItem.created_date, DateTime.Now, itemfamily, soldFrilItem.item_id);
            //アカウントの販売数を+1
            account.hanbai_num++;
            //最終出品時刻を更新
            account.last_exhibitTime_str = DateTime.Now.ToString();
            accountDBHelper.updateAccount(account.DBId, account);

            //在庫数を1減らす
            if (itemParentDic.ContainsKey(merlandItemDBId)) {
                //在庫管理している=親IDが存在する場合のみ在庫を更新
                string parent_id = itemParentDic[merlandItemDBId];
                int zaikonum = zaikoDBHelper.getZaikoNum(parent_id);
                bool parent_exist = zaikoDBHelper.isexistParentid(parent_id);
                if (parent_exist) zaikoDBHelper.updateZaikoInfo(parent_id, zaikonum - 1);
            }
            //売れた商品の出品履歴削除
            shuppinRirekiDBHelper.deleteRireki(soldFrilItem.item_id);
            //値下げカウントも削除
            new NesageCntDBHelper().deleteNesageCnt(soldFrilItem.item_id);
        }

        //商品を出品することができるのか
        public static int canExhibitItem(Common.Account account, FrilItem merlandItem) {
            //圏外数がオプションで設定した値を超えているNG
            if (Settings.getCheckKengaiStatusOnExhibit() && account.kengai_num != -1 && account.kengai_num >= Settings.getKengaiNotExhibitBorder()) {
                Log.Logger.Info(account.nickname + "の圏外数は" + account.kengai_num + "のため出品スキップ");
                return -1;
            }
            //在庫数が0以下ならNG
            ZaikoDBHelper zaikoDBHelper = new ZaikoDBHelper();
            int zaikonum = zaikoDBHelper.getZaikoNum(merlandItem.parent_id);
            bool parent_exist = zaikoDBHelper.isexistParentid(merlandItem.parent_id);
            if (parent_exist && zaikonum <= 0) {
                Log.Logger.Info("在庫数が0以下なので出品スキップ");
                return -2;
            }
            return 1;
        }

        //出品成功した際の共通DB操作 予約情報にIDを書き込む操作は書かれていない
        public static void updateDBOnExhibitCommon(Common.Account account, FrilAPI api, FrilItem exhibittedFrilItem, int merlandItemDBId, bool reexhibit_flag = false) {
            ShuppinRirekiDBHelper shuppinRirekiDBHelper = new ShuppinRirekiDBHelper();
            ExhibitLogDBHelper exhibitLogDBHelper = new ExhibitLogDBHelper();
            AccountDBHelper accountDBHelper = new AccountDBHelper();
            ItemFamilyDBHelper itemFamilyDBHelper = new ItemFamilyDBHelper();
            //出品履歴を追加
            shuppinRirekiDBHelper.addShuppinRireki(new ShuppinRirekiDBHelper.ShuppinRireki(merlandItemDBId, account.DBId, exhibittedFrilItem.item_id, reexhibit_flag));
            //集計ログに「出品」を追加
            var itemfamily = itemFamilyDBHelper.getItemFamilyFromItemDBId(merlandItemDBId);
            exhibitLogDBHelper.addExhibitLog(api.nickname, exhibittedFrilItem.created_date, itemfamily, exhibittedFrilItem.item_id);
            //アカウントの出品数を+1
            account.exhibit_cnt++;
            accountDBHelper.updateAccount(account.DBId, account);
        }
        //再出品成功した際のDB操作
        public static void updateDBOnReExhibit(Common.Account account, FrilAPI api, FrilItem exhibittedFrilItem, int merlandItemDBId, bool reexhibit_flag) {
            ShuppinRirekiDBHelper shuppinRirekiDBHelper = new ShuppinRirekiDBHelper();
            ExhibitLogDBHelper exhibitLogDBHelper = new ExhibitLogDBHelper();
            AccountDBHelper accountDBHelper = new AccountDBHelper();
            ItemFamilyDBHelper itemFamilyDBHelper = new ItemFamilyDBHelper();
            //出品履歴を追加
            shuppinRirekiDBHelper.addShuppinRireki(new ShuppinRirekiDBHelper.ShuppinRireki(merlandItemDBId, account.DBId, exhibittedFrilItem.item_id, reexhibit_flag));
            //集計ログに「再出品」を追加
            var itemfamily = itemFamilyDBHelper.getItemFamilyFromItemDBId(merlandItemDBId);
            exhibitLogDBHelper.addReexhibitLog(api.nickname, exhibittedFrilItem.created_date, itemfamily, exhibittedFrilItem.item_id);
            //アカウントの出品数を+1
            account.exhibit_cnt++;
            accountDBHelper.updateAccount(account.DBId, account);
        }

        //削除・停止成功した際の共通操作
        public static void updateDBOnCancelOrStop(FrilItem item) {
            //出品履歴を削除
            new ItemNoteDBHelper().deleteItemNote(item.item_id);
            //商品備考を削除
            new ShuppinRirekiDBHelper().deleteRireki(item.item_id);
            //値下げカウントを削除
            new NesageCntDBHelper().deleteNesageCnt(item.item_id);
        }
        //指定した値下げ条件に基づいて値下げを実行する
        //返り値:true; 値下げ実行 false:値下げしpi
        public static bool ExecuteNesage(FrilItem item, FrilAPI api, string[] nesage_ng_list) {
            //値下げ除外リストに含まれる場合は値下げしない
            if (Array.IndexOf(nesage_ng_list, item.item_id) >= 0) {
                Log.Logger.Info("値下げNG対象のため値下げしない: " + item.item_id);
                return false;
            }
            //値下げ回数DBに情報があるか調べる（なければ0）
            int old_nesage_cnt = new NesageCntDBHelper().getNesageCnt(item.item_id);
            //値下げ回数が設定した回数以上なら値下げスキップ
            if (old_nesage_cnt >= Settings.getNesageKaisu()) {
                Log.Logger.Info("値下げ最大回数に達したため値下げしない: " + item.item_id);
                return false;
            }
            //いいね数が指定した数に達していなかったら値下げスキップ
            if (item.num_likes < Settings.getDoNesageLowerLikeLimit()) {
                Log.Logger.Info("いいね数が指定数よりも少ないので値下げしない: " + item.item_id);
                return false;
            }
            int rate = Settings.getNesageRate();
            int discount = item.s_price * rate / 100;
            discount = (discount % 10 == 0) ? discount : ((discount / 10) + 1) * 10; //10の位で切り上げ
            int oldprice = item.s_price;
            item.s_price -= discount;
            bool res = api.Edit(item, item.imageurls);
            if (res) {
                //値下げ回数DB更新
                new NesageCntDBHelper().nesageCntIncrement(item.item_id);
                //値下げログ追加
                new NesageLogDBHelper().addNesageLog(item, api.nickname, oldprice, item.s_price, old_nesage_cnt + 1);
                Log.Logger.Info("値下げ成功 :" + item.item_id);
                return true;
            } else {
                Log.Logger.Error("値下げ失敗: " + item.item_id);
                return false;
            }
        }
        //値下げメインロジック
        public static void doNesage() {
            int nesage_groupkind_id = Settings.getNesageGroupKindid();
            var nesage_group_belongs = new GroupBelongDBHelper().selectItemByGroupID(nesage_groupkind_id);
            var nesage_accountid_list = new List<int>();
            string[] nesage_ng_list = Settings.getNesageNGList().ToArray();
            var nesageCntDBHelper = new NesageCntDBHelper();
            var nesageLogDBHelper = new NesageLogDBHelper();
            //値下げ実行対象のアカウントリスト取得
            foreach (var gb in nesage_group_belongs) nesage_accountid_list.Add(gb.AccountID);
            var nesage_accoutns = new AccountDBHelper().selectItem(nesage_accountid_list);
            foreach (Common.Account account in nesage_accoutns) {
                //アカウントが出品している商品を詳細含めて取得
                FrilAPI api = Common.checkFrilAPI(new FrilAPI(account.email,account.password));
                var on_sale_items = api.GetAllItemsWithSellers(api.sellerid, new List<int> { 1 }, false, true);
                foreach (var item in on_sale_items) {
                    bool res = ExecuteNesage(item, api, nesage_ng_list);
                    //値下げ間隔スリープ
                    if (res) System.Threading.Thread.Sleep(Settings.getNesageInterval() * 60 * 1000);
                }
            }
        }
    }

}