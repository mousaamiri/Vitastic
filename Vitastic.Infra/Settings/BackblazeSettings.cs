namespace Vitastic.Infra.Settings;

public sealed class BackblazeSettings
{
    public const string SectionName = "Storage:BackblazeB2";
    public string KeyId => "0056f946e72ce560000000003";
    public string KeyName => "vitastic-backblaze-key";
    public string ApplicationKey => "K005qFMpo89rmlmQdKCZ735tzkjGI6c";
    public string BucketId => "061f3924060e67d29cce0516";
    public string BucketName => "vitastic";
    public string Endpoint => "https://s3.us-east-005.backblazeb2.com";
}
//ReadMe:
/*
 ================ Backblaze B2 Cloud Storage || BUCKET DATA  ================
 vitasticCreated:February 17, 2026
Bucket ID:061f3924060e67d29cce0516
Type:Private
File Lifecycle:Keep all versions
Snapshots:0
Current Files:0
Current Size:0 bytes
Endpoint:s3.us-east-005.backblazeb2.com
Encryption:Disabled
 ================ API KEY ================
Success! Your new application key has been created. It will only appear here once.

keyID:
0056f946e72ce560000000003
keyName:
vitastic-backblaze-key
applicationKey:
K005qFMpo89rmlmQdKCZ735tzkjGI6c*/
