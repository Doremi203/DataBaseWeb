using System.Text.Json.Serialization;
using DataAccess.Interfaces;

namespace DataAccess.Models {
    public record Buyer : IEntity {
        [JsonPropertyName("id")] public int Id { get; }
        [JsonPropertyName("name")] public string Name { get; }
        [JsonPropertyName("surname")] public string Surname { get; }
        [JsonPropertyName("city")] public string City { get; }
        [JsonPropertyName("country")] public string Country { get; }
        
        public Buyer(string name, string surname, string city, string country) {
            Id = _entityCounter++;
            Name = name;
            Surname = surname;
            City = city;
            Country = country;
        }

        [JsonConstructor]
        public Buyer(int id, string name, string surname, string city, string country) : this(name, surname, city, country) {
            Id = id;
        }
        
        private static int _entityCounter;
    }
}