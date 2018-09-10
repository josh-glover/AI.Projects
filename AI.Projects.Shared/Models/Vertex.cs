namespace AI.Projects.Shared.Models
{
    public struct Vertex
    {
        public int Level { get; set; }
        public City Point { get; set; }
        public City Parent { get; set; }
    }
}
