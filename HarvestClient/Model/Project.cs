namespace HarvestClient.Model
{
    public class Project
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Code { get; set; }

        public Client Client { get; set; }
    }
}
