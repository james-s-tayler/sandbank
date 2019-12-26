echo "=====DELETING_API_CALLS======="
rm /tmp/localstack/data/sns_api_calls.json
touch /tmp/localstack/data/sns_api_calls.json
rm /tmp/localstack/data/sqs_api_calls.json
touch /tmp/localstack/data/sqs_api_calls.json

echo "======DEPLOYING_INFRA========="
echo "Listing Queues..."
aws --endpoint-url=http://localhost:4576 sqs list-queues
echo "Listing Topics..."
aws --endpoint-url=http://localhost:4575 sns list-topics
echo "Listing Subscriptions..."
aws --endpoint-url=http://localhost:4575 sns list-subscriptions

echo "Creating Queue..."
aws --endpoint-url=http://localhost:4576 sqs create-queue --queue-name outbound-transaction-queue

echo "Creating Topic..."
aws --endpoint-url=http://localhost:4575 sns create-topic --name outbound-transaction-topic
aws --endpoint-url=http://localhost:4575 sns set-topic-attributes --attribute-name DisplayName --attribute-value OutboundTransactionTopic  --topic-arn arn:aws:sns:us-east-1:000000000000:outbound-transaction-topic

echo "Subscribing Queue To Topic..."
aws --endpoint-url=http://localhost:4575 sns subscribe --topic-arn arn:aws:sns:us-east-1:000000000000:outbound-transaction-topic --protocol sqs --notification-endpoint arn:aws:sqs:us-east-1:000000000000:outbound-transaction-queue

echo "Listing Queues Again..."
aws --endpoint-url=http://localhost:4576 sqs list-queues
echo "Listing Topics Again..."
aws --endpoint-url=http://localhost:4575 sns list-topics
echo "Listing Subscriptions Again..."
aws --endpoint-url=http://localhost:4575 sns list-subscriptions


echo "Testing Publish To Topic..."
aws --endpoint-url=http://localhost:4575 sns publish --topic-arn arn:aws:sns:us-east-1:000000000000:outbound-transaction-topic --message hello

echo "Testing Receive Message On Queue"
aws --endpoint-url=http://localhost:4576 sqs receive-message --queue-url  http://localhost:4576/queue/outbound-transaction-queue
echo "======FINISHED_DEPLOYING========="
