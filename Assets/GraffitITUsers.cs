using Amazon.DynamoDBv2.DataModel;
[DynamoDBTable("GraffitITUsers")]
public class GraffitITUsers
{
    [DynamoDBHashKey] // Partition key
    public int UserID { get; set; }

    public string UserEmail { get; set; }

    public string UserPassword { get; set; }

    public string UserName { get; set; }

    public string UserBio { get; set; }
}
