namespace AI.Projects.Shared.Models
{
    public struct Vertex
    {
        /// <summary>
        /// A property that stores the minimum level that the vertex can be reached
        /// </summary>
        public int Level { get; set; }
        /// <summary>
        /// A property that stores the city that the vertex represents
        /// </summary>
        public City Point { get; set; }
        /// <summary>
        /// A property that stores the city that connects to the current city
        /// </summary>
        public City Parent { get; set; }
    }
}
