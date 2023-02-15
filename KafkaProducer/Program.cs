using Common;
using Confluent.Kafka;
using Confluent.Kafka.SyncOverAsync;
using Confluent.SchemaRegistry;
using Confluent.SchemaRegistry.Serdes;

var config = new ProducerConfig { BootstrapServers = "localhost:29094" };

var schemaRegistryConfig = new SchemaRegistryConfig
{
    Url = "localhost:8085",
};
var schemaRegistry = new CachedSchemaRegistryClient(schemaRegistryConfig);

using var producer = new ProducerBuilder<string, Trade>(config)
    .SetValueSerializer(new JsonSerializer<Trade>(schemaRegistry).AsSyncOverAsync())
    .Build();

while (true)
{
    Console.WriteLine("Enter a trade (Symbol-Volume) ");
    var symbol = Console.ReadLine() ?? "";
    Console.WriteLine("Enter a trade volume ");
    var volume = int.Parse(Console.ReadLine() ?? "0");
    var trade = new Trade(symbol, volume);

    producer.Produce("trade-topic", new Message<string, Trade>
    {
        Key = trade.Symbol,
        Value = trade
    });
    Console.WriteLine($"Trade sent");
}