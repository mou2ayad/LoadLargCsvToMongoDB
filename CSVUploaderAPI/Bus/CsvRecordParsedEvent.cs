using CSVUploaderAPI.Contract;
using CSVUploaderAPI.Model;

namespace CSVUploaderAPI.Bus
{
    public class CsvRecordParsedEvent :IDomainEvent<CsvRecordParsedEvent>
    {
        public Clothe Record { set; get; }
        public string FromFile { set; get; }

        public static CsvRecordParsedEvent From(Clothe record, string fromFile) => 
            new() {FromFile = fromFile, Record = record};

        public CsvRecordParsedEvent GetEvent() => this;
    }
}
