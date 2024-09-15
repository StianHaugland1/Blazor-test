using System.Runtime.CompilerServices;
using Microsoft.EntityFrameworkCore;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.EntityFrameworkCore;

public class MongoDbContext : DbContext
{
    public DbSet<Player> Players { get; init; }
    public DbSet<Match> Matches { get; init; }
    public MongoDbContext(DbContextOptions<MongoDbContext> options) : base(options) {}
}

[Collection("players")]
public class Player
{
    [BsonId]
    public ObjectId Id { get; set; }

    [BsonElement("name")]
    public string Name { get; set; } = "Name not found";

    [BsonElement("nickname")]
    public string Nickname { get; set; } = "Player";
    
    [BsonElement("emoji")]
    public string Emoji { get; set; } = "🤓";

    [BsonElement("authId")]
    public required string AuthId { get; set; }

    [BsonElement("wins")]
    public  int Wins { get; set; }

    [BsonElement("losses")]
    public  int Losses { get; set; }
    
    [BsonElement("totalMatches")]
    public  int TotalMatches { get; set; }

    [BsonElement("rating")]
    public  int Rating { get; set; }
}


[Collection("matches")]
public class Match
{
    [BsonId]
    public required ObjectId Id { get; set; }
    
    [BsonElement("player1")]
    public required ObjectId Player1 { get; set; }
    
    [BsonElement("player2")]
    public required ObjectId Player2 { get; set; }

    [BsonElement("winner")]
    public required ObjectId Winner { get; set; }
}