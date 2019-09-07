aws --endpoint-url=http://localhost:4575 sns create-topic --name outbound-transaction-topic
aws --endpoint-url=http://localhost:4575 sns set-topic-attributes --attribute-name DisplayName --attribute-value OutboundTransactionTopic  --topic-arn arn:aws:sns:us-east-1:000000000000:outbound-transaction-topic
aws --endpoint-url=http://localhost:4576 sqs create-queue --queue-name outbound-transaction-queue
aws --endpoint-url=http://localhost:4575 sns subscribe --topic-arn arn:aws:sns:us-east-1:000000000000:outbound-transaction-topic --protocol sqs --notification-endpoint arn:aws:sqs:us-east-1:queue:outbound-transaction-queue

#test
#aws --endpoint-url=http://localhost:4575 sns publish --topic-arn arn:aws:sns:us-east-1:000000000000:outbound-transaction-topic --message hello
#aws --endpoint-url=http://localhost:4576 sqs receive-message --queue-url  http://localhost:4576/queue/outbound-transaction-queue
