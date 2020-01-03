# .NET Core Lambda: DynamoDB Streams -> S3

This Lambda requires a role that can do the following:
* Push logs to cloudwatch
* Read from DynamoDB / DynamoDB Streams
* Write files to S3

Deploy function to AWS Lambda
```
    dotnet lambda deploy-function SandBank_Lambda_ConfigAuditTrail --function-role SandBank.Lambda.ConfigAuditTrail
```
