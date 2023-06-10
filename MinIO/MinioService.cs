//using minio;

//namespace webapi.minio
//{
//    public class minioservice
//    {
//        private readonly minioclient _minio;
//        private readonly string _bucketname;

//        public minioservice(string endpoint, string accesskey, string secretkey, string bucketname)
//        {
//            _minio = new minioclient(endpoint, accesskey, secretkey);
//            _bucketname = bucketname;
//        }

//        public async task<string> uploadfileasync(stream stream, string objectname)
//        {
//            await _minio.makebucketasync(_bucketname);

//            stream.position = 0;
//            await _minio.putobjectasync(_bucketname, objectname, stream, stream.length, "application/octet-stream");
//            return $"{_minio.endpoint}/{_bucketname}/{objectname}";
//            //var url = await _minio.presignedgetobjectasync(_bucketname, objectname, timespan.fromdays(7));

//            //return url;

//        }

//        public async task removeobjectasync(string objectname)
//        {
//            await _minio.removeobjectasync(_bucketname, objectname);
//        }


//    }
//}
