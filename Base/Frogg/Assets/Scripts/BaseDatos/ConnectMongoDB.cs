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

    // Cambiamos a Task para poder esperar a que termine de guardar antes de leer la tabla
    // Cambia la definición de EnviarDatosAMongoDB para que devuelva Task
    public async Task EnviarDatosAMongoDB(PlayerData playerData)
    {
        BsonDocument document = new BsonDocument
    {
        { "name", playerData.playerName }, // <-- Guardamos el nombre
        { "coins", playerData.coins },
        { "score", playerData.score },
        { "currentSkin", playerData.currentSkin },
        { "lastSave", playerData.lastSave }
    };

        try
        {
            await playersCollection.InsertOneAsync(document);
            Debug.Log("¡Datos de " + playerData.playerName + " guardados!");
        }
        catch (System.Exception e)
        {
            Debug.LogError("Error al insertar: " + e.Message);
        }
    }

    // Asegúrate de que esta función tenga el Limit(10)
    public async Task<string> ObtenerTop10Arcade()
    {
        string ranking = "      LOS 10 MEJORES\n";
        ranking += "--------------------------\n";
        try
        {
            var sort = Builders<BsonDocument>.Sort.Descending("score");
            // Forzamos el límite a 10 explícitamente
            var lista = await playersCollection.Find(new BsonDocument()).Sort(sort).Limit(10).ToListAsync();

            int pos = 1;
            foreach (var doc in lista)
            {
                // 1. Intentamos sacar el nombre. Si no existe en la base de datos, ponemos "ANÓNIMO"
                string nombreJugador = doc.Contains("name") ? doc["name"].AsString : "ANÓNIMO";

                // 2. Sacamos la puntuación
                int puntos = doc.Contains("score") ? doc["score"].AsInt32 : 0;

                // 3. Formateamos la línea: 
                // {pos:00} es el número de posición (01, 02...)
                // {nombreJugador,-10} alinea el nombre a la izquierda con un espacio de 10 caracteres
                ranking += $"{pos:00}   {nombreJugador,-10}   {puntos:000000}\n";

                pos++;
            }
        }
        catch
        {
            ranking = "ERROR DE CONEXIÓN";
        }
        return ranking;
    }
}