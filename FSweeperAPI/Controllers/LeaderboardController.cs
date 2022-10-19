using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.JsonPatch;
using System.Collections;
using System.Collections.Generic;
using MongoDB.Driver;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System;
using System.Text.Json;
using FSweeperAPI.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Numerics;
using Microsoft.AspNetCore.Diagnostics;
using System.Net;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace FSweeperAPI.Controllers
{
    [Route("api/leaderboard")]
    [ApiController]
    public class LeaderboardController : ControllerBase
    {
        private const string MONGO_URI = "mongodb+srv://ZnoKunG:znoksy139@kittaphotcluster.kvysbjm.mongodb.net/?retryWrites=true&w=majority";
        private const string DATABASE_NAME = "fsweeper";
        private static MongoClient client = new MongoClient(MONGO_URI);
        private static IMongoDatabase db = client.GetDatabase(DATABASE_NAME);
        private static IMongoCollection<Player> playerScoreCollection = db.GetCollection<Player>("player_score");

        // GET: api/<LeaderboardController>

        [HttpGet]
        public PlayerList GetAllPlayers()
        {
            var response = playerScoreCollection.Find(new BsonDocument()).SortBy(bson => bson.score).ToList();
            PlayerList result = new PlayerList();
            result.list = response;
            return result;
        }

        [HttpGet("{name}")]
        public async Task<ActionResult<Player>> GetPlayer(string? name)
        {
            var filter = Builders<Player>.Filter.Eq(s => s.name, name);
            var response = await playerScoreCollection.Find(filter).SingleOrDefaultAsync();
            if (response == null) return StatusCode(StatusCodes.Status500InternalServerError, $"No exist player name {name}"); ;
            return response;
        }

        [HttpPost]
        public async Task<ActionResult<Player>> AddPlayer([FromBody] Player player)
        {
            try
            {
                Player newPlayer = new Player { name = player.name, score = player.score };
                await playerScoreCollection.InsertOneAsync(newPlayer);
                return Content($"Add player name {newPlayer.name} with score {newPlayer.score} success");
            }
            catch (Exception) {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error Inserting Player Data name " + player.name + " with score " + player.score.ToString());
            }
        }

        [HttpPatch]
        public async Task<ActionResult<Player>> UpdatePlayerScore([FromBody] Player player)
        {
            try
            {
                FilterDefinition<Player> filter = new BsonDocument
                {
                    { "name", player.name }, { "score" , new BsonDocument("$gt", player.score) }
                };
                var update = Builders<Player>.Update.Set(s => s.score, player.score);
                var response = await playerScoreCollection.FindOneAndUpdateAsync(filter, update, new FindOneAndUpdateOptions<Player> { IsUpsert = true });
                return Content($"Update player name {player.name} with score {player.score} success");
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error Inserting Player Data name " + player.name + " with score " + player.score.ToString());
            }
        }

        [HttpDelete]
        public async Task<ActionResult<Player>> DeleteAllPlayers()
        {
            try {
                var response = await playerScoreCollection.DeleteManyAsync(bson => true);
                return Content("Delete all players success!");
            } catch {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error deleting all players");
            }
        }
    }
}
