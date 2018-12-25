using Amazon;
using Amazon.S3;
using Autofac;

namespace Common.Files
{
    public class FilesModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<FileResolver>().As<IFileResolver>().SingleInstance();
            builder.RegisterType<AwsS3FileHandler>().As<IFileHandler>().SingleInstance();
            builder.Register(c =>
            {
                var settings = c.Resolve<AwsS3Settings>();

                return new AmazonS3Client(settings.AccessKey, settings.SecretKey,
                    RegionEndpoint.GetBySystemName(settings.Region));
            })
            .As<IAmazonS3>()
            .SingleInstance();
        }
    }
}