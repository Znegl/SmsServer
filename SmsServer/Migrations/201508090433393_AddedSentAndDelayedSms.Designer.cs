// <auto-generated />
namespace SmsServer.Migrations
{
    using System.CodeDom.Compiler;
    using System.Data.Entity.Migrations;
    using System.Data.Entity.Migrations.Infrastructure;
    using System.Resources;
    
    [GeneratedCode("EntityFramework.Migrations", "6.1.3-40302")]
    public sealed partial class AddedSentAndDelayedSms : IMigrationMetadata
    {
        private readonly ResourceManager Resources = new ResourceManager(typeof(AddedSentAndDelayedSms));
        
        string IMigrationMetadata.Id
        {
            get { return "201508090433393_AddedSentAndDelayedSms"; }
        }
        
        string IMigrationMetadata.Source
        {
            get { return null; }
        }
        
        string IMigrationMetadata.Target
        {
            get { return Resources.GetString("Target"); }
        }
    }
}
