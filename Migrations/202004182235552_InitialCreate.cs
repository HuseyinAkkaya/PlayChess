namespace Chess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.GameRooms",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        WhiteUserId = c.String(),
                        BlackUserId = c.String(),
                        Turn = c.Int(nullable: false),
                        GameStatus = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Pieces",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Color = c.Int(nullable: false),
                        Status = c.Int(nullable: false),
                        Col = c.Int(nullable: false),
                        Row = c.Int(nullable: false),
                        GameRoomId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.GameRooms", t => t.GameRoomId, cascadeDelete: true)
                .Index(t => t.GameRoomId);
            
            CreateTable(
                "dbo.UserConnections",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Userid = c.String(),
                        Connectionid = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Users",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UId = c.String(),
                        Title = c.Int(nullable: false),
                        Name = c.String(),
                        Surname = c.String(),
                        WonCount = c.Int(nullable: false),
                        DrawCount = c.Int(nullable: false),
                        LoseCount = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Pieces", "GameRoomId", "dbo.GameRooms");
            DropIndex("dbo.Pieces", new[] { "GameRoomId" });
            DropTable("dbo.Users");
            DropTable("dbo.UserConnections");
            DropTable("dbo.Pieces");
            DropTable("dbo.GameRooms");
        }
    }
}
