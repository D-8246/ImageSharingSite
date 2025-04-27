using Microsoft.Data.SqlClient;
using static System.Net.Mime.MediaTypeNames;

namespace HW4._21.Data
{
    public class Image
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Location { get; set; }
        public string Password { get; set; }
        public int Views { get; set; }
    }

    public class ImageDB
    {
        private readonly string _connectionString;

        public ImageDB(string connectionString)
        {
            _connectionString = connectionString;
        }

        public int Add(Image image)
        {
            using var connection = new SqlConnection(_connectionString);
            using var cmd = connection.CreateCommand();
            cmd.CommandText = "INSERT INTO Images VALUES (@name, @location, @password, 0) " +
                "SELECT SCOPE_IDENTITY()";
            cmd.Parameters.AddWithValue("@name", image.Name);
            cmd.Parameters.AddWithValue("@location", image.Location);
            cmd.Parameters.AddWithValue("@password", image.Password);
            connection.Open();
            return (int)(decimal)cmd.ExecuteScalar();
        }

        public Image GetImageById(int id)
        {
            using var connection = new SqlConnection(_connectionString);
            using var cmd = connection.CreateCommand();
            cmd.CommandText = "SELECT * FROM Images WHERE Id = @id";
            cmd.Parameters.AddWithValue("@id", id);
            connection.Open();
            var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                return new Image
                {
                    Id = (int)reader["Id"],
                    Name = (string)reader["Name"],
                    Location = (string)reader["Location"],
                    Password = (string)reader["Password"],
                    Views = (int)reader["Views"],
                };
            }
            return null;
        }

        public void IncrementViewsForImage(Image image)
        {
            using var connection = new SqlConnection(_connectionString);
            using var cmd = connection.CreateCommand();
            cmd.CommandText = "UPDATE Images SET Views += 1 WHERE Id = @id";
            cmd.Parameters.AddWithValue("@id", image.Id);
            connection.Open();
            cmd.ExecuteNonQuery();
        }
    }
}
