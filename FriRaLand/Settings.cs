using FriLand.DBHelper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FriLand {
    class Settings {
        //デフォルト値などもここにある
        public static int getIkkatuShuppinInterval() {
            SettingsDBHelper settingsDBHelper = new SettingsDBHelper();
            int rst = 3;
            string t1 = settingsDBHelper.getSettingValue(Common.ikkatu_shuppin_interval);
            if (t1 != null) {
                try {
                    rst = int.Parse(t1);
                } catch {
                }
            }
            return rst;
        }

        public static int getIkkatuHassouInterval() {
            SettingsDBHelper settingsDBHelper = new SettingsDBHelper();
            int rst = 3;
            string t1 = settingsDBHelper.getSettingValue(Common.ikkatu_hassou_interval);
            if (t1 != null) {
                try {
                    rst = int.Parse(t1);
                } catch {
                }
            }
            return rst;
        }

        public static int getIkkatuHyoukaInterval() {
            SettingsDBHelper settingsDBHelper = new SettingsDBHelper();
            int rst = 3;
            string t1 = settingsDBHelper.getSettingValue(Common.ikkatu_hyouka_interval);
            if (t1 != null) {
                try {
                    rst = int.Parse(t1);
                } catch {
                }
            }
            return rst;
        }

        public static int getIkkatuShukkinInterval() {
            SettingsDBHelper settingsDBHelper = new SettingsDBHelper();
            int rst = 10;
            string t1 = settingsDBHelper.getSettingValue(Common.ikkatu_shukkin_interval);
            if (t1 != null) {
                try {
                    rst = int.Parse(t1);
                } catch {
                }
            }
            return rst;
        }
        public static int getIkkatuTorikesiInterval() {
            SettingsDBHelper settingsDBHelper = new SettingsDBHelper();
            int rst = 3;
            string t1 = settingsDBHelper.getSettingValue(Common.ikkatu_torikesi_interval);
            if (t1 != null) {
                try {
                    rst = int.Parse(t1);
                } catch {
                }
            }
            return rst;
        }
        public static int getReexhibitCheckInterval() {
            SettingsDBHelper settingsDBHelper = new SettingsDBHelper();
            int rst = 10;
            string t1 = settingsDBHelper.getSettingValue(Common.reexhibit_check_interval);
            if (t1 != null) {
                try {
                    rst = int.Parse(t1);
                } catch {
                }
            }
            return rst;
        }
        public static bool getUseProxyAuto() {
            SettingsDBHelper settingsDBHelper = new SettingsDBHelper();
            string t1 = settingsDBHelper.getSettingValue(Common.use_proxy_auto);
            if (t1 != null && t1 == "True") {
                try {
                    return true;
                } catch {
                }
            }
            return false;
        }

        public static bool getUseProxyManual() {
            SettingsDBHelper settingsDBHelper = new SettingsDBHelper();
            string t1 = settingsDBHelper.getSettingValue(Common.use_proxy_manual);
            if (t1 != null && t1 == "True") {
                try {
                    return true;
                } catch {
                }
            }
            return false;
        }

        public static bool getDoNotAutoTokenRefresh() {
            SettingsDBHelper settingsDBHelper = new SettingsDBHelper();
            string t1 = settingsDBHelper.getSettingValue(Common.donot_auto_token_refresh);
            if (t1 != null && t1 == "True") {
                try {
                    return true;
                } catch {
                }
            }
            return false;
        }

        public static bool getCheckKengaiStatusOnExhibit() {
            SettingsDBHelper settingsDBHelper = new SettingsDBHelper();
            string t1 = settingsDBHelper.getSettingValue(Common.check_kengai_status_onexhibit);
            if (t1 != null && t1 == "True") {
                try {
                    return true;
                } catch {
                }
            }
            return false;
        }

        public static int getAutoKengaiCheckIntervalMinute() {
            SettingsDBHelper settingsDBHelper = new SettingsDBHelper();
            int rst = 30;
            string t1 = settingsDBHelper.getSettingValue(Common.auto_check_kengai_interval);
            if (t1 != null) {
                try {
                    rst = int.Parse(t1);
                } catch {
                }
            }
            return rst;
        }

        public static int getKengaiNotExhibitBorder() {
            SettingsDBHelper settingsDBHelper = new SettingsDBHelper();
            int rst = 5;
            string t1 = settingsDBHelper.getSettingValue(Common.kengai_notexhibit_border);
            if (t1 != null) {
                try {
                    rst = int.Parse(t1);
                } catch {
                }
            }
            return rst;
        }

        private static List<string> getStrListFromSettingDB(string key) {
            SettingsDBHelper settingsDBHelper = new SettingsDBHelper();
            List<string> rst = new List<string>();
            string base64str = settingsDBHelper.getSettingValue(key);
            string xmlstr = Common.Base64Decode(base64str);
            if (string.IsNullOrEmpty(base64str)) return rst;
            try {
                System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(typeof(List<string>));
                using (TextReader reader = new StringReader(xmlstr)) {
                    rst = (List<string>)serializer.Deserialize(reader);
                }
                return rst;
            } catch (Exception ex) {
                return rst;
            }
        }

        private static void setStrListToSettingDB(string key, List<string> saveStrList) {
            SettingsDBHelper settingsDBHelper = new SettingsDBHelper();
            string xmlstr = "";
            try {
                System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(typeof(List<string>));
                using (StringWriter textWriter = new StringWriter()) {
                    serializer.Serialize(textWriter, saveStrList);
                    xmlstr = textWriter.ToString();
                }
            } catch (Exception ex) {

            }
            string base64str = Common.Base64Encode(xmlstr);
            settingsDBHelper.updateSettings(key, base64str);
        }
        public static List<string> getCommentTemplate() {
            return getStrListFromSettingDB(Common.comment_template);
        }

        public static void setCommentTemplate(List<string> template_list) {
            setStrListToSettingDB(Common.comment_template, template_list);
        }

        public static List<string> getCommentTemplateTitle() {
            return getStrListFromSettingDB(Common.comment_template_title);
        }

        public static void setCommentTemplateTitle(List<string> template_list) {
            setStrListToSettingDB(Common.comment_template_title, template_list);
        }

        public static List<string> getTransactionMessageTemplate() {
            return getStrListFromSettingDB(Common.transaction_message_template);
        }

        public static void setTransactionMessageTemplate(List<string> template_list) {
            setStrListToSettingDB(Common.transaction_message_template, template_list);
        }

        public static List<string> getTransactionMessageTemplateTitle() {
            return getStrListFromSettingDB(Common.transaction_message_template_title);
        }

        public static void setTransactionMessageTemplateTitle(List<string> template_list) {
            setStrListToSettingDB(Common.transaction_message_template_title, template_list);
        }
        public static List<string> getSellerReviewMessageTemplate() {
            return getStrListFromSettingDB(Common.seller_review_message_template);
        }
        public static void setSellerReviewMessageTemplate(List<string> template_list) {
            setStrListToSettingDB(Common.seller_review_message_template, template_list);
        }
        public static List<string> getSellerReviewMessageTemplateTitle() {
            return getStrListFromSettingDB(Common.seller_review_message_template_title);
        }
        public static void setSellerReviewMessageTemplateTitle(List<string> template_list) {
            setStrListToSettingDB(Common.seller_review_message_template_title, template_list);
        }
        public static List<string> getBuyerReviewMessageTemplate() {
            return getStrListFromSettingDB(Common.buyer_review_message_template);
        }
        public static void setBuyerReviewMessageTemplate(List<string> template_list) {
            setStrListToSettingDB(Common.buyer_review_message_template, template_list);
        }
        public static List<string> getBuyerReviewMessageTemplateTitle() {
            return getStrListFromSettingDB(Common.buyer_review_message_template_title);
        }
        public static void setBuyerReviewMessageTemplateTitle(List<string> template_list) {
            setStrListToSettingDB(Common.buyer_review_message_template_title, template_list);
        }
        public static List<string> getShippingNotificationCommentMessageTemplate() {
            return getStrListFromSettingDB(Common.shipping_notification_comment_message_template);
        }
        public static void setShippingNotificationCommentMessageTemplate(List<string> template_list) {
            setStrListToSettingDB(Common.shipping_notification_comment_message_template, template_list);
        }
        public static List<string> getShippingNotificationCommentMessageTemplateTitle() {
            return getStrListFromSettingDB(Common.shipping_notification_comment_message_template_title);
        }
        public static void setShippingNotificationCommentMessageTemplateTitle(List<string> title_list) {
            setStrListToSettingDB(Common.shipping_notification_comment_message_template_title, title_list);
        }
        public static void updateTemplateSettingDBForAddTitle() {
            //Ver4.5から4.6で定型文コメントにタイトルをつけられるようになったので
            //Ver4.5から4.6にアップデート時はタイトル数は0, 内容数は0以上
            //タイトル数<定型文数の場合、空白タイトルをDBに追加する
            int t1 = getCommentTemplate().Count;
            var n1 = getCommentTemplateTitle();
            int t2 = getTransactionMessageTemplate().Count;
            var n2 = getTransactionMessageTemplateTitle();
            int t3 = getBuyerReviewMessageTemplate().Count;
            var n3 = getBuyerReviewMessageTemplateTitle();
            int t4 = getSellerReviewMessageTemplate().Count;
            var n4 = getSellerReviewMessageTemplateTitle();
            int r1 = t1 - n1.Count;
            int r2 = t2 - n2.Count;
            int r3 = t3 - n3.Count;
            int r4 = t4 - n4.Count;
            if (t1 > n1.Count) for (int i = 0; i < r1; i++) n1.Add("");
            if (t2 > n2.Count) for (int i = 0; i < r2; i++) n2.Add("");
            if (t3 > n3.Count) for (int i = 0; i < r3; i++) n3.Add("");
            if (t4 > n4.Count) for (int i = 0; i < r4; i++) n4.Add("");
            setCommentTemplateTitle(n1);
            setTransactionMessageTemplateTitle(n2);
            setBuyerReviewMessageTemplateTitle(n3);
            setSellerReviewMessageTemplateTitle(n4);
        }
        public static List<string> getItemNameHeaderList() {
            return getStrListFromSettingDB(Common.item_name_header_list);
        }
        public static void setItemNameHeaderList(List<string> header_list) {
            setStrListToSettingDB(Common.item_name_header_list, header_list);
        }
        public static int getItemNameMaxLength() {
            SettingsDBHelper settingsDBHelper = new SettingsDBHelper();
            int rst = 40;
            string t1 = settingsDBHelper.getSettingValue(Common.item_name_max_length);
            if (t1 != null) {
                try {
                    rst = int.Parse(t1);
                } catch {
                }
            }
            return rst;
        }
        public static string getIkkatuShukkinPassword() {
            SettingsDBHelper settingsDBHelper = new SettingsDBHelper();
            string pass = settingsDBHelper.getSettingValue(Common.ikkatu_shukkin_password);
            if (string.IsNullOrEmpty(pass)) return "";
            return pass;
        }

        public static string getAutoBuyURL() {
            SettingsDBHelper settingDBHelper = new SettingsDBHelper();
            string url = settingDBHelper.getSettingValue(Common.autobuy_url);
            if (string.IsNullOrEmpty(url)) return "";
            else return url;
        }

        public static int getAutoBuyInterval() {
            SettingsDBHelper settingsDBHelper = new SettingsDBHelper();
            int rst = 10;
            string t1 = settingsDBHelper.getSettingValue(Common.autobuy_interval);
            if (t1 != null) {
                try {
                    rst = int.Parse(t1);
                } catch {
                }
            }
            return rst;
        }

        public static int getAutoBuyNum() {
            SettingsDBHelper settingsDBHelper = new SettingsDBHelper();
            int rst = 1;
            string t1 = settingsDBHelper.getSettingValue(Common.autobuy_num);
            if (t1 != null) {
                try {
                    rst = int.Parse(t1);
                } catch {
                }
            }
            return rst;
        }

        public static int getDailyExhibitStartHour() {
            SettingsDBHelper settingsDBHelper = new SettingsDBHelper();
            int rst = 6;
            string t1 = settingsDBHelper.getSettingValue(Common.daily_exhibit_start_hour);
            if (t1 != null) {
                try {
                    rst = int.Parse(t1);
                } catch {
                }
            }
            return rst;
        }
        public static int getDailyExhibitStartMinute() {
            SettingsDBHelper settingsDBHelper = new SettingsDBHelper();
            int rst = 0;
            string t1 = settingsDBHelper.getSettingValue(Common.daily_exhibit_start_minute);
            if (t1 != null) {
                try {
                    rst = int.Parse(t1);
                } catch {
                }
            }
            return rst;
        }
        public static int getDailyExhibitIntervalDay() {
            SettingsDBHelper settingsDBHelper = new SettingsDBHelper();
            int rst = 0;
            string t1 = settingsDBHelper.getSettingValue(Common.daily_exhibit_interval_day);
            if (t1 != null) {
                try {
                    rst = int.Parse(t1);
                } catch {
                }
            }
            return rst;
        }
        public static int getDailyExhibitIntervalHour() {
            SettingsDBHelper settingsDBHelper = new SettingsDBHelper();
            int rst = 0;
            string t1 = settingsDBHelper.getSettingValue(Common.daily_exhibit_interval_hour);
            if (t1 != null) {
                try {
                    rst = int.Parse(t1);
                } catch {
                }
            }
            return rst;
        }
        public static int getDailyExhibitIntervalMinute() {
            SettingsDBHelper settingsDBHelper = new SettingsDBHelper();
            int rst = 10;
            string t1 = settingsDBHelper.getSettingValue(Common.daily_exhibit_interval_minute);
            if (t1 != null) {
                try {
                    rst = int.Parse(t1);
                } catch {
                }
            }
            return rst;
        }
        public static int getDailyExhibitRandomIntervalDay1() {
            SettingsDBHelper settingsDBHelper = new SettingsDBHelper();
            int rst = 0;
            string t1 = settingsDBHelper.getSettingValue(Common.daily_exhibit_interval_random_day1);
            if (t1 != null) {
                try {
                    rst = int.Parse(t1);
                } catch {
                }
            }
            return rst;
        }
        public static int getDailyExhibitRandomIntervalHour1() {
            SettingsDBHelper settingsDBHelper = new SettingsDBHelper();
            int rst = 0;
            string t1 = settingsDBHelper.getSettingValue(Common.daily_exhibit_interval_random_hour1);
            if (t1 != null) {
                try {
                    rst = int.Parse(t1);
                } catch {
                }
            }
            return rst;
        }
        public static int getDailyExhibitRandomIntervalMinute1() {
            SettingsDBHelper settingsDBHelper = new SettingsDBHelper();
            int rst = 10;
            string t1 = settingsDBHelper.getSettingValue(Common.daily_exhibit_interval_random_minute1);
            if (t1 != null) {
                try {
                    rst = int.Parse(t1);
                } catch {
                }
            }
            return rst;
        }
        public static int getDailyExhibitRandomIntervalDay2() {
            SettingsDBHelper settingsDBHelper = new SettingsDBHelper();
            int rst = 0;
            string t1 = settingsDBHelper.getSettingValue(Common.daily_exhibit_interval_random_day2);
            if (t1 != null) {
                try {
                    rst = int.Parse(t1);
                } catch {
                }
            }
            return rst;
        }
        public static int getDailyExhibitRandomIntervalHour2() {
            SettingsDBHelper settingsDBHelper = new SettingsDBHelper();
            int rst = 0;
            string t1 = settingsDBHelper.getSettingValue(Common.daily_exhibit_interval_random_hour2);
            if (t1 != null) {
                try {
                    rst = int.Parse(t1);
                } catch {
                }
            }
            return rst;
        }
        public static int getDailyExhibitRandomIntervalMinute2() {
            SettingsDBHelper settingsDBHelper = new SettingsDBHelper();
            int rst = 10;
            string t1 = settingsDBHelper.getSettingValue(Common.daily_exhibit_interval_random_minute2);
            if (t1 != null) {
                try {
                    rst = int.Parse(t1);
                } catch {
                }
            }
            return rst;
        }
        public static bool getDailyExhibitIntervalIsRandom() {
            SettingsDBHelper settingsDBHelper = new SettingsDBHelper();
            string t1 = settingsDBHelper.getSettingValue(Common.daily_exhibit_interval_israndom);
            if (t1 != null && t1 == "True") {
                try {
                    return true;
                } catch {
                }
            }
            return false;
        }
        public static bool getDailyExhibitUseDelete() {
            SettingsDBHelper settingsDBHelper = new SettingsDBHelper();
            string t1 = settingsDBHelper.getSettingValue(Common.daily_exhibit_use_delete);
            if (t1 != null && t1 == "False") {
                try {
                    return false;
                } catch {
                }
            }
            return true;
        }
        public static int getDailyExhibitDeleteDay() {
            SettingsDBHelper settingsDBHelper = new SettingsDBHelper();
            int rst = 1;
            string t1 = settingsDBHelper.getSettingValue(Common.daily_exhibit_delete_day);
            if (t1 != null) {
                try {
                    rst = int.Parse(t1);
                } catch {
                }
            }
            return rst;
        }
        public static int getDailyExhibitDeleteHour() {
            SettingsDBHelper settingsDBHelper = new SettingsDBHelper();
            int rst = 0;
            string t1 = settingsDBHelper.getSettingValue(Common.daily_exhibit_delete_hour);
            if (t1 != null) {
                try {
                    rst = int.Parse(t1);
                } catch {
                }
            }
            return rst;
        }
        public static int getDailyExhibitDeleteMinute() {
            SettingsDBHelper settingsDBHelper = new SettingsDBHelper();
            int rst = 0;
            string t1 = settingsDBHelper.getSettingValue(Common.daily_exhibit_delete_minute);
            if (t1 != null) {
                try {
                    rst = int.Parse(t1);
                } catch {
                }
            }
            return rst;
        }
        public static bool getDailyExhibitConsiderFavorite() {
            SettingsDBHelper settingsDBHelper = new SettingsDBHelper();
            string t1 = settingsDBHelper.getSettingValue(Common.daily_exhibit_consider_favorite);
            if (t1 != null && t1 == "False") {
                try {
                    return false;
                } catch {
                }
            }
            return true;
        }
        public static bool getDailyExhibitConsiderComment() {
            SettingsDBHelper settingsDBHelper = new SettingsDBHelper();
            string t1 = settingsDBHelper.getSettingValue(Common.daily_exhibit_consider_comment);
            if (t1 != null && t1 == "False") {
                try {
                    return false;
                } catch {
                }
            }
            return true;
        }
        public static bool getDailyExhibitUseDelete2() {
            SettingsDBHelper settingsDBHelper = new SettingsDBHelper();
            string t1 = settingsDBHelper.getSettingValue(Common.daily_exhibit_use_delete2);
            if (t1 != null && t1 == "False") {
                try {
                    return false;
                } catch {
                }
            }
            return true;
        }
        public static int getDailyExhibitDeleteDay2() {
            SettingsDBHelper settingsDBHelper = new SettingsDBHelper();
            int rst = 2;
            string t1 = settingsDBHelper.getSettingValue(Common.daily_exhibit_delete_day2);
            if (t1 != null) {
                try {
                    rst = int.Parse(t1);
                } catch {
                }
            }
            return rst;
        }
        public static int getDailyExhibitDeleteHour2() {
            SettingsDBHelper settingsDBHelper = new SettingsDBHelper();
            int rst = 0;
            string t1 = settingsDBHelper.getSettingValue(Common.daily_exhibit_delete_hour2);
            if (t1 != null) {
                try {
                    rst = int.Parse(t1);
                } catch {
                }
            }
            return rst;
        }
        public static int getDailyExhibitDeleteMinute2() {
            SettingsDBHelper settingsDBHelper = new SettingsDBHelper();
            int rst = 0;
            string t1 = settingsDBHelper.getSettingValue(Common.daily_exhibit_delete_minute2);
            if (t1 != null) {
                try {
                    rst = int.Parse(t1);
                } catch {
                }
            }
            return rst;
        }
        public static bool getDailyExhibitConsiderFavorite2() {
            SettingsDBHelper settingsDBHelper = new SettingsDBHelper();
            string t1 = settingsDBHelper.getSettingValue(Common.daily_exhibit_consider_favorite2);
            if (t1 != null && t1 == "True") {
                try {
                    return true;
                } catch {
                }
            }
            return false;
        }
        public static bool getDailyExhibitConsiderComment2() {
            SettingsDBHelper settingsDBHelper = new SettingsDBHelper();
            string t1 = settingsDBHelper.getSettingValue(Common.daily_exhibit_consider_comment2);
            if (t1 != null && t1 == "False") {
                try {
                    return false;
                } catch {
                }
            }
            return true;
        }
        public static bool getDailyExhibitDoReexhibit() {
            SettingsDBHelper settingsDBHelper = new SettingsDBHelper();
            string t1 = settingsDBHelper.getSettingValue(Common.daily_exhibit_reexhibit_flag);
            if (t1 != null && t1 == "False") {
                try {
                    return false;
                } catch {
                }
            }
            return true;
        }
        public static bool useNotification() {
            SettingsDBHelper settingsDBHelper = new SettingsDBHelper();
            string t1 = settingsDBHelper.getSettingValue(Common.use_notification);
            if (t1 != null && t1 == "True") {
                try {
                    return true;
                } catch {
                }
            }
            return false;
        }

        public static int getNotificationInterval() {
            SettingsDBHelper settingsDBHelper = new SettingsDBHelper();
            int rst = 5;
            string t1 = settingsDBHelper.getSettingValue(Common.notification_interval);
            if (t1 != null) {
                try {
                    rst = int.Parse(t1);
                } catch {
                }
            }
            return rst;
        }
        public static DateTime lastNotificationDate() {
            //設定ではないが設定に保存, 最新の通知の日時を取得
            SettingsDBHelper settingsDBHelper = new SettingsDBHelper();
            DateTime rst = new DateTime(1970, 1, 1);
            string t1 = settingsDBHelper.getSettingValue(Common.last_notification_date);
            if (t1 != null) {
                try {
                    rst = DateTime.Parse(t1);
                } catch {
                }
            }
            return rst;
        }
        //疑似購入で商品を購入後・自動で発送通知を送るかどうか
        public static bool getAutoSendShippingNotificationOnFakeBuy() {
            SettingsDBHelper settingsDBHelper = new SettingsDBHelper();
            string t1 = settingsDBHelper.getSettingValue(Common.auto_send_shipping_notification_on_fake_buy);
            if (t1 != null && t1 == "True") {
                try {
                    return true;
                } catch {
                }
            }
            return false;
        }
        //出品者評価後購入者評価を自動で行うか（疑似購入）
        public static bool getAutoReviewBuyerOnReviewSeller() {
            SettingsDBHelper settingsDBHelper = new SettingsDBHelper();
            string t1 = settingsDBHelper.getSettingValue(Common.auto_review_buyer_on_review_seller);
            if (t1 != null && t1 == "False") {
                try {
                    return false;
                } catch {
                }
            }
            return true;
        }
        //コメント画面・取引メッセージ画面の画面サイズを保存するか
        public static bool getSaveMessageWindowSize() {
            SettingsDBHelper settingsDBHelper = new SettingsDBHelper();
            string t1 = settingsDBHelper.getSettingValue(Common.save_message_window_size);
            if (t1 != null && t1 == "True") {
                try {
                    return true;
                } catch {
                }
            }
            return false;
        }

        //コメント画面・取引メッセージ画面の画面サイズ
        public static int getMessageWIndowSizeWidth() {
            SettingsDBHelper settingsDBHelper = new SettingsDBHelper();
            int rst = 787;
            string t1 = settingsDBHelper.getSettingValue(Common.message_window_size_width);
            if (t1 != null) {
                try {
                    rst = int.Parse(t1);
                } catch {
                }
            }
            return rst;
        }
        //コメント画面・取引メッセージ画面の画面サイズ
        public static int getMessageWIndowSizeHeight() {
            SettingsDBHelper settingsDBHelper = new SettingsDBHelper();
            int rst = 600;
            string t1 = settingsDBHelper.getSettingValue(Common.message_window_size_height);
            if (t1 != null) {
                try {
                    rst = int.Parse(t1);
                } catch {
                }
            }
            return rst;
        }

        //最後に受信したお知らせの時間
        public static DateTime getLatestOsiraseCreatedTime() {
            SettingsDBHelper settingsDBHelper = new SettingsDBHelper();
            DateTime rst = new DateTime(1970, 1, 1);
            string t1 = settingsDBHelper.getSettingValue(Common.last_osirase_createdTime);
            if (t1 != null) {
                try {
                    rst = DateTime.Parse(t1);
                } catch {
                }
            }
            return rst;
        }
        //擬似購入画面で購入処理完了から何分後に発送通知を送信し始めるか
        public static int getAutoBuyStartShippingNotificationInterval() {
            SettingsDBHelper settingsDBHelper = new SettingsDBHelper();
            int rst = 60;
            string t1 = settingsDBHelper.getSettingValue(Common.autobuy_start_shipping_notification_interval);
            if (t1 != null) {
                try {
                    rst = int.Parse(t1);
                } catch {
                }
            }
            return rst;
        }
        //値下げを行うか
        public static bool getNesageEnabled() {
            SettingsDBHelper settingsDBHelper = new SettingsDBHelper();
            string t1 = settingsDBHelper.getSettingValue(Common.nesage_enabled);
            if (t1 != null && t1 == "True") {
                try {
                    return true;
                } catch {
                }
            }
            return false;
        }
        //値下げ時刻(hour)
        public static int getNesageHour() {
            SettingsDBHelper settingsDBHelper = new SettingsDBHelper();
            int rst = 0;
            string t1 = settingsDBHelper.getSettingValue(Common.nesage_hour);
            if (t1 != null) {
                try {
                    rst = int.Parse(t1);
                } catch {
                }
            }
            return rst;
        }
        //値下げ時刻(minute)
        public static int getNesageMinute() {
            SettingsDBHelper settingsDBHelper = new SettingsDBHelper();
            int rst = 0;
            string t1 = settingsDBHelper.getSettingValue(Common.nesage_minute);
            if (t1 != null) {
                try {
                    rst = int.Parse(t1);
                } catch {
                }
            }
            return rst;
        }
        //値下げグループ
        public static int getNesageGroupKindid() {
            SettingsDBHelper settingsDBHelper = new SettingsDBHelper();
            int rst = -1;
            string t1 = settingsDBHelper.getSettingValue(Common.nesage_groupkind_id);
            if (t1 != null) {
                try {
                    rst = int.Parse(t1);
                } catch {
                }
            }
            return rst;
        }
        //値下げ回数
        public static int getNesageKaisu() {
            SettingsDBHelper settingsDBHelper = new SettingsDBHelper();
            int rst = 2;
            string t1 = settingsDBHelper.getSettingValue(Common.nesage_kaisu);
            if (t1 != null) {
                try {
                    rst = int.Parse(t1);
                } catch {
                }
            }
            return rst;
        }
        //値下げを行う最低いいね数
        public static int getDoNesageLowerLikeLimit() {
            SettingsDBHelper settingsDBHelper = new SettingsDBHelper();
            int rst = 3;
            string t1 = settingsDBHelper.getSettingValue(Common.nesage_like_lowerlimit);
            if (t1 != null) {
                try {
                    rst = int.Parse(t1);
                } catch {
                }
            }
            return rst;
        }
        //値下げ率
        public static int getNesageRate() {
            SettingsDBHelper settingsDBHelper = new SettingsDBHelper();
            int rst = 10;
            string t1 = settingsDBHelper.getSettingValue(Common.nesage_rate);
            if (t1 != null) {
                try {
                    rst = int.Parse(t1);
                } catch {
                }
            }
            return rst;
        }
        //値下げ除外リスト
        public static List<string> getNesageNGList() {
            return getStrListFromSettingDB(Common.nesage_ng_list);
        }
        public static void setNesageNGList(List<string> ng_list) {
            setStrListToSettingDB(Common.nesage_ng_list, ng_list);
        }
        //値下げの実行間隔
        public static int getNesageInterval() {
            SettingsDBHelper settingsDBHelper = new SettingsDBHelper();
            int rst = 1;
            string t1 = settingsDBHelper.getSettingValue(Common.nesage_interval);
            if (t1 != null) {
                try {
                    rst = int.Parse(t1);
                } catch {
                }
            }
            return rst;
        }

        //手動の値下げ値上げ方法がどちらか trueならレート　falseなら固定金額
        public static bool getManualNeageNesageMethodIsRate() {
            SettingsDBHelper settingsDBHelper = new SettingsDBHelper();
            string t1 = settingsDBHelper.getSettingValue(Common.manual_neage_nesage_method);
            if (t1 != null && t1 == "False") {
                try {
                    return false;
                } catch {
                }
            }
            return true;
        }
        //手動の値下げ値上げのレート
        public static int getManualNeageNesageRate() {
            SettingsDBHelper settingsDBHelper = new SettingsDBHelper();
            int rst = 0;
            string t1 = settingsDBHelper.getSettingValue(Common.manual_neage_nesage_rate);
            if (t1 != null) {
                try {
                    rst = int.Parse(t1);
                } catch {
                }
            }
            return rst;
        }
        //手動の値下げ値上げの固定金額
        public static int getManualNeageNesageFixPrice() {
            SettingsDBHelper settingsDBHelper = new SettingsDBHelper();
            int rst = 0;
            string t1 = settingsDBHelper.getSettingValue(Common.manual_neage_nesage_fixprice);
            if (t1 != null) {
                try {
                    rst = int.Parse(t1);
                } catch {
                }
            }
            return rst;
        }
    }
}
