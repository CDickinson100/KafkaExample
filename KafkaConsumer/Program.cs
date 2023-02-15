using Common;
using Confluent.Kafka;
using Confluent.Kafka.SyncOverAsync;
using Confluent.SchemaRegistry.Serdes;

Console.WriteLine("What is the GroupId?");
var group = Console.ReadLine();

var config = new ConsumerConfig
{
    BootstrapServers = "localhost:29094",
    GroupId = group ?? "trade-consumer-group"
};

using var consumer = new ConsumerBuilder<string, Trade>(config)
    .SetValueDeserializer(new JsonDeserializer<Trade>().AsSyncOverAsync())
    .Build();

consumer.Subscribe("trade-topic");

Console.WriteLine("Subscribed to Kafka Stream");

while (true)
{
    var message = consumer.Consume();
    var trade = message.Message.Value;
    Console.WriteLine($"Consumed trade: Symbol = {trade.Symbol}, Volume = {trade.Volume} OffSet = {message.Offset.Value}");
}