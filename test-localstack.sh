#aws --endpoint-url=http://localhost:4575 sns publish --topic-arn arn:aws:sns:us-east-1:000000000000:outbound-transaction-topic --message hello
aws --endpoint-url=http://localhost:4576 sqs receive-message --queue-url  http://localhost:4576/queue/outbound-transaction-queue
#aws --endpoint-url=http://localhost:4576 sqs delete-message --queue-url http://localhost:4576/queue/outbound-transaction-queue --receipt-handle 6a241331-d82b-4339-be35-afb25d214753#3499c325-9d04-47a8-bc2f-2ea794220ee1
