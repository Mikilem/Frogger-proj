using UnityEngine;
using MongoDB.Driver;
using MongoDB.Bson;

public class ConnectMongoDB : MonoBehaviour
{
    private MongoClient client;
    private IMongoDatabase database;
    private IMongoCollection<BsonDocument> playersCollection;

    void Start()
    {
        // Establecer la cadena de conexión a MongoDB Atlas
        string connectionString = "mongodb+srv://a23sersansan_db_user:Hola@cluster0.qp4mqk8.mongodb.net/?retryWrites=true&w=majority";

        try
        {
            // Conectar al cliente de MongoDB
            client = new MongoClient(connectionString);
            database = client.GetDatabase("GameDB"); // Nombre de la base de datos en MongoDB
            playersCollection = database.GetCollection<BsonDocument>("Players"); // Colección de jugadores

            Debug.Log("Conexión exitosa a MongoDB!");
        }
        catch (System.Exception e)
        {
            Debug.LogError("Error al conectar con MongoDB: " + e.Message);
        }
    }

    // Método para enviar los datos directamente a MongoDB
    public void EnviarDatosAMongoDB(PlayerData playerData)
    {
        // Crear un documento BSON a partir de los datos del jugador
        BsonDocument document = new BsonDocument
        {
            { "coins", playerData.coins },
            { "score", playerData.score },
            { "currentSkin", playerData.currentSkin },
            { "lastSave", playerData.lastSave }
        };

        // Intentar insertar el documento BSON en la colección MongoDB
        try
        {
            playersCollection.InsertOne(document);
            Debug.Log("¡Datos enviados correctamente a MongoDB!");
        }
        catch (System.Exception e)
        {
            Debug.LogError("Error al insertar los datos en MongoDB: " + e.Message);
        }
    }
}