namespace AI.Projects.Project1.Models
{
    public class City
    {
        /// <summary>
        /// A property that stores the cities index
        /// </summary>
        public int Index { get; set; }
        /// <summary>
        /// A property that stores the cities x position
        /// </summary>
        public double XPosition { get; set; }
        /// <summary>
        /// A property that stores the cities y position
        /// </summary>
        public double YPosition { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="index">The cities index</param>
        /// <param name="x">The cities x position</param>
        /// <param name="y">The cities y position</param>
        public City(int index, double x, double y)
        {
            Index = index;
            XPosition = x;
            YPosition = y;
        }
    }
}
