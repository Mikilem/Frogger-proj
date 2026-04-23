using UnityEngine;
using MongoDB.Bson;
using MongoDB.Driver;

public class ConnectMongoDB : MonoBehaviour
{
    private MongoClient client;
    private IMongoDatabase database;
    public static IMongoCollection<BsonDocument> usersCollection;
    public PlayerData playerData; // Asegúrate de que la clase PlayerData esté definida correctamente

    BsonDocument document;

    void Start()
    {
        /**
        * Esta cadena es la cadena de conexión a MongoDB Atlas, tiene mi usuario y contraseña, y el nombre del cluster. 
        * (Me dijo el david que usaramos un "superusuario" usamos el mio y ya*/
        string connectionString = "mongodb+srv://a23sersansan_db_user:Hola@cluster0.qp4mqk8.mongodb.net/?appName=Cluster0";

        try
        {
            client = new MongoClient(connectionString); // Conexión a MongoDB.
            database = client.GetDatabase("GameBD"); // Nombre de la base de datos.
            usersCollection = database.GetCollection<BsonDocument>("a23sersansan_db_user"); // Datos recogidos del jugador.
            Debug.Log("Se ha conectado a la BD");
        }
        catch (System.Exception e)
        {
            Debug.LogError("MongoDB Connection Error: " + e.Message); // Si da error nos lo muestra por consola
        }
    }



    // Método para enviar los datos
    public void enviarDatos()
    {
        // Crear un documento BSON con los datos del jugador
        document = new BsonDocument
        {
            { "coins", playerData.coins }, // Guardar la cantidad de monedas
            { "score", playerData.score }, // Guardar la puntuación
            { "currentSkin", playerData.currentSkin }, // Guardar el skin actual
            { "lastSave", playerData.lastSave } // Guardar la fecha de la última guardada
        };

        try
        {
            // Insertar el documento en la colección de MongoDB
            usersCollection.InsertOne(document);
            Debug.Log("¡Datos enviados a MongoDB Atlas!");
        }
        catch (System.Exception e)
        {
            Debug.LogError("Error al insertar los datos: " + e.Message); // Si ocurre algún error lo muestra
        }
    }

    private void OnApplicationQuit()
    {
        enviarDatos();
    }

}