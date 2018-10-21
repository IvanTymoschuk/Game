namespace Client
{
    internal class Ship
    {
        public string Name { get; set; }
        public int Length { get; set; }

        public virtual string ToString()
        {
            return string.Format(Name + " | " + Length.ToString());
        }
    }
}