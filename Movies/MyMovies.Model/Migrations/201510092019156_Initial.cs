namespace MyMovies.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Initial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Filesets",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        filename = c.String(maxLength: 255),
                        path = c.String(maxLength: 255),
                        hash = c.Long(),
                        extension = c.String(maxLength: 15, unicode: false),
                        type = c.String(maxLength: 15, unicode: false),
                    })
                .PrimaryKey(t => t.id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Filesets");
        }
    }
}
