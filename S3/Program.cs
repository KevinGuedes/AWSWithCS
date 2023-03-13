using Amazon.S3;
using Amazon.S3.Model;

var cts = new CancellationTokenSource();
var s3Client = new AmazonS3Client();

//await using var inputStream = new FileStream("./kevinsface.png", FileMode.Open, FileAccess.Read);
//var putObjectRequest = new PutObjectRequest
//{
//    BucketName = "kevinstudyaws",
//    Key = "images/versioning.png",
//    ContentType = "image/png",
//    InputStream = inputStream,
//};
//var response = await s3Client.PutObjectAsync(putObjectRequest, cts.Token);

var getObjectRequest = new GetObjectRequest
{
    BucketName = "kevinstudyaws",
    Key = "images/versioning.png",
};

var response = await s3Client.GetObjectAsync(getObjectRequest, cts.Token);
using var memoryStream = new MemoryStream();
await response.ResponseStream.CopyToAsync(memoryStream, cts.Token);

memoryStream.Position = 0;
var fileStream = new FileStream("facefroms3.png", FileMode.Create, FileAccess.Write);
memoryStream.CopyTo(fileStream);
fileStream.Close();
memoryStream.Close();

