using UnityEngine;
using MongoDB.Driver;
using MongoDB.Bson;
using System.Collections.Generic;
using System.Threading.Tasks;

public class ConnectMongoDB : MonoBehaviour
{
    private MongoClient client;
    private IMongoDatabase database;
    private IMongoCollection<BsonDocument> playersCollection;

    void Start()
    {
        string connectionString = "mongodb+srv://a23sersansan_db_user:Hola@cluster0.qp4mqk8.mongodb.net/?retryWrites=true&w=majority";
        try
        {
            client = new MongoClient(connectionString);
            database = client.GetDatabase("GameDB");
            playersCollection = database.GetCollection<BsonDocument>("Players");
            Debug.Log("Conexión exitosa a MongoDB!");
        }
        catch (System.Exception e)
        {
            Debug.LogError("Error al conectar con MongoDB: " + e.Message);
        }
    }

    public async void EnviarDatosAMongoDB(PlayerData playerData)
    {
        BsonDocument document = new BsonDocument {
            { "coins", playerData.coins },
            { "score", playerData.score },
            { "currentSkin", playerData.currentSkin },
            { "lastSave", playerData.lastSave }
        };
        try
        {
            await playersCollection.InsertOneAsync(document);
        }
        catch (System.Exception e)
        {
            Debug.LogError("Error al insertar: " + e.Message);
        }
    }

    // Esta es la función que le falta a tu script para corregir el primer error
    public async Task<string> ObtenerTop10Arcade()
    {
        string ranking = "      LOS 10 MEJORES\n";
        ranking += "--------------------------\n";
        try
        {
            var sort = Builders<BsonDocument>.Sort.Descending("score");
            var lista = await playersCollection.Find(new BsonDocument()).Sort(sort).Limit(10).ToListAsync();

            int pos = 1;
            foreach (var doc in lista)
            {
                int puntos = doc.Contains("score") ? doc["score"].AsInt32 : 0;
                ranking += $"{pos:000}   GMR   {puntos:00000000}\n";
                pos++;
            }
        }
        catch
        {
            ranking = "ERROR DE CONEXION";
        }
        return ranking;
    }

    // Mantén esta por si acaso la usas en otro lado
    public async Task<string> ObtenerTextoLeaderboard()
    {
        return await ObtenerTop10Arcade();
    }
}