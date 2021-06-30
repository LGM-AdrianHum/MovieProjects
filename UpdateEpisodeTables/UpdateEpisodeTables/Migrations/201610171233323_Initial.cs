namespace UpdateEpisodeTables.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Initial : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Shows", "Status", c => c.String(maxLength: 30));
            AddColumn("dbo.Shows", "CreateDate", c => c.DateTime(nullable: false));
            AlterColumn("dbo.Shows", "Name", c => c.String(maxLength: 128));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Shows", "Name", c => c.String());
            DropColumn("dbo.Shows", "CreateDate");
            DropColumn("dbo.Shows", "Status");
        }
    }
}
