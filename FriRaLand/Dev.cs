using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RakuLand {
    public static class Dev {
        //エラー発生行とかを書いてくれる
        static public void printE(Exception ex) {
            StackTrace trace = new StackTrace(ex, true); //第二引数のtrueがファイルや行番号をキャプチャするため必要
            foreach (var frame in trace.GetFrames()) {
                Console.WriteLine("**ファイル:"+frame.GetFileName());     //filename
                Console.WriteLine("**行:" + frame.GetFileLineNumber());   //line number
            }
        }


    }
}
