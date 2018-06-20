namespace FriRaLand {
    using System;
    using System.Data.Entity;
    using System.Linq;

    public class FrilDbContext : DbContext {
        // コンテキストは、アプリケーションの構成ファイル (App.config または Web.config) から 'FrilDbContext' 
        // 接続文字列を使用するように構成されています。既定では、この接続文字列は LocalDb インスタンス上
        // の 'FriRaLand.FrilDbContext' データベースを対象としています。 
        // 
        // 別のデータベースとデータベース プロバイダーまたはそのいずれかを対象とする場合は、
        // アプリケーション構成ファイルで 'FrilDbContext' 接続文字列を変更してください。
        public FrilDbContext()
            : base("name=FrilDbContext") {
        }
        public DbSet<Models.FrilItemDB> Items { get; set; }
        // モデルに含めるエンティティ型ごとに DbSet を追加します。Code First モデルの構成および使用の
        // 詳細については、http://go.microsoft.com/fwlink/?LinkId=390109 を参照してください。

        // public virtual DbSet<MyEntity> MyEntities { get; set; }
    }

    //public class MyEntity
    //{
    //    public int Id { get; set; }
    //    public string Name { get; set; }
    //}
}