using System;

namespace CSVUploaderAPI.Model
{
    public class ClotheDoa
    {
        public ClotheDoa()
        {
            
        }

        public static ClotheDoa From(Clothe clothe, string fromFile)
            => new ()
            {
                Clothe = clothe, 
                CreateDate = DateTime.UtcNow, 
                FromFile = fromFile,
                Id = Guid.NewGuid().ToString()
            };
        public string Id { set; get; }

        public string FromFile { set; get; }
        public DateTime CreateDate { set; get; }

        public Clothe Clothe { set; get; }
    }
}
