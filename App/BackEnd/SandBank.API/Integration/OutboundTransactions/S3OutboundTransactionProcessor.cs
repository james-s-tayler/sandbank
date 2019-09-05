using System;
using System.Threading.Tasks;
using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Util;
using Microsoft.Extensions.Configuration;
using Domain.Transaction;

namespace Integration.OutboundTransactions
{
    public class S3OutboundTransactionProcessor : IOutboundTransactionProcessor
    {
        private readonly IConfiguration _configuration;

        public S3OutboundTransactionProcessor(IConfiguration configuration) => _configuration = configuration;
        
        public async Task Process(Transaction outboundTransaction)
        {
            var outboundBucketName = "fecs-outbound-transactions";
            var s3Client = GetS3Client();

            try
            {
                if (!await AmazonS3Util.DoesS3BucketExistV2Async(s3Client, outboundBucketName))
                {
                    //skip processing because 3rd party bucket does not exist
                    return;
                }
                
                var fileName = $"{DateTime.UtcNow.Ticks}.json";

                var jsonPayload = TransformTransaction(outboundTransaction);

                var putRequest = new PutObjectRequest
                {
                    BucketName = outboundBucketName,
                    ContentBody = jsonPayload,
                    Key = fileName,
                    ServerSideEncryptionMethod = ServerSideEncryptionMethod.AES256
                };

                var putObjectResponse = await s3Client.PutObjectAsync(putRequest);
                
                //got a response? Good for you.
            }
            catch (AmazonS3Exception s3Exception)
            {
                //log it
            }
            catch (Exception exception)
            {
                //log it
            }
        }

        private string TransformTransaction(Transaction outboundTransaction)
        {
            return "";
        }

        private IAmazonS3 GetS3Client()
        {
            var bucketRegion = RegionEndpoint.USEast1;
            var s3Client = new AmazonS3Client(bucketRegion);
            return s3Client;
        }
    }
}